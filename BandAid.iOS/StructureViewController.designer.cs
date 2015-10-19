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
	[Register ("StructureViewController")]
	partial class StructureViewController
	{
		[Outlet]
		UIKit.UISlider biasSlider { get; set; }

		[Outlet]
		UIKit.UILabel currentVoltageLabel { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem settingsButton { get; set; }

		[Action ("OnStructuresClicked:")]
		partial void OnStructuresClicked (Foundation.NSObject sender);

		[Action ("OnToggleClicked:")]
		partial void OnToggleClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (currentVoltageLabel != null) {
				currentVoltageLabel.Dispose ();
				currentVoltageLabel = null;
			}

			if (biasSlider != null) {
				biasSlider.Dispose ();
				biasSlider = null;
			}

			if (settingsButton != null) {
				settingsButton.Dispose ();
				settingsButton = null;
			}
		}
	}
}
