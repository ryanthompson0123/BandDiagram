using Foundation;
using UIKit;
using Band;
using System.ComponentModel;
using System;

namespace BandAid.iOS
{
    public class StructureParameterListViewController : UITableViewController
    {
        public StructureParameterListViewModel ViewModel { get; set; }

        public StructureParameterListViewController()
            : base(UITableViewStyle.Grouped)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            TableView.RegisterClassForCellReuse(typeof(UITableViewCell), new NSString("paramCell"));
            TableView.Source = new ParameterListSource(ViewModel);
            TableView.ContentInset = new UIEdgeInsets(64, 0, 0, 0);
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

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case "Parameters":
                        TableView.ReloadData();
                        break;
                }
            });
        }

        class ParameterListSource : UITableViewSource
        {
            private readonly StructureParameterListViewModel viewModel;

            public ParameterListSource(StructureParameterListViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public override string TitleForHeader(UITableView tableView, nint section)
            {
                return viewModel.Parameters[(int)section].TitleText;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                if (viewModel.Parameters == null) return 0;

                return viewModel.Parameters.Count;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return viewModel.Parameters[(int)section].VoltageDropText == null ? 0 : 2;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = new UITableViewCell(UITableViewCellStyle.Default, "paramCell");
                var layer = viewModel.Parameters[indexPath.Section];

                if (indexPath.Row == 0)
                {
                    cell.TextLabel.Text = layer.CapacitanceText;
                }
                else
                {
                    cell.TextLabel.Text = layer.VoltageDropText;
                }

                return cell;
            }
        }
    }
}