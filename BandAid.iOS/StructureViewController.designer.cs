// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace BandAid.iOS
{
	[Register ("StructureViewController")]
	partial class StructureViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISlider biasSlider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel maxVoltageLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel minVoltageLabel { get; set; }

		[Outlet]
		MonoTouch.SpriteKit.SKView plotView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem SettingsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel zeroVoltageLabel { get; set; }

		[Action ("StructuresTouched:")]
		partial void StructuresTouched (MonoTouch.Foundation.NSObject sender);

		[Action ("ToggleTouched:")]
		partial void ToggleTouched (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (biasSlider != null) {
				biasSlider.Dispose ();
				biasSlider = null;
			}

			if (maxVoltageLabel != null) {
				maxVoltageLabel.Dispose ();
				maxVoltageLabel = null;
			}

			if (minVoltageLabel != null) {
				minVoltageLabel.Dispose ();
				minVoltageLabel = null;
			}

			if (plotView != null) {
				plotView.Dispose ();
				plotView = null;
			}

			if (SettingsButton != null) {
				SettingsButton.Dispose ();
				SettingsButton = null;
			}

			if (zeroVoltageLabel != null) {
				zeroVoltageLabel.Dispose ();
				zeroVoltageLabel = null;
			}
		}
	}
}
