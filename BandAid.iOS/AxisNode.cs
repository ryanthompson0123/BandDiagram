using System;
using SpriteKit;
using CoreGraphics;
using Band;
using CoreGraphics;
using UIKit;

namespace BandAid.iOS
{
    public sealed class AxisNode : SKNode
    {
        public AxisViewModel ViewModel { get; private set; }
        public CGSize Size { get; private set; }
        
        private SKLabelNode titleNode;

        private string titleValue;
        public string Title
        {
            get { return titleValue; }
            set
            {
                titleValue = value;
                if (titleNode != null)
                {
                    titleNode.RemoveFromParent();
                    titleNode = null;
                }

                titleNode = new SKLabelNode("HelveticaNeue-Bold");
                titleNode.FontSize = 20f;
                titleNode.FontColor = UIColor.Black;

                titleNode.Text = titleValue;

                switch (ViewModel.AxisType)
                {
                    case AxisType.PrimaryY:
                        titleNode.ZRotation = (float)Math.PI / 2;
                        titleNode.Position = new CGPoint(Size.Width / 4, Size.Height / 2);
                        break;
                    case AxisType.SecondaryY:
                        titleNode.ZRotation = (float)-Math.PI / 2;
                        titleNode.Position = new CGPoint(3 * Size.Width / 4, Size.Height / 2);
                        break;
                    case AxisType.X:
                        titleNode.Position = new CGPoint(Size.Width / 2, Size.Height / 4);
                        break;
                }

                AddChild(titleNode);
            }
        }

        public AxisNode(AxisViewModel viewModel, CGSize size)
        {
            ViewModel = viewModel;
            Size = size;

            Title = ViewModel.TitleText;

            switch (ViewModel.AxisType)
            {
                case AxisType.PrimaryY:
                    DrawLeftAxis();
                    break;
                case AxisType.X:
                    DrawBottomAxis();
                    break;
            }
        }

        private void DrawLeftAxis()
        {
            var axis = new SKShapeNode();
            var pathToDraw = new CGPath();
            pathToDraw.MoveToPoint(new CGPoint(Size.Width - 8f, Size.Height));
            pathToDraw.AddLineToPoint(new CGPoint(Size.Width - 8f, 0));

            for (var i = ViewModel.AxisBounds.Max; i >= ViewModel.AxisBounds.Min;
                i = i - ViewModel.MajorAxisSpan)
            {
                var yCoord = GetYCoord(i);
                pathToDraw.MoveToPoint(new CGPoint(Size.Width - 12f, yCoord));
                pathToDraw.AddLineToPoint(new CGPoint(Size.Width - 8f, yCoord));

                var labelNode = new SKLabelNode();
                labelNode.Text = string.Format("{0:0.0}", i);
                labelNode.Position = new CGPoint(Size.Width - 32f, yCoord - 8f);
                labelNode.FontColor = UIColor.Black;
                labelNode.FontSize = 16f;
                AddChild(labelNode);
            }

            axis.Path = pathToDraw;
            axis.StrokeColor = UIColor.Black;
            axis.LineWidth = 2.0f;
            axis.Position = new CGPoint(0, 0);
            AddChild(axis);
        }

        private void DrawBottomAxis()
        {
            var axis = new SKShapeNode();
            var pathToDraw = new CGPath();
            pathToDraw.MoveToPoint(new CGPoint(Size.Width, Size.Height - 8f));
            pathToDraw.AddLineToPoint(new CGPoint(0, Size.Height - 8f));

            for (var i = ViewModel.AxisBounds.Max; i >= ViewModel.AxisBounds.Min;
                i = i - ViewModel.MajorAxisSpan)
            {
                var xCoord = GetXCoord(i);
                pathToDraw.MoveToPoint(new CGPoint(xCoord, Size.Height - 12f));
                pathToDraw.AddLineToPoint(new CGPoint(xCoord, Size.Height - 8f));

                var labelNode = new SKLabelNode();
                labelNode.Text = string.Format("{0:0.0}", i);
                labelNode.Position = new CGPoint(xCoord - 8f, Size.Height - 32f);
                labelNode.FontColor = UIColor.Black;
                labelNode.FontSize = 16f;
                AddChild(labelNode);
            }

            axis.Path = pathToDraw;
            axis.StrokeColor = UIColor.Black;
            axis.LineWidth = 2.0f;
            axis.Position = new CGPoint(0, 0);
            AddChild(axis);
        }

        private float GetDistance(double value)
        {
            return (float)(ViewModel.AxisBounds.Max - value)
                / (float)(ViewModel.AxisBounds.Max - ViewModel.AxisBounds.Min);
        }

        private nfloat GetYCoord(double value)
        {

            return Size.Height - (GetDistance(value) * Size.Height);
        }

        private nfloat GetXCoord(double value)
        {

            return Size.Width - (GetDistance(value) * Size.Width);
        }
    }
}