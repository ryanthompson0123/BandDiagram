using System;
using Band.Units;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Band
{
    public enum PlotType
    {
        Energy, Potential, ChargeDensity, ElectricField
    }

    public class TestBenchViewModel : ObservableObject
    {
        private bool needsScreenshotValue;
        public bool NeedsScreenshot
        {
            get { return needsScreenshotValue; }
            set { SetProperty(ref needsScreenshotValue, value); }
        }

        private PlotAnimationGrouping plotStepsValue;
        public PlotAnimationGrouping PlotSteps
        {
            get { return plotStepsValue; }
            set { SetProperty(ref plotStepsValue, value); }
        }

        private Dictionary<int, Structure> structureStepsValue;
        public Dictionary<int, Structure> StructureSteps
        {
            get { return structureStepsValue; }
            set
            {
                SetProperty(ref structureStepsValue, value);
                RecalculateCurrentPlotSteps();
            }
        }

        private PlotType plotTypeValue;
        public PlotType PlotType
        {
            get { return plotTypeValue; }
            set
            {
                SetProperty(ref plotTypeValue, value);
                RecalculateCurrentPlotSteps();
            }
        }

        public TestBenchViewModel()
        {
            currentVoltageValue = new ElectricPotential(1.0);
            minVoltageValue = new ElectricPotential(-2.0);
            maxVoltageValue = new ElectricPotential(2.0);
            stepSizeValue = new ElectricPotential(0.25);
            plotTypeValue = PlotType.Energy;

            currentStepValue = StepForPotential(CurrentVoltage);
            ReferenceStructure = CreateSiO2TestStructure();
        }

        public TestBenchViewModel(double currentVoltage, double minVoltage, double maxVoltage,
            double stepSize, PlotType plotType, Structure refStructure, string name)
        {
            currentVoltageValue = new ElectricPotential(currentVoltage);
            minVoltageValue = new ElectricPotential(minVoltage);
            maxVoltageValue = new ElectricPotential(maxVoltage);
            stepSizeValue = new ElectricPotential(stepSize);
            nameValue = name;
            plotTypeValue = plotType;

            currentStepValue = StepForPotential(CurrentVoltage);
            ReferenceStructure = refStructure;
        }

        public void Set(double maxVoltage, double minVoltage, double stepSize)
        {
            maxVoltageValue = new ElectricPotential(maxVoltage);
            minVoltageValue = new ElectricPotential(minVoltage);
            stepSizeValue = new ElectricPotential(stepSize);
            RecalculateAllSteps();
        }

        private async void RecalculateAllSteps()
        {
            if (!ReferenceStructure.IsValid) return;

            await Task.Run(() =>
            {
                Debug.WriteLine(String.Format("Beginning calculation of {0} steps", StepCount));
                var stopwatch = Stopwatch.StartNew();

                var steps = Enumerable.Range(0, StepCount).Select(s => PotentialForStep(s).RoundMillivolts)
                    .ToDictionary(p => p, p =>
                {
                    return StructureSteps.ContainsKey(p) ? StructureSteps[p] : 
                        ReferenceStructure.DeepClone(ElectricPotential.FromMillivolts((double)p), new Temperature(300.0));
                });

                Debug.WriteLine(String.Format("Finished all calculations in {0} ms", stopwatch.ElapsedMilliseconds));

                Device.BeginInvokeOnMainThread(() =>
                {
                    StructureSteps = steps;
                });
            });
        }

        private void RecalculateCurrentPlotSteps()
        {
            if (StructureSteps.Count == 0) return;

            PlotSteps = PlotAnimationGrouping.Create(StructureSteps.Keys
                .OrderBy(k => k)
                .Select(k => CreatePlot(StructureSteps[k]))
                .ToList());

            NeedsScreenshot = true;
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

        

        private static Structure CreateSiO2TestStructure()
        {
            var topMetal = new Metal(Length.FromNanometers(4));
            topMetal.SetWorkFunction(Energy.FromElectronVolts(4.45));
            topMetal.FillColor = "#ff0000";
            topMetal.Name = "TiN";

            var oxide = new Dielectric(Length.FromNanometers(2));
            oxide.DielectricConstant = 3.9;
            oxide.BandGap = Energy.FromElectronVolts(8.9);
            oxide.ElectronAffinity = Energy.FromElectronVolts(0.95);
            oxide.FillColor = "#804040";
            oxide.Name = "SiO2";

            var semiconductor = new Semiconductor();
            semiconductor.BandGap = Energy.FromElectronVolts(1.1252);
            semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
            semiconductor.DielectricConstant = 11.7;
            semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
            semiconductor.DopingType = DopingType.N;
            semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);
            semiconductor.FillColor = "#00ff00";
            semiconductor.Name = "Si";

            var structure = new Structure();
            structure.Temperature = new Temperature(300);
            structure.AddLayer(semiconductor);
            structure.AddLayer(oxide);
            structure.AddLayer(topMetal);

            return structure;
        }
    }
}