using System;
using UIKit;

namespace BandAid.iOS
{
    static class CustomUIColor
    {
        public static UIColor FromHexString(string hexValue)
        {
            if (String.IsNullOrEmpty(hexValue)) return UIColor.Black;
            var colorString = hexValue.Replace("#", "");
            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIKit.UIColor.FromRGB(red, green, blue);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIKit.UIColor.FromRGB(red, green, blue);
                    }
                case 8: // #AARRGGBB
                    {
                        var alpha = Convert.ToInt32(colorString.Substring(0, 2), 16)/255f;
                        red = Convert.ToInt32(colorString.Substring(2, 2), 16)/255f;
                        green = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(6, 2), 16) / 255f;
                        return UIKit.UIColor.FromRGBA(red, green, blue, alpha);
                    }
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB, or #AARRGGBB", hexValue));

            }
        }

		public static string ToHexString(this UIColor color)
		{
			var r = (int)(255.0 * color.CGColor.Components[0]);
			var g = (int)(255.0 * color.CGColor.Components[1]);
			var b = (int)(255.0 * color.CGColor.Components[2]);
			var a = (int)(255.0 * color.CGColor.Alpha);

			return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", a, r, g, b);
		}
    }
}