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
	[Register ("ColorPickerHueGridViewController")]
	partial class ColorPickerHueGridViewController
	{
		[Outlet]
		UIKit.UIImageView colorBar { get; set; }

		[Outlet]
		UIKit.UICollectionView colorCollection { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (colorBar != null) {
				colorBar.Dispose ();
				colorBar = null;
			}

			if (colorCollection != null) {
				colorCollection.Dispose ();
				colorCollection = null;
			}
		}
	}
}
