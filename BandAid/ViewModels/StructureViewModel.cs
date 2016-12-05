using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Band
{
    public class StructureViewModel : ObservableObject
    {
        private LayerViewModel directEditLayerValue;
        public LayerViewModel DirectEditLayer
        {
            get { return directEditLayerValue; }
            set { SetProperty(ref directEditLayerValue, value); }
        }

        private ObservableCollection<LayerViewModel> layersValue;
        public ObservableCollection<LayerViewModel> Layers
        {
            get { return layersValue; }
            set { SetProperty(ref layersValue, value); }
        }

        public bool CurrentLayoutIsInvalid
        {
            get { return !structure.IsValid; }
        }

        public bool CurrentLayoutHasNoSolution
        {
            get { return structure.NoSolution; }
        }

        private Structure structure;

        public StructureViewModel(Structure structure)
        {
            this.structure = structure;

            Layers = new ObservableCollection<LayerViewModel>(
                structure.Layers.Select(l => new LayerViewModel(l)));
        }

        public void MoveLayer(LayerViewModel viewModel, int position)
        {
            structure.MoveLayer(viewModel.Material, position);
            Layers.Move(Layers.IndexOf(viewModel), position);
        }

        public void DeleteLayer(LayerViewModel viewModel)
        {
            structure.RemoveLayer(viewModel.Material);
            Layers.Remove(viewModel);
        }

        public void DuplicateLayer(LayerViewModel viewModel)
        {
            var duplicateMaterial = viewModel.Material.DeepClone();
            var newIndex = Layers.IndexOf(viewModel);
            var newViewModel = new LayerViewModel(duplicateMaterial);

            structure.InsertLayer(newIndex + 1, duplicateMaterial);
            Layers.Insert(newIndex + 1, newViewModel);
        }

        public void AddLayer(LayerViewModel viewModel)
        {
            // If the bottom layer is a semiconductor or metal, then there's no point
            // in adding this material below that, because it's not valid.
            if (structure.Layers.Count > 0 && structure.BottomLayer.MaterialType != MaterialType.Dielectric)
            {
                Layers.Insert(Layers.Count - 1, viewModel);
            }
            else
            {
                Layers.Add(viewModel);
            }

            structure.AddLayer(viewModel.Material);
        }

        public void ReplaceLayer(LayerViewModel viewModel, int position)
        {
            Layers.RemoveAt(position);
            Layers.Insert(position, viewModel);

            structure.ReplaceLayer(viewModel.Material, position);
        }

        public void SetDirectEditMaterial(Material material)
        {
            DirectEditLayer = Layers.FirstOrDefault(l => l.Material == material);
        }
    }
}
