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
	[Register ("MaterialSelectViewController")]
	partial class MaterialSelectViewController
	{
		[Outlet]
		BandAid.iOS.QuadColumnTableHeaderView HeaderView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeaderView != null) {
				HeaderView.Dispose ();
				HeaderView = null;
			}
		}
	}
}
