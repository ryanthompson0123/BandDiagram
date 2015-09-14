using System;
using Band.Units;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;

namespace Band
{
    public class TestBenchViewModel : ObservableObject
    {
        private TestBench testBenchValue;
        public TestBench TestBench
        {
            get { return testBenchValue; }
            set
            {
                SetProperty(ref testBenchValue, value);

                TestBench.PropertyChanged += TestBench_PropertyChanged;
            }
        }

        private bool needsScreenshotValue;
        public bool NeedsScreenshot
        {
            get { return needsScreenshotValue; }
            private set { SetProperty(ref needsScreenshotValue, value); }
        }

        private string titleTextValue;
        public string TitleText
        {
            get { return titleTextValue; }
            set
            {
                var oldName = titleTextValue;
                SetProperty(ref titleTextValue, value);

                TestBench.Name = value;
                RenameTestBench(oldName);
            }
        }

        private string currentVoltageTextValue;
        public string CurrentVoltageText
        {
            get { return currentVoltageTextValue; }
            private set { SetProperty(ref currentVoltageTextValue, value); }
        }

        private double biasSliderMaxValueValue;
        public double BiasSliderMaxValue
        {
            get { return biasSliderMaxValueValue; }
            private set { SetProperty(ref biasSliderMaxValueValue, value); }
        }

        private double biasSliderMinValueValue;
        public double BiasSliderMinValue
        {
            get { return biasSliderMinValueValue; }
            private set { SetProperty(ref biasSliderMinValueValue, value); }
        }

        public PlotType PlotType
        {
            set
            {
                Scene.PlotType = value;
                NeedsScreenshot = true;
            }
        }

        // Don't observe these because they get created once and passed to their respective
        // views. Those views will observe these objects individually.
        public StructureParameterListViewModel StructureParameterList { get; set; }
        public StructureSceneViewModel Scene { get; set; }

        public TestBenchViewModel(TestBench testBench)
        {
            TestBench = testBench;

            StructureParameterList = new StructureParameterListViewModel(TestBench.Structure);
            Scene = new StructureSceneViewModel();

            TitleText = TestBench.Name;
        }

        private void TestBench_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentStep":
                    StructureParameterList.Structure = TestBench.CurrentStructure;
                    break;
                case "Steps":
                    StructureParameterList.Structure = TestBench.CurrentStructure;
                    Scene.Steps = TestBench.Steps;
                    break;
                case "Name":
                    TitleText = TestBench.Name;
                    break;
                case "CurrentVoltage":
                    CurrentVoltageText = TestBench.CurrentVoltage.ToString();
                    break;
                case "MaxVoltage":
                    BiasSliderMaxValue = TestBench.MaxVoltage.Volts;
                    break;
                case "MinVoltage":
                    BiasSliderMinValue = TestBench.MinVoltage.Volts;
                    break;
            }
        }

        public void SetSelectedBias(double bias)
        {
            // This code 'snaps' the user's desired voltage to the nearest
            // step that we've calculated ahead of time.
            var roundingFactor = 1 / (float)TestBench.StepSize.Volts;
            var roundValue = bias * roundingFactor;
            var roundedValue = Math.Round(roundValue, MidpointRounding.AwayFromZero);
            var realValue = roundedValue / roundingFactor;

            // Set the current voltage to the snapped voltage.
            TestBench.CurrentVoltage = new ElectricPotential(realValue);
        }

        private async void RenameTestBench(string oldName)
        {
            // If we are setting for first time, or the name didn't change,
            // we don't need to do anything.
            if (string.IsNullOrEmpty(oldName) || oldName == TitleText) return;

            var fileManager = DependencyService.Get<IFileManager>();
            await fileManager.MoveTestBenchAsync(TestBench, oldName);
        }
    }
}