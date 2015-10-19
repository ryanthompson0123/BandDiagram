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
        public string TitleText { get; private set; }
        public string DataFile { get; private set; }
        public string ImageFile { get; private set; }

        public StructureGalleryItemViewModel(string title)
        {
            var fileManager = DependencyService.Get<IFileManager>();
            TitleText = title;
            ImageFile = fileManager.GetScreenshotPath(title);
        }
    }
}