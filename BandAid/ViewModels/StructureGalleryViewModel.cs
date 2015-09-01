using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Band.ViewModels
{
    public class StructureGalleryViewModel : ObservableObject
    {
        private ObservableCollection<StructureGalleryItemViewModel> itemsValue;
        public ObservableCollection<StructureGalleryItemViewModel> Items
        {
            get { return itemsValue; }
            private set { SetProperty(ref itemsValue, value); }
        }
    }
}