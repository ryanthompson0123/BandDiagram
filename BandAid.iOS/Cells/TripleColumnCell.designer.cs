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
	[Register ("TripleColumnCell")]
	partial class TripleColumnCell
	{
		[Outlet]
		UIKit.UILabel Column1Label { get; set; }

		[Outlet]
		UIKit.UILabel Column2Label { get; set; }

		[Outlet]
		UIKit.UILabel Column3Label { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (Column1Label != null) {
				Column1Label.Dispose ();
				Column1Label = null;
			}

			if (Column2Label != null) {
				Column2Label.Dispose ();
				Column2Label = null;
			}

			if (Column3Label != null) {
				Column3Label.Dispose ();
				Column3Label = null;
			}
		}
	}
}
