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
    public enum PlotType
    {
        Energy, Potential, ChargeDensity, ElectricField
    }

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
            set { SetProperty(ref needsScreenshotValue, value); }
        }

        // Don't observe these because they get created once and passed to their respective
        // views. Those views will observe these objects individually.
        public StructureParameterListViewModel StructureParameterList { get; set; }
        public PlotViewModel Plot { get; set; }

        public TestBenchViewModel(TestBench testBench)
        {
            TestBench = testBench;

            StructureParameterList = new StructureParameterListViewModel();
            Plot = new PlotViewModel();
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
                    Plot.Steps = TestBench.Steps;
                    break;
            }
        }

        public void SetPlotType(PlotType plotType)
        {
            Plot.PlotType = plotType;
            NeedsScreenshot = true;
        }
    }
}