using System;
using CoreGraphics;
using UIKit;

namespace BandAid.iOS
{
    // Adapted from http://stackoverflow.com/a/20091771/1351938
    partial class TableViewCellDividerView : UIView
    {
        public TableViewCellDividerView(IntPtr handle)
            : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Constraints.Length == 0)
            {
                var width = Frame.Size.Width;
                var height = Frame.Size.Height;

                if (width == 1)
                {
                    width = width / UIScreen.MainScreen.Scale;
                }

                if (height == 0)
                {
                    height = 1 / UIScreen.MainScreen.Scale;
                }

                if (height == 1)
                {
                    height = height / UIScreen.MainScreen.Scale;
                }

                Frame = new CGRect(Frame.X, Frame.Y, width, height);
            }
            else
            {
                foreach (var constraint in Constraints)
                {
                    if ((constraint.FirstAttribute == NSLayoutAttribute.Width
                         || constraint.FirstAttribute == NSLayoutAttribute.Height)
                        && constraint.Constant == 1)
                    {
                        constraint.Constant /= UIScreen.MainScreen.Scale;
                    }
                }
            }
        }
    }
}
