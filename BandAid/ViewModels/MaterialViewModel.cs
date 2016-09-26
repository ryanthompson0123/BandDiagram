using System;
using System.Collections.ObjectModel;
using Band.Units;

namespace Band
{
    public class MaterialViewModel : ObservableObject
    {
        private readonly string[] dielectricLabels = { "Dielectric Constant", "Band Gap", "Electron Affinity" };
        private readonly string[] metalLabels = { "Work Function" };
        private readonly string[] semiconductorLabels = { "Dielectric Constant", "Band Gap", "Electron Affinity", 
            "Intrinsic Carrier Concentration", "Dopant Concentration" };

        private string titleTextValue;
        public string TitleText
        {
            get { return titleTextValue; }
            set { SetProperty(ref titleTextValue, value); }
        }

        private ObservableCollection<string> columnsValue;
        public ObservableCollection<string> Columns
        {
            get { return columnsValue; }
            set { SetProperty(ref columnsValue, value); }
        }

        private Material materialValue;
        public Material Material
        {
            get { return materialValue; }
            set { SetProperty(ref materialValue, value); }
        }

        public MaterialViewModel(Material material)
        {
            Material = material;
            TitleText = material.Name;
            Columns = new ObservableCollection<string>();

            switch (material.MaterialType)
            {
                case MaterialType.Dielectric:
                    var dielectric = (Dielectric)material;
                    Columns.Add(string.Format("{0:F2}", dielectric.DielectricConstant));
                    Columns.Add(string.Format("{0:F2}", dielectric.BandGap.ElectronVolts));
                    Columns.Add(string.Format("{0:F2}", dielectric.ElectronAffinity.ElectronVolts));
                    break;
                case MaterialType.Metal:
                    var metal = (Metal)material;
                    Columns.Add(string.Format("{0:F3}", metal.WorkFunction.ElectronVolts));
                    break;
                case MaterialType.Semiconductor:
                    var semiconductor = (Semiconductor)material;
                    semiconductor.Temperature = new Temperature(300);
                    semiconductor.IntrinsicCarrierConcentration.CustomConstructor = Concentration.FromPerCubicCentimeter;
                    Columns.Add(string.Format("{0:F2}", semiconductor.DielectricConstant));
                    Columns.Add(string.Format("{0:F3}", semiconductor.BandGap.Evaluate().ElectronVolts));
                    Columns.Add(string.Format("{0:F2}", semiconductor.ElectronAffinity.ElectronVolts));
                    Columns.Add(string.Format("{0:0.0#E+00}", semiconductor.IntrinsicCarrierConcentration.Evaluate().PerCubicCentimeter));
                    break;
            }
        }

        public double GetSortValue(int columnIndex)
        {
            double parsed;

            double.TryParse(Columns[columnIndex], out parsed);

            return parsed;
        }
    }
}

