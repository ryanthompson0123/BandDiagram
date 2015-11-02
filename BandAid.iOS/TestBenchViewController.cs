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
            NavigationItem.RightBarButtonItems = RightBarButtonItems;

            View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            chartSegments.ValueChanged += chartSegments_ValueChanged;
            GraphView.AnimationValueChanged += GraphView_AnimationValueChanged;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
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

        bool firstGraph = true;

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
                        break;
                    case "PlotGroup":
                        GraphView.SetPlotGroup(ViewModel.PlotGroup);

                        if (firstGraph)
                        {
                            GraphView.SetAnimationValue(ViewModel.PlotGroup.AnimationAxis.Midpoint, false);
                            firstGraph = false;
                        }
                        break;
                }
            });
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

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "layersPopoverSegue")
            {
                var destination = (UINavigationController)segue.DestinationViewController;
                var layers = (StructureTableViewController)destination.ChildViewControllers[0];
                layers.ViewModel = new StructureViewModel(ViewModel.TestBench.Structure);
            }

            if (segue.Identifier == "settingsPopoverSegue")
            {
                var settings = (SettingsViewController)segue.DestinationViewController;
                settings.ViewModel = ViewModel.GetSettingsViewModel();

                var popover = settings.PopoverPresentationController;
                popover.Delegate = new SettingsPopoverDelegate(ViewModel);
            }
        }

        private async void OnPlayClicked(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.BeginIgnoringInteractionEvents();

            await GraphView.RunSweepAnimationAsync();

            UIApplication.SharedApplication.EndIgnoringInteractionEvents();
        }

        UIBarButtonItem playButton;
        private UIBarButtonItem[] RightBarButtonItems
        {
            get
            {
                playButton = new UIBarButtonItem(UIBarButtonSystemItem.Play);
                playButton.Clicked += OnPlayClicked;

                return new []
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.Action),
                    NavigationItem.RightBarButtonItem,
                    playButton
                };
            }
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
                await viewModel.TestBench.ComputeIfNeededAsync();
            }
        }
    }
}