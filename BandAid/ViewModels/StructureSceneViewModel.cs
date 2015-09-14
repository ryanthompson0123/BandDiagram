using System.Collections.Generic;
using System.Linq;

namespace Band
{
    public class StructureSceneViewModel : ObservableObject
    {
        private PlotAnimationGrouping plotGroup;

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

        private Dictionary<int, Structure> stepsValue;
        public Dictionary<int, Structure> Steps
        {
            get { return stepsValue; }
            set
            {
                SetProperty(ref stepsValue, value);
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

        private int currentStepValue;
        public int CurrentStep
        {
            get { return currentStepValue; }
            set { SetProperty(ref currentStepValue, value); }
        }

        private bool needsScreenshotValue;
        public bool NeedsScreenshot
        {
            get { return needsScreenshotValue; }
            set { SetProperty(ref needsScreenshotValue, value); }
        }

        private void UpdatePlot()
        {
           plotGroup = PlotAnimationGrouping.Create(Steps.Keys
                .OrderBy(k => k)
                .Select(k => CreatePlot(Steps[k]))
                .ToList());

            Plots = plotGroup.Plots
                .Select(p => new PlotViewModel(plotGroup, p))
                .ToList();

            PrimaryYAxis = new AxisViewModel(plotGroup, AxisType.PrimaryY);
            XAxis = new AxisViewModel(plotGroup, AxisType.X);
        }

        private Plot CreatePlot(Structure structure)
        {
            return Plot.Create(structure, PlotType);
        }
    }
}
