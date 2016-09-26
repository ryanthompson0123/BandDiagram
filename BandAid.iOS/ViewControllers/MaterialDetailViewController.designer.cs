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
	[Register ("MaterialDetailViewController")]
	partial class MaterialDetailViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem TrashButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem UpdateButton { get; set; }

		[Action ("OnCopyClicked:")]
		partial void OnCopyClicked (Foundation.NSObject sender);

		[Action ("OnSaveClicked:")]
		partial void OnSaveClicked (Foundation.NSObject sender);

		[Action ("OnTrashClicked:")]
		partial void OnTrashClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (UpdateButton != null) {
				UpdateButton.Dispose ();
				UpdateButton = null;
			}

			if (TrashButton != null) {
				TrashButton.Dispose ();
				TrashButton = null;
			}
		}
	}
}
