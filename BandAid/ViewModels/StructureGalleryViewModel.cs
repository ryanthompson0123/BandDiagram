using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Band
{
    public class StructureGalleryViewModel : ObservableObject
    {
        private List<StructureGalleryItemViewModel> itemsValue;
        public List<StructureGalleryItemViewModel> Items
        {
            get { return itemsValue; }
            set { SetProperty(ref itemsValue, value); }
        }

        public StructureGalleryViewModel()
        {
            LoadItems();
        }

        private async void LoadItems()
        {
            var fileManager = DependencyService.Get<IFileManager>();

            var benchNames = await fileManager.EnumerateTestBenchesAsync();

            Items = benchNames.Select(n => new StructureGalleryItemViewModel(n)).ToList();
        }
    }
}