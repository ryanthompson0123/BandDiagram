
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SpriteKit;
using Band;
using System.ComponentModel;
using Band.Units;

namespace BandAid.iOS
{
    public partial class StructureViewController : UIViewController
    {
        public StructureViewModel Structure { get; set; }

        public StructureViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            Structure = new StructureViewModel();
            Structure.PropertyChanged += Structure_PropertyChanged;
            ToolbarItems = GetBottomButtonItems(ToolbarItems);
            NavigationItem.RightBarButtonItems = RightBarButtonItems;

            View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

            plotView.ShowsFPS = true;
            plotView.ShowsNodeCount = true;
            plotView.PresentScene(new StructurePlotScene(plotView.Bounds.Size, Structure));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            biasSlider.ValueChanged += biasSlider_ValueChanged;
            chartSegments.ValueChanged += chartSegments_ValueChanged;
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
            minVoltageLabel.Text = Structure.MinVoltage.Volts.ToString();
            maxVoltageLabel.Text = Structure.MaxVoltage.Volts.ToString();
            biasSlider.MinValue = (float)Structure.MinVoltage.Volts;
            biasSlider.MaxValue = (float)Structure.MaxVoltage.Volts;
        }

        void biasSlider_ValueChanged (object sender, EventArgs e)
        {
            var roundingFactor = 1 / (float)Structure.StepSize.Volts;
            var roundValue = biasSlider.Value * roundingFactor;
            var roundedValue = Math.Round(roundValue, MidpointRounding.AwayFromZero);
            var realValue = roundedValue / roundingFactor;

            zeroVoltageLabel.Text = realValue.ToString();
            Structure.CurrentVoltage = new ElectricPotential(realValue);
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

        private UIBarButtonItem[] RightBarButtonItems
        {
            get
            {
                return new []
                {
                    new UIBarButtonItem(UIBarButtonSystemItem.Action),
                    NavigationItem.RightBarButtonItem,
                    new UIBarButtonItem(UIBarButtonSystemItem.Play)
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