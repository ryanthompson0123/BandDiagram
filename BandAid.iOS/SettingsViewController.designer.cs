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
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		UIKit.UITextField maxVoltageField { get; set; }

		[Outlet]
		UIKit.UITextField minVoltageField { get; set; }

		[Outlet]
		UIKit.UITextField stepSizeField { get; set; }

		[Action ("OnMaxVoltageEditingChanged:")]
		partial void OnMaxVoltageEditingChanged (Foundation.NSObject sender);

		[Action ("OnMinVoltageEditingChanged:")]
		partial void OnMinVoltageEditingChanged (Foundation.NSObject sender);

		[Action ("OnStepSizeEditingChanged:")]
		partial void OnStepSizeEditingChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (maxVoltageField != null) {
				maxVoltageField.Dispose ();
				maxVoltageField = null;
			}

			if (minVoltageField != null) {
				minVoltageField.Dispose ();
				minVoltageField = null;
			}

			if (stepSizeField != null) {
				stepSizeField.Dispose ();
				stepSizeField = null;
			}
		}
	}
}
