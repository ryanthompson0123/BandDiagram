using System;
using Band.Units;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

namespace Band
{
    public enum PlotType
    {
        Energy, Potential, ChargeDensity, ElectricField
    }

    public class StructureViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private List<Plot> PlotSteps { get; set; }

        private Structure referenceStructureValue;
        public Structure ReferenceStructure
        {
            get { return referenceStructureValue; }
            set
            {
                SetProperty(ref referenceStructureValue, value);
                RecalculateAllSteps();
            }
        }

        private Plot plotValue;
        public Plot Plot
        {
            get { return plotValue; }
            private set { SetProperty(ref plotValue, value); }
        }

        private ElectricPotential currentVoltageValue;
        public ElectricPotential CurrentVoltage
        {
            get { return currentVoltageValue; }
            set
            {
                SetProperty(ref currentVoltageValue, value);
                Plot = PlotSteps[StepForPotential(value)];
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
                RecalculateAllSteps();
            }
        }

        public int StepCount
        {
            get { return (int)((MaxVoltage - MinVoltage) / StepSize) + 1; }
        }

        public StructureViewModel()
        {
            referenceStructureValue = CreateSiO2TestStructure();
            currentVoltageValue = new ElectricPotential(1.0);
            minVoltageValue = new ElectricPotential(-2.0);
            maxVoltageValue = new ElectricPotential(2.0);
            stepSizeValue = new ElectricPotential(0.25);
            plotTypeValue = PlotType.Energy;

            RecalculateAllSteps();
        }

        private void RecalculateAllSteps()
        {
            PlotSteps = null;

            if (!ReferenceStructure.IsValid) return;

            PlotSteps = Enumerable.Range(0, StepCount).Select(s =>
            {
                var structure = ReferenceStructure.DeepClone();
                structure.Bias = PotentialForStep(s);
                structure.Temperature = new Temperature(300.0);
                return CreatePlot(structure);
            }).ToList();
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
            topMetal.Name = "TiN";

            var oxide = new Dielectric(Length.FromNanometers(2));
            oxide.DielectricConstant = 3.9;
            oxide.BandGap = Energy.FromElectronVolts(8.9);
            oxide.ElectronAffinity = Energy.FromElectronVolts(0.95);
            oxide.Name = "SiO2";

            var semiconductor = new Semiconductor();
            semiconductor.BandGap = Energy.FromElectronVolts(1.1252);
            semiconductor.ElectronAffinity = Energy.FromElectronVolts(4.05);
            semiconductor.DielectricConstant = 11.7;
            semiconductor.IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(1.41E10);
            semiconductor.DopingType = DopingType.N;
            semiconductor.DopantConcentration = Concentration.FromPerCubicCentimeter(1E18);
            semiconductor.Name = "Si";

            var structure = new Structure();
            structure.Temperature = new Temperature(300);
            structure.AddLayer(semiconductor);
            structure.AddLayer(oxide);
            structure.AddLayer(topMetal);

            return structure;
        }

        bool SetProperty<T>(ref T storage, T value, 
            [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}