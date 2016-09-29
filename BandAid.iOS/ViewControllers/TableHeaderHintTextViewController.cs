using System;

using UIKit;

namespace BandAid.iOS
{
    public partial class TableHeaderHintTextViewController : UIViewController
    {
        public string HintText
        {
            get; set;
        }

        public TableHeaderHintTextViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HintTextLabel.Text = HintText;
        }
    }
}

