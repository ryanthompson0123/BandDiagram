using System;
using Band;

using UIKit;

namespace BandAid.iOS
{
    public partial class StructurePointDetailViewController : UIViewController
    {
        public StructurePointDetailViewModel ViewModel { get; set; }

        public StructurePointDetailViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = ViewModel.TitleText;
            LocationLabel.Text = ViewModel.LocationText;
            EFieldLabel.Text = ViewModel.EFieldText;
            PotentialLabel.Text = ViewModel.PotentialText;
        }
    }
}
