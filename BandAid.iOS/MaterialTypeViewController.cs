using System;
using CoreGraphics;

using Foundation;
using UIKit;
using Band;

namespace BandAid.iOS
{
    public partial class MaterialTypeViewController : UITableViewController
    {
        public MaterialTypeViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PreferredContentSize = new CGSize(360, 540);
        }

        async partial void OnCancelTouched(NSObject sender)
        {
            await PresentingViewController.DismissViewControllerAsync(true);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            var destination = (MaterialSelectViewController)segue.DestinationViewController;

            MaterialType materialType = default(MaterialType);
            switch (segue.Identifier)
            {
                case "MetalSegue":
                    materialType = MaterialType.Metal;
                    break;
                case "DielectricSegue":
                    materialType = MaterialType.Dielectric;
                    break;
                case "SemiconductorSegue":
                    materialType = MaterialType.Semiconductor;
                    break;
            }

            destination.ViewModel = new MaterialSelectViewModel(materialType);
        }
    }
}

