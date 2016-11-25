using System;
using CoreGraphics;
using Band;
using UIKit;
using System.Collections.Generic;

namespace BandAid.iOS
{
    public sealed partial class AxisView : UIView
    {
        private List<UILabel> tickLabels;

        private readonly UILabel titleLabel;

        private PlotAxis axisValue;

        public PlotAxis Axis
        {
            get { return axisValue; }
            set
            {
                axisValue = value;
                titleLabel.Text = value.Title;

                ResetScaleAndOffset();
                SetUpCoordinateDelegates();

                BuildTickLabels();
                UpdateAxis();
            }
        }

        public AxisView(IntPtr handle)
            : base(handle)
        {
            titleLabel = new UILabel(new CGRect());
            titleLabel.Font = UIFont.BoldSystemFontOfSize(20f);
            titleLabel.TextColor = UIColor.Black;
            AddSubview(titleLabel);
        }

        private nfloat baseOffset = 0.0f;
        private nfloat drawOffset = 0.0f;

        private nfloat baseScale = 1.0f;
        private nfloat drawScale = 1.0f;

        private Func<nfloat, double> valueForCoordDelegate;
        private Func<double, nfloat> getCoordDelegate;
        private Func<nfloat, nfloat> getScaledMaxPanDelegate;

        private void ResetScaleAndOffset()
        {
            baseOffset = 0.0f;
            drawOffset = 0.0f;

            baseScale = 1.0f;
            drawScale = 1.0f;
        }

        private void SetUpCoordinateDelegates()
        {
            switch (Axis.AxisType)
            {
                case AxisType.PrimaryY:
                    valueForCoordDelegate = ValueForYCoord;
                    getCoordDelegate = GetYCoord;
                    getScaledMaxPanDelegate = GetScaledMaxPanDeltaY;
                    break;
                case AxisType.X:
                    valueForCoordDelegate = ValueForXCoord;
                    getCoordDelegate = GetXCoord;
                    getScaledMaxPanDelegate = GetScaledMaxPanDeltaX;
                    break;
                    
            }
        }

        public void ZoomBy(nfloat newScale, nfloat anchor)
        {
            drawScale = baseScale;
            drawOffset = baseOffset;

            var anchorValue = valueForCoordDelegate(anchor);
            Console.WriteLine(anchorValue);

            drawScale = baseScale * newScale;
            if (drawScale < 1.0f)
            {
                drawScale = 1.0f;
            }

            var newAnchor = getCoordDelegate(anchorValue);


            drawOffset = newAnchor - anchor + drawOffset;

            ConstrainDrawOffset();
            UpdateAxis();
        }

        public void ZoomTo(nfloat newScale, nfloat anchor)
        {
            drawScale = baseScale;
            drawOffset = baseOffset;

            var anchorValue = valueForCoordDelegate(anchor);
            Console.WriteLine(anchorValue);

            drawScale = baseScale * newScale;
            if (drawScale < 1.0f)
            {
                drawScale = 1.0f;
            }

            var newAnchor = getCoordDelegate(anchorValue);

            drawOffset = newAnchor - anchor + drawOffset;
            ConstrainDrawOffset();

            baseOffset = drawOffset;
            baseScale = drawScale;
            UpdateAxis();
        }

        public void PanBy(nfloat amount)
        {
            drawOffset = baseOffset - amount;

            ConstrainDrawOffset();
            UpdateAxis();
        }

        public void PanTo(nfloat amount)
        {
            drawOffset = baseOffset - amount;
            ConstrainDrawOffset();

            baseOffset = drawOffset;
            UpdateAxis();
        }

        private void ConstrainDrawOffset()
        {
            if (drawOffset < 0)
            {
                drawOffset = 0;
            }

            if (drawOffset > getScaledMaxPanDelegate(drawScale))
            {
                drawOffset = getScaledMaxPanDelegate(drawScale);
            }
        }

        private nfloat GetScaledMaxPanDeltaX(nfloat targetScale)
        {
            return Bounds.Width * targetScale - Bounds.Width;
        }

        private nfloat GetScaledMaxPanDeltaY(nfloat targetScale)
        {
            return Bounds.Height * targetScale - Bounds.Height;
        }

        private void UpdateAxis()
        {
            SetNeedsDisplay();
            SetNeedsLayout();
        }

        public override void Draw(CGRect rect)
        {
            if (Axis == null) return;

            switch (Axis.AxisType)
            {
                case AxisType.PrimaryY:
                    DrawLeftAxis(rect);
                    break;
                case AxisType.X:
                    DrawBottomAxis(rect);
                    break;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Axis == null) return;

            switch (Axis.AxisType)
            {
                case AxisType.PrimaryY:
                    LayoutVerticalTickLabels();
                    break;
                case AxisType.X:
                    LayoutHorizontalTickLabels();
                    break;
            }

            LayoutTitleLabel();
        }

        private void BuildTickLabels()
        {
            if (tickLabels != null)
            {
                foreach (var label in tickLabels)
                {
                    label.RemoveFromSuperview();
                    label.Dispose();
                }
            }

            tickLabels = new List<UILabel>();

            foreach (var tick in Axis.TickLabels)
            {
                var tickLabel = new UILabel(new CGRect());
                tickLabel.Text = tick;
                tickLabel.TextColor = UIColor.Black;
                tickLabel.Font = UIFont.SystemFontOfSize(16f);

                tickLabels.Add(tickLabel);
                AddSubview(tickLabel);
            }
        }

        private void LayoutVerticalTickLabels()
        {
            for (var i = 0; i < tickLabels.Count; i++)
            {
                var tickLabel = tickLabels[i];
                var yCoord = GetYCoordForIndex(i);

                tickLabel.Hidden = yCoord <= 0.0 || yCoord >= Bounds.Height + 1.0f;
                tickLabel.SizeToFit();
                tickLabel.Center = new CGPoint(72f, yCoord);
            }
        }

        private void LayoutHorizontalTickLabels()
        {
            for (var i = 0; i < tickLabels.Count; i++)
            {
                var tickLabel = tickLabels[i];
                var xCoord = GetXCoordForIndex(i);

                tickLabel.Hidden = xCoord <= -1.0f || xCoord >= Bounds.Width;
                tickLabel.SizeToFit();
                tickLabel.Center = new CGPoint(xCoord, Bounds.Height - 52f);
            }
        }

        private void LayoutTitleLabel()
        {
            titleLabel.SizeToFit();

            switch (Axis.AxisType)
            {
                case AxisType.PrimaryY:
                    titleLabel.Transform = CGAffineTransform.MakeRotation(-(nfloat)Math.PI / 2f);
                    titleLabel.Center = new CGPoint(Bounds.Width / 4, Bounds.Height / 2);
                    break;
                case AxisType.SecondaryY:
                    titleLabel.Transform = CGAffineTransform.MakeRotation(-(nfloat)Math.PI / 2f);
                    titleLabel.Center = new CGPoint(3 * Bounds.Width / 4, Bounds.Height / 2);
                    break;
                case AxisType.X:
                    titleLabel.Center = new CGPoint(Bounds.Width / 2, 3 * Bounds.Height / 4);
                    break;
            }
        }

        private nfloat GetYCoordForIndex(int index)
        {
            var value = Axis.GetTickValue(index);
            return GetYCoord(value);
        }

        private nfloat GetXCoordForIndex(int index)
        {
            var value = Axis.GetTickValue(index);
            return GetXCoord(value);
        }

        private void DrawLeftAxis(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();
            context.SetStrokeColor(UIColor.Black.CGColor);
            context.SetLineWidth(2.0f);

            context.MoveTo(Bounds.Width - 16f, Bounds.Height);
            context.AddLineToPoint(Bounds.Width - 16f, 0);

            for (var i = 0; i < tickLabels.Count; i++)
            {
                var yCoord = GetYCoordForIndex(i);

                if (i == 0) yCoord -= 1;
                if (i == tickLabels.Count - 1) yCoord += 1;

                if (yCoord >= 0.0f && yCoord <= Bounds.Height)
                {
                    context.MoveTo(Bounds.Width - 28f, yCoord);
                    context.AddLineToPoint(Bounds.Width - 16f, yCoord);
                }
            }

            context.StrokePath();
        }

        private void DrawBottomAxis(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();
            context.SetStrokeColor(UIColor.Black.CGColor);
            context.SetLineWidth(2.0f);

            context.MoveTo(Bounds.Width, 16f);
            context.AddLineToPoint(0, 16f);

            for (var i = 0; i < tickLabels.Count; i++)
            {
                var xCoord = GetXCoordForIndex(i);

                if (i == 0) xCoord += 1;

                if (xCoord >= 0.0f && xCoord <= Bounds.Width)
                {
                    context.MoveTo(xCoord, 28f);
                    context.AddLineToPoint(xCoord, 16f);
                }
            }

            context.StrokePath();
        }

        private nfloat YRatio
        {
            get { return Bounds.Height * drawScale / (nfloat)Axis.Range; }
        }

        private nfloat XRatio
        {
            get { return Bounds.Width * drawScale / (nfloat)Axis.Range; }
        }

        private nfloat GetYCoord(double value)
        {
            return (nfloat)(Axis.Max - value) * YRatio - drawOffset;
        }

        private nfloat GetXCoord(double value)
        {
            return (nfloat)(value - Axis.Min) * XRatio - drawOffset;
        }

        private double ValueForYCoord(nfloat yCoord)
        {
            return Axis.Max - (yCoord + drawOffset) / YRatio;
        }

        private double ValueForXCoord(nfloat xCoord)
        {
            return Axis.Min + (xCoord + drawOffset) / XRatio;
        }
    }
}