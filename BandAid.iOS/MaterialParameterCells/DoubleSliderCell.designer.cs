// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BandAid.iOS
{
	[Register ("DoubleSliderCell")]
	partial class DoubleSliderCell
	{
		[Outlet]
		UIKit.UILabel titleLabel { get; set; }

		[Outlet]
		UIKit.UITextField valueField { get; set; }

		[Outlet]
		UIKit.UISlider valueSlider { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}

			if (valueField != null) {
				valueField.Dispose ();
				valueField = null;
			}

			if (valueSlider != null) {
				valueSlider.Dispose ();
				valueSlider = null;
			}
		}
	}
}
