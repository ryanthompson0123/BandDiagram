
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BandAid.iOS
{
    public partial class StructureCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("structureGridItem");

        public UIImage Image
        {
            get { return imageView.Image; }
            set { imageView.Image = value; }
        }

        public string Title
        {
            get { return titleLabel.Text; }
            set { titleLabel.Text = value; }
        }

        public StructureCollectionViewCell(IntPtr handle)
            : base(handle)
        {
        }
    }
}