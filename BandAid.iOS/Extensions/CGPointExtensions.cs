using System;
using CoreGraphics;

namespace BandAid.iOS
{
    public static class CGPointExtensions
    {
        public static CGPoint Add(this CGPoint point, CGPoint other)
        {
            return new CGPoint(point.X + other.X, point.Y + other.Y);
        }

        public static CGPoint Subtract(this CGPoint point, CGPoint other)
        {
            return new CGPoint(point.X - other.X, point.Y - other.Y);
        }

        public static CGPoint Divide(this CGPoint point, nfloat scale)
        {
            return new CGPoint(point.X / scale, point.Y / scale);
        }

        public static CGPoint Multiply(this CGPoint point, nfloat scale)
        {
            return new CGPoint(point.X * scale, point.Y * scale);
        }
    }
}
