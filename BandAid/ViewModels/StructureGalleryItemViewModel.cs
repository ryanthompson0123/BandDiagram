using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Band
{
    public class StructureGalleryItemViewModel : ObservableObject
    {
        private bool isSelectedValue;
        public bool IsSelected
        {
            get { return isSelectedValue; }
            set { SetProperty(ref isSelectedValue, value); }
        }

        private string titleTextValue;
        public string TitleText
        {
            get { return titleTextValue; }
            private set { SetProperty(ref titleTextValue, value); }
        }

        private string dataFileValue;
        public string DataFile
        {
            get { return dataFileValue; }
            private set { SetProperty(ref dataFileValue, value); }
        }

        private string imageFileValue;
        public string ImageFile
        {
            get { return imageFileValue; }
            private set { SetProperty(ref imageFileValue, value); }
        }

        public StructureGalleryItemViewModel(string title)
        {
            var fileManager = DependencyService.Get<IFileManager>();
            TitleText = title;
            ImageFile = fileManager.GetScreenshotPath(title);
        }
    }
}