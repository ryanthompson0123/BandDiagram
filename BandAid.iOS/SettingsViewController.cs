
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

            TableView.Delegate = new SettingsViewDelegate(this);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            minVoltageField.EditingChanged += minVoltageField_EditingChanged;
            maxVoltageField.EditingChanged += maxVoltageField_EditingChanged;
            stepSizeField.EditingChanged += stepSizeField_EditingChanged;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (somethingChanged)
            {
                Structure.Set(
                    ParseOrDefault(maxVoltageField.Text, 2.0),
                    ParseOrDefault(minVoltageField.Text, -2.0),
                    ParseOrDefault(stepSizeField.Text, 0.25)
                );
            }

            minVoltageField.EditingChanged -= minVoltageField_EditingChanged;
            maxVoltageField.EditingChanged -= maxVoltageField_EditingChanged;
            stepSizeField.EditingChanged -= stepSizeField_EditingChanged;
        }

        private static double ParseOrDefault(string s, double fallback)
        {
            double value;
            if (Double.TryParse(s, out value))
            {
                return value;
            }

            return fallback;
        }

        private bool somethingChanged;

        private void minVoltageField_EditingChanged(object sender, EventArgs e)
        {
            somethingChanged = true;
        }

        private void maxVoltageField_EditingChanged(object sender, EventArgs e)
        {
            somethingChanged = true;
        }

        private void stepSizeField_EditingChanged(object sender, EventArgs e)
        {
            somethingChanged = true;
        }

        public void OnRowSelected(int index)
        {
            switch (index)
            {
                case 0:
                    minVoltageField.BecomeFirstResponder();
                    minVoltageField.SelectAll(this);
                    break;
                case 1:
                    maxVoltageField.BecomeFirstResponder();
                    maxVoltageField.SelectAll(this);
                    break;
                case 2:
                    stepSizeField.BecomeFirstResponder();
                    stepSizeField.SelectAll(this);
                    break;
            }
        }

        class SettingsViewDelegate : UITableViewDelegate
        {
            private readonly SettingsViewController viewController;
            public SettingsViewDelegate(SettingsViewController viewController)
            {
                this.viewController = viewController;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                viewController.OnRowSelected(indexPath.Row);
            }
        }
    }
}

