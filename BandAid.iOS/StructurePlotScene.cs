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

        public StructureViewModel Structure { get; set; }

        private PlotNode plotNode;
        private SizeF size;

        public StructurePlotScene(SizeF size, StructureViewModel structure)
            : base(size)
        {
            this.size = size;

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

            plotNode = new PlotNode(Structure.PlotSteps, size);
            plotNode.Position = new PointF(0, 0);
            plotNode.PlotStep(Structure.CurrentStep);
            AddChild(plotNode);
        }
    }
}