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

        public static Plot Create(Structure structure, PlotType plotType)
        {
            var plot = Create(plotType);

            Length thickness = Length.Zero;
            foreach (var layer in structure.Layers)
            {
                switch (plotType)
                {
                    case PlotType.Energy:
                        foreach (var dataset in layer.GetEnergyDatasets(thickness))
                        {
                            plot.DataSets.Add(dataset);
                        }
                        break;
                    case PlotType.ElectricField:
                        plot.DataSets.Add(layer.GetElectricFieldDataset(thickness));
                        break;
                    case PlotType.ChargeDensity:
                        plot.DataSets.Add(layer.GetChargeDensityDataset(thickness));
                        break;
                    case PlotType.Potential:
                        plot.DataSets.Add(layer.GetPotentialDataset(thickness));
                        break;
                }

                thickness += layer.Thickness;
            }

            return plot;
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

        public int MinorYAxisTicks { get; set; }
        public int MinorXAxisTicks { get; set; }

        public double MajorXAxisSpan { get; set; }
        public double MajorYAxisSpan { get; set; }

        public List<Plot> Plots { get; set; }

        protected PlotAnimationGrouping()
        {
            MinorXAxisTicks = 1;
            MinorYAxisTicks = 1;
        }

        public static PlotAnimationGrouping Create(List<Plot> plots)
        {
            var allXBounds = plots.Select(p => p.XAxisBounds).ToList();
            var allYBounds = plots.Select(p => p.YAxisBounds).ToList();

            var xBounds = new PlotAxisBounds
            {
                Max = Math.Ceiling(allXBounds.Max(b => b.Max)),
                Min = Math.Floor(allXBounds.Min(b => b.Min))
            };

            var yBounds = new PlotAxisBounds
            {
                Max = Math.Ceiling(allYBounds.Max(b => b.Max)),
                Min = Math.Floor(allYBounds.Min(b => b.Min))
            };

            return new PlotAnimationGrouping
            {
                Plots = plots,
                XAxisBounds = xBounds,
                YAxisBounds = yBounds,
                MajorXAxisSpan = FigureTickValue(xBounds, 1.0),
                MajorYAxisSpan = FigureTickValue(yBounds, 1.0)
            };
        }

        private static double FigureTickValue(PlotAxisBounds bounds, double value)
        {
            var thisOne = (bounds.Max - bounds.Min) / value;
            var bigger = (bounds.Max - bounds.Min) / (value * 2);
            var smaller = (bounds.Max - bounds.Min) / (value / 2);

            if (thisOne < 10 && bigger < 10 && smaller < 10)
            {
                return FigureTickValue(bounds, value / 2);
            }

            if (thisOne > 10 && bigger > 10 && smaller > 10)
            {
                return FigureTickValue(bounds, value * 2);
            }

            var numbers = new[] { value, value * 2, value / 2 };

            return numbers.OrderBy(v => Math.Abs((long)((bounds.Max - bounds.Min) / v) - 10)).First();
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