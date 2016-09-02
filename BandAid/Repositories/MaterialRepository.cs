using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;

namespace Band
{
    public class MaterialRepository
    {
        IFileManager files;
        public MaterialRepository()
        {
            files = DependencyService.Get<IFileManager>();
        }

        public async Task<IEnumerable<Material>> GetAsync(MaterialType materialType)
        {
            var dataStr = await files.GetMaterialDataAsync(materialType);
            return JsonConvert.DeserializeObject<List<Material>>(dataStr);
        }

        public async Task DeleteAsync(Material material)
        {
            var materials = (await GetAsync(material.MaterialType)).ToList();

            var target = materials.FirstOrDefault(m => m.Id == material.Id);

            if (target != null)
            {
                materials.Remove(target);
            }

            var dataStr = JsonConvert.SerializeObject(materials);

            await files.PutMaterialDataAsync(material.MaterialType, dataStr);
        }

        public async Task PutAsync(Material material)
        {
            var materials = (await GetAsync(material.MaterialType)).ToList();

            var target = materials.FirstOrDefault(m => m.Id == material.Id);

            if (target != null)
            {
                var targetIndex = materials.IndexOf(target);
                materials[targetIndex] = material;
            }
            else
            {
                materials.Add(material);
                materials = materials.OrderBy(m => m.Name).ToList();
            }

            var dataStr = JsonConvert.SerializeObject(materials);

            await files.PutMaterialDataAsync(material.MaterialType, dataStr);
        }
    }
}
