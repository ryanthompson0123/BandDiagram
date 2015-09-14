using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Band
{
    public class StructureParameterItemViewModel : ObservableObject
    {
        private string titleValue;
        public string TitleText
        {
            get { return titleValue; }
            set { SetProperty(ref titleValue, value); }
        }

        private string capacitanceValue;
        public string CapacitanceText
        {
            get { return capacitanceValue; }
            set { SetProperty(ref capacitanceValue, value); }
        }

        private string voltageDropValue;
        public string VoltageDropText
        {
            get { return voltageDropValue; }
            set { SetProperty(ref voltageDropValue, value); }
        }

        public StructureParameterItemViewModel(Material layer)
        {
            TitleText = layer.Name;

            if (layer is Metal) return;

            if (layer is Dielectric)
            {
                var dielectric = (Dielectric)layer;

                CapacitanceText = dielectric.OxideCapacitance.FaradsPerSquareCentimeter.ToString();
                VoltageDropText = dielectric.VoltageDrop.Volts.ToString();
            }
            else
            {
                var semiconductor = (Semiconductor)layer;

                CapacitanceText = semiconductor.CapacitanceDensity.FaradsPerSquareCentimeter.ToString();
                VoltageDropText = semiconductor.EvalPoints[0].Potential.Volts.ToString();
            }
        }
    }
}
