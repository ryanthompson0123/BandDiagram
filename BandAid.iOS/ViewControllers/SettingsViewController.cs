
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using Band;
using Band.Units;
using System.ComponentModel;

namespace BandAid.iOS
{
    public partial class SettingsViewController : UITableViewController
    {
        public SettingsViewModel ViewModel { get; set; }

        public SettingsViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            minVoltageField.Text = ViewModel.MinVoltageText;
            maxVoltageField.Text = ViewModel.MaxVoltageText;
            stepSizeField.Text = ViewModel.StepSizeText;

            TableView.Delegate = new SettingsViewDelegate(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StepSizeText":
                    stepSizeField.Text = ViewModel.StepSizeText;
                    break;
                case "MaxVoltageText":
                    maxVoltageField.Text = ViewModel.MaxVoltageText;
                    break;
                case "MinVoltageText":
                    minVoltageField.Text = ViewModel.MinVoltageText;
                    break;
            }
        }

        partial void OnMaxVoltageEditingChanged(NSObject sender)
        {
            ViewModel.MaxVoltageText = maxVoltageField.Text;
        }

        partial void OnMinVoltageEditingChanged(NSObject sender)
        {
            ViewModel.MinVoltageText = minVoltageField.Text;
        }

        partial void OnStepSizeEditingChanged(NSObject sender)
        {
            ViewModel.StepSizeText = stepSizeField.Text;
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

