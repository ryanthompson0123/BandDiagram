using System;

namespace Band
{
    public class SettingsViewModel : ObservableObject
    {

        private string stepSizeTextValue;
        public string StepSizeText
        {
            get { return stepSizeTextValue; }
            set { SetProperty(ref stepSizeTextValue, value); }
        }

        private string maxVoltageTextValue;
        public string MaxVoltageText
        {
            get { return maxVoltageTextValue; }
            set { SetProperty(ref maxVoltageTextValue, value); }
        }

        private string minVoltageTextValue;
        public string MinVoltageText
        {
            get { return minVoltageTextValue; }
            set { SetProperty(ref minVoltageTextValue, value); }
        }

        public SettingsViewModel()
        {
        }
    }
}

