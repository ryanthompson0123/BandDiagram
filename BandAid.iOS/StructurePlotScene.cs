using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using System.Linq;
using Band;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.IO;

namespace BandAid.iOS
{
    public sealed class StructurePlotScene : SKScene
    {
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public double MinX { get; set; }
        public double MaxX { get; set; }

        public float TopMargin { get; set; }
        public float LeftYAxisMargin { get; set; }
        public float RightYAxisMargin { get; set; }

        public float XAxisMargin { get; set; }

        public bool IsRightAxisVisible { get; set; }
        public StructureViewModel Structure { get; set; }

        private PlotNode plotNode;
        private AxisNode leftNode;
        private AxisNode bottomNode;

        public StructurePlotScene(SizeF size, StructureViewModel structure)
            : base(size)
        {
            TopMargin = 50f;
            LeftYAxisMargin = 100f;
            RightYAxisMargin = 100f;
            XAxisMargin = 100f;

            BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            MinX = 0.0;
            MaxX = 50.0;

            Structure = structure;

            Structure.PropertyChanged += (sender, e) => 
            {
                if (e.PropertyName == "CurrentStep")
                {
                    plotNode.PlotStep(Structure.CurrentStep);
                }

                if (e.PropertyName == "PlotSteps")
                {
                    SetUpPlot();
                }
            };

            SetUpPlot();
        }

        public void SetUpPlot()
        {
            if (plotNode != null)
            {
                plotNode.RemoveFromParent();
                plotNode = null;
            }

            if (leftNode != null)
            {
                leftNode.RemoveFromParent();
                leftNode = null;
            }

            if (bottomNode != null)
            {
                bottomNode.RemoveFromParent();
                bottomNode = null;
            }

            plotNode = new PlotNode(Structure.PlotSteps, new SizeF(
                Size.Width - LeftYAxisMargin - RightYAxisMargin,
                Size.Height - XAxisMargin - TopMargin));
            plotNode.Position = new PointF(LeftYAxisMargin, XAxisMargin);
            plotNode.PlotStep(Structure.CurrentStep);
            AddChild(plotNode);

            leftNode = new AxisNode(Structure.PlotSteps, Axis.Left, new SizeF(100f, Size.Height - XAxisMargin - TopMargin));
            leftNode.Position = new PointF(0, XAxisMargin);
            AddChild(leftNode);

            bottomNode = new AxisNode(Structure.PlotSteps, Axis.Bottom, 
                new SizeF(Size.Width - LeftYAxisMargin - RightYAxisMargin, 100f));
            bottomNode.Position = new PointF(LeftYAxisMargin, 0);
            AddChild(bottomNode);

            if (Structure.NeedsScreenshot)
            {
                TakeScreenshot();
                Structure.NeedsScreenshot = false;
            }
        }

        public void TakeScreenshot()
        {
            UIGraphics.BeginImageContextWithOptions(View.Bounds.Size, false, 0);
            View.DrawViewHierarchy(this.View.Bounds, true);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var png = image.AsPNG();
            var bytes = new byte[png.Length];

            System.Runtime.InteropServices.Marshal.Copy(png.Bytes, bytes, 0, Convert.ToInt32(png.Length));

            var documents = NSFileManager.DefaultManager.GetUrls(
                NSSearchPathDirectory.DocumentDirectory, 
                NSSearchPathDomain.User)[0].Path;

            var outfile = Path.Combine(documents, Structure.Name + ".png");

            File.WriteAllBytes(outfile, bytes);
        }
    }
}