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
	[Register ("StructurePointDetailViewController")]
	partial class StructurePointDetailViewController
	{
		[Outlet]
		UIKit.UILabel EFieldLabel { get; set; }

		[Outlet]
		UIKit.UILabel LocationLabel { get; set; }

		[Outlet]
		UIKit.UILabel PotentialLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (LocationLabel != null) {
				LocationLabel.Dispose ();
				LocationLabel = null;
			}

			if (EFieldLabel != null) {
				EFieldLabel.Dispose ();
				EFieldLabel = null;
			}

			if (PotentialLabel != null) {
				PotentialLabel.Dispose ();
				PotentialLabel = null;
			}
		}
	}
}
