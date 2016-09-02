using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Band
{
    [JsonConverter(typeof(Converter))]
    public struct Color
    {
        private long value;
        internal long Value
        {
            get { return value; }
            set { this.value = value; }
        }
        
        public byte A
        {
            get { return (byte)(Value >> 24); }
        }

        public byte R
        {
            get { return (byte)(Value >> 16); }
        }

        public byte G
        {
            get { return (byte)(Value >> 8); }
        }

        public byte B
        {
            get { return (byte)Value; }
        }

        // Ported from http://www.docjar.com/html/api/java/awt/Color.java.html
        public float GetBrightness()
        {
            var maxval = Math.Max(R, Math.Max(G, B));

            return maxval / 255.0f;
        }

        // Ported from http://www.docjar.com/html/api/java/awt/Color.java.html
        public float GetSaturation()
        {
            var minval = Math.Min(R, Math.Min(G, B));
            var maxval = Math.Max(R, Math.Max(G, B));

            if (maxval != 0)
            {
                return (maxval - minval) / maxval;
            }

            return 0.0f;
        }

        // Ported from http://www.docjar.com/html/api/java/awt/Color.java.html
        public float GetHue()
        {
            if (GetSaturation() <= float.Epsilon)
            {
                return 0.0f;
            }

            var minval = Math.Min(R, Math.Min(G, B));
            var maxval = Math.Max(R, Math.Max(G, B));
            float hue;

            float redc = (maxval - R) / (maxval - minval);
            float greenc = (maxval - G) / (maxval - minval);
            float bluec = (maxval - B) / (maxval - minval);

            if (R == maxval)
            {
                hue = bluec - greenc;
            }
            else if (G == maxval)
            {
                hue = 2.0f + redc - bluec;
            }
            else
            {
                hue = 4.0f + greenc - redc;
            }

            hue = hue / 6.0f;

            if (hue < 0)
            {
                hue = hue + 1.0f;
            }

            return hue;
        }

        public static Color FromHexString(string hexValue)
        {
            if (string.IsNullOrEmpty(hexValue)) return FromArgb(0, 0, 0, 0);
            var colorString = hexValue.Replace("#", "");
            int red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16);
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16);
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16);
                        return FromArgb(255, red, green, blue);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16);
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16);
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16);
                        return FromArgb(255, red, green, blue);
                    }
                case 8: // #AARRGGBB
                    {
                        var alpha = Convert.ToInt32(colorString.Substring(0, 2), 16);
                        red = Convert.ToInt32(colorString.Substring(2, 2), 16);
                        green = Convert.ToInt32(colorString.Substring(4, 2), 16);
                        blue = Convert.ToInt32(colorString.Substring(6, 2), 16);
                        return FromArgb(alpha, red, green, blue);
                    }
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB, or #AARRGGBB", hexValue));

            }
        }

        public string ToHexString()
        {
            if (A == 255)
            {
                return string.Format("#{0:X2}{1:X2}{2:X2}", R, G, B);
            }
            else
            {
                return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", A, R, G, B);
            }
        }

        public static Color FromHsb(float h, float s, float b)
        {
            var H = h * 360;

            var c = b * s;

            var hPrime = H / 60;
            var x = c * (1.0f - Math.Abs((hPrime % 2) - 1));

            float r1, g1, b1;

            if (0 <= hPrime && hPrime < 1.0f)
            {
                r1 = c;
                g1 = x;
                b1 = 0;
            }
            else if (1.0f <= hPrime && hPrime < 2.0f)
            {
                r1 = x;
                g1 = c;
                b1 = 0;
            }
            else if (2.0f <= hPrime && hPrime < 3.0f)
            {
                r1 = 0;
                g1 = c;
                b1 = x;
            }
            else if (3.0f <= hPrime && hPrime < 4.0f)
            {
                r1 = 0;
                g1 = x;
                b1 = c;
            }
            else if (4.0f <= hPrime && hPrime < 5.0f)
            {
                r1 = x;
                g1 = 0;
                b1 = c;
            }
            else
            {
                r1 = c;
                g1 = 0;
                b1 = x;
            }


            var m = b - c;

            return FromArgb(
                255,
                (int)((r1 + m) * 255),
                (int)((g1 + m) * 255),
                (int)((b1 + m) * 255)
            );
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckARGBValues(alpha, red, green, blue);
            var color = new Color();
            color.Value = (int)((uint)alpha << 24) + (red << 16) + (green << 8) + blue;
            return color;
        }

        private static void CheckARGBValues(int alpha, int red, int green, int blue)
        {
            if ((alpha > 255) || (alpha < 0))
                throw CreateColorArgumentException(alpha, "alpha");
            CheckRGBValues(red, green, blue);
        }

        private static void CheckRGBValues(int red, int green, int blue)
        {
            if ((red > 255) || (red < 0))
                throw CreateColorArgumentException(red, "red");
            if ((green > 255) || (green < 0))
                throw CreateColorArgumentException(green, "green");
            if ((blue > 255) || (blue < 0))
                throw CreateColorArgumentException(blue, "blue");
        }

        private static ArgumentException CreateColorArgumentException(int value, string color)
        {
            return new ArgumentException(string.Format("'{0}' is not a valid"
              + " value for '{1}'. '{1}' should be greater or equal to 0 and"
              + " less than or equal to 255.", value, color));
        }

        public static Color Black
        {
            get { return FromArgb(255, 0, 0, 0); }
        }

        public static Color Clear
        {
            get { return FromArgb(0, 0, 0, 0); }
        }

        public class Converter : ExtendedJsonConverter<Color>
        {
            protected override Color Deserialize(Type objectType, JToken jToken)
            {
                if (jToken == null || jToken.Type == JTokenType.Null)
                {
                    return Clear;
                }

                return FromHexString(jToken.ToObject<string>());
            }

            protected override JToken Serialize(Color value)
            {
                return JToken.FromObject(value.ToHexString());
            }
        }
    }
}