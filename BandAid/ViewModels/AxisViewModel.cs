//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//
//namespace Band
//{
//    
//
//    public class AxisViewModel : ObservableObject
//    {
//        private string titleTextValue;
//        public string TitleText
//        {
//            get { return titleTextValue; }
//            private set { SetProperty(ref titleTextValue, value); }
//        }
//
//        private AxisType axisTypeValue;
//        public AxisType AxisType
//        {
//            get { return axisTypeValue; }
//            private set { SetProperty(ref axisTypeValue, value); }
//        }
//
//        private PlotAxis axisBoundsValue;
//        public PlotAxis AxisBounds
//        {
//            get { return axisBoundsValue; }
//            private set { SetProperty(ref axisBoundsValue, value); }
//        }
//
//        private double majorAxisSpanValue;
//        public double MajorAxisSpan
//        {
//            get { return majorAxisSpanValue; }
//            private set { SetProperty(ref majorAxisSpanValue, value); }
//        }
//
//        private readonly PlotAnimationGrouping plotGroup;
//
//        public AxisViewModel(PlotAnimationGrouping plotGroup, AxisType type)
//        {
//            if (plotGroup == null)
//            {
//                throw new ArgumentNullException();
//            }
//
//            this.plotGroup = plotGroup;
//            AxisType = type;
//
//            SetUp();
//        }
//
//        private void SetUp()
//        {
//            switch (AxisType)
//            {
//                case AxisType.PrimaryY:
//                    TitleText = plotGroup.Plots[0].YAxisLabel;
//                    AxisBounds = plotGroup.YAxis;
//                    MajorAxisSpan = plotGroup.MajorYAxisSpan;
//                    break;
//                case AxisType.X:
//                    TitleText = plotGroup.Plots[0].XAxisLabel;
//                    AxisBounds = plotGroup.XAxis;
//                    MajorAxisSpan = plotGroup.MajorXAxisSpan;
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}