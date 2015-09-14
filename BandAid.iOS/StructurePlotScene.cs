using System;
using MonoTouch.SpriteKit;
using System.Drawing;
using System.Linq;
using Band;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;

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

        private List<PlotNode> plotNodes;
        private PlotNode currentPlotNode;
        private AxisNode primaryYAxisNode;
        private AxisNode xAxisNode;

        private StructureSceneViewModel viewModelValue;
        public StructureSceneViewModel ViewModel
        {
            get { return viewModelValue; }
            set
            {
                if (viewModelValue != null)
                {
                    viewModelValue.PropertyChanged -= ViewModel_PropertyChanged;
                }

                viewModelValue = value;

                if (viewModelValue != null)
                {
                    viewModelValue.PropertyChanged += ViewModel_PropertyChanged;
                }
            }
        }

        public SizeF PlotSize
        {
            get
            {
                return new SizeF(Size.Width - LeftYAxisMargin - RightYAxisMargin,
                    Size.Height - XAxisMargin - TopMargin);
            }
        }

        public PointF PlotPosition
        {
            get {  return new PointF(LeftYAxisMargin, XAxisMargin); }
        }

        public SizeF PrimaryYAxisSize
        {
            get {  return new SizeF(100f, Size.Height - XAxisMargin - TopMargin); }
        }

        public SizeF XAxisSize
        {
            get { return new SizeF(Size.Width - LeftYAxisMargin - RightYAxisMargin, 100f); }
        }

        public StructurePlotScene(SizeF size)
            : base(size)
        {
            TopMargin = 50f;
            LeftYAxisMargin = 100f;
            RightYAxisMargin = 100f;
            XAxisMargin = 100f;

            BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            MinX = 0.0;
            MaxX = 50.0;
        }

        public override void DidMoveToView(SKView view)
        {
            base.DidMoveToView(view);

            SetUpPlot();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentStep":
                    DisplayStep(ViewModel.CurrentStep);
                    break;
                case "Plots":
                    SetUpPlot();
                    break;
            }
        }

        private void DisplayStep(int step)
        {
            if (plotNodes == null || plotNodes.Count < step) return;

            if (currentPlotNode != null)
            {
                currentPlotNode.RemoveFromParent();
            }

            currentPlotNode = plotNodes[step];
            AddChild(currentPlotNode);
        }

        private void SetUpPlot()
        {
            if (ViewModel.Plots == null) return;

            plotNodes = ViewModel.Plots.Select(p => CreatePlotNode(p)).ToList();

            SetUpPrimaryYAxis();
            SetUpXAxis();
            DisplayStep(ViewModel.CurrentStep);
        }

        private void SetUpPrimaryYAxis()
        {
            if (primaryYAxisNode != null)
            {
                primaryYAxisNode.RemoveFromParent();
            }

            primaryYAxisNode = new AxisNode(ViewModel.PrimaryYAxis, PrimaryYAxisSize);
            primaryYAxisNode.Position = new PointF(0, XAxisMargin);
            AddChild(primaryYAxisNode);
        }

        private void SetUpXAxis()
        {
            if (xAxisNode != null)
            {
                xAxisNode.RemoveFromParent();
            }

            xAxisNode = new AxisNode(ViewModel.XAxis, XAxisSize);
            xAxisNode.Position = new PointF(LeftYAxisMargin, 0);
            AddChild(xAxisNode);
        }

        private PlotNode CreatePlotNode(PlotViewModel viewModel)
        {
            return new PlotNode(viewModel, PlotSize)
            {
                Position = PlotPosition
            };
        }

        private bool screenshotNextFrame = false;

        public override void Update(double currentTime)
        {
            base.Update(currentTime);

            if (screenshotNextFrame)
            {
                TakeScreenshot();
                screenshotNextFrame = false;
            }

            if (ViewModel.NeedsScreenshot)
            {
                screenshotNextFrame = true;
                ViewModel.NeedsScreenshot = false;
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
            var outDir = Path.Combine(documents, "save");
            var outfile = Path.Combine(outDir, ViewModel.Name + ".png");

            File.WriteAllBytes(outfile, bytes);
        }
    }
}