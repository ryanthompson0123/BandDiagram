using System;
using Band.Units;

namespace Band
{
    public class StructurePointDetailViewModel : ObservableObject
    {
        private string titleTextValue;
        public string TitleText
        {
            get { return titleTextValue; }
            set { SetProperty(ref titleTextValue, value); }
        }

        private string locationTextValue;
        public string LocationText
        {
            get { return locationTextValue; }
            set { SetProperty(ref locationTextValue, value); }
        }

        private string eFieldTextValue;
        public string EFieldText
        {
            get { return eFieldTextValue; }
            set { SetProperty(ref eFieldTextValue, value); }
        }

        private string potentialTextValue;
        public string PotentialText
        {
            get { return potentialTextValue; }
            set { SetProperty(ref potentialTextValue, value); }
        }

        public StructurePointDetailViewModel(Structure structure, PlotDataPoint point)
        {
            var location = Length.FromNanometers(point.X);
            var layer = structure.GetLayer(location);

            TitleText = layer.Name;
            LocationText = location.NanometersToString("{00:F3} nm");
            EFieldText = structure.GetElectricField(location).MegavoltsPerCentimeterToString("{00:F3} MV/cm");
            PotentialText = structure.GetPotential(location).ToString("{00:F3} V");
        }
    }
}
