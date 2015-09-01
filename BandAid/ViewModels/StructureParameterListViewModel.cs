using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Structure structureValue;
        public Structure Structure
        {
            get { return structureValue; }
            set
            {
                SetProperty(ref structureValue, value);
                UpdateItems();
            }
        }

        private void UpdateItems()
        {
            Parameters = Structure.Layers.Select(l => new StructureParameterItemViewModel(l)).ToList();
        }
    }
}
