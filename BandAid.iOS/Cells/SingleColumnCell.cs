using System;

using Foundation;
using UIKit;

namespace BandAid.iOS
{
    public partial class SingleColumnCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("SingleColumnCell");

        public string TitleText
        {
            set { TitleLabel.Text = value; }
        }

        public string Column1Text
        {
            set { ColumnLabel.Text = value; }
        }

        public SingleColumnCell(IntPtr handle) 
            : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
