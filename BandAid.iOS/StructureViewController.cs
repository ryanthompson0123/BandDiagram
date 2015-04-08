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
    public partial class StructureViewController : UIViewController
    {
        public StructureViewModel Structure { get; set; }

        private StructureParameterListViewController parameterList;

        private StructurePlotScene plotScene;

        public UITextField TitleText { get; set; }

        public StructureViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            if (Structure == null)
            {
                Structure = new StructureViewModel();
                Structure.Name = FigureOutNextName();
            }
                
            parameterList = new StructureParameterListViewController(Structure);
            parameterList.View.Frame = new RectangleF(-200f, 0f, 200f, View.Frame.Height);

            SetUpTitleText();

            Structure.PropertyChanged += Structure_PropertyChanged;
            ToolbarItems = GetBottomButtonItems(ToolbarItems);
            NavigationItem.RightBarButtonItems = RightBarButtonItems;
            NavigationItem.TitleView = TitleText;

            View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

            plotView.ShowsFPS = true;
            plotView.ShowsNodeCount = true;
            plotScene = new StructurePlotScene(plotView.Bounds.Size, Structure);
            plotView.PresentScene(plotScene);
        }

        private void SetUpTitleText()
        {
            TitleText = new UITextField(new RectangleF(0, 0, 400, 44))
            {
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear,
                Font = UIFont.BoldSystemFontOfSize(18.0f),
                Text = Structure.Name,
                ClearButtonMode = UITextFieldViewMode.WhileEditing
            };

            TitleText.Ended += (sender, e) =>
            {
                if (String.IsNullOrEmpty(TitleText.Text))
                {
                    TitleText.Text = Structure.Name;
                }
                else
                {
                    RenameStructure(Structure.Name, TitleText.Text);
                    Structure.Name = TitleText.Text;
                }
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            biasSlider.ValueChanged += biasSlider_ValueChanged;
            chartSegments.ValueChanged += chartSegments_ValueChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            plotScene.TakeScreenshot();
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
                    Structure.PlotType = PlotType.Energy;
                    break;
                case 1:
                    Structure.PlotType = PlotType.Potential;
                    break;
                case 2:
                    Structure.PlotType = PlotType.ElectricField;
                    break;
                case 3:
                    Structure.PlotType = PlotType.ChargeDensity;
                    break;
            }
        }

        void Structure_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            //minVoltageLabel.Text = Structure.MinVoltage.Volts.ToString();
            //maxVoltageLabel.Text = Structure.MaxVoltage.Volts.ToString();
            biasSlider.MinValue = (float)Structure.MinVoltage.Volts;
            biasSlider.MaxValue = (float)Structure.MaxVoltage.Volts;

            if (e.PropertyName == "PlotSteps" || e.PropertyName == "CurrentVoltage")
            {
                parameterList.TableView.ReloadData();
            }

            if (e.PropertyName == "PlotSteps")
            {
                SaveStructure();
            }
        }

        void biasSlider_ValueChanged (object sender, EventArgs e)
        {
            var roundingFactor = 1 / (float)Structure.StepSize.Volts;
            var roundValue = biasSlider.Value * roundingFactor;
            var roundedValue = Math.Round(roundValue, MidpointRounding.AwayFromZero);
            var realValue = roundedValue / roundingFactor;

            zeroVoltageLabel.Text = realValue.ToString() + " V";
            Structure.CurrentVoltage = new ElectricPotential(realValue);
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

                    plotScene.SetUpPlot();
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

                    plotScene.SetUpPlot();
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
                layers.Structure = Structure;
            }

            if (segue.Identifier == "settingsPopoverSegue")
            {
                var settings = (SettingsViewController)segue.DestinationViewController;
                settings.Structure = Structure;
            }
        }

        private async void OnPlayClicked(object sender, EventArgs e)
        {
            chartSegments.UserInteractionEnabled = false;
            biasSlider.UserInteractionEnabled = false;

            var max = new ElectricPotential(biasSlider.MaxValue).RoundMillivolts;
            var min = new ElectricPotential(biasSlider.MinValue).RoundMillivolts;
            var step = Structure.StepSize.RoundMillivolts;
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

        private string DocumentsPath
        {
            get
            {
                return NSFileManager.DefaultManager.GetUrls(
                    NSSearchPathDirectory.DocumentDirectory, 
                    NSSearchPathDomain.User)[0].Path;
            }
        }

        private string FigureOutNextName()
        {
            var nextName = "MyStructure";
            var outDir = Path.Combine(DocumentsPath, "save");

            var nextPath = Path.Combine(outDir, nextName + ".json");
            var nextNumber = 0;
            var tryAgain = File.Exists(nextPath);

            while (tryAgain)
            {
                nextNumber++;
                nextName = "MyStructure" + nextNumber;
                nextPath = Path.Combine(outDir, nextName + ".json");
                tryAgain = File.Exists(nextPath);
            }

            return nextName;
        }

        private void SaveStructure()
        {
            var obj = new JObject();
            obj["name"] = Structure.Name;
            obj["minVoltage"] = Structure.MinVoltage.Volts;
            obj["maxVoltage"] = Structure.MaxVoltage.Volts;
            obj["stepSize"] = Structure.StepSize.Volts;
            obj["currentStep"] = Structure.CurrentVoltage.Volts;

            var structure = JsonConvert.SerializeObject(Structure.ReferenceStructure,
                                new StructureConverter());
            obj["referenceStructure"] = JObject.Parse(structure);

            var outDir = Path.Combine(DocumentsPath, "save");
            var filename = Path.Combine(outDir, Structure.Name + ".json");

            Console.WriteLine(filename);
            File.WriteAllText(filename, obj.ToString());
        }

        private void RenameStructure(string oldName, string newName)
        {
            var outDir = Path.Combine(DocumentsPath, "save");
            var oldFileName = Path.Combine(outDir, oldName + ".json");
            var newFileName = Path.Combine(outDir, newName + ".json");

            var oldScreenshotName = Path.Combine(outDir, oldName + ".png");
            var newScreenshotName = Path.Combine(outDir, newName + ".png");

            File.Move(oldFileName, newFileName);
            File.Move(oldScreenshotName, newScreenshotName);
        }
    }
}