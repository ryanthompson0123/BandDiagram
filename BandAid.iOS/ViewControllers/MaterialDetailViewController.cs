using System;
using Band;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Linq;

namespace BandAid.iOS
{
	public partial class MaterialDetailViewController : UITableViewController
	{
		public MaterialDetailViewModel ViewModel { get; set; }

		public MaterialDetailViewController(IntPtr handle) 
			: base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			TableView.Source = new DetailSource(this);

            if (ViewModel.EditMode == EditMode.InStructure)
            {
                Title = ViewModel.Material.Name;
                NavigationItem.LeftBarButtonItem = null;
                NavigationItem.RightBarButtonItem = null;
                NavigationController.SetToolbarHidden(false, true);
                var newItems = ToolbarItems.ToList();
                newItems.Remove(UpdateButton);
                ToolbarItems = newItems.ToArray();
            }
            else if (ViewModel.EditMode == EditMode.Existing)
            {
                Title = ViewModel.Material.Name;
                NavigationItem.LeftBarButtonItem = null;
                NavigationItem.RightBarButtonItem = new UIBarButtonItem("Add", UIBarButtonItemStyle.Done, (sender, e) => OnAddClicked());
                NavigationController.SetToolbarHidden(false, true);
            }
            else
            {
                Title = string.Format("New {0}", ViewModel.Material.MaterialType);
                var newItems = ToolbarItems.ToList();
                newItems.Remove(UpdateButton);
                ToolbarItems = newItems.ToArray();
            }
		}

        private void OnAddClicked()
        {
            PerformSegue("AddLayerSegue", this);
        }

        partial void OnSaveClicked(NSObject sender)
        {
            PerformSegue("SaveFormSegue", this);
        }

        partial void OnTrashClicked(NSObject sender)
        {
            if (ViewModel.EditMode == EditMode.Existing)
            {
                ConfirmDelete();
                return;
            }

            PerformSegue("TrashSegue", this);
        }

        public void ConfirmDelete()
        {
            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);

            var name = string.Format("Delete {0}", ViewModel.Material.Name);
            var action = UIAlertAction.Create(name, UIAlertActionStyle.Destructive, (obj) => PerformSegue("TrashSegue", this));

            alert.AddAction(action);

            var popPresenter = alert.PopoverPresentationController;

            popPresenter.BarButtonItem = TrashButton;
            popPresenter.SourceView = View;

            PresentViewController(alert, true, null);
        }

		[Action("UnwindFromColorPicker:")]
		public void UnwindFromColorPicker(UIStoryboardSegue segue)
		{
			var colorPickerController = (ColorPickerHueGridViewController)segue.SourceViewController;

			var selectedIndex = TableView.IndexPathForSelectedRow;
			var selectedParameter = ViewModel.MaterialParameterSections[selectedIndex.Section][selectedIndex.Row]
											 as MaterialParameterViewModel<Color>;

            selectedParameter.Value = colorPickerController.ViewModel.SelectedColor;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "ColorPickerSegue")
			{
                var selectedIndex = TableView.IndexPathForSelectedRow;
                var selectedParameter = ViewModel.MaterialParameterSections[selectedIndex.Section][selectedIndex.Row]
                                                 as MaterialParameterViewModel<Color>;
                
				var dest = (ColorPickerHueGridViewController)segue.DestinationViewController;
                dest.ViewModel = new ColorPickerViewModel(selectedParameter.Value);
			}
		}

		class DetailSource : UITableViewSource
		{
			private readonly MaterialDetailViewModel viewModel;
			private readonly MaterialDetailViewController viewController;

			public DetailSource(MaterialDetailViewController viewController)
			{
				this.viewController = viewController;
				viewModel = viewController.ViewModel;
			}

			private MaterialParameterViewModel GetParameter(NSIndexPath indexPath)
			{
				return viewModel.MaterialParameterSections[indexPath.Section][indexPath.Row];
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				var parameter = GetParameter(indexPath);

				switch (parameter.ParameterType)
				{
					case ParameterType.Notes:
						return 160.0f;
					default:
						return 66.0f;
				}
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var parameter = GetParameter(indexPath);

				switch (parameter.ParameterType)
				{
					case ParameterType.Name:
                    case ParameterType.IntrinsicCarrierConcentration:
                    case ParameterType.DopantConcentration:
                    case ParameterType.SemiconductorBandGap:
						var textInputCell = (TextInputCell)tableView.DequeueReusableCell(TextInputCell.Key);
						textInputCell.ViewModel = (MaterialParameterViewModel<string>)parameter;
						textInputCell.Initialize();
					return textInputCell;
					case ParameterType.BandGap:
					case ParameterType.Thickness:
					case ParameterType.DielectricConstant:
					case ParameterType.ElectronAffinity:
					case ParameterType.WorkFunction:
					case ParameterType.Temperature:
						var doubleSliderCell = (DoubleSliderCell)tableView.DequeueReusableCell(DoubleSliderCell.Key);
						doubleSliderCell.ViewModel = (NumericMaterialParameterViewModel)parameter;
						doubleSliderCell.Initialize();
						return doubleSliderCell;
					case ParameterType.Notes:
						var textAreaCell = (TextAreaCell)tableView.DequeueReusableCell(TextAreaCell.Key);
						textAreaCell.ViewModel = (MaterialParameterViewModel<string>)parameter;
						textAreaCell.Initialize();
						return textAreaCell;
					case ParameterType.PlotColor:
						var colorPickerCell = (ColorPickerCell)tableView.DequeueReusableCell(ColorPickerCell.Key);
						colorPickerCell.ViewModel = (MaterialParameterViewModel<Color>)parameter;
						colorPickerCell.Initialize();
						return colorPickerCell;
					case ParameterType.DopingType:
						var multiButtonCell = (MultiButtonCell)tableView.DequeueReusableCell(MultiButtonCell.Key);
						multiButtonCell.ViewModel = (MaterialParameterViewModel<DopingType>)parameter;
						multiButtonCell.Initialize();
						return multiButtonCell;
					default:
						return tableView.DequeueReusableCell(TextInputCell.Key);
				}
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return viewModel.MaterialParameterSections[(int)section].Count;
			}
    
			public override nint NumberOfSections(UITableView tableView)
			{
				return viewModel.MaterialParameterSections.Count;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var parameter = GetParameter(indexPath);

				if (parameter.ParameterType == ParameterType.PlotColor)
				{
					viewController.PerformSegue("ColorPickerSegue", viewController);
					return;
				}

				var cell = (BaseParameterCell)tableView.CellAt(indexPath);
				cell.OnSelected();
			}
		}
	}
}