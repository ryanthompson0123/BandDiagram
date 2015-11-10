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
                BuildTickLabels();
                SetNeedsDisplay();
                SetNeedsLayout();
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
                var yCoord = GetLabelYCoord(i);
                tickLabel.SizeToFit();
                tickLabel.Center = new CGPoint(72f, yCoord);
            }
        }

        private void LayoutHorizontalTickLabels()
        {
            for (var i = 0; i < tickLabels.Count; i++)
            {
                var tickLabel = tickLabels[i];
                var xCoord = GetLabelXCoord(i);
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

        private nfloat GetLabelXCoord(int index)
        {
            var xValue = index * Axis.MajorSpan;
            return GetXCoord(xValue / Axis.Range);
        }

        private nfloat GetLabelYCoord(int index)
        {
            var yValue = index * Axis.MajorSpan;
            return GetYCoord(yValue / Axis.Range);
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
                var yCoord = GetYCoord(i * Axis.MajorSpan / Axis.Range);

                if (i == 0) yCoord -= 1;
                if (i == tickLabels.Count - 1) yCoord += 1;

                context.MoveTo(Bounds.Width - 28f, yCoord);
                context.AddLineToPoint(Bounds.Width - 16f, yCoord);
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
                var xCoord = GetXCoord(i * Axis.MajorSpan / Axis.Range);

                if (i == 0) xCoord += 1;
                context.MoveTo(xCoord, 28f);
                context.AddLineToPoint(xCoord, 16f);
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