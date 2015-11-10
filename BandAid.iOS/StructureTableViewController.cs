
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using Band;
using System.ComponentModel;

namespace BandAid.iOS
{
    public partial class StructureTableViewController : UITableViewController
    {
        public StructureViewModel ViewModel { get; set; }

        public StructureTableViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            TableView.Source = new LayersTableSource(TableView, ViewModel);
            TableView.SetEditing(true, false);

            PreferredContentSize = new CGSize(360, 540);
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
            // Not currently used
        }

        partial void OnDuplicateClicked(NSObject sender)
        {
            var indexPath = TableView.IndexPathForSelectedRow;
            if (indexPath == null) return;

            var tappedLayer = ViewModel.Layers[indexPath.Row];
            ViewModel.DuplicateLayer(tappedLayer);

            TableView.DeselectRow(indexPath, true);
        }

        partial void OnTrashClicked(NSObject sender)
        {
            var indexPath = TableView.IndexPathForSelectedRow;
            if (indexPath == null) return;

            var deletedLayer = ViewModel.Layers[indexPath.Row];
            ViewModel.DeleteLayer(deletedLayer);
        }

        [Action("UnwindToStructure:")]
        public void UnwindToStructure(UIStoryboardSegue segue)
        {
            var sourceVc = (MaterialSelectViewController)segue.SourceViewController;
            var selectedMaterial = sourceVc.ViewModel.SelectedMaterial;

            ViewModel.AddLayer(new LayerViewModel(selectedMaterial));
        }

        class LayersTableSource : UITableViewSource
        {
            private readonly StructureViewModel viewModel;

            public LayersTableSource(UITableView tableView, StructureViewModel viewModel)
            {
                this.viewModel = viewModel;
                viewModel.Layers.CollectionChanged += (sender, e) => tableView.ReloadData();
            }

            #region implemented abstract members of UITableViewSource
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell("LayerCell");

                if (indexPath.Row == viewModel.Layers.Count)
                {
                    cell.TextLabel.TextColor = UIColor.Red;
                    cell.DetailTextLabel.Text = "";
                    cell.Accessory = UITableViewCellAccessory.None;

                    if (viewModel.CurrentLayoutIsInvalid)
                    {
                        cell.TextLabel.Text = "This structure is invalid";
                    }
                    else if (viewModel.CurrentLayoutHasNoSolution)
                    {
                        cell.TextLabel.Text = "No soultion found for this structure";
                    }

                    return cell;
                }

                var layerVm = viewModel.Layers[indexPath.Row];

                cell.TextLabel.TextColor = UIColor.Black;
                cell.TextLabel.Text = layerVm.NameText;
                cell.DetailTextLabel.Text = layerVm.MaterialTypeText;

                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (viewModel.CurrentLayoutHasNoSolution ||
                    viewModel.CurrentLayoutIsInvalid)
                {
                    return viewModel.Layers.Count + 1;
                }

                return viewModel.Layers.Count;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                if (indexPath.Row == viewModel.Layers.Count) return false;  // Error row

                return true;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, 
                NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.None;
            }

            public override bool ShouldIndentWhileEditing(UITableView tableView, 
                NSIndexPath indexPath)
            {
                return false;
            }

            public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
            {
                if (indexPath.Row == viewModel.Layers.Count) return false; // Error row
                return true;
            }

            public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, 
                NSIndexPath destinationIndexPath)
            {
                var movedLayer = viewModel.Layers[sourceIndexPath.Row];

                if (destinationIndexPath.Row == viewModel.Layers.Count)
                {
                    viewModel.MoveLayer(movedLayer, destinationIndexPath.Row - 1);
                }
                else
                {
                    viewModel.MoveLayer(movedLayer, destinationIndexPath.Row);
                }

                tableView.ReloadData();
            }

            #endregion
        }
    }
}