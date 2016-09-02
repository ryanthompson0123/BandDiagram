using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Band;

using Foundation;
using UIKit;

namespace BandAid.iOS
{
    public partial class ColorPickerHueGridViewController : UIViewController
    {
		private UITapGestureRecognizer colorBarTapRecognizer;

		public ColorPickerViewModel ViewModel { get; set; }

		public ColorPickerHueGridViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			colorCollection.Source = new ColorPickerSource(this);
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;

			colorBarTapRecognizer = new UITapGestureRecognizer(ColorBarTapped);
			colorBar.AddGestureRecognizer(colorBarTapRecognizer);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

			colorBar.RemoveGestureRecognizer(colorBarTapRecognizer);
		}

		void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "CurrentPalette":
					colorCollection.ReloadData();
					break;
			}
		}

		public void ColorBarTapped(UITapGestureRecognizer recognizer)
        {
			var point = recognizer.LocationInView(colorBar);
			var paletteSize = colorBar.Frame.Width / 12.0f;

			var paletteIndex = (int)(point.X / paletteSize);

			ViewModel.CurrentPalette = ViewModel.Palettes[paletteIndex];
        }

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "UnwindColorPickerSegue")
			{
				var selectedIndex = colorCollection.GetIndexPathsForSelectedItems()[0];
				ViewModel.SelectedColor = ViewModel.CurrentPalette.Colors[selectedIndex.Row];
			}
		}

		class ColorPickerSource : UICollectionViewSource
		{
			private ColorPickerHueGridViewController viewController;
			private ColorPickerViewModel viewModel;

			public ColorPickerSource(ColorPickerHueGridViewController viewController)
			{
				this.viewController = viewController;
				viewModel = viewController.ViewModel;
			}

			public override nint GetItemsCount(UICollectionView collectionView, nint section)
			{
				return viewModel.CurrentPalette.Colors.Count;
			}

			public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
			{
				var cell = collectionView.DequeueReusableCell(ColorPickerColorCell.Key, indexPath)
										 as ColorPickerColorCell;

				cell.Color = viewModel.CurrentPalette.Colors[indexPath.Row];
				cell.Initialize();
				return cell;
			}
		}
    }
}

