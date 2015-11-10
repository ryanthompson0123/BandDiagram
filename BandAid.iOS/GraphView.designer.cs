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
	[Register ("GraphView")]
	partial class GraphView
	{
		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		UIKit.UISlider animationAxisSlider { get; set; }

		[Outlet]
		UIKit.UILabel animationAxisTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel animationAxisValueLabel { get; set; }

		[Outlet]
		BandAid.iOS.GridView gridView { get; set; }

		[Outlet]
		BandAid.iOS.PlotView plotView { get; set; }

		[Outlet]
		BandAid.iOS.AxisView primaryXAxisView { get; set; }

		[Outlet]
		BandAid.iOS.AxisView primaryYAxisView { get; set; }

		[Outlet]
		BandAid.iOS.AxisView secondaryYAxisView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (animationAxisSlider != null) {
				animationAxisSlider.Dispose ();
				animationAxisSlider = null;
			}

			if (animationAxisTitleLabel != null) {
				animationAxisTitleLabel.Dispose ();
				animationAxisTitleLabel = null;
			}

			if (animationAxisValueLabel != null) {
				animationAxisValueLabel.Dispose ();
				animationAxisValueLabel = null;
			}

			if (gridView != null) {
				gridView.Dispose ();
				gridView = null;
			}

			if (plotView != null) {
				plotView.Dispose ();
				plotView = null;
			}

			if (primaryXAxisView != null) {
				primaryXAxisView.Dispose ();
				primaryXAxisView = null;
			}

			if (primaryYAxisView != null) {
				primaryYAxisView.Dispose ();
				primaryYAxisView = null;
			}

			if (secondaryYAxisView != null) {
				secondaryYAxisView.Dispose ();
				secondaryYAxisView = null;
			}
		}
	}
}
