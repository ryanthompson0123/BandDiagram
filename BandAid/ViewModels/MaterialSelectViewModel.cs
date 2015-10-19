using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
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

        public MaterialSelectViewModel(MaterialType materialType)
        {
            MaterialType = materialType;
            LoadMaterials();
        }

        private async void LoadMaterials()
        {
            var repo = new MaterialRepository();

            var loadedMaterials = await repo.GetMaterialsAsync(MaterialType);

            Materials = new ObservableCollection<MaterialViewModel>(
                loadedMaterials.Select(m => new MaterialViewModel(m)));
        }
    }
}