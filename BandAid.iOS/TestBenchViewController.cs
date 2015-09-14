using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;
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

        private StructurePlotScene plotScene;

        public UITextField TitleText { get; set; }

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

            parameterList.View.Frame = new RectangleF(-200f, 0f, 200f, View.Frame.Height);

            SetUpTitleText();
            NavigationItem.TitleView = TitleText;

            ToolbarItems = GetBottomButtonItems(ToolbarItems);
            NavigationItem.RightBarButtonItems = RightBarButtonItems;

            View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

            plotView.ShowsFPS = true;
            plotView.ShowsNodeCount = true;
            plotScene = new StructurePlotScene(plotView.Bounds.Size)
            {
                ViewModel = ViewModel.Scene
            };
            plotView.PresentScene(plotScene);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            biasSlider.ValueChanged += biasSlider_ValueChanged;
            //chartSegments.ValueChanged += chartSegments_ValueChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            plotScene.TakeScreenshot();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            biasSlider.ValueChanged -= biasSlider_ValueChanged;
        }

        private void SetUpTitleText()
        {
            TitleText = new UITextField(new RectangleF(0, 0, 400, 44))
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
            switch (chartSegments.SelectedSegment)
            {
                case 0:
                    ViewModel.PlotType = PlotType.Energy;
                    break;
                case 1:
                    ViewModel.PlotType = PlotType.Potential;
                    break;
                case 2:
                    ViewModel.PlotType = PlotType.ElectricField;
                    break;
                case 3:
                    ViewModel.PlotType = PlotType.ChargeDensity;
                    break;
            }
        }

        void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TitleText":
                    TitleText.Text = ViewModel.TitleText;
                    break;
                case "CurrentVoltageText":
                    zeroVoltageLabel.Text = ViewModel.CurrentVoltageText;
                    break;
                case "BiasSliderMaxValue":
                    biasSlider.MaxValue = (float)ViewModel.BiasSliderMaxValue;
                    break;
                case "BiasSliderMinValue":
                    biasSlider.MinValue = (float)ViewModel.BiasSliderMinValue;
                    break;
            }
        }

        void biasSlider_ValueChanged (object sender, EventArgs e)
        {
            ViewModel.SetSelectedBias(biasSlider.Value);
        }

        bool toggleIsOpen;
        bool firstTime = true;
        async partial void ToggleTouched(NSObject sender)
        {
            if (toggleIsOpen)
            {
                await UIView.AnimateAsync(.3, () =>
                {
                    View.Frame = new RectangleF(0, 0, View.Frame.Width + 200, View.Frame.Height);
                    parameterList.View.Frame = new RectangleF(-200, 0, 
                        200, View.Frame.Height);
                    View.LayoutIfNeeded();
                });

                toggleIsOpen = false;
            }
            else
            {
                if (firstTime)
                {
                    View.Superview.AddSubview(parameterList.View);
                    firstTime = false;
                }

                await UIView.AnimateAsync(.3, () =>
                {
                    View.Frame = new RectangleF(200, 0, View.Frame.Width - 200, View.Frame.Height);
                    parameterList.View.Frame = new RectangleF(0, 0, 
                        200, View.Frame.Height);
                    View.LayoutIfNeeded();
                });

                toggleIsOpen = true;
            }
        }

        async partial void StructuresTouched(NSObject sender)
        {
            await NavigationController.PresentingViewController.DismissViewControllerAsync(true);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "layersPopoverSegue")
            {
                var destination = (UINavigationController)segue.DestinationViewController;
                var layers = (LayersTableViewController)destination.ChildViewControllers[0];
                layers.Structure = ViewModel;
            }

            if (segue.Identifier == "settingsPopoverSegue")
            {
                var settings = (SettingsViewController)segue.DestinationViewController;
                settings.Structure = ViewModel;
            }
        }

        private async void OnPlayClicked(object sender, EventArgs e)
        {
            chartSegments.UserInteractionEnabled = false;
            biasSlider.UserInteractionEnabled = false;

            var max = new ElectricPotential(biasSlider.MaxValue).RoundMillivolts;
            var min = new ElectricPotential(biasSlider.MinValue).RoundMillivolts;
            var step = ViewModel.TestBench.StepSize.RoundMillivolts;
            var delay = 2000 / ((max - min) / step);    // Whole animation should take 2s

            await Task.Run(async () =>
            {
                for (var i = min; i <= max; i += step)
                {
                    if (i == min)
                    {
                        InvokeOnMainThread(() => 
                        {
                            biasSlider.SetValue((float)i / (float)1000.0, false);
                            biasSlider_ValueChanged(this, EventArgs.Empty);
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() => 
                        {
                            biasSlider.SetValue((float)i / (float)1000.0, true);
                            biasSlider_ValueChanged(this, EventArgs.Empty);
                        });
                    }

                    await Task.Delay(delay);
                }
            });

            chartSegments.UserInteractionEnabled = true;
            biasSlider.UserInteractionEnabled = true;
        }

        private UIBarButtonItem[] RightBarButtonItems
        {
            get
            {
                var playButton = new UIBarButtonItem(UIBarButtonSystemItem.Play);
                //playButton.Clicked += OnPlayClicked;

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
    }
}