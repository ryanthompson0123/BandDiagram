using System;
using System.Collections.Generic;
using MonoTouch.SpriteKit;
using Band;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;

namespace BandAid.iOS
{
    public class PlotNode : SKNode
    {
        public PlotAnimationGrouping PlotGrouping { get; set; }

        public PlotAxisBounds XAxisBounds { get; set; }
        public PlotAxisBounds YAxisBounds { get; set; }

        public float XRatio { get; set; }
        public float YRatio { get; set; }

        public SizeF Size { get; set; }

        private SKSpriteNode DataSetNode { get; set; }

        public PlotNode(PlotAnimationGrouping plotGrouping, SizeF size)
        {
            if (plotGrouping == null || plotGrouping.Plots.Count == 0) return;

            Size = size;
            XAxisBounds = plotGrouping.XAxisBounds;
            YAxisBounds = plotGrouping.YAxisBounds;

            XRatio = size.Width / (float)(XAxisBounds.Max - XAxisBounds.Min);
            YRatio = size.Height / (float)(YAxisBounds.Max - YAxisBounds.Min);

            DrawBorder(size);

            PlotGrouping = plotGrouping;
        }

        public void PlotStep(int step)
        {
            if (PlotGrouping == null) return;

            if (DataSetNode != null)
            {
                DataSetNode.RemoveFromParent();
                DataSetNode = null;
            }

            var plot = PlotGrouping.Plots[step];

            UIGraphics.BeginImageContext(Size);
            foreach (var dataSet in plot.DataSets)
            {
                PlotDataSetNode(dataSet);
            }

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            DataSetNode = new SKSpriteNode(SKTexture.FromImage(image));
            DataSetNode.Position = new PointF(Size.Width/2, Size.Height/2);
            AddChild(DataSetNode);
        }

        private void PlotDataSetNode(PlotDataSet dataSet)
        {
            var layer = new CAShapeLayer();
            layer.Frame = new RectangleF(new PointF(0, 0), Size);

            var pathToDraw = new CGPath();
            var startCoord = GetCoord(dataSet.DataPoints[0]);
            pathToDraw.MoveToPoint(startCoord);

            for (var i = 1; i < dataSet.DataPoints.Count; i++)
            {
                var coord = GetCoord(dataSet.DataPoints[i]);
                pathToDraw.AddLineToPoint(coord);
            }
            layer.Path = pathToDraw;
            layer.StrokeColor = UIColor.Red.CGColor;
            layer.FillColor = UIColor.Clear.CGColor;
            layer.LineWidth = 2.0f;
            layer.BackgroundColor = UIColor.Clear.CGColor;

            layer.RenderInContext(UIGraphics.GetCurrentContext());
        }

        private void DrawBorder(SizeF size)
        {
            var border = new SKShapeNode();
            var pathToDraw = CGPath.FromRect(new RectangleF(new PointF(0, 0),
                new SizeF(size.Width, size.Height)));
            border.Path = pathToDraw;
            border.StrokeColor = UIColor.Black;
            border.LineWidth = 2.0f;
            border.Position = new PointF(0, 0);
            AddChild(border);
        }

        private PointF GetCoord(PlotDataPoint dataPoint)
        {
            return new PointF
            {
                X = (float)(dataPoint.X - XAxisBounds.Min) * XRatio,
                Y = (float)(YAxisBounds.Max - dataPoint.Y) * YRatio
            };
        }
    }
}