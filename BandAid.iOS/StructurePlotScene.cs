using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using Band;

namespace BandAid.iOS
{
    public class StructurePlotScene : SKScene
    {
        public StructureViewModel Structure { get; set; }

        public StructurePlotScene(SizeF size, StructureViewModel structure)
            : base(size)
        {
            Structure = structure;

            Structure.PropertyChanged += (sender, e) => 
            {
                if (e.PropertyName == "Plot")
                {
                    DrawPlot();
                }
            };

            DrawPlot();
        }

        private void DrawPlot()
        {

        }
    }
}