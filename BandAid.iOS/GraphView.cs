﻿using System;
using SpriteKit;
using UIKit;
using Band;
using System.Threading.Tasks;

namespace BandAid.iOS
{
    public partial class GraphView : UIView
    {
        public event EventHandler<EventArgs> AnimationValueChanged;
        public event EventHandler<PointTappedEventArgs> PointLongPressed;
        public event EventHandler<PointPinchedEventArgs> PointPinched;
        public event EventHandler<PointTappedEventArgs> PointTapped;

        public AxisView PrimaryYAxis
        {
            get { return primaryYAxisView; }
        }

        public AxisView SecondaryYAxis
        {
            get { return secondaryYAxisView; }
        }

        public AxisView PrimaryXAxis
        {
            get { return primaryXAxisView; }
        }

        public GridView Grid
        {
            get { return gridView; }
        }

        public PlotView Plot
        {
            get { return plotView; }
        }

        public UISlider Slider
        {
            get { return animationAxisSlider; }
        }

        public UILabel AnimationAxisTitle
        {
            get { return animationAxisTitleLabel; }
        }

        public UILabel AnimationAxisValue
        {
            get { return animationAxisValueLabel; }
        }

        public UIActivityIndicatorView ActivityIndicator
        {
            get { return activityIndicator; }
        }

        public double AnimationValue
        {
            get { return PlotGrouping.AnimationAxis.GetClosestMajorTick(Slider.Value); }
        }

        // Milliseconds
        public int Duration { get; set; }
            
        public PlotAnimationGrouping PlotGrouping { get; private set; }

        public GraphView(IntPtr handle)
            : base(handle)
        {
            Duration = 2000;
        }

        private UILongPressGestureRecognizer longPressRecognizer;
        private UIPinchGestureRecognizer pinchRecognizer;
        private UITapGestureRecognizer tapRecognizer;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Slider.ValueChanged += Slider_ValueChanged;

            pinchRecognizer = new UIPinchGestureRecognizer(OnPlotPinch);

            longPressRecognizer = new UILongPressGestureRecognizer(OnPlotLongPress);

            tapRecognizer = new UITapGestureRecognizer(OnPlotTap);
            tapRecognizer.RequireGestureRecognizerToFail(longPressRecognizer);

            Plot.AddGestureRecognizer(pinchRecognizer);
            Plot.AddGestureRecognizer(longPressRecognizer);
            Plot.AddGestureRecognizer(tapRecognizer);
        }

        private void OnPlotPinch(UIPinchGestureRecognizer recognizer)
        {
            if (PointPinched != null)
            {
                var location = recognizer.LocationInView(Plot);
                var dataPoint = Plot.DataPointForPoint(location);

                PointPinched(this, new PointPinchedEventArgs
                {
                    PlotDataPoint = dataPoint,
                    Scale = (float)recognizer.Scale
                });
            }
        }

        private void OnPlotLongPress(UILongPressGestureRecognizer recognizer)
        {
            if (PointLongPressed != null)
            {
                var location = recognizer.LocationInView(Plot);
                var dataPoint = Plot.DataPointForPoint(location);

                PointLongPressed(this, new PointTappedEventArgs
                {
                    PlotDataPoint = dataPoint
                });
            }
        }

        private void OnPlotTap(UITapGestureRecognizer recognizer)
        {
            if (PointTapped != null)
            {
                var location = recognizer.LocationInView(Plot);
                var dataPoint = Plot.DataPointForPoint(location);

                PointTapped(this, new PointTappedEventArgs
                {
                    PlotDataPoint = dataPoint
                });
            }
        }

        public void SetPlotGroup(PlotAnimationGrouping value)
        {
            PlotGrouping = value;

            PrimaryYAxis.Axis = value.YAxis;
            PrimaryXAxis.Axis = value.XAxis;
            Grid.YAxis = value.YAxis;
            Grid.XAxis = value.XAxis;
            Plot.PlotGroup = value;
            Slider.MaxValue = (float)value.AnimationAxis.Max;
            Slider.MinValue = (float)value.AnimationAxis.Min;
            AnimationAxisTitle.Text = value.AnimationAxis.Title;

            ActivityIndicator.StopAnimating();
        }

        public void SetAnimationValue(double value, bool animated)
        {
            Slider.SetValue((float)value, animated);
            Slider_ValueChanged(this, EventArgs.Empty);
        }

        public async Task RunSweepAnimationAsync()
        {
            Slider.UserInteractionEnabled = false;

            var axis = PlotGrouping.AnimationAxis;

            // Figure out step time for desired animation length
            var delay = Duration / (int)((axis.Max - axis.Min) / axis.MajorSpan);

            await Task.Run(async () =>
            {
                for (var i = 0; i < axis.MajorTickCount; i++)
                {
                    var nextValue = axis.Min + i * axis.MajorSpan;

                    if (i == 0)
                    {
                        InvokeOnMainThread(() => SetAnimationValue(nextValue, false));
                    }
                    else
                    {
                        InvokeOnMainThread(() => SetAnimationValue(nextValue, true));
                    }
                        
                    await Task.Delay(delay);
                }
            });

            Slider.UserInteractionEnabled = true;
        }

        public UIImage RenderToImage()
        {
            UIGraphics.BeginImageContextWithOptions(Bounds.Size, Opaque, UIScreen.MainScreen.Scale);
            Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }

        public UIImage RenderPlotToImage()
        {
            UIGraphics.BeginImageContextWithOptions(Plot.Bounds.Size, false, UIScreen.MainScreen.Scale);
            Plot.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            AnimationAxisValue.Text = string.Format("{0:0.0}", AnimationValue);
            Plot.SelectPlot(Slider.Value);

            OnAnimationValueChanged(EventArgs.Empty);
        }

        private void OnAnimationValueChanged(EventArgs e)
        {
            if (AnimationValueChanged != null)
            {
                AnimationValueChanged(this, e);
            }
        }
    }

    public class PointTappedEventArgs : EventArgs
    {
        public PlotDataPoint PlotDataPoint { get; set; }
    }

    public class PointPinchedEventArgs : PointTappedEventArgs
    {
        public float Scale { get; set; }
    }
}