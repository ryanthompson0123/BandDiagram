using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Band
{
	public class ColorPickerPaletteViewModel : ObservableObject
	{
		public int Index { get; set; }


		private ObservableCollection<Color> colorsValue;
		public ObservableCollection<Color> Colors
		{
			get { return colorsValue; }
			set { SetProperty(ref colorsValue, value); }
		}

		public ColorPickerPaletteViewModel()
		{
			Colors = new ObservableCollection<Color>();
		}
	}

	public class ColorPickerViewModel : ObservableObject
	{
		private const int ColorCount = 24;

		public List<ColorPickerPaletteViewModel> Palettes { get; set; }

		private ColorPickerPaletteViewModel currentPaletteValue;
		public ColorPickerPaletteViewModel CurrentPalette
		{
			get { return currentPaletteValue; }
			set { SetProperty(ref currentPaletteValue, value); }
		}

		private Color selectedColorValue;
		public Color SelectedColor
		{
			get { return selectedColorValue; }
			set { SetProperty(ref selectedColorValue, value); }
		}

		public ColorPickerViewModel(Color startingColor)
		{
			Palettes = new List<ColorPickerPaletteViewModel>();
			InitColors();

            var startingHue = startingColor.GetHue();

            var startingIndex = (int)Math.Floor(startingHue * 12);

            CurrentPalette = Palettes[startingIndex];
		}

		private void InitColors()
		{
			for (var i = 0; i < 12; i++)
			{
				var palette = new ColorPickerPaletteViewModel
				{
					Index = i
				};

				var hue = (i * 30f / 360f);

				for (var j = 0; j < ColorCount; j++)
				{
					var row = j / 4;
					var column = j % 4;

					var saturation = column * 0.25f + 0.25f;
					var brightness = 1f - row * 0.12f;

                    var color = Color.FromHsb(hue, saturation, brightness);

					palette.Colors.Add(color);
				}

				Palettes.Add(palette);
			}
		}
	}
}