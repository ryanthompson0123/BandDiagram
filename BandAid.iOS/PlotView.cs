using SpriteKit;
using Band;
using CoreGraphics;
using UIKit;
using CoreAnimation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace BandAid.iOS
{
    public sealed partial class PlotView : UIView
    {
        private PlotAnimationGrouping plotGroupValue;
        public PlotAnimationGrouping PlotGroup
        {
            get { return plotGroupValue; }
            set
            {
                plotGroupValue = value;
                CalculatePlotRect();
                CalculatePaths(value);
                RemoveSublayers();
                
                if (!laidOut)
                {
                    SetNeedsLayout();
                }
                else
                {
                    plotTransform = CalculateAffineTransform(plotRect, Bounds);
                    UpdatePlot();
                }
            }
        }

        private bool laidOut;

        private List<List<CGPath>> plotPaths;
        private CGAffineTransform plotTransform;
        private CGRect plotRect;

        private int selectedPlotIndex;

        public nfloat XRatio
        {
            get { return Bounds.Width / (nfloat)(PlotGroup.XAxis.Max - PlotGroup.XAxis.Min); }
        }

        public nfloat YRatio
        {
            get { return Bounds.Height / (nfloat)(PlotGroup.YAxis.Max - PlotGroup.YAxis.Min); }
        }

        public PlotView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Layer.BorderWidth = 2.0f;
            Layer.BorderColor = UIColor.Black.CGColor;
        }

        public override void LayoutSubviews()
        {
            // Now that we have our bounds, we can calculate the transform and draw the plot
            plotTransform = CalculateAffineTransform(plotRect, Bounds);
            UpdatePlot();
            laidOut = true;
        }

        public void SelectPlot(double value)
        {
            var nextIndex = PlotGroup.GetIndexOfClosestPlot(value);
            if (selectedPlotIndex != nextIndex)
            {
                selectedPlotIndex = nextIndex;
                UpdatePlot();
            }
        }

        private void RemoveSublayers()
        {
            foreach (var layer in Layer.Sublayers.Skip(1))
            {
                layer.RemoveFromSuperLayer();
            }
        }

        private void UpdatePlot()
        {
            if (PlotGroup == null || PlotGroup.Plots[0] == null 
                || PlotGroup.Plots[0].DataSets == null) return;

            if (Bounds.Width == 0)
            {
                LayoutIfNeeded();
            }
            else if (Layer.Sublayers == null || Layer.Sublayers.Length == 1)
            {
                AddSublayers();
            }
            else
            {
                ReplaceSublayers();
            }
        }

        private void AddSublayers()
        {
            var selectedDatasets = PlotGroup.Plots[selectedPlotIndex].DataSets;
            var selectedPaths = plotPaths[selectedPlotIndex];

            for (var i = 0; i < selectedDatasets.Count; i++)
            {
                var dataSet = selectedDatasets[i];
                var path = selectedPaths[i];

                var layer = new CAShapeLayer();

                layer.Frame = Bounds;
                layer.StrokeColor = dataSet.PlotColor.ToUIColor().CGColor;
                layer.FillColor = UIColor.Clear.CGColor;
                layer.LineWidth = (float)dataSet.LineThickness;
                layer.Path = new CGPath(path, plotTransform);

                Layer.AddSublayer(layer);
            }
        }

        private void ReplaceSublayers()
        {
            var selectedPaths = plotPaths[selectedPlotIndex];

            for (var i = 0; i < selectedPaths.Count; i++)
            {
                var path = selectedPaths[i];

                var layer = (CAShapeLayer)Layer.Sublayers[i + 1];

                layer.Path = new CGPath(path, plotTransform);
                //layer.DidChangeValue("path");
            }
        }

        private void CalculatePaths(PlotAnimationGrouping plotGroup)
        {
            var stopwatch = Stopwatch.StartNew();
            plotPaths = plotGroup.Plots.Select(p => p.DataSets.Select(d => CalculatePath(d)).ToList()).ToList();
            var time = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();

            Console.WriteLine("Time to draw paths: {0}ms", time);
        }

        private void CalculatePlotRect()
        {
            // Calculate the CGRect of the plot. We invert the yAxis, because in CGPath, 
            // y axis is positive in the down direction.
            plotRect = new CGRect(0, PlotGroup.YAxis.Max, 
                PlotGroup.XAxis.Max - PlotGroup.XAxis.Min, 
                -(PlotGroup.YAxis.Max - PlotGroup.YAxis.Min));
        }

        private static CGAffineTransform CalculateAffineTransform(CGRect sourceRect, CGRect targetRect)
        {
            var transform = CGAffineTransform.MakeTranslation(targetRect.X - sourceRect.X, 
                targetRect.Y - sourceRect.Y); 
            transform.Scale(targetRect.Width / sourceRect.Width, 
                targetRect.Height / sourceRect.Height);
            return transform;
        }

        private static CGPath CalculatePath(PlotDataSet dataSet)
        {
            if (dataSet == null || dataSet.DataPoints == null 
                || dataSet.DataPoints.Count == 0) return null;

            var firstPoint = dataSet.DataPoints[0];

            var path = new CGPath();
            path.MoveToPoint((nfloat)firstPoint.X, (nfloat)firstPoint.Y);

            foreach (var dataPoint in dataSet.DataPoints.Skip(1))
            {
                path.AddLineToPoint((nfloat)dataPoint.X, (nfloat)dataPoint.Y);
            }

            return path;
        }
    }
}