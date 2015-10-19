using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Band
{
    public class MaterialRepository
    {
        public MaterialRepository()
        {
        }

        public async Task<IEnumerable<Material>> GetMaterialsAsync(MaterialType materialType)
        {
            var mgr = DependencyService.Get<IFileManager>();
            var dataStr = await mgr.GetMaterialDataAsync(materialType);
            return JsonConvert.DeserializeObject<List<Material>>(dataStr);
        }
    }
}

