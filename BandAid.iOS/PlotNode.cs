using SpriteKit;
using Band;
using CoreGraphics;
using UIKit;
using CoreAnimation;
using System;

namespace BandAid.iOS
{
    public class PlotNode : SKNode
    {
        public PlotViewModel ViewModel { get; private set; }
        public CGSize Size { get; private set; }
        private SKTexture plotTexture;

        public nfloat XRatio
        {
            get { return Size.Width / (nfloat)(ViewModel.XAxisBounds.Max - ViewModel.XAxisBounds.Min); }
        }

        public nfloat YRatio
        {
            get { return Size.Height / (nfloat)(ViewModel.YAxisBounds.Max - ViewModel.YAxisBounds.Min); }
        }

        public PlotNode(PlotViewModel viewModel, CGSize size)
        {
            ViewModel = viewModel;
            Size = size;
            
            DrawBorder();
            DrawPlot();
        }

        public void DrawPlot()
        {
            if (ViewModel == null || ViewModel.DataSets == null) return;

            UIGraphics.BeginImageContext(Size);
            foreach (var dataSet in ViewModel.DataSets)
            {
                PlotDataSetNode(dataSet);
            }

            using (var image = UIGraphics.GetImageFromCurrentImageContext())
            {
                plotTexture = SKTexture.FromImage(image);
                var dataSetNode = new SKSpriteNode(SKTexture.FromImage(image));
                dataSetNode.Position = new CGPoint(Size.Width / 2, Size.Height / 2);
                AddChild(dataSetNode);
            }

            UIGraphics.EndImageContext();
        }

        private void PlotDataSetNode(PlotDataSet dataSet)
        {
            var layer = new CAShapeLayer();
            layer.Frame = new CGRect(new CGPoint(0, 0), Size);

            var pathToDraw = new CGPath();
            var startCoord = GetCoord(dataSet.DataPoints[0]);
            pathToDraw.MoveToPoint(startCoord);

            for (var i = 1; i < dataSet.DataPoints.Count; i++)
            {
                var coord = GetCoord(dataSet.DataPoints[i]);
                pathToDraw.AddLineToPoint(coord);
            }
            layer.Path = pathToDraw;

            layer.StrokeColor = CustomUIColor.FromHexString(dataSet.PlotColor).CGColor;
            layer.FillColor = UIColor.Clear.CGColor;
            layer.LineWidth = (float)dataSet.LineThickness;
            //layer.BackgroundColor = UIColor.Clear.CGColor;

            layer.RenderInContext(UIGraphics.GetCurrentContext());
        }

        private void DrawBorder()
        {
            var border = new SKShapeNode();
            var pathToDraw = CGPath.FromRect(new CGRect(new CGPoint(0, 0), Size));
            border.Path = pathToDraw;
            border.StrokeColor = UIColor.Black;
            border.LineWidth = 2.0f;
            border.Position = new CGPoint(0, 0);
            AddChild(border);
        }

        private CGPoint GetCoord(PlotDataPoint dataPoint)
        {
            return new CGPoint
            {
                X = (float)(dataPoint.X - ViewModel.XAxisBounds.Min) * XRatio,
                Y = (float)(ViewModel.YAxisBounds.Max - dataPoint.Y) * YRatio
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (plotTexture != null)
                {
                    plotTexture.Dispose();
                    plotTexture = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}