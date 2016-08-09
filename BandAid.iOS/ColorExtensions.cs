using System;
using Band;
using UIKit;

namespace BandAid.iOS
{
    public static class ColorExtensions
    {
        public static UIColor ToUIColor(this Color color)
        {
            return UIColor.FromRGBA(color.R, color.G, color.B, color.A);
        }
    }
}

