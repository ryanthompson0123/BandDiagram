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
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextField maxVoltageField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField minVoltageField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField stepSizeField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (minVoltageField != null) {
				minVoltageField.Dispose ();
				minVoltageField = null;
			}

			if (maxVoltageField != null) {
				maxVoltageField.Dispose ();
				maxVoltageField = null;
			}

			if (stepSizeField != null) {
				stepSizeField.Dispose ();
				stepSizeField = null;
			}
		}
	}
}
