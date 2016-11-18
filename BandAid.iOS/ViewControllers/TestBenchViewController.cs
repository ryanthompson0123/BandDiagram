using System;
using CoreGraphics;

using Foundation;
using UIKit;
using SpriteKit;
using Band;
using System.ComponentModel;
using Band.Units;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;

namespace BandAid.iOS
{
    public partial class TestBenchViewController : UIViewController
    {
        public TestBenchViewModel ViewModel { get; set; }

        private StructureParameterListViewController parameterList;

        public UITextField TitleText { get; set; }

        public GraphView GraphView
        {
            get { return graphView; }
        }

        public UIBarButtonItem ToggleButton
        {
            get { return ToolbarItems[0]; }
        }

        public TestBenchViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            parameterList = new StructureParameterListViewController
            {
                ViewModel = ViewModel.StructureParameterList
            };

            parameterList.View.Frame = new CGRect(-200f, 0f, 200f, View.Frame.Height);

            SetUpTitleText();
            NavigationItem.TitleView = TitleText;

            ToolbarItems = GetBottomButtonItems(ToolbarItems);

            View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

            vfbLabel.Text = "";
            eotLabel.Text = "";
            cstackLabel.Text = "";
            vthLabel.Text = "";
            graphView.UserInteractionEnabled = false;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            chartSegments.ValueChanged += chartSegments_ValueChanged;
            GraphView.AnimationValueChanged += GraphView_AnimationValueChanged;
            GraphView.PointLongPressed += GraphView_PointLongPressed;
            GraphView.PointTapped += GraphView_PointTapped;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            chartSegments.ValueChanged -= chartSegments_ValueChanged;
            GraphView.AnimationValueChanged -= GraphView_AnimationValueChanged;
            GraphView.PointLongPressed -= GraphView_PointLongPressed;

        }

        private void SetUpTitleText()
        {
            TitleText = new UITextField(new CGRect(0, 0, 400, 44))
            {
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Font = UIFont.BoldSystemFontOfSize(18.0f),
                Text = ViewModel.TitleText,
                ClearButtonMode = UITextFieldViewMode.WhileEditing
            };

            TitleText.Ended += (sender, e) =>
            {
                if (string.IsNullOrEmpty(TitleText.Text))
                {
                    TitleText.Text = ViewModel.TitleText;
                }
                else
                {
                    ViewModel.TitleText = TitleText.Text;
                }
            };
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            var touch = (UITouch)evt.AllTouches.AnyObject;

            if (!(touch.View is UITextField))
            {
                TitleText.EndEditing(true);
            }

            base.TouchesBegan(touches, evt);
        }

        void chartSegments_ValueChanged (object sender, EventArgs e)
        {
            graphView.ActivityIndicator.StartAnimating();

            switch (chartSegments.SelectedSegment)
            {
                case 0:
                    ViewModel.SetSelectedPlotType(PlotType.Energy);
                    break;
                case 1:
                    ViewModel.SetSelectedPlotType(PlotType.Potential);
                    break;
                case 2:
                    ViewModel.SetSelectedPlotType(PlotType.ElectricField);
                    break;
                case 3:
                    ViewModel.SetSelectedPlotType(PlotType.ChargeDensity);
                    break;
            }
        }

        private void GraphView_AnimationValueChanged (object sender, EventArgs e)
        {
            ViewModel.SetSelectedBias(GraphView.AnimationValue);
        }

        private Material longPressedMaterial;
        void GraphView_PointLongPressed(object sender, PointTappedEventArgs e)
        {
            longPressedMaterial = ViewModel.GetMaterialAtPoint(e.PlotDataPoint);
            PerformSegue("layersPopoverSegue", this);
        }

        private PlotDataPoint tappedPoint;
        private CGPoint tappedLocation;
        void GraphView_PointTapped(object sender, PointTappedEventArgs e)
        {
            tappedPoint = e.PlotDataPoint;
            tappedLocation = e.Location;
            PerformSegue("locationPopoverSegue", this);
        }

        void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case "TitleText":
                        TitleText.Text = ViewModel.TitleText;
                        break;
                    case "Computing":
                        if (ViewModel.Computing && ViewModel.TestBench.Structure.IsValid) graphView.ActivityIndicator.StartAnimating();
                        graphView.UserInteractionEnabled = !ViewModel.Computing;
                        break;
                    case "PlotGroup":
                        GraphView.SetPlotGroup(ViewModel.PlotGroup);
                        GraphView.SetAnimationValue(ViewModel.TestBench.CurrentVoltage.Volts, false);
                        break;
                    case "NeedsScreenshot":
                        if (ViewModel.NeedsScreenshot)
                        {
                            TakeScreenshot();
                        }
                        break;
                    case "FlatbandVoltageText":
                        vfbLabel.Text = ViewModel.FlatbandVoltageText;
                        break;
                    case "EotText":
                        eotLabel.Text = ViewModel.EotText;
                        break;
                    case "CstackText":
                        cstackLabel.Text = ViewModel.CstackText;
                        break;
                    case "ThresholdVoltageText":
                        vthLabel.Text = ViewModel.ThresholdVoltageText;
                        break;
                }
            });
        }

        private async void TakeScreenshot()
        {
            await TakeScreenshotAsync();
        }

        private async Task TakeScreenshotAsync()
        {
            var image = GraphView.RenderPlotToImage();

            using (var imageData = image.AsPNG())
            {
                using (var imageStream = imageData.AsStream())
                {
                    await ViewModel.SaveScreenshotAsync(imageStream);
                }
            }
        }

        bool toggleIsOpen;
        bool firstTime = true;
        bool toggling = false;
        async partial void OnToggleClicked(NSObject sender)
        {
            if (toggling) return;

            toggling = true;
            if (toggleIsOpen)
            {
                ToggleButton.Image = UIImage.FromBundle("toggle");

                parameterList.ViewWillDisappear(true);
                await UIView.AnimateAsync(.3, () =>
                {
                    View.Frame = new CGRect(0, 0, 
                        View.Frame.Width + 200, View.Frame.Height);
                    
                    parameterList.View.Frame = new CGRect(-200, 0, 
                        200, View.Frame.Height);
                    View.LayoutIfNeeded();
                    // For speed we just redraw the showing plot
                    //plotScene.RedrawCurrentPlot();
                });

                // After the animation is done, we start redrawing all the plots
                //plotScene.SetUpPlot(false);
                toggleIsOpen = false;
                parameterList.ViewDidDisappear(true);
            }
            else
            {
                ToggleButton.Image = UIImage.FromBundle("untoggle");

                if (firstTime)
                {
                    View.Superview.AddSubview(parameterList.View);
                    firstTime = false;
                }

                parameterList.ViewWillAppear(true);
                await UIView.AnimateAsync(.3, () =>
                {
                    View.Frame = new CGRect(200, 0, View.Frame.Width - 200, View.Frame.Height);
                    parameterList.View.Frame = new CGRect(0, 0, 
                        200, View.Frame.Height);
                    View.LayoutIfNeeded();
                    // For speed we just redraw the showing plot
                    //plotScene.RedrawCurrentPlot();
                });

                // After the animation is done, we start redrawing all the plots
                //plotScene.SetUpPlot(false);
                toggleIsOpen = true;
                parameterList.ViewDidAppear(true);
            }

            toggling = false;
        }

        async partial void OnPlayClicked(NSObject sender)
        {
            UIApplication.SharedApplication.BeginIgnoringInteractionEvents();

            await GraphView.RunSweepAnimationAsync();

            UIApplication.SharedApplication.EndIgnoringInteractionEvents();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "layersPopoverSegue")
            {
                var destination = (UINavigationController)segue.DestinationViewController;
                var layers = (StructureTableViewController)destination.ChildViewControllers[0];

                layers.ViewModel = new StructureViewModel(ViewModel.TestBench.Structure);
                layers.ViewModel.SetDirectEditMaterial(longPressedMaterial);
                longPressedMaterial = null;
            }

            if (segue.Identifier == "locationPopoverSegue")
            {
                DoLocationPopoverSegue(segue);
            }

            if (segue.Identifier == "settingsPopoverSegue")
            {
                var settings = (SettingsViewController)segue.DestinationViewController;
                settings.ViewModel = ViewModel.GetSettingsViewModel();

                var popover = settings.PopoverPresentationController;
                popover.Delegate = new SettingsPopoverDelegate(ViewModel);
            }

            if (segue.Identifier == "unwindToGallery")
            {
                ViewModel.SaveTestBench();
            }
        }

        private void DoLocationPopoverSegue(UIStoryboardSegue segue)
        {
            var viewController = (StructurePointDetailViewController)segue.DestinationViewController;
            var popover = viewController.PopoverPresentationController;
            popover.SourceView = GraphView.Plot;
            popover.SourceRect = new CGRect(tappedLocation.X, tappedLocation.Y, 10f, 10f);

            viewController.ViewModel = new StructurePointDetailViewModel(ViewModel.TestBench.CurrentStructure, tappedPoint);
        }

        private readonly UISegmentedControl chartSegments = new UISegmentedControl(new[] {
                "Energy",
                "Potential",
                "Electric Field",
                "Charge Density"
            }) {
                SelectedSegment = 0
            };
        
        private UIBarButtonItem[] GetBottomButtonItems(UIBarButtonItem[] items)
        {
            var array = new UIBarButtonItem[items.Length + 1];

            for (var i = 0; i < array.Length; i++)
            {
                if (i < items.Length / 2)
                {
                    array[i] = items[i];
                }

                if (i == items.Length / 2)
                {
                    array[i] = new UIBarButtonItem
                    {
                        CustomView = chartSegments
                    };
                }

                if (i > items.Length / 2)
                {
                    array[i] = items[i - 1];
                }
            }

            return array;
        }

        class SettingsPopoverDelegate : UIPopoverPresentationControllerDelegate
        {
            private readonly TestBenchViewModel viewModel;
            public SettingsPopoverDelegate(TestBenchViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public override void DidDismissPopover(UIPopoverPresentationController popoverPresentationController)
            {
                var settingsVc = (SettingsViewController)popoverPresentationController
                    .PresentedViewController;
                var settingsVm = settingsVc.ViewModel;
                viewModel.UpdateSettings(
                    settingsVm.MinVoltageText,
                    settingsVm.MaxVoltageText,
                    settingsVm.StepSizeText);
            }
        }

        class StructurePopoverDelegate : UIPopoverPresentationControllerDelegate
        {
            private readonly TestBenchViewModel viewModel;
            public StructurePopoverDelegate(TestBenchViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public async override void DidDismissPopover(UIPopoverPresentationController popoverPresentationController)
            {
                await viewModel.TestBench.ComputeIfNeededAsync(default(CancellationToken));
            }
        }
    }
}