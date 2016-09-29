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
        public const string Phi = "Φ";
        public const string Chi = "χ";
        public const string Kappa = "κ";
        public const string UpArrowLeftAligned = " ▲";
        public const string UpArrow = "▲";
        public const string DownArrowLeftAligned = " ▼";
        public const string DownArrow = "▼";

        private ObservableCollection<MaterialViewModel> materialsValue;
        public ObservableCollection<MaterialViewModel> Materials
        {
            get { return materialsValue; }
            set { SetProperty(ref materialsValue, value); }
        }

        private ObservableCollection<string> columnHeadersValue;
        public ObservableCollection<string> ColumnHeaders
        {
            get { return columnHeadersValue; }
            set { SetProperty(ref columnHeadersValue, value); }
        }

        private List<string> headerHintsValue;
        public List<string> HeaderHints
        {
            get { return headerHintsValue; }
            set { SetProperty(ref headerHintsValue, value); }
        }

        private string tableTitleValue;
        public string TableTitle
        {
            get { return tableTitleValue; }
            set { SetProperty(ref tableTitleValue, value); }
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

        private bool sortByTitleValue;
        public bool SortByTitle
        {
            get { return sortByTitleValue; }
            set { SetProperty(ref sortByTitleValue, value); }
        }

        private int columnSortIndexValue;
        public int ColumnSortIndex
        {
            get { return columnSortIndexValue; }
            set { SetProperty(ref columnSortIndexValue, value); }
        }

        private bool sortDescendingValue;
        public bool SortDescending
        {
            get { return sortDescendingValue; }
            set { SetProperty(ref sortDescendingValue, value); }
        }

        private MaterialRepository materials;

        public MaterialSelectViewModel(MaterialType materialType)
        {
            materials = new MaterialRepository();
            Materials = new ObservableCollection<MaterialViewModel>();
            ColumnHeaders = new ObservableCollection<string>();
            HeaderHints = new List<string>();

            MaterialType = materialType;
            SortByTitle = true;
            ColumnSortIndex = -1;

            SetUpColumnHeaders();
            LoadMaterials();
        }

        private void SetUpColumnHeaders()
        {
            TableTitle = string.Format("Name{0}", SortByTitle ? SortDescending ? DownArrowLeftAligned : UpArrowLeftAligned : "");

            ColumnHeaders.Clear();
            HeaderHints.Clear();

            switch (MaterialType)
            {
                case MaterialType.Metal:
                    ColumnHeaders.Add(GetSortedLabel(0, Phi));
                    HeaderHints.Add("Work Function (eV)");
                    break;
                case MaterialType.Dielectric:
                    ColumnHeaders.Add(GetSortedLabel(0, Kappa));
                    HeaderHints.Add("Dielectric Constant");
                    ColumnHeaders.Add(GetSortedLabel(1, "Eg"));
                    HeaderHints.Add("Band Gap (eV)");
                    ColumnHeaders.Add(GetSortedLabel(2, Phi));
                    HeaderHints.Add("Electron Affinity (eV)");
                    break;
                case MaterialType.Semiconductor:
                    ColumnHeaders.Add(GetSortedLabel(0, Kappa));
                    HeaderHints.Add("Dielectric Constant");
                    ColumnHeaders.Add(GetSortedLabel(1, "Eg"));
                    HeaderHints.Add("Band Gap (eV)");
                    ColumnHeaders.Add(GetSortedLabel(2, Phi));
                    HeaderHints.Add("Electron Affinity (eV)");
                    ColumnHeaders.Add(GetSortedLabel(3, "ni"));
                    HeaderHints.Add("Intrinsic Carrier Concentration (cm\u207a\u00b2)");
                    break;
            }
        }

        private string GetSortedLabel(int index, string title)
        {
            if (ColumnSortIndex != index) return title;


            return string.Format("{1} {0}", title, SortDescending ? DownArrow : UpArrow);
        }

        private async void LoadMaterials()
        {
            var loadedMaterials = await materials.GetAsync(MaterialType);

            Materials.Clear();

            var mats =
                loadedMaterials
                .Select(m => new MaterialViewModel(m));


            if (SortByTitle)
            {
                if (SortDescending)
                {
                    mats = mats.OrderByDescending(m => m.TitleText);
                }
                else
                {
                    mats = mats.OrderBy(m => m.TitleText);
                }
            }
            else
            {
                if (SortDescending)
                {
                    mats = mats.OrderByDescending(m => m.GetSortValue(ColumnSortIndex));
                }
                else
                {
                    mats = mats.OrderBy(m => m.GetSortValue(ColumnSortIndex));
                }
            }

            mats.ForEach(Materials.Add);
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

        public void OnTitleClicked()
        {
            if (SortByTitle)
            {
                SortDescending = !SortDescending;
            }

            SortByTitle = true;
            ColumnSortIndex = -1;
            SetUpColumnHeaders();
            LoadMaterials();
        }

        public void OnColumnClicked(int index)
        {
            if (ColumnSortIndex == index)
            {
                SortDescending = !SortDescending;
            }

            ColumnSortIndex = index;
            SortByTitle = false;
            SetUpColumnHeaders();
            LoadMaterials();
        }
    }
}