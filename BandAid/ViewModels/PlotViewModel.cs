using Band.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Band
{
    public class PlotViewModel : ObservableObject
    {
        private PlotAnimationGrouping plotGroupValue;
        public PlotAnimationGrouping PlotGroup
        {
            get { return plotGroupValue; }
            private set { SetProperty(ref plotGroupValue, value); }
        }

        private PlotType plotTypeValue;
        public PlotType PlotType
        {
            get { return plotTypeValue; }
            set
            {
                SetProperty(ref plotTypeValue, value);
                UpdatePlot();
            }
        }

        private Dictionary<int, Structure> stepsValue;
        public Dictionary<int, Structure> Steps
        {
            get { return stepsValue; }
            set
            {
                SetProperty(ref stepsValue, value);
                UpdatePlot();
            }
        }

        private void UpdatePlot()
        {
            PlotGroup = PlotAnimationGrouping.Create(Steps.Keys
                .OrderBy(k => k)
                .Select(k => CreatePlot(Steps[k]))
                .ToList());
        }

        private Plot CreatePlot(Structure structure)
        {
            var plot = Plot.Create(PlotType);

            Length thickness = Length.Zero;
            foreach (var layer in structure.Layers)
            {
                switch (PlotType)
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

    }
}
