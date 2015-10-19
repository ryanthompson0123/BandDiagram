using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Band
{
    public class StructureSceneViewModel : ObservableObject
    {
        private PlotAnimationGrouping plotGroup;

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

        private PlotType plotTypeValue;
        public PlotType PlotType
        {
            get { return plotTypeValue; }
            set
            {
                SetProperty(ref plotTypeValue, value);
                UpdatePlot();
            }
        }

        private List<PlotViewModel> plotsValue;
        public List<PlotViewModel> Plots
        {
            get { return plotsValue; }
            set { SetProperty(ref plotsValue, value); }
        }

        private AxisViewModel primaryYAxisValue;
        public AxisViewModel PrimaryYAxis
        {
            get { return primaryYAxisValue; }
            set { SetProperty(ref primaryYAxisValue, value); }
        }

        private AxisViewModel xAxisValue;
        public AxisViewModel XAxis
        {
            get { return xAxisValue; }
            set { SetProperty(ref xAxisValue, value); }
        }

        private int currentPlotIndexValue;
        public int CurrentPlotIndex
        {
            get { return currentPlotIndexValue; }
            set { SetProperty(ref currentPlotIndexValue, value); }
        }

        private bool needsScreenshotValue;
        public bool NeedsScreenshot
        {
            get { return needsScreenshotValue; }
            set { SetProperty(ref needsScreenshotValue, value); }
        }

        private string structureNameValue;
        public string StructureName
        {
            get { return structureNameValue; }
            set { SetProperty(ref structureNameValue, value); }
        }

        public StructureSceneViewModel(TestBench testBench)
        {
            TestBench = testBench;
            CurrentPlotIndex = testBench.CurrentIndex;
            UpdatePlot();
        }

        private void TestBench_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Steps":
                    UpdatePlot();
                    break;
                case "CurrentIndex":
                    CurrentPlotIndex = TestBench.CurrentIndex;
                    break;
            }
        }

        private void UpdatePlot()
        {
            if (TestBench.Steps.Count == 0) return;

            plotGroup = PlotAnimationGrouping.Create(TestBench.Steps
                .Select(s => CreatePlot(s))
                .ToList());
            
            PrimaryYAxis = new AxisViewModel(plotGroup, AxisType.PrimaryY);
            XAxis = new AxisViewModel(plotGroup, AxisType.X);

            Plots = plotGroup.Plots
                .Select(p => new PlotViewModel(plotGroup, p))
                .ToList();
        }

        private Plot CreatePlot(Structure structure)
        {
            return Plot.Create(structure, PlotType);
        }
    }
}
