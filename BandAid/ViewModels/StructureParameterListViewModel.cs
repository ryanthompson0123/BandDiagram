using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Band
{
    public class StructureParameterListViewModel : ObservableObject
    {
        private List<StructureParameterItemViewModel> parametersValue;
        public List<StructureParameterItemViewModel> Parameters
        {
            get { return parametersValue; }
            private set { SetProperty(ref parametersValue, value); }
        }

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

        public StructureParameterListViewModel(TestBench testBench)
        {
            Parameters = new List<StructureParameterItemViewModel>();

            TestBench = testBench;
        }

        private void TestBench_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Steps":
                case "CurrentIndex":
                    if (!TestBench.NeedsCompute)
                    {
                        UpdateParameters(TestBench.CurrentStructure);
                    }
                    break;
            }
        }

        private void UpdateParameters(Structure structure)
        {
            Parameters = structure.Layers.Select(l => new StructureParameterItemViewModel(l)).ToList();
        }
    }
}
