using System;
using Band;
using Foundation;
using UIKit;

namespace BandAid.iOS
{
	public partial class ColorPickerColorCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("ColorPickerColorCell");

        public Color Color { get; set; }

		protected ColorPickerColorCell(IntPtr handle) 
			: base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public void Initialize()
		{
            BackgroundColor = Color.ToUIColor();
		}

		public override void ApplyLayoutAttributes(UICollectionViewLayoutAttributes layoutAttributes)
		{
			base.ApplyLayoutAttributes(layoutAttributes);

			Layer.CornerRadius = Frame.Width / 2;
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			BackgroundColor = UIColor.Clear;
		}
	}
}
