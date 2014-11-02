using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Band
{
    public class Plot
    {
        public string Name { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public PlotAxisBounds XAxisBounds { get; set; }
        public PlotAxisBounds YAxisBounds { get; set; }

        public ObservableCollection<PlotDataSet> DataSets { get; set; }

        protected Plot()
        {
            DataSets = new ObservableCollection<PlotDataSet>();
            DataSets.CollectionChanged += (sender, e) => 
            {
                var allPoints = DataSets.SelectMany(s => s.DataPoints).ToList();
                XAxisBounds = new PlotAxisBounds
                {
                    Max = allPoints.Max(p => p.X),
                    Min = allPoints.Min(p => p.X)
                };

                YAxisBounds = new PlotAxisBounds
                {
                    Max = allPoints.Max(p => p.Y),
                    Min = allPoints.Min(p => p.Y)
                };
            };
        }

        public static Plot Create(PlotType plotType)
        {
            switch (plotType)
            {
                case PlotType.Energy:
                    return new Plot

                    {
                        Name = "Energy",
                        YAxisLabel = "Energy (eV)",
                        XAxisLabel = "Distance (nm)"
                    };
                case PlotType.ChargeDensity:
                    return new Plot
                    {
                        Name = "Charge Density",
                        YAxisLabel = "Charge Density (C/cm2)",
                        XAxisLabel = "Distance (nm)"
                    };
                case PlotType.ElectricField:
                    return new Plot
                    {
                        Name = "Electric Field",
                        YAxisLabel = "Electric Field (MV/cm)",
                        XAxisLabel = "Distance (nm)"
                    };
                case PlotType.Potential:
                    return new Plot
                    {
                        Name = "Potential",
                        YAxisLabel = "Potential (V)",
                        XAxisLabel = "Distance (nm)"
                    };
                default:
                    return new Plot
                    {
                        Name = "Unknown Plot",
                        YAxisLabel = "Unknown Unit",
                        XAxisLabel = "Unknown Unit"
                    };
            }
        }
    }

    public class PlotAnimationGrouping
    {
        public PlotAxisBounds XAxisBounds { get; set; }
        public PlotAxisBounds YAxisBounds { get; set; }

        public List<Plot> Plots { get; set; }

        protected PlotAnimationGrouping()
        {
        }

        public static PlotAnimationGrouping Create(List<Plot> plots)
        {
            var allXBounds = plots.Select(p => p.XAxisBounds).ToList();
            var allYBounds = plots.Select(p => p.YAxisBounds).ToList();

            var xBounds = new PlotAxisBounds
            {
                Max = allXBounds.Max(b => b.Max),
                Min = allXBounds.Min(b => b.Min)
            };

            var yBounds = new PlotAxisBounds
            {
                Max = allYBounds.Max(b => b.Max),
                Min = allYBounds.Min(b => b.Min)
            };

            return new PlotAnimationGrouping
            {
                Plots = plots,
                XAxisBounds = xBounds,
                YAxisBounds = yBounds
            };
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

    public class PlotAxisBounds
    {
        public double Min { get; set; }
        public double Max { get; set; }
    }
}