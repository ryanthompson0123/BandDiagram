using System;
using System.Collections.Generic;
using System.Linq;

namespace Band
{
    public class PlotViewModel : ObservableObject
    {
        private Plot plotValue;
        public Plot Plot
        {
            get { return plotValue; }
            private set { SetProperty(ref plotValue, value); }
        }

        public PlotAxisBounds XAxisBounds
        {
            get { return plotGroup.XAxisBounds; }
        }

        public PlotAxisBounds YAxisBounds
        {
            get { return plotGroup.YAxisBounds; }
        }

        public List<PlotDataSet> DataSets
        {
            get { return Plot.DataSets.ToList(); }
        }

        private readonly PlotAnimationGrouping plotGroup;

        public PlotViewModel(PlotAnimationGrouping plotGroup, Plot plot)
        {
            if (plotGroup == null || plot == null)
            {
                throw new ArgumentNullException();
            }

            this.plotGroup = plotGroup;
            Plot = plot;
        }
    }
}
