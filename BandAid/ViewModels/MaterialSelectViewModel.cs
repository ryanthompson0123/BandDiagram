using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Band.Units;

namespace Band
{
    public class MaterialSelectViewModel : ObservableObject
    {
        private ObservableCollection<MaterialViewModel> materialsValue;
        public ObservableCollection<MaterialViewModel> Materials
        {
            get { return materialsValue; }
            set { SetProperty(ref materialsValue, value); }
        }

        private Material selectedMaterialValue;
        public Material SelectedMaterial
        {
            get { return selectedMaterialValue; }
            set { SetProperty(ref selectedMaterialValue, value); }
        }

        private MaterialType materialTypeValue;
        public MaterialType MaterialType
        {
            get { return materialTypeValue; }
            set { SetProperty(ref materialTypeValue, value); }
        }

        private MaterialRepository materials;

        public MaterialSelectViewModel(MaterialType materialType)
        {
            materials = new MaterialRepository();
            MaterialType = materialType;
            Materials = new ObservableCollection<MaterialViewModel>();
            LoadMaterials();
        }

        private async void LoadMaterials()
        {
            var loadedMaterials = await materials.GetAsync(MaterialType);

            Materials.Clear();

            loadedMaterials
                .Select(m => new MaterialViewModel(m))
                .ForEach(Materials.Add);
        }

        public async void SaveMaterial(Material m)
        {
            await materials.PutAsync(m);
            LoadMaterials();
        }

        public async void DuplicateMaterial(Material m)
        {
            var duplicate = m.DeepClone();

            duplicate.Name = duplicate.Name + " Copy";
            duplicate.Id = Guid.NewGuid().ToString();

            await materials.PutAsync(duplicate);
            LoadMaterials();
        }

        public async void DeleteMaterial(Material m)
        {
            await materials.DeleteAsync(m);
            LoadMaterials();
        }
    }
}