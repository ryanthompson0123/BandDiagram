using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using Band;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace BandAid.iOS
{
    public enum Axis { Right, Left, Bottom }
    
    public sealed class AxisNode : SKNode
    {
        private readonly PlotAnimationGrouping plotGrouping;
        private readonly SizeF size;

        public AxisNode(PlotAnimationGrouping plotGrouping, Axis axis, SizeF size)
        {
            if (plotGrouping == null || plotGrouping.Plots.Count == 0) return;

            this.size = size;
            this.plotGrouping = plotGrouping;

            switch (axis)
            {
                case Axis.Left:
                    DrawLeftAxis();
                    break;
                case Axis.Bottom:
                    DrawBottomAxis();
                    break;
                default:
                    break;
            }
        }

        private void DrawLeftAxis()
        {
            var axis = new SKShapeNode();
            var pathToDraw = new CGPath();
            pathToDraw.MoveToPoint(new PointF(size.Width - 8f, size.Height));
            pathToDraw.AddLineToPoint(new PointF(size.Width - 8f, 0));

            for (var i = plotGrouping.YAxisBounds.Max; i >= plotGrouping.YAxisBounds.Min;
                i = i - plotGrouping.MajorYAxisSpan)
            {
                var yCoord = GetYCoord(i);
                pathToDraw.MoveToPoint(new PointF(size.Width - 12f, yCoord));
                pathToDraw.AddLineToPoint(new PointF(size.Width - 8f, yCoord));

                var labelNode = new SKLabelNode();
                labelNode.Text = String.Format("{0:0.0}", i);
                labelNode.Position = new PointF(size.Width - 32f, yCoord - 8f);
                labelNode.FontColor = UIColor.Black;
                labelNode.FontSize = 16f;
                AddChild(labelNode);
            }

            axis.Path = pathToDraw;
            axis.StrokeColor = UIColor.Black;
            axis.LineWidth = 2.0f;
            axis.Position = new PointF(0, 0);
            AddChild(axis);
        }

        private void DrawBottomAxis()
        {
            var axis = new SKShapeNode();
            var pathToDraw = new CGPath();
            pathToDraw.MoveToPoint(new PointF(size.Width, size.Height - 8f));
            pathToDraw.AddLineToPoint(new PointF(0, size.Height - 8f));

            for (var i = plotGrouping.XAxisBounds.Max; i >= plotGrouping.XAxisBounds.Min;
                i = i - plotGrouping.MajorXAxisSpan)
            {
                var xCoord = GetXCoord(i);
                pathToDraw.MoveToPoint(new PointF(xCoord, size.Height - 12f));
                pathToDraw.AddLineToPoint(new PointF(xCoord, size.Height - 8f));

                var labelNode = new SKLabelNode();
                labelNode.Text = String.Format("{0:0.0}", i);
                labelNode.Position = new PointF(xCoord - 8f, size.Height - 32f);
                labelNode.FontColor = UIColor.Black;
                labelNode.FontSize = 16f;
                AddChild(labelNode);
            }

            axis.Path = pathToDraw;
            axis.StrokeColor = UIColor.Black;
            axis.LineWidth = 2.0f;
            axis.Position = new PointF(0, 0);
            AddChild(axis);
        }

        private float GetYCoord(double value)
        {
            var distanceDown = (plotGrouping.YAxisBounds.Max - value) 
                / (plotGrouping.YAxisBounds.Max - plotGrouping.YAxisBounds.Min);

            return size.Height - ((float)distanceDown * size.Height);
        }

        private float GetXCoord(double value)
        {
            var distanceOver = (plotGrouping.XAxisBounds.Max - value)
                / (plotGrouping.XAxisBounds.Max - plotGrouping.XAxisBounds.Min);

            return size.Width - ((float)distanceOver * size.Width);
        }
    }
}