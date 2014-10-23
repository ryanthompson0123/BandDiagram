using System;
using Band.Units;
using System.ComponentModel;

namespace Band
{
    public class StructureViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public Structure ReferenceStructure { get; set; }

        public ElectricPotential CurrentVoltage { get; set; }
        public ElectricPotential MinVoltage { get; set; }
        public ElectricPotential MaxVoltage { get; set; }
        public ElectricPotential StepSize { get; set; }

        public int StepCount
        {
            get { return (int)((MaxVoltage - MinVoltage) / StepSize); }
        }

        public StructureViewModel()
        {
            ReferenceStructure = CreateSiO2TestStructure();
            CurrentVoltage = new ElectricPotential(1.0);
            MinVoltage = new ElectricPotential(-2.0);
            MaxVoltage = new ElectricPotential(2.0);
            StepSize = new ElectricPotential(0.25);
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
    }
}