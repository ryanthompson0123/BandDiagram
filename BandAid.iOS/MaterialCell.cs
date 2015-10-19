
using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace BandAid.iOS
{
    public partial class MaterialCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("MaterialCell");

        public UILabel TitleLabel
        {
            get { return titleLabel; }
        }

        public UILabel LeftColumnLabel
        {
            get { return leftColumnLabel; }
        }

        public UILabel RightColumnLabel
        {
            get { return rightColumnLabel; }
        }

        public MaterialCell(IntPtr handle)
            : base(handle)
        {
        }
    }
}