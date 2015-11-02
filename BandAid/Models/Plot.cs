using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Band.Units;

namespace Band
{
    public enum PlotType
    {
        Energy, Potential, ChargeDensity, ElectricField
    }

    public class Plot
    {
        public string Title { get; set; }
        public PlotAxis XAxis { get; set; }
        public PlotAxis YAxis { get; set; }

        public ObservableCollection<PlotDataSet> DataSets { get; set; }

        public Plot(List<PlotDataSet> dataSets)
        {
            DataSets = new ObservableCollection<PlotDataSet>(dataSets);
            XAxis = new PlotAxis();
            YAxis = new PlotAxis();
        }

        public void AutoScale()
        {
            var allPoints = DataSets.SelectMany(s => s.DataPoints).ToList();
            XAxis.Max = allPoints.Max(p => p.X);
            XAxis.Min = allPoints.Min(p => p.X);

            YAxis.Max = allPoints.Max(p => p.Y);
            YAxis.Min = allPoints.Min(p => p.Y);
        }

        internal static double CalculateSpanForBestFit(double max, double min, double value)
        {
            var thisOne = (max - min) / value;
            var bigger = (max - min) / (value * 2);
            var smaller = (max - min) / (value / 2);

            if (thisOne < 10 && bigger < 10 && smaller < 10)
            {
                return CalculateSpanForBestFit(max, min, value / 2);
            }

            if (thisOne > 10 && bigger > 10 && smaller > 10)
            {
                return CalculateSpanForBestFit(max, min, value * 2);
            }

            var numbers = new[] { value, value * 2, value / 2 };

            return numbers.OrderBy(v => Math.Abs((long)((max - min) / v) - 10)).First();
        }
    }

    public class PlotAnimationGrouping
    {
        public PlotAxis XAxis { get; private set; }
        public PlotAxis YAxis { get; private set; }
        public PlotAxis AnimationAxis { get; private set; }

        public List<Plot> Plots { get; private set; }

        public PlotAnimationGrouping(PlotAxis animationAxis, Func<double, Plot> provider)
        {
            AnimationAxis = animationAxis;

            Plots = Enumerable.Range(0, animationAxis.MajorTickCount)
                .Select(s => s * animationAxis.MajorSpan + animationAxis.Min)
                .Select(provider)
                .ToList();

            var allXBounds = Plots.Select(p => p.XAxis).ToList();
            var allYBounds = Plots.Select(p => p.YAxis).ToList();

            var xMax = Math.Ceiling(allXBounds.Max(b => b.Max));
            var xMin = Math.Floor(allXBounds.Min(b => b.Min));
            var xTick = Plots[0].XAxis.MajorSpan > 0 ? 
                Plots[0].XAxis.MajorSpan : Plot.CalculateSpanForBestFit(xMax, xMin, 1.0);

            XAxis = new PlotAxis
            {
                AxisType = AxisType.X,
                Max = xMax,
                Min = xMin,
                MajorSpan = xTick,
                Title = Plots[0].XAxis.Title
            };

            var yMax = Math.Ceiling(allYBounds.Max(b => b.Max));
            var yMin = Math.Floor(allYBounds.Min(b => b.Min));
            var yTick = Plots[0].YAxis.MajorSpan > 0 ?
                Plots[0].YAxis.MajorSpan : Plot.CalculateSpanForBestFit(yMax, yMin, 1.0);

            YAxis = new PlotAxis
            {
                AxisType = AxisType.PrimaryY,
                Max = yMax,
                Min = yMin,
                MajorSpan = yTick,
                Title = Plots[0].YAxis.Title
            };
        }

        public int GetIndexOfClosestPlot(double value)
        {
            return AnimationAxis.GetClosestMajorTickIndex(value);
        }
    }

    public class PlotDataSet
    {
        public string Name { get; set; }
        public string PlotColor { get; set; }
        public int LineThickness { get; set; }

        public List<PlotDataPoint> DataPoints { get; set; }

        public PlotDataSet()
        {
            DataPoints = new List<PlotDataPoint>();
        }
    }

    public class PlotDataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public enum AxisType { PrimaryY, SecondaryY, X }

    public class PlotAxis
    {
        public AxisType AxisType { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double MajorSpan { get; set; }
        public double MinorSpan { get; set; }
        public string Title { get; set; }

        public double Range
        {
            get { return Max - Min; }
        }

        public double Midpoint
        {
            get { return Min + (Range / 2); }
        }

        public int MajorTickCount
        {
            get { return (int)(Range / MajorSpan) + 1; }
        }

        public List<string> TickLabels
        {
            get
            {
                return MajorTicks
                    .Select(t => string.Format("{0:0.0}", t))
                    .ToList();
            }
        }

        public List<double> MajorTicks
        {
            get
            {
                return Enumerable
                    .Range(0, MajorTickCount)
                    .Select(s => s * MajorSpan + Min)
                    .ToList();
            }
        }

        public int GetClosestMajorTickIndex(double value)
        {
            var stepFraction = (value - Min) / MajorSpan;

            return Convert.ToInt32(stepFraction);
        }

        public double GetClosestMajorTick(double value)
        {
            var step = GetClosestMajorTickIndex(value);
            return step * MajorSpan + Min;
        }

        public double GetTickValue(int tickNumber)
        {
            return tickNumber * MajorSpan + Min;
        }
    }
}