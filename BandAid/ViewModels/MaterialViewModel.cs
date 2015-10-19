using System;
using System.Collections.ObjectModel;

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

        private string leftTextValue;
        public string LeftText
        {
            get { return leftTextValue; }
            set { SetProperty(ref leftTextValue, value); }
        }

        private string rightTextValue;
        public string RightText
        {
            get { return rightTextValue; }
            set { SetProperty(ref rightTextValue, value); }
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

            switch (material.MaterialType)
            {
                case MaterialType.Dielectric:
                    var dielectric = (Dielectric)material;
                    LeftText = String.Join("\n", dielectricLabels);
                    RightText = String.Format("{0}\n{1}\n{2}", dielectric.DielectricConstant,
                        dielectric.BandGap.ElectronVolts, dielectric.ElectronAffinity.ElectronVolts);
                    break;
                case MaterialType.Metal:
                    var metal = (Metal)material;
                    LeftText = metalLabels[0];
                    RightText = String.Format("{0}", metal.WorkFunction.ElectronVolts);
                    break;
                case MaterialType.Semiconductor:
                    var semiconductor = (Semiconductor)material;
                    LeftText = String.Join("\n", semiconductorLabels);
                    RightText = String.Format("{0}\n{1}\n{2}\n{3}\n{4}",
                        semiconductor.DielectricConstant, semiconductor.BandGap.ElectronVolts,
                        semiconductor.ElectronAffinity.ElectronVolts, 
                        semiconductor.IntrinsicCarrierConcentration.PerCubicCentimeter,
                        semiconductor.DopantConcentration.PerCubicCentimeter);
                    break;
            }
        }
    }
}

