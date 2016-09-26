using System;

using Foundation;
using UIKit;

namespace BandAid.iOS
{
    public partial class TripleColumnCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("TripleColumnCell");

        public string TitleText
        {
            set { TitleLabel.Text = value; }
        }

        public string Column1Text
        {
            set { Column1Label.Text = value; }
        }

        public string Column2Text
        {
            set { Column2Label.Text = value; }
        }

        public string Column3Text
        {
            set { Column3Label.Text = value; }
        }

        public TripleColumnCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
