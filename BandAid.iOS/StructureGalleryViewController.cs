
using System;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Band;
using Band.Units;
using Newtonsoft.Json;

namespace BandAid.iOS
{
    public partial class StructureGalleryViewController : UICollectionViewController
    {
        public List<string> Structures { get; set; }

        public StructureGalleryViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Structures = new List<string>();
            CollectionView.Source = new StructureSource(this);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            LoadItems();
            CollectionView.ReloadData();
        }

        private void LoadItems()
        {
            if (!Directory.Exists(StructuresPath))
            {
                Directory.CreateDirectory(StructuresPath);
            }
                
            Structures = Directory.EnumerateFiles(StructuresPath)
                .Where(f => f.Contains("json"))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }

        private TestBenchViewModel targetStructure;

        private void LoadStructure(string name)
        {
            var structurePath = Path.Combine(StructuresPath, name + ".json");
            var data = File.ReadAllText(structurePath);
            var dataObj = JObject.Parse(data);

            var minV = (double)dataObj["minVoltage"];
            var maxV = (double)dataObj["maxVoltage"];
            var step = (double)dataObj["stepSize"];
            var currentStep = (double)dataObj["currentStep"];
            var refStruct = JsonConvert.DeserializeObject<Structure>(
                dataObj["referenceStructure"].ToString(), new StructureConverter());

            targetStructure = new TestBenchViewModel(currentStep, minV, maxV,
                step, PlotType.Energy, refStruct, name);
            PerformSegue("showStructure", this);
        }

        partial void AddTouched(NSObject sender)
        {
            PerformSegue("showStructure", this);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (targetStructure != null)
            {
                var dest = (UINavigationController)segue.DestinationViewController;
                var structCtl = (StructureViewController)dest.ViewControllers[0];

                structCtl.Structure = targetStructure;
                targetStructure = null;
            }
        }

        class StructureSource : UICollectionViewSource
        {
            private readonly StructureGalleryViewController viewController;
            public StructureSource(StructureGalleryViewController viewController)
            {
                this.viewController = viewController;
            }

            public override int GetItemsCount(UICollectionView collectionView, int section)
            {
                return viewController.Structures.Count;
            }

            public override int NumberOfSections(UICollectionView collectionView)
            {
                return 1;
            }

            public override UICollectionViewCell GetCell(UICollectionView collectionView, 
                NSIndexPath indexPath)
            {
                var cell = (StructureCollectionViewCell)collectionView.DequeueReusableCell(
                               StructureCollectionViewCell.Key, indexPath);

                cell.Title = viewController.Structures[indexPath.Row];
                cell.Image = GetImage(indexPath);

                return cell;
            }

            public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
            {
                viewController.LoadStructure(viewController.Structures[indexPath.Row]);
            }

            private UIImage GetImage(NSIndexPath indexPath)
            {
                var structureName = viewController.Structures[indexPath.Row];

                var documents = NSFileManager.DefaultManager.GetUrls(
                    NSSearchPathDirectory.DocumentDirectory, 
                    NSSearchPathDomain.User)[0].Path;
                var saveDirPath = Path.Combine(documents, "save");
                var imagePath = Path.Combine(saveDirPath, structureName + ".png");

                Console.WriteLine(imagePath);
                var imageData = NSData.FromFile(imagePath);
                return new UIImage(imageData);
            }
        }
    }
}