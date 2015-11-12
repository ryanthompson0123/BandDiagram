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
	[Register ("TestBenchViewController")]
	partial class TestBenchViewController
	{
		[Outlet]
		UIKit.UILabel cstackLabel { get; set; }

		[Outlet]
		UIKit.UILabel eotLabel { get; set; }

		[Outlet]
		BandAid.iOS.GraphView graphView { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem settingsButton { get; set; }

		[Outlet]
		UIKit.UILabel vfbLabel { get; set; }

		[Outlet]
		UIKit.UILabel vthLabel { get; set; }

		[Action ("OnPlayClicked:")]
		partial void OnPlayClicked (Foundation.NSObject sender);

		[Action ("OnSettingsClicked:")]
		partial void OnSettingsClicked (Foundation.NSObject sender);

		[Action ("OnToggleClicked:")]
		partial void OnToggleClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (graphView != null) {
				graphView.Dispose ();
				graphView = null;
			}

			if (settingsButton != null) {
				settingsButton.Dispose ();
				settingsButton = null;
			}

			if (vfbLabel != null) {
				vfbLabel.Dispose ();
				vfbLabel = null;
			}

			if (eotLabel != null) {
				eotLabel.Dispose ();
				eotLabel = null;
			}

			if (cstackLabel != null) {
				cstackLabel.Dispose ();
				cstackLabel = null;
			}

			if (vthLabel != null) {
				vthLabel.Dispose ();
				vthLabel = null;
			}
		}
	}
}
