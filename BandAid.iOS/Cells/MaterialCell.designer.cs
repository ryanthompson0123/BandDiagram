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
	[Register ("MaterialCell")]
	partial class MaterialCell
	{
		[Outlet]
		UIKit.UILabel leftColumnLabel { get; set; }

		[Outlet]
		UIKit.UILabel rightColumnLabel { get; set; }

		[Outlet]
		UIKit.UILabel titleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}

			if (leftColumnLabel != null) {
				leftColumnLabel.Dispose ();
				leftColumnLabel = null;
			}

			if (rightColumnLabel != null) {
				rightColumnLabel.Dispose ();
				rightColumnLabel = null;
			}
		}
	}
}
