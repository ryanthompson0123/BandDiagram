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
	[Register ("QuadColumnTableHeaderView")]
	partial class QuadColumnTableHeaderView
	{
		[Outlet]
		UIKit.UIButton Column1Button { get; set; }

		[Outlet]
		UIKit.UIButton Column2Button { get; set; }

		[Outlet]
		UIKit.UIButton Column3Button { get; set; }

		[Outlet]
		UIKit.UIButton Column4Button { get; set; }

		[Outlet]
		UIKit.UIButton TitleButton { get; set; }

		[Action ("Column1Clicked:")]
		partial void Column1Clicked (Foundation.NSObject sender);

		[Action ("Column2Clicked:")]
		partial void Column2Clicked (Foundation.NSObject sender);

		[Action ("Column3Clicked:")]
		partial void Column3Clicked (Foundation.NSObject sender);

		[Action ("Column4Clicked:")]
		partial void Column4Clicked (Foundation.NSObject sender);

		[Action ("TitleClicked:")]
		partial void TitleClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Column1Button != null) {
				Column1Button.Dispose ();
				Column1Button = null;
			}

			if (Column2Button != null) {
				Column2Button.Dispose ();
				Column2Button = null;
			}

			if (Column3Button != null) {
				Column3Button.Dispose ();
				Column3Button = null;
			}

			if (Column4Button != null) {
				Column4Button.Dispose ();
				Column4Button = null;
			}

			if (TitleButton != null) {
				TitleButton.Dispose ();
				TitleButton = null;
			}
		}
	}
}
