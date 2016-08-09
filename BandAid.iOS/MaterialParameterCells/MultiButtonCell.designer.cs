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
	[Register ("MultiButtonCell")]
	partial class MultiButtonCell
	{
		[Outlet]
		UIKit.UILabel titleLabel { get; set; }

		[Outlet]
		UIKit.UISegmentedControl valueSegment { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}

			if (valueSegment != null) {
				valueSegment.Dispose ();
				valueSegment = null;
			}
		}
	}
}
