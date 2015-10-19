﻿using System;

namespace Band
{
    public class LayerViewModel : ObservableObject
    {
        private string nameTextValue;
        public string NameText
        {
            get { return nameTextValue; }
            set { SetProperty(ref nameTextValue, value); }
        }

        private string materialTypeTextValue;
        public string MaterialTypeText
        {
            get { return materialTypeTextValue; }
            set { SetProperty(ref materialTypeTextValue, value); }
        }

        public Material Material { get; set; }

        public LayerViewModel(Material material)
        {
            Material = material;
            NameText = material.Name;
            MaterialTypeText = material.MaterialType.ToString();
        }
    }
}

