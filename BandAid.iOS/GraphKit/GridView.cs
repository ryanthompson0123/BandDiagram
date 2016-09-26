using System;
using UIKit;
using Band;
using CoreGraphics;

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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Layer.MasksToBounds = true;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            DrawXGridLines();
            DrawYGridLines();
        }

        private CGPoint baseAnchor = new CGPoint(0, 0);
        private CGPoint drawAnchor = new CGPoint(0, 0);

        private nfloat baseScale = 1.0f;
        private nfloat drawScale = 1.0f;

        public void ZoomBy(nfloat newScale, CGPoint anchor)
        {
            drawScale = baseScale;
            drawAnchor = baseAnchor;

            var anchorPoint = new CGPoint(ValueForXCoord(anchor.X), ValueForYCoord(anchor.Y));

            drawScale = baseScale * newScale;
            if (drawScale < 1.0f)
            {
                drawScale = 1.0f;
            }

            var newAnchor = new CGPoint(GetXCoord(anchorPoint.X), GetYCoord(anchorPoint.Y));

            drawAnchor = new CGPoint(newAnchor.X - anchor.X + drawAnchor.X, newAnchor.Y - anchor.Y + drawAnchor.Y);

            ConstrainDrawAnchor();
            UpdateGrid();
        }

        public void ZoomTo(nfloat newScale, CGPoint anchor)
        {
            drawScale = baseScale;
            drawAnchor = baseAnchor;

            var anchorPoint = new CGPoint(ValueForXCoord(anchor.X), ValueForYCoord(anchor.Y));

            drawScale = baseScale * newScale;
            if (drawScale < 1.0f)
            {
                drawScale = 1.0f;
            }

            var newAnchor = new CGPoint(GetXCoord(anchorPoint.X), GetYCoord(anchorPoint.Y));

            drawAnchor = new CGPoint(newAnchor.X - anchor.X + drawAnchor.X, newAnchor.Y - anchor.Y + drawAnchor.Y);
            ConstrainDrawAnchor();

            baseAnchor = drawAnchor;
            baseScale = drawScale;
            UpdateGrid();
        }

        public void PanBy(CGPoint translation)
        {
            drawAnchor = baseAnchor.Subtract(translation);

            ConstrainDrawAnchor();
            UpdateGrid();
        }

        public void PanTo(CGPoint translation)
        {
            drawAnchor = baseAnchor.Subtract(translation);
            ConstrainDrawAnchor();

            baseAnchor = drawAnchor;
            UpdateGrid();
        }

        private void ConstrainDrawAnchor()
        {
            var constrainedX = drawAnchor.X;
            var constrainedY = drawAnchor.Y;

            if (constrainedX < 0)
            {
                constrainedX = 0;
            }

            if (constrainedX > GetScaledMaxPanDeltaX(drawScale))
            {
                constrainedX = GetScaledMaxPanDeltaX(drawScale);
            }

            if (constrainedY < 0)
            {
                constrainedY = 0;
            }

            if (constrainedY > GetScaledMaxPanDeltaY(drawScale))
            {
                constrainedY = GetScaledMaxPanDeltaY(drawScale);
            }

            drawAnchor = new CGPoint(constrainedX, constrainedY);
        }

        private nfloat GetScaledMaxPanDeltaX(nfloat targetScale)
        {
            return Bounds.Width * targetScale - Bounds.Width;
        }

        private nfloat GetScaledMaxPanDeltaY(nfloat targetScale)
        {
            return Bounds.Height * targetScale - Bounds.Height;
        }

        private void UpdateGrid()
        {
            SetNeedsDisplay();
            SetNeedsLayout();
        }

        private void DrawXGridLines()
        {
            if (XAxis == null) return;

            var context = UIGraphics.GetCurrentContext();
            context.SetStrokeColor(new CGColor(235f / 255f, 1f));
            context.SetLineWidth(2.0f);
            context.SetLineDash(0, new nfloat[] { 4f, 4f });

            for (var i = 0; i < XAxis.MajorTickCount; i++)
            {
                var xCoord = GetXCoordForIndex(i);

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
            context.SetStrokeColor(new CGColor(235f / 255f, 1f));
            context.SetLineWidth(2.0f);
            context.SetLineDash(0, new nfloat[] { 4f, 4f });

            for (var i = 0; i < YAxis.MajorTickCount; i++)
            {
                var yCoord = GetYCoordForIndex(i);

                if (i == 0) yCoord -= 1;
                if (i == YAxis.MajorTickCount - 1) yCoord += 1;


                context.MoveTo(0, yCoord);
                context.AddLineToPoint(Bounds.Width, yCoord);
            }

            context.StrokePath();
        }

        private nfloat GetYCoordForIndex(int index)
        {
            var value = YAxis.GetTickValue(index);
            return GetYCoord(value);
        }

        private nfloat GetXCoordForIndex(int index)
        {
            var value = XAxis.GetTickValue(index);
            return GetXCoord(value);
        }

        private nfloat YRatio
        {
            get { return Bounds.Height * drawScale / (nfloat)YAxis.Range; }
        }

        private nfloat XRatio
        {
            get { return Bounds.Width * drawScale / (nfloat)XAxis.Range; }
        }

        private nfloat GetYCoord(double value)
        {
            return (nfloat)(YAxis.Max - value) * YRatio - drawAnchor.Y;
        }

        private nfloat GetXCoord(double value)
        {
            return (nfloat)(value - XAxis.Min) * XRatio - drawAnchor.X;
        }

        private double ValueForYCoord(nfloat yCoord)
        {
            return YAxis.Max - (yCoord + drawAnchor.Y) / YRatio;
        }

        private double ValueForXCoord(nfloat xCoord)
        {
            return XAxis.Min + (xCoord + drawAnchor.X) / XRatio;
        }
    }
}