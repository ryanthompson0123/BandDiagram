using System;

using Foundation;
using UIKit;

namespace BandAid.iOS
{
    public partial class MathExpressionCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("MathExpressionCell");
        public static readonly UINib Nib;

        static MathExpressionCell()
        {
            Nib = UINib.FromName("MathExpressionCell", NSBundle.MainBundle);
        }

        protected MathExpressionCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
