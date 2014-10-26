// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BandAid.iOS
{
	[Register ("StructureViewController")]
	partial class StructureViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem SettingsButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (SettingsButton != null) {
				SettingsButton.Dispose ();
				SettingsButton = null;
			}
		}
	}
}
