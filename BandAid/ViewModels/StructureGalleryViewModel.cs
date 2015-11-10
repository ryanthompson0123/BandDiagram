using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

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

        private bool isEditingValue;
        public bool IsEditing
        {
            get { return isEditingValue; }
            set
            {
                SetProperty(ref isEditingValue, value);
                UpdateTitleText();
            }
        }

        private string titleTextValue;
        public string TitleText
        {
            get { return titleTextValue; }
            set { SetProperty(ref titleTextValue, value); }
        }

        public StructureGalleryViewModel()
        {
            LoadItems();
        }

        public async void LoadItems()
        {
            await LoadItemsAsync();
        }

        private async Task LoadItemsAsync()
        {
            var fileManager = DependencyService.Get<IFileManager>();

            var benchNames = await fileManager.EnumerateTestBenchesAsync();

            RemoveItemEventHandlers();

            Items = benchNames.OrderBy(n => n).Select(n => new StructureGalleryItemViewModel(n)).ToList();

            AddItemEventHandlers();
        }

        public async Task DeleteSelectedItemsAsync()
        {
            var fileManager = DependencyService.Get<IFileManager>();
            var selectedItems = Items.Where(i => i.IsSelected).ToList();

            foreach (var item in selectedItems)
            {
                await fileManager.DeleteTestBenchAsync(item.TitleText);
                item.PropertyChanged -= Item_PropertyChanged;
                Items.Remove(item);
            }
        }

        public async Task DuplicateSelectedItemsAsync()
        {
            var fileManager = DependencyService.Get<IFileManager>();
            var selectedItems = Items.Where(i => i.IsSelected).ToList();

            foreach (var item in selectedItems)
            {
                // Build the name for the copy.
                var dupName = FindNextName(string.Format("{0} - Copy", item.TitleText));

                // Load the source, change the name, and save the copy.
                var testBench = await fileManager.LoadTestBenchAsync(item.TitleText);
                testBench.Name = dupName;
                await fileManager.SaveTestBenchAsync(testBench);

                // Copy the screenshot.
                await fileManager.CopyScreenshotAsync(item.TitleText, dupName);

                // Insert the new test bench.
                var newItem = new StructureGalleryItemViewModel(dupName);
                newItem.PropertyChanged += Item_PropertyChanged;

                Items.Insert(Items.IndexOf(item) + 1, newItem);
            }
        }

        private string FindNextName(string name)
        {
            var nextName = name;
            var nameTry = 0;

            while (Items.Any(i => i.TitleText == nextName))
            {
                nameTry++;
                nextName = string.Format("{0} {1}", name, nameTry);
            }

            return nextName;
        }

        private void RemoveItemEventHandlers()
        {
            if (Items == null) return;

            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void AddItemEventHandlers()
        {
            if (Items == null) return;

            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsSelected":
                    UpdateTitleText();
                    break;
            }
        }

        private void UpdateTitleText()
        {
            if (IsEditing)
            {
                var selectedCount = Items.Count(i => i.IsSelected);

                if (selectedCount == 0)
                {
                    TitleText = "Select a Structure";
                }
                else if (selectedCount == 1)
                {
                    TitleText = "1 Structure Selected";
                }
                else
                {
                    TitleText = string.Format("{0} Structures Selected", selectedCount);
                }
            }
            else
            {
                TitleText = "Structures";
            }
        }
    }
}