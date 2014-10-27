using System;
using System.Collections.Generic;

namespace Band
{
    public class Plot
    {
        public string Name { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public List<PlotDataSet> DataSets { get; set; }

        protected Plot()
        {
            DataSets = new List<PlotDataSet>();
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

    public class PlotDataSet
    {
        public string Name { get; set; }
        public string PlotColor { get; set; }
        public int LineThickness { get; set; }

        public List<Tuple<double, double>> DataPoints { get; set; }

        public PlotDataSet()
        {
            DataPoints = new List<Tuple<double, double>>();
        }
    }
}