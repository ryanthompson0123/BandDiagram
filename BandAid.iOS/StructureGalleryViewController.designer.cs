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
	[Register ("StructureGalleryViewController")]
	partial class StructureGalleryViewController
	{
		[Outlet]
		UIKit.UIBarButtonItem addButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem doneButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem duplicateButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem editButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem trashButton { get; set; }

		[Action ("AddTouched:")]
		partial void AddTouched (Foundation.NSObject sender);

		[Action ("OnDoneClicked:")]
		partial void OnDoneClicked (Foundation.NSObject sender);

		[Action ("OnDuplicateClicked:")]
		partial void OnDuplicateClicked (Foundation.NSObject sender);

		[Action ("OnEditClicked:")]
		partial void OnEditClicked (Foundation.NSObject sender);

		[Action ("OnEditTouched:")]
		partial void OnEditTouched (Foundation.NSObject sender);

		[Action ("OnTrashClicked:")]
		partial void OnTrashClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (addButton != null) {
				addButton.Dispose ();
				addButton = null;
			}

			if (duplicateButton != null) {
				duplicateButton.Dispose ();
				duplicateButton = null;
			}

			if (trashButton != null) {
				trashButton.Dispose ();
				trashButton = null;
			}

			if (editButton != null) {
				editButton.Dispose ();
				editButton = null;
			}

			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}
		}
	}
}
