using System;
using CoreGraphics;

using Foundation;
using UIKit;
using Band;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Band.Units;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;

namespace BandAid.iOS
{
    public partial class MaterialSelectViewController : UITableViewController
    {
        public MaterialSelectViewModel ViewModel { get; set; }

        public MaterialSelectViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            TableView.Source = new MaterialSource(this);

            Title = string.Format("{0}s", ViewModel.MaterialType);

            SetUpColumnHeaders();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.SetToolbarHidden(true, true);
            ViewModel.Materials.CollectionChanged += Materials_CollectionChanged;
            HeaderView.TitleClick += HeaderView_TitleClick;
            HeaderView.ColumnClick += HeaderView_ColumnClick;
            HeaderView.ColumnLongPress += HeaderView_ColumnLongPress;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel.Materials.CollectionChanged -= Materials_CollectionChanged;
            HeaderView.TitleClick -= HeaderView_TitleClick;
            HeaderView.ColumnClick -= HeaderView_ColumnClick;
            HeaderView.ColumnLongPress -= HeaderView_ColumnLongPress;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void SetUpColumnHeaders()
        {
            HeaderView.TitleText = ViewModel.TableTitle;
            HeaderView.Headers = ViewModel.ColumnHeaders;
        }

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "SelectMaterialSegue")
			{
				var destination = (MaterialDetailViewController)segue.DestinationViewController;
				var selectedMaterial = ViewModel.Materials[TableView.IndexPathForSelectedRow.Row].Material;

                destination.NavigationItem.LeftBarButtonItem = null;
                destination.ViewModel = new MaterialDetailViewModel(selectedMaterial, EditMode.Existing);
			}

            if (segue.Identifier == "AddMaterialSegue")
            {
                var nav = (UINavigationController)segue.DestinationViewController;
                var destination = (MaterialDetailViewController)nav.ChildViewControllers[0];

                destination.ViewModel = new MaterialDetailViewModel(ViewModel.MaterialType, EditMode.New);
            }

            if (segue.Identifier == "HeaderHintPopoverSegue")
            {
                var viewController = (TableHeaderHintTextViewController)segue.DestinationViewController;
                var popover = viewController.PopoverPresentationController;
                popover.SourceView = HeaderView;
                popover.SourceRect = new CGRect(
                    longPressedButton.Frame.X + longPressedButton.Frame.Width / 2,
                    longPressedButton.Frame.Y + longPressedButton.Frame.Height / 4, 5f, 5f);

                viewController.HintText = ViewModel.HeaderHints[longPressedColumnIndex];
            }
		}

        [Action("UnwindFromMaterialForm:")]
        public void UnwindFromMaterialForm(UIStoryboardSegue segue)
        {
            var source = (MaterialDetailViewController)segue.SourceViewController;
            var material = source.ViewModel.Material;

            if (segue.Identifier == "SaveFormSegue")
            {
                ViewModel.SaveMaterial(material);
            }

            if (segue.Identifier == "TrashSegue")
            {
                ViewModel.DeleteMaterial(material);
            }

            if (segue.Identifier == "DuplicateSegue")
            {
                ViewModel.DuplicateMaterial(material);
            }
        }

        void Materials_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TableView.ReloadData();
        }

        void HeaderView_TitleClick(object sender, EventArgs e)
        {
            ViewModel.OnTitleClicked();
        }

        void HeaderView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ViewModel.OnColumnClicked(e.ClickedIndex);
        }

        private int longPressedColumnIndex;
        private UIButton longPressedButton;
        void HeaderView_ColumnLongPress(object sender, ColumnLongPressEventArgs e)
        {
            longPressedColumnIndex = e.Index;
            longPressedButton = e.Button;
            PerformSegue("HeaderHintPopoverSegue", this);
        }

        void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TableTitle":
                    HeaderView.TitleText = ViewModel.TableTitle;
                    break;
            }
        }

        class MaterialSource : UITableViewSource
        {
            private readonly MaterialSelectViewModel viewModel;
            private readonly MaterialSelectViewController viewController;

            public MaterialSource(MaterialSelectViewController viewController)
            {
                this.viewController = viewController;

                viewModel = viewController.ViewModel;
                viewModel.Materials.CollectionChanged += (sender, e) => viewController.TableView.ReloadData();
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                switch (viewModel.MaterialType)
                {
                    case MaterialType.Dielectric:
                        return 44.0f + (14.0f * 2.0f);
                    case MaterialType.Metal:
                        return 44.0f;
                    case MaterialType.Semiconductor:
                        return 44.0f + (14.0f * 4.0f);
                    default:
                        return 44.0f;
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                switch (viewModel.MaterialType)
                {
                    case MaterialType.Metal:
                        return GetSingleCell(tableView, indexPath);
                    case MaterialType.Dielectric:
                        return GetTripleCell(tableView, indexPath);
                    case MaterialType.Semiconductor:
                        return GetQuadCell(tableView, indexPath);
                    default:
                        return null;
                }
            }

            private UITableViewCell GetSingleCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = (SingleColumnCell)tableView.DequeueReusableCell(SingleColumnCell.Key);

                var materialVm = viewModel.Materials[indexPath.Row];
                cell.TitleText = materialVm.TitleText;
                cell.Column1Text = materialVm.Columns[0];

                return cell;
            }

            private UITableViewCell GetTripleCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = (TripleColumnCell)tableView.DequeueReusableCell(TripleColumnCell.Key);

                var materialVm = viewModel.Materials[indexPath.Row];
                cell.TitleText = materialVm.TitleText;
                cell.Column1Text = materialVm.Columns[0];
                cell.Column2Text = materialVm.Columns[1];
                cell.Column3Text = materialVm.Columns[2];

                return cell;
            }

            private UITableViewCell GetQuadCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = (QuadColumnCell)tableView.DequeueReusableCell(QuadColumnCell.Key);

                var materialVm = viewModel.Materials[indexPath.Row];
                cell.TitleText = materialVm.TitleText;
                cell.Column1Text = materialVm.Columns[0];
                cell.Column2Text = materialVm.Columns[1];
                cell.Column3Text = materialVm.Columns[2];
                cell.Column4Text = materialVm.Columns[3];

                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                if (viewModel.Materials == null) return 0;

                return viewModel.Materials.Count;
            }
        }
    }
}