using System;
using SpriteKit;
using UIKit;
using Band;
using System.Threading.Tasks;

namespace BandAid.iOS
{
    public partial class GraphView : UIView
    {
        public event EventHandler<EventArgs> AnimationValueChanged;

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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Slider.ValueChanged += Slider_ValueChanged;
        }

        public void SetPlotGroup(PlotAnimationGrouping value)
        {
            PlotGrouping = value;

            PrimaryYAxis.Axis = value.YAxis;
            PrimaryXAxis.Axis = value.XAxis;
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
}