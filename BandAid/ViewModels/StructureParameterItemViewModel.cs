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
        public string Title
        {
            get { return titleValue; }
            set { SetProperty(ref titleValue, value); }
        }

        private string capacitanceValue;
        public string Capacitance
        {
            get { return capacitanceValue; }
            set { SetProperty(ref capacitanceValue, value); }
        }

        private string voltageDropValue;
        public string VoltageDrop
        {
            get { return voltageDropValue; }
            set { SetProperty(ref voltageDropValue, value); }
        }

        public StructureParameterItemViewModel(Material layer)
        {
            Title = layer.Name;

            if (layer is Metal) return;

            if (layer is Dielectric)
            {
                var dielectric = (Dielectric)layer;

                Capacitance = dielectric.OxideCapacitance.FaradsPerSquareCentimeter.ToString();
                VoltageDrop = dielectric.VoltageDrop.Volts.ToString();
            }
            else
            {
                var semiconductor = (Semiconductor)layer;

                Capacitance = semiconductor.CapacitanceDensity.FaradsPerSquareCentimeter.ToString();
                VoltageDrop = semiconductor.EvalPoints[0].Potential.Volts.ToString();
            }
        }
    }
}
