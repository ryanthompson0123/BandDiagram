
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BandAid.iOS
{
    public partial class MaterialTypeViewController : UITableViewController
    {
        public LayersTableViewController LayersController { get; set; }

        public MaterialTypeViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PreferredContentSize = new SizeF(360, 540);
        }

        async partial void OnCancelTouched(NSObject sender)
        {
            await PresentingViewController.DismissViewControllerAsync(true);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            var destination = (MaterialSelectViewController)segue.DestinationViewController;
            destination.LayersController = LayersController;

            switch (segue.Identifier)
            {
                case "MetalSegue":
                    destination.MaterialType = Band.MaterialType.Metal;
                    break;
                case "DielectricSegue":
                    destination.MaterialType = Band.MaterialType.Dielectric;
                    break;
                case "SemiconductorSegue":
                    destination.MaterialType = Band.MaterialType.Semiconductor;
                    break;
            }
        }
    }
}

