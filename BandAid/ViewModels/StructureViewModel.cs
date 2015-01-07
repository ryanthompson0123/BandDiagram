using System;
using Band.Units;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

    public class StructureViewModel : ObservableObject
    {
        private PlotAnimationGrouping plotStepsValue;
        public PlotAnimationGrouping PlotSteps
        {
            get { return plotStepsValue; }
            set { SetProperty(ref plotStepsValue, value); }
        }

        private Structure referenceStructureValue;
        public Structure ReferenceStructure
        {
            get { return referenceStructureValue; }
            set
            {
                SetProperty(ref referenceStructureValue, value);
                value.PropertyChanged += (sender, e) => 
                {
                    if (e.PropertyName == "Layers")
                    {
                        StructureSteps = new Dictionary<ElectricPotential, Structure>();
                        RecalculateAllSteps();
                    }
                };

                StructureSteps = new Dictionary<ElectricPotential, Structure>();
                RecalculateAllSteps();
            }
        }

        private Dictionary<ElectricPotential, Structure> structureStepsValue;
        public Dictionary<ElectricPotential, Structure> StructureSteps
        {
            get { return structureStepsValue; }
            set
            {
                SetProperty(ref structureStepsValue, value);
                RecalculateCurrentPlotSteps();
            }
        }

        private ElectricPotential currentVoltageValue;
        public ElectricPotential CurrentVoltage
        {
            get { return currentVoltageValue; }
            set
            {
                SetProperty(ref currentVoltageValue, value);
                CurrentStep = StepForPotential(value);
            }
        }

        private int currentStepValue;
        public int CurrentStep
        {
            get { return currentStepValue; }
            set
            {
                SetProperty(ref currentStepValue, value);
            }
        }

        private ElectricPotential minVoltageValue;
        public ElectricPotential MinVoltage
        {
            get { return minVoltageValue; }
            set
            {
                SetProperty(ref minVoltageValue, value);
                RecalculateAllSteps();
            }
        }

        private ElectricPotential maxVoltageValue;
        public ElectricPotential MaxVoltage
        {
            get { return maxVoltageValue; }
            set
            {
                SetProperty(ref maxVoltageValue, value);
                RecalculateAllSteps();
            }
        }

        private ElectricPotential stepSizeValue;
        public ElectricPotential StepSize
        {
            get { return stepSizeValue; }
            set
            {
                if (value.Volts == 0.0) return;
                SetProperty(ref stepSizeValue, value);
                RecalculateAllSteps();
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

        public int StepCount
        {
            get { return (int)((MaxVoltage - MinVoltage) / StepSize) + 1; }
        }

        public StructureViewModel()
        {
            currentVoltageValue = new ElectricPotential(1.0);
            minVoltageValue = new ElectricPotential(-2.0);
            maxVoltageValue = new ElectricPotential(2.0);
            stepSizeValue = new ElectricPotential(0.25);
            plotTypeValue = PlotType.Energy;

            currentStepValue = StepForPotential(CurrentVoltage);
            ReferenceStructure = CreateSiO2TestStructure();
        }

        private async void RecalculateAllSteps()
        {
            if (!ReferenceStructure.IsValid) return;

            await Task.Run(() =>
            {
                Debug.WriteLine(String.Format("Beginning calculation of {0} steps", StepCount));
                var stopwatch = Stopwatch.StartNew();

                var steps = Enumerable.Range(0, StepCount).Select(s => PotentialForStep(s))
                .ToDictionary(p => p, p =>
                {
                    return StructureSteps.ContainsKey(p) ? StructureSteps[p] : 
                        ReferenceStructure.DeepClone(p, new Temperature(300.0));
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
                .OrderBy(k => k.Volts)
                .Select(k => CreatePlot(StructureSteps[k]))
                .ToList());
        }

        private Plot CreatePlot(Structure structure)
        {
            var stopwatch = Stopwatch.StartNew();
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

            Debug.WriteLine(String.Format("Created plot in {0} ms", stopwatch.ElapsedMilliseconds));
            return plot;
        }

        private ElectricPotential PotentialForStep(int step)
        {
            return MinVoltage + StepSize * step;
        }

        private int StepForPotential(ElectricPotential potential)
        {
            return (int)((potential - MinVoltage) / StepSize);
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