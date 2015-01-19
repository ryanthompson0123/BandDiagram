using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using System.Linq;
using Band;
using MonoTouch.UIKit;

namespace BandAid.iOS
{
    public sealed class StructurePlotScene : SKScene
    {
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public double MinX { get; set; }
        public double MaxX { get; set; }

        public float LeftYAxisMargin { get; set; }
        public float RightYAxisMargin { get; set; }

        public float XAxisMargin { get; set; }

        public bool IsRightAxisVisible { get; set; }
        public StructureViewModel Structure { get; set; }

        private PlotNode plotNode;
        private AxisNode leftNode;
        private AxisNode bottomNode;

        private SizeF size;

        public StructurePlotScene(SizeF size, StructureViewModel structure)
            : base(size)
        {
            this.size = size;

            LeftYAxisMargin = 100f;
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

        private void SetUpPlot()
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
                size.Width - LeftYAxisMargin - RightYAxisMargin,
                size.Height - XAxisMargin));
            plotNode.Position = new PointF(LeftYAxisMargin, XAxisMargin);
            plotNode.PlotStep(Structure.CurrentStep);
            AddChild(plotNode);

            leftNode = new AxisNode(Structure.PlotSteps, Axis.Left, new SizeF(100f, size.Height - XAxisMargin));
            leftNode.Position = new PointF(0, XAxisMargin);
            AddChild(leftNode);

            bottomNode = new AxisNode(Structure.PlotSteps, Axis.Bottom, 
                new SizeF(size.Width - LeftYAxisMargin - RightYAxisMargin, 100f));
            bottomNode.Position = new PointF(LeftYAxisMargin, 0);
            AddChild(bottomNode);
        }
    }
}