using System;
using SpriteKit;
using UIKit;

namespace BandAid.iOS
{
    public partial class PlotView : SKView
    {
        public UIActivityIndicatorView ActivityIndicator
        {
            get { return activityIndicator; }
        }

        public PlotView(IntPtr handle)
            : base(handle)
        {
        }
    }
}