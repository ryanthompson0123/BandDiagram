
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Band;
using Band.Units;

namespace BandAid.iOS
{
    public partial class SettingsViewController : UITableViewController
    {
        public StructureViewModel Structure { get; set; }

        public SettingsViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            minVoltageField.Text = Structure.MinVoltage.Volts.ToString();
            maxVoltageField.Text = Structure.MaxVoltage.Volts.ToString();
            stepSizeField.Text = Structure.StepSize.Volts.ToString();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            minVoltageField.EditingChanged += minVoltageField_ValueChanged;
            maxVoltageField.EditingChanged += maxVoltageField_ValueChanged;
            stepSizeField.EditingChanged += stepSizeField_ValueChanged;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            minVoltageField.ValueChanged -= minVoltageField_ValueChanged;
            maxVoltageField.ValueChanged -= maxVoltageField_ValueChanged;
            stepSizeField.ValueChanged -= stepSizeField_ValueChanged;
        }

        private void minVoltageField_ValueChanged(object sender, EventArgs e)
        {
            Structure.MinVoltage = new ElectricPotential(Double.Parse(minVoltageField.Text));
        }

        private void maxVoltageField_ValueChanged(object sender, EventArgs e)
        {
            Structure.MaxVoltage = new ElectricPotential(Double.Parse(maxVoltageField.Text));
        }

        private void stepSizeField_ValueChanged(object sender, EventArgs e)
        {
            Structure.StepSize = new ElectricPotential(Double.Parse(stepSizeField.Text));
        }
    }
}

