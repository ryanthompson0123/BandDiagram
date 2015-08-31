using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Band.ViewModels
{
    public class StructureGalleryItemViewModel : ObservableObject
    {
        public string Title { get; set; }
        public string DataFile { get; set; }
        public string ImageFile { get; set; }
    }
}