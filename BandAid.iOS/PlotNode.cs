using System;
using System.Collections.Generic;
using MonoTouch.SpriteKit;
using Band;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

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

        private List<SKShapeNode> DataSetNodes { get; set; }

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

            if (DataSetNodes != null)
            {
                RemoveChildren(DataSetNodes.ToArray());
            }

            DataSetNodes = new List<SKShapeNode>();

            var plot = PlotGrouping.Plots[step];

            foreach (var dataSet in plot.DataSets)
            {
                var node = CreateDataSetNode(dataSet);
                node.Position = new PointF(0, 0);
                AddChild(node);

                DataSetNodes.Add(node);
            }
        }

        private SKShapeNode CreateDataSetNode(PlotDataSet dataSet)
        {
            var line = new SKShapeNode();
            var pathToDraw = new CGPath();
            var startCoord = GetCoord(dataSet.DataPoints[0]);
            pathToDraw.MoveToPoint(startCoord);

            for (var i = 1; i < dataSet.DataPoints.Count; i++)
            {
                var coord = GetCoord(dataSet.DataPoints[i]);
                pathToDraw.AddLineToPoint(coord);
            }

            line.Path = pathToDraw;
            line.StrokeColor = UIColor.Red;
            line.LineWidth = 2.0f;
            return line;
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
                Y = (float)(dataPoint.Y - YAxisBounds.Min) * YRatio
            };
        }
    }
}