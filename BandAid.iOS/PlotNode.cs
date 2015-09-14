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
        public PlotViewModel ViewModel { get; private set; }
        public SizeF Size { get; private set; }

        public float XRatio
        {
            get { return Size.Width / (float)(ViewModel.XAxisBounds.Max - ViewModel.XAxisBounds.Min); }
        }

        public float YRatio
        {
            get { return Size.Height / (float)(ViewModel.YAxisBounds.Max - ViewModel.YAxisBounds.Min); }
        }

        public PlotNode(PlotViewModel viewModel, SizeF size)
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

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var dataSetNode = new SKSpriteNode(SKTexture.FromImage(image));
            dataSetNode.Position = new PointF(Size.Width/2, Size.Height/2);
            AddChild(dataSetNode);
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

            layer.StrokeColor = CustomUIColor.FromHexString(dataSet.PlotColor).CGColor;
            layer.FillColor = UIColor.Clear.CGColor;
            layer.LineWidth = (float)dataSet.LineThickness;
            //layer.BackgroundColor = UIColor.Clear.CGColor;

            layer.RenderInContext(UIGraphics.GetCurrentContext());
        }

        private void DrawBorder()
        {
            var border = new SKShapeNode();
            var pathToDraw = CGPath.FromRect(new RectangleF(new PointF(0, 0), Size));
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
                X = (float)(dataPoint.X - ViewModel.XAxisBounds.Min) * XRatio,
                Y = (float)(ViewModel.YAxisBounds.Max - dataPoint.Y) * YRatio
            };
        }
    }
}