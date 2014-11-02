using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using System.Linq;
using Band;

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

        public StructurePlotScene(SizeF size, StructureViewModel structure)
            : base(size)
        {
            MinX = 0.0;
            MaxX = 50.0;

            Structure = structure;

            Structure.PropertyChanged += (sender, e) => 
            {
                if (e.PropertyName == "CurrentStep")
                {
                    plotNode.PlotStep(Structure.CurrentStep);
                }
            };

            plotNode = new PlotNode(Structure.PlotSteps, size);
            plotNode.Position = new PointF(0, 0);
            plotNode.PlotStep(Structure.CurrentStep);
            AddChild(plotNode);
        }
    }
}