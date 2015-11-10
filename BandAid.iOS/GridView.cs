using System;
using UIKit;
using Band;

namespace BandAid.iOS
{
    public sealed partial class GridView : UIView
    {
        private PlotAxis xAxisValue;
        public PlotAxis XAxis
        {
            get { return xAxisValue; }
            set
            {
                xAxisValue = value;
                SetNeedsDisplay();
            }
        }

        private PlotAxis yAxisValue;
        public PlotAxis YAxis
        {
            get { return yAxisValue; }
            set
            {
                yAxisValue = value;
                SetNeedsDisplay();
                SetNeedsLayout();
            }
        }

        public GridView(IntPtr handle)
            : base(handle)
        {
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            DrawXGridLines();
            DrawYGridLines();
        }

        private void DrawXGridLines()
        {
            if (XAxis == null) return;

            var context = UIGraphics.GetCurrentContext();
            context.SetStrokeColor(new CoreGraphics.CGColor(235f / 255f, 1f));
            context.SetLineWidth(2.0f);
            context.SetLineDash(0, new [] { (nfloat)4f, (nfloat)4f });

            for (var i = 0; i < XAxis.MajorTickCount; i++)
            {
                var xCoord = GetXCoord(i * XAxis.MajorSpan / XAxis.Range);

                if (i == 0) xCoord -= 1;
                if (i == XAxis.MajorTickCount - 1) xCoord += 1;

                context.MoveTo(xCoord, 0);
                context.AddLineToPoint(xCoord, Bounds.Height);
            }

            context.StrokePath();
        }

        private void DrawYGridLines()
        {
            if (YAxis == null) return;

            var context = UIGraphics.GetCurrentContext();
            context.SetStrokeColor(new CoreGraphics.CGColor(235f / 255f, 1f));
            context.SetLineWidth(2.0f);
            context.SetLineDash(0, new [] { (nfloat)4f, (nfloat)4f });

            for (var i = 0; i < YAxis.MajorTickCount; i++)
            {
                var yCoord = GetYCoord(i * YAxis.MajorSpan / YAxis.Range);

                if (i == 0) yCoord -= 1;
                if (i == YAxis.MajorTickCount - 1) yCoord += 1;


                context.MoveTo(0, yCoord);
                context.AddLineToPoint(Bounds.Width, yCoord);
            }

            context.StrokePath();
        }

        private nfloat GetYCoord(double value)
        {
            return Bounds.Height - ((nfloat)value * Bounds.Height);
        }

        private nfloat GetXCoord(double value)
        {
            return (nfloat)value * Bounds.Width;
        }
    }
}