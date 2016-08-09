using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Band
{
	public class ColorPickerPaletteViewModel : ObservableObject
	{
		public int Index { get; set; }


		private ObservableCollection<ColorPickerColorViewModel> colorsValue;
		public ObservableCollection<ColorPickerColorViewModel> Colors
		{
			get { return colorsValue; }
			set { SetProperty(ref colorsValue, value); }
		}

		public ColorPickerPaletteViewModel()
		{
			Colors = new ObservableCollection<ColorPickerColorViewModel>();
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

		private ColorPickerColorViewModel selectedColorValue;
		public ColorPickerColorViewModel SelectedColor
		{
			get { return selectedColorValue; }
			set { SetProperty(ref selectedColorValue, value); }
		}

		public ColorPickerViewModel()
		{
			Palettes = new List<ColorPickerPaletteViewModel>();
			InitColors();

			CurrentPalette = Palettes[0];
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

					var color = new ColorPickerColorViewModel
					{
						Hue = hue,
						Saturation = saturation,
						Brightness = brightness
					};

					palette.Colors.Add(color);
				}

				Palettes.Add(palette);
			}
		}
	}

	public class ColorPickerColorViewModel : ObservableObject
	{
		private float hueValue;
		public float Hue
		{
			get { return hueValue; }
			set { SetProperty(ref hueValue, value); }
		}

		private float saturationValue;
		public float Saturation
		{
			get { return saturationValue; }
			set { SetProperty(ref saturationValue, value); }
		}

		private float brightnessValue;
		public float Brightness
		{
			get { return brightnessValue; }
			set { SetProperty(ref brightnessValue, value); }
		}

		public string HexCode
		{
			get
			{
				var rgb = RgbValue;
				return string.Format("#{0:X2}{1:X2}{2:X2}", rgb[0], rgb[1], rgb[2]);
			}
		}

		// HSB -> RGB conversion from http://stackoverflow.com/a/19338652
		public int[] RgbValue
		{
			get
			{
				float r, g, b;

				if (Saturation <= float.Epsilon)
				{
					// Achromatic
					r = g = b = Brightness;
				}
				else
				{
					var q = Brightness < 0.5f ? Brightness * (1 + Saturation) : Brightness + Saturation - Brightness * Saturation;
					var p = 2 * Brightness - q;
					r = HueToRgb(p, q, Hue + 1.0f / 3.0f);
					g = HueToRgb(p, q, Hue);
					b = HueToRgb(p, q, Hue - 1f / 3f);
				}

				return new[] { (int)(r * 255), (int)(g * 255), (int)(b * 255) };
			}
		}

		private float HueToRgb(float p, float q, float t)
		{
			if (t < 0f) t += 1f;
			if (t > 1f) t -= 1f;
			if (t < 1f / 6f) return p + (q - p) * 6f *t;
			if (t < 1f / 2f) return q;
			if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
			return p;
		}
	}
}