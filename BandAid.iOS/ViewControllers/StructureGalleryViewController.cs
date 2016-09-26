using System;
using System.Linq;

using Foundation;
using UIKit;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Band;
using Band.Units;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BandAid.iOS
{
    public partial class StructureGalleryViewController : UICollectionViewController
    {
        public UIBarButtonItem[] NormalLeftItems
        {
            get { return new [] { addButton }; }
        }

        public UIBarButtonItem[] NormalRightItems
        {
            get { return new [] { editButton }; }
        }

        public UIBarButtonItem[] EditingLeftItems
        {
            get { return new [] { duplicateButton, trashButton }; }
        }

        public UIBarButtonItem[] EditingRightItems
        {
            get { return new [] { doneButton }; }
        }

        public static UIColor HighlightColor = new UIColor(
            0.0f, 122.0f / 255.0f, 255.0f / 255.0f, 1.0f);

        private UIColor OriginalBarTintColor;
        private UIColor OriginalTintColor;

        public StructureGalleryViewModel ViewModel { get; set; }

        public StructureGalleryViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.LeftBarButtonItems = NormalLeftItems;
            NavigationItem.RightBarButtonItems = NormalRightItems;

            ViewModel = new StructureGalleryViewModel();
            CollectionView.Source = new StructureSource(this);
            CollectionView.ContentInset = new UIEdgeInsets(20f, 20f, 20f, 20f);
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

        void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Items":
                    CollectionView.ReloadData();
                    break;
                case "TitleText":
                    Title = ViewModel.TitleText;
                    break;
            }
        }

        public async override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var dest = (UINavigationController)segue.DestinationViewController;
            var tbController = (TestBenchViewController)dest.ViewControllers[0];

            if (segue.Identifier == "newTestBench")
            {
                tbController.ViewModel = new TestBenchViewModel(await TestBench.CreateDefaultAsync());
            }

            if (segue.Identifier == "editTestBench")
            {
                var name = ViewModel.Items[CollectionView.GetIndexPathsForSelectedItems()[0].Row].TitleText;
                var testBench = await TestBench.CreateAsync(name);
                tbController.ViewModel = new TestBenchViewModel(testBench);
            }
        }

        [Action("UnwindToGallery:")]
        public void UnwindToGallery(UIStoryboardSegue segue)
        {
            ViewModel.LoadItems();
            CollectionView.ReloadData();
        }

        partial void OnEditClicked(NSObject sender)
        {
            ViewModel.IsEditing = true;

            SaveOriginalColors();

            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            NavigationController.NavigationBar.BarTintColor = HighlightColor;
            NavigationController.NavigationBar.TintColor = UIColor.White;

            NavigationItem.LeftBarButtonItems = EditingLeftItems;
            NavigationItem.RightBarButtonItems = EditingRightItems;

            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;

            CollectionView.AllowsMultipleSelection = true;
        }

        async partial void OnDuplicateClicked(NSObject sender)
        {
            var selectedCount = ViewModel.Items.Count(i => i.IsSelected);

            if (selectedCount == 1)
            {
                DuplicateSelectedItems();
                return;
            }

            var duplicateMessage = string.Format("Duplicate {0} Structures", selectedCount);

            var alert = UIAlertController.Create(null, null, 
                UIAlertControllerStyle.ActionSheet);

            alert.AddAction(UIAlertAction.Create(duplicateMessage, 
                UIAlertActionStyle.Default, action => DuplicateSelectedItems()));

            if (alert.PopoverPresentationController != null)
            {
                alert.PopoverPresentationController.BarButtonItem = duplicateButton;
            }

            await PresentViewControllerAsync(alert, true);
        }

        async partial void OnTrashClicked(NSObject sender)
        {
            var selectedCount = ViewModel.Items.Count(i => i.IsSelected);

            var deleteMessage = selectedCount == 1 ? "Delete Structure" 
                : string.Format("Delete {0} Structures", selectedCount);

            var alert = UIAlertController.Create(null, null, 
                UIAlertControllerStyle.ActionSheet);

            alert.AddAction(UIAlertAction.Create(deleteMessage, 
                UIAlertActionStyle.Destructive, action => DeleteSelectedItems()));

            if (alert.PopoverPresentationController != null)
            {
                alert.PopoverPresentationController.BarButtonItem = trashButton;
            }

            await PresentViewControllerAsync(alert, true);
        }

        partial void OnDoneClicked(NSObject sender)
        {
            ViewModel.IsEditing = false;

            NavigationController.NavigationBar.BarTintColor = OriginalBarTintColor;
            NavigationController.NavigationBar.TintColor = OriginalTintColor;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.Black
            };

            NavigationItem.LeftBarButtonItems = NormalLeftItems;
            NavigationItem.RightBarButtonItems = NormalRightItems;

            NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;

            DeselectAllItems();

            CollectionView.AllowsMultipleSelection = false;
        }

        private async void DeleteSelectedItems()
        {
            var selectedIndeces = CollectionView.GetIndexPathsForSelectedItems();

            await ViewModel.DeleteSelectedItemsAsync();
            DeselectAllItems();
            CollectionView.DeleteItems(selectedIndeces);
        }

        private async void DuplicateSelectedItems()
        {
            var insertIndeces = CollectionView.GetIndexPathsForSelectedItems()
                .OrderBy(i => i.Row).ToArray();
            for (var i = 0; i < insertIndeces.Length; i++)
            {
                var nextIndex = insertIndeces[i];
                insertIndeces[i] = NSIndexPath.FromRowSection(nextIndex.Row + i + 1, nextIndex.Section);
            }

            await ViewModel.DuplicateSelectedItemsAsync();
            DeselectAllItems();
            CollectionView.InsertItems(insertIndeces);
        }

        private void DeselectAllItems()
        {
            foreach (var item in ViewModel.Items)
            {
                item.IsSelected = false;
            }

            foreach (var item in CollectionView.GetIndexPathsForSelectedItems())
            {
                CollectionView.DeselectItem(item, true);
            }
        }

        private void SaveOriginalColors()
        {
            OriginalBarTintColor = NavigationController.NavigationBar.BarTintColor;
            OriginalTintColor = NavigationController.NavigationBar.TintColor;
        }

        class StructureSource : UICollectionViewSource
        {
            private readonly StructureGalleryViewController viewController;
            private readonly StructureGalleryViewModel viewModel;

            public StructureSource(StructureGalleryViewController viewController)
            {
                this.viewController = viewController;
                this.viewModel = viewController.ViewModel;
            }

            public override nint GetItemsCount(UICollectionView collectionView, nint section)
            {
                return viewModel.Items.Count;
            }

            public override nint NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, 
                NSIndexPath indexPath)
            {
                var cell = (StructureCollectionViewCell)collectionView.DequeueReusableCell(
                               StructureCollectionViewCell.Key, indexPath);

                var item = viewModel.Items[indexPath.Row];

                cell.ViewModel = item;

                return cell;
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                if (!viewModel.IsEditing)
                {
                    viewController.PerformSegue("editTestBench", viewController);
                    return;
                }

                var item = viewModel.Items[indexPath.Row];

                item.IsSelected = true;
            }

            public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                var item = viewModel.Items[indexPath.Row];

                item.IsSelected = false;
            }
        }
    }
}