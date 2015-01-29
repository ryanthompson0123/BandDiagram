
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Band;

namespace BandAid.iOS
{
    public partial class LayersTableViewController : UITableViewController
    {
        public StructureViewModel Structure { get; set; }

        public LayersTableViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            NavigationItem.RightBarButtonItems = RightBarButtonItems;
            TableView.Source = new LayersTableSource(this);
            TableView.SetEditing(true, false);

            PreferredContentSize = new SizeF(360, 540);
        }

        private UIBarButtonItem[] RightBarButtonItems
        {
            get
            {
                return new []
                {
                    NavigationItem.RightBarButtonItem,
                    new UIBarButtonItem
                    {
                        Title = "Duplicate",
                        Image = UIImage.FromBundle("copy")
                    }
                };
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            NavigationItem.RightBarButtonItems[1].Clicked += OnDuplicateTapped;
        }

        private void OnDuplicateTapped(object sender, EventArgs e)
        {
            var indexPath = TableView.IndexPathForSelectedRow;
            if (indexPath == null) return;

            var tappedLayer = Structure.ReferenceStructure.Layers[indexPath.Row];
            var duplicateLayer = tappedLayer.DeepClone();
            Structure.ReferenceStructure.InsertLayer(indexPath.Row + 1, duplicateLayer);
            TableView.InsertRows(new []
            { 
                NSIndexPath.FromRowSection(indexPath.Row + 1, indexPath.Section) 
            }, UITableViewRowAnimation.Automatic);
            TableView.DeselectRow(indexPath, true);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "AddSegue")
            {
                var destination = (MaterialTypeViewController)((UINavigationController)
                    segue.DestinationViewController).ViewControllers[0];
                destination.LayersController = this;
            }
        }

        partial void OnTrashTapped(UIBarButtonItem sender)
        {
            var indexPath = TableView.IndexPathForSelectedRow;
            if (indexPath == null) return;
            var removedLayer = Structure.ReferenceStructure.Layers[indexPath.Row];
            Structure.ReferenceStructure.RemoveLayer(removedLayer);
            TableView.DeleteRows(new [] { indexPath }, UITableViewRowAnimation.Automatic);
        }

        public void OnLayerAdded(Material layer)
        {
            Structure.ReferenceStructure.AddLayer(layer);
            TableView.InsertRows(new []
            {
                NSIndexPath.FromRowSection(0, 0)
            }, UITableViewRowAnimation.Automatic);
        }

        class LayersTableSource : UITableViewSource
        {
            readonly LayersTableViewController vc;
            public LayersTableSource(LayersTableViewController vc)
            {
                this.vc = vc;
            }

            #region implemented abstract members of UITableViewSource
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell("LayerCell");

                var material = vc.Structure.ReferenceStructure.Layers[indexPath.Row];
                cell.TextLabel.Text = material.Name;
                cell.DetailTextLabel.Text = material.GetType().Name;

                return cell;
            }
            public override int RowsInSection(UITableView tableview, int section)
            {
                return vc.Structure.ReferenceStructure.Layers.Count;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.None;
            }

            public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
            {
                return false;
            }

            public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }

            public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
            {
                var refStruct = vc.Structure.ReferenceStructure;
                var movedLayer = refStruct.Layers[sourceIndexPath.Row];

                refStruct.MoveLayer(movedLayer, destinationIndexPath.Row);
            }
            #endregion
        }
    }
}