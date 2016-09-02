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
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.SetToolbarHidden(true, true);
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

        //public void OnRowSelected(int row)
        //{
        //    var material = ViewModel.Materials[row].Material;

        //    if (material.MaterialType == MaterialType.Semiconductor)
        //    {
        //        OnSemiconductorSelected((Semiconductor)material);
        //    }
        //    else
        //    {
        //        OnMaterialSelected(material);
        //    }
        //}

        //private async void OnSemiconductorSelected(Semiconductor semiconductor)
        //{
        //    var alert = UIAlertController.Create("Select Doping Type", "", UIAlertControllerStyle.Alert);
        //    alert.AddAction(UIAlertAction.Create("N Type", UIAlertActionStyle.Default, action =>
        //    {
        //        ViewModel.SelectedMaterial = semiconductor.DeepClone(DopingType.N);
        //        PerformSegue("unwindToStructure", this);
        //    }));
        //    alert.AddAction(UIAlertAction.Create("P Type", UIAlertActionStyle.Default, action =>
        //    {
        //        ViewModel.SelectedMaterial = semiconductor.DeepClone(DopingType.P);
        //        PerformSegue("unwindToStructure", this);
        //    }));

        //    await PresentViewControllerAsync(alert, true);
        //}

        //private async void OnMaterialSelected(Material material)
        //{
        //    var alert = UIAlertController.Create("Enter Thickness (nm)", "", UIAlertControllerStyle.Alert);
        //    alert.AddTextField(textField =>
        //    {
        //        textField.KeyboardType = UIKeyboardType.DecimalPad;
        //    });

        //    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, action =>
        //    {
        //        double thickness;
        //        if (!double.TryParse(alert.TextFields[0].Text, out thickness))
        //        {
        //            OnMaterialSelected(material);
        //            return;
        //        }

        //        ViewModel.SelectedMaterial = material.WithThickness(Length.FromNanometers(thickness));
        //        PerformSegue("unwindToStructure", this);
        //    }));

        //    await PresentViewControllerAsync(alert, true);
        //}

        //private static string DocumentsPath
        //{
        //    get
        //    {
        //        return NSFileManager.DefaultManager.GetUrls(
        //            NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
        //    }
        //}

        //private static string MetalsPath
        //{
        //    get { return Path.Combine(DocumentsPath, "metals.json"); }
        //}

        //private static string DielectricsPath
        //{
        //    get { return Path.Combine(DocumentsPath, "dielectrics.json"); }
        //}

        //private static string SemiconductorsPath
        //{
        //    get { return Path.Combine(DocumentsPath, "semiconductors.json"); }
        //}

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
				var cell = (MaterialCell)tableView.DequeueReusableCell(MaterialCell.Key);

                var materialVm = viewModel.Materials[indexPath.Row];

                cell.TitleLabel.Text = materialVm.TitleText;
                cell.LeftColumnLabel.Text = materialVm.LeftText;
                cell.RightColumnLabel.Text = materialVm.RightText;

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