using System;
using System.Collections.Generic;

namespace Band
{
	public enum ParameterType
	{
		Name,
		Notes,
		ElectronAffinity,
		DielectricConstant,
		WorkFunction,
		BandGap,
		IntrinsicCarrierConcentration,
		DopantConcentration,
		DopingType,
		PlotColor,
		Thickness,
		Temperature
	}

	public enum EditMode
	{
		New,
		Existing,
		InStructure
	}

	public class MaterialDetailViewModel : ObservableObject
	{
		public List<List<MaterialParameterViewModel>> MaterialParameterSections { get; private set; }

		public EditMode EditMode { get; private set; }
		public Material Material { get; private set; }

		public MaterialDetailViewModel(Material material, EditMode mode)
		{
			EditMode = mode;
			Material = Material;

			MaterialParameterSections = BuildForm(material);
		}

		private List<List<MaterialParameterViewModel>> BuildForm(Material material)
		{
			var form = new List<List<MaterialParameterViewModel>>();

			form.Add(GetNameSection(material));

			if (material.MaterialType != MaterialType.Semiconductor)
			{
				form.Add(GetThicknessSection(material));
			}
			else
			{
				form.Add(GetTemperatureSection());
			}

			form.Add(GetTypeSpecificParameterSection(material));
			form.Add(GetPlotColorSection(material));
			form.Add(GetNotesSection(material));

			return form;
		}

		private static List<MaterialParameterViewModel> GetNameSection(Material material)
		{
			var titleSection = new List<MaterialParameterViewModel>();

			titleSection.Add(new MaterialParameterViewModel<string>(ParameterType.Name)
			{
				Value = material.Name
			});

			return titleSection;
		}

		private static List<MaterialParameterViewModel> GetThicknessSection(Material material)
		{
			var thicknessSection = new List<MaterialParameterViewModel>();

			thicknessSection.Add(new NumericMaterialParameterViewModel(ParameterType.Thickness)
			{
				Minimum = 0.1,
				Maximum = 10.0,
				StepSize = 0.1,
				Value = material.Thickness != null ? material.Thickness.Nanometers : 5.0
			});

			return thicknessSection;
		}

		private static List<MaterialParameterViewModel> GetTemperatureSection()
		{
			var temperatureSection = new List<MaterialParameterViewModel>();

			temperatureSection.Add(new NumericMaterialParameterViewModel(ParameterType.Temperature)
			{
				Minimum = 100.0,
				Maximum = 500.0,
				StepSize = 25.0
			});

			return temperatureSection;
		}

		private List<MaterialParameterViewModel> GetTypeSpecificParameterSection(Material material)
		{
			switch (material.MaterialType)
			{
				case MaterialType.Metal:
					return GetMetalParameterSection((Metal)material);
				case MaterialType.Dielectric:
					return GetDielectricParameterSection((Dielectric)material);
				case MaterialType.Semiconductor:
					return GetSemiconductorParameterSection((Semiconductor)material);
				default:
					return new List<MaterialParameterViewModel>();
			}
		}

		private static List<MaterialParameterViewModel> GetMetalParameterSection(Metal material)
		{
			var metalSection = new List<MaterialParameterViewModel>();

			metalSection.Add(new NumericMaterialParameterViewModel(ParameterType.WorkFunction)
			{
				Minimum = 0.0,
				Maximum = 10,
				StepSize = 0.1,
				Value = material.WorkFunction.ElectronVolts,
			});

			return metalSection;
		}

		private static List<MaterialParameterViewModel> GetDielectricParameterSection(Dielectric material)
		{
			var dielectricSection = new List<MaterialParameterViewModel>();

			dielectricSection.Add(new NumericMaterialParameterViewModel(ParameterType.BandGap)
			{
				Minimum = 0,
				Maximum = 10,
				StepSize = 0.1,
				Value = material.BandGap.ElectronVolts
			});

			dielectricSection.Add(new NumericMaterialParameterViewModel(ParameterType.ElectronAffinity)
			{
				Minimum = 0,
				Maximum = 5,
				StepSize = 0.05,
				Value = material.ElectronAffinity.ElectronVolts
			});

			dielectricSection.Add(new NumericMaterialParameterViewModel(ParameterType.DielectricConstant)
			{
				Minimum = 0,
				Maximum = 30,
				StepSize = 1,
				Value = material.DielectricConstant
			});

			return dielectricSection;
		}

		private static List<MaterialParameterViewModel> GetSemiconductorParameterSection(Semiconductor material)
		{
			var semiconductorSection = new List<MaterialParameterViewModel>();

			semiconductorSection.Add(new NumericMaterialParameterViewModel(ParameterType.BandGap)
			{
				Minimum = 0,
				Maximum = 10,
				StepSize = 0.1,
				Value = material.BandGap.ElectronVolts
			});

			semiconductorSection.Add(new NumericMaterialParameterViewModel(ParameterType.ElectronAffinity)
			{
				Minimum = 0,
				Maximum = 5,
				StepSize = 0.05,
				Value = material.ElectronAffinity.ElectronVolts
			});

			semiconductorSection.Add(new NumericMaterialParameterViewModel(ParameterType.DielectricConstant)
			{
				Minimum = 0,
				Maximum = 30,
				StepSize = 1,
				Value = material.DielectricConstant
			});

			semiconductorSection.Add(new NumericMaterialParameterViewModel(ParameterType.IntrinsicCarrierConcentration)
			{
				Value = material.IntrinsicCarrierConcentration.PerCubicCentimeter
			});

			semiconductorSection.Add(new MaterialParameterViewModel<DopingType>(ParameterType.DopingType)
			{
				Value = material.DopingType
			});

			semiconductorSection.Add(new NumericMaterialParameterViewModel(ParameterType.DopantConcentration)
			{
				Value = material.DopantConcentration.PerCubicCentimeter
			});

			return semiconductorSection;
		}

		private static List<MaterialParameterViewModel> GetPlotColorSection(Material material)
		{
			var plotColorSection = new List<MaterialParameterViewModel>();
			plotColorSection.Add(new MaterialParameterViewModel<Color>(ParameterType.PlotColor)
			{
				Value = material.FillColor
			});

			return plotColorSection;
		}

		private static List<MaterialParameterViewModel> GetNotesSection(Material material)
		{
			var notesSection = new List<MaterialParameterViewModel>();
			notesSection.Add(new MaterialParameterViewModel<string>(ParameterType.Notes)
			{
				Value = material.Notes
			});

			return notesSection;
		}
	}

	public abstract class MaterialParameterViewModel : ObservableObject
	{
		public string TitleText { get; private set; }

		public ParameterType ParameterType { get; private set; }

		protected MaterialParameterViewModel(ParameterType type)
		{
			TitleText = GetTitle(type);
			ParameterType = type;
		}

		private static string GetTitle(ParameterType type)
		{
			switch (type)
			{
				case ParameterType.Name:
					return "Name";
				case ParameterType.Notes:
					return "Notes";
				case ParameterType.BandGap:
					return "Band Gap (eV)";
				case ParameterType.DopingType:
					return "Doping Type";
				case ParameterType.DielectricConstant:
					return "Dielectric Constant";
				case ParameterType.WorkFunction:
					return "Work Function (eV)";
				case ParameterType.ElectronAffinity:
					return "Electron Affinity (eV)";
				case ParameterType.IntrinsicCarrierConcentration:
					return "Intrinsic Carrier Concentration (cm-3)";
				case ParameterType.DopantConcentration:
					return "Dopant Concentration (cm-3)";
				case ParameterType.PlotColor:
					return "Plot Color";
				case ParameterType.Thickness:
					return "Thickness (nm)";
				case ParameterType.Temperature:
					return "Temperature (K)";
				default:
					return "Unknown";
			}
		}
	}

	public class MaterialParameterViewModel<TValue> : MaterialParameterViewModel
	{
		protected TValue valueValue;
		public TValue Value
		{
			get { return valueValue; }
			set
			{
				if (!ShouldSetValue(value)) return;

				if (SetProperty(ref valueValue, value))
				{
					OnValueSet(value);
				}
			}
		}

		protected virtual bool ShouldSetValue(TValue value)
		{
			return true;
		}

		protected virtual void OnValueSet(TValue value)
		{
		}

		public MaterialParameterViewModel(ParameterType type)
			: base(type)
		{
		}
	}

	public class NumericMaterialParameterViewModel : MaterialParameterViewModel<double>
	{
		public double Minimum { get; set; }
		public double Maximum { get; set; }
		public double StepSize { get; set; }

		private float sliderValueValue;
		public float SliderValue
		{
			get { return sliderValueValue; }
			set
			{
				if (SetProperty(ref sliderValueValue, value))
				{
					Value = GetRoundedValue(value);
				}
			}
		}

		private string textInputValueValue;
		public string TextInputValue
		{
			get { return textInputValueValue; }
			set
			{
				if (SetProperty(ref textInputValueValue, value))
				{
					double newValue;
					if (!double.TryParse(value, out newValue)) return;

					Value = newValue;
				}
			}
		}

		public NumericMaterialParameterViewModel(ParameterType type)
			: base(type)
		{
		}

		protected override bool ShouldSetValue(double value)
		{
			return Math.Abs(value - GetRoundedValue((float)Value)) > double.Epsilon;
		}

		protected override void OnValueSet(double value)
		{
			TextInputValue = GetFormattedValue(value);
			SliderValue = (float)value;
		}

		private string GetFormattedValue(double value)
		{
			var formatString = "{0:0.#############}";

			return string.Format(formatString, value);
		}

		private double GetRoundedValue(float value)
		{
			var steps = value / StepSize;

			var stepInt = Math.Round(steps, MidpointRounding.AwayFromZero);

			return stepInt * StepSize;
		}
	}
}