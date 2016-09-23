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
                    ResetTransform();
                    UpdatePlot();
                }
            }
        }

        private bool laidOut;

        private List<List<CGPath>> plotPaths;
        private CGAffineTransform plotTransform;
        private CGAffineTransform baseTransform;
        private CGAffineTransform lastTransform;

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
            Layer.MasksToBounds = true;
        }

        public override void LayoutSubviews()
        {
            // Now that we have our bounds, we can calculate the transform and draw the plot
            ResetTransform();

            UpdatePlot();
            laidOut = true;
        }

        void ResetTransform()
        {
            baseTransform = CalculateAffineTransform(plotRect, Bounds);
            lastTransform = baseTransform;
            plotTransform = baseTransform;
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

        private nfloat cumulativeScale = 1.0f;

        public void ZoomBy(nfloat newScale, CGPoint newAnchor)
        {
            // Figure out what the final scale will be after this transform.
            var targetScale = cumulativeScale * newScale;

            // If it's less than 1, we don't allow that at this point, so we constrain it to 1.0.
            if (targetScale < 1.0f)
            {
                newScale = 1.0f / cumulativeScale;
            }

            // Copy the last transform, scale it, then translate it.
            var nextTransform = lastTransform;
            nextTransform.Scale(newScale, newScale);
            nextTransform.Translate(-(newAnchor.X * newScale - newAnchor.X), -(newAnchor.Y * newScale -  newAnchor.Y));

            // Now constrain the plot to the bounds of the chart using the final scale.
            var constrainScale = cumulativeScale * newScale;
            nextTransform = ConstrainTransformToBounds(nextTransform, constrainScale);

            // Update the plot transform
            plotTransform = nextTransform;
            UpdatePlot();
        }

        public void ZoomTo(nfloat newScale, CGPoint newAnchor)
        {
            // Figure out what the final scale will be after this transform.
            var targetScale = cumulativeScale * newScale;

            // If it's less than 1, we don't allow that at this point, so we constrain it to 1.0.
            if (targetScale < 1.0f)
            {
                newScale = 1.0f / cumulativeScale;
            }

            // Scale and translate the last transform.
            lastTransform.Scale(newScale, newScale);
            lastTransform.Translate(-GetZoomOffsetX(newScale, newAnchor), -GetZoomOffsetY(newScale, newAnchor));

            // Update the cumulative scale.
            cumulativeScale *= newScale;

            // Constrain the plot to the bounds of the chart using the new cumulative scale.
            lastTransform = ConstrainTransformToBounds(lastTransform, cumulativeScale);

            // Update the ploat transform.
            plotTransform = lastTransform;
            UpdatePlot();
        }

        private nfloat GetZoomOffsetX(nfloat newScale, CGPoint newAnchor)
        {
            return newAnchor.X * newScale - newAnchor.X;
        }

        private nfloat GetZoomOffsetY(nfloat newScale, CGPoint newAnchor)
        {
            return newAnchor.Y * newScale - newAnchor.Y;
        }

        public void PanBy(CGPoint translation)
        {
            // Copy the last transform and translate it by the given amount.
            var nextTransform = lastTransform;
            nextTransform.Translate(translation.X, translation.Y);

            // Constrain the transform to the bounds of the plot.
            nextTransform = ConstrainTransformToBounds(nextTransform, cumulativeScale);

            // Update the plot transform.
            plotTransform = nextTransform;
            UpdatePlot();
        }

        public void PanTo(CGPoint translation)
        {
            // Copy the last transform and translate it by the given amount.
            var nextTransform = lastTransform;
            nextTransform.Translate(translation.X, translation.Y);

            // Constrain the transform to the bounds of the plot.
            nextTransform = ConstrainTransformToBounds(nextTransform, cumulativeScale);

            // Update the plot transform.
            lastTransform = nextTransform;
            plotTransform = nextTransform;
            UpdatePlot();
        }



        CGAffineTransform ConstrainTransformToBounds(CGAffineTransform nextTransform, nfloat targetScale)
        {
            // The left edge of the plot can never go further right than 0.
            if (nextTransform.x0 > 0)
            {
                nextTransform.x0 = 0;
            }

            // The left edge of the plot can never go further left than
            // the delta between the original plot size and the scaled size.
            if (nextTransform.x0 < -GetScaledMaxPanDeltaX(targetScale))
            {
                nextTransform.x0 = -GetScaledMaxPanDeltaX(targetScale);
            }

            // The top edge of the plot can never go higher than the delta between the 
            // original plot size and the scaled size above the original top edge location.
            if (nextTransform.y0 < baseTransform.y0 - GetScaledMaxPanDeltaY(targetScale))
            {
                nextTransform.y0 = baseTransform.y0 - GetScaledMaxPanDeltaY(targetScale);
            }

            // The top edge of the plot can never go lower than the original location
            // of the top edge of the plot in the scaled coordinate system.
            if (nextTransform.y0 > baseTransform.y0 * targetScale)
            {
                nextTransform.y0 = baseTransform.y0 * targetScale;
            }

            return nextTransform;
        }

        private nfloat GetScaledMaxPanDeltaX(nfloat targetScale)
        {
            return Bounds.Width * targetScale - Bounds.Width;
        }

        private nfloat GetScaledMaxPanDeltaY(nfloat targetScale)
        {
            return Bounds.Height * targetScale - Bounds.Height;
        }

        public PlotDataPoint DataPointForPoint(CGPoint point)
        {
            var totalXDistance = point.X - plotTransform.x0;
            var totalYDistance = plotTransform.y0 - baseTransform.y0 * cumulativeScale + point.Y;

            return new PlotDataPoint
            {
                X = totalXDistance / (cumulativeScale * XRatio),
                Y = totalYDistance / (cumulativeScale * YRatio)
            };
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

        private static CGAffineTransform CalculateAffineTransform(CGRect sourceRect, CGRect targetRect, nfloat scale, CGPoint anchor)
        {
            var transform = CGAffineTransform.MakeTranslation(targetRect.X - sourceRect.X,
                                                              targetRect.Y - sourceRect.Y);
            transform.Scale(targetRect.Width / sourceRect.Width * scale,
                targetRect.Height / sourceRect.Height * scale);


            //transform.Translate(-(anchor.X * scale - anchor.X), -(anchor.Y * scale - anchor.Y));

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