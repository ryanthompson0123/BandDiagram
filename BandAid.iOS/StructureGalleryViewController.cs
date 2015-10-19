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

namespace BandAid.iOS
{
    public partial class StructureGalleryViewController : UICollectionViewController
    {
        public StructureGalleryViewModel ViewModel { get; set; }

        public StructureGalleryViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new StructureGalleryViewModel();
            CollectionView.Source = new StructureSource(ViewModel);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Items":
                    CollectionView.ReloadData();
                    break;
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
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
        }

        class StructureSource : UICollectionViewSource
        {
            private readonly StructureGalleryViewModel viewModel;

            public StructureSource(StructureGalleryViewModel viewModel)
            {
                this.viewModel = viewModel;
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

                cell.Title = item.TitleText;
                cell.Image = GetImage(item.ImageFile);

                return cell;
            }

            private UIImage GetImage(string path)
            {
                var imageData = NSData.FromFile(path);

                if (imageData == null) return null;

                return new UIImage(imageData);
            }
        }
    }
}