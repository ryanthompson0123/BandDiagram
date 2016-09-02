using System;
using System.Collections.Generic;
using System.Reflection;
using Band.Units;

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
        SemiconductorBandGap,
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

        public MaterialDetailViewModel(MaterialType materialType, EditMode mode)
        {
            EditMode = mode;
            Material = Material.Create(materialType);

            MaterialParameterSections = BuildForm(Material);
        }

		public MaterialDetailViewModel(Material material, EditMode mode)
		{
			EditMode = mode;
            if (mode != EditMode.InStructure)
            {
                if (material.MaterialType == MaterialType.Semiconductor)
                {
                    Material = material.WithThickness(Length.FromNanometers(50.0));
                }
                else
                {
                    Material = material.WithThickness(Length.FromNanometers(5.0));
                }
            }
            else
            {
                Material = material;
            }

			MaterialParameterSections = BuildForm(Material);
		}

		private List<List<MaterialParameterViewModel>> BuildForm(Material material)
		{
			var form = new List<List<MaterialParameterViewModel>>();

			form.Add(GetNameSection(material));
			form.Add(GetTypeSpecificParameterSection(material));
			form.Add(GetPlotColorSection(material));
			form.Add(GetNotesSection(material));

			return form;
		}

		private static List<MaterialParameterViewModel> GetNameSection(Material material)
		{
			var titleSection = new List<MaterialParameterViewModel>();
            var titleField = new MaterialParameterViewModel<string>(ParameterType.Name)
			{
				Value = material.Name
			};

            titleField.PropertyChanged += (sender, e) => material.Name = titleField.Value;

            titleSection.Add(titleField);
			return titleSection;
		}

        private static NumericMaterialParameterViewModel GetThicknessSection(Material material)
		{
            var thicknessField = new NumericMaterialParameterViewModel(ParameterType.Thickness)
            {
                Minimum = 0.1,
                Maximum = 10.0,
                StepSize = 0.1,
                Value = material.Thickness.Nanometers
            };

            thicknessField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.Thickness = Length.FromNanometers(thicknessField.Value);
            };

            return thicknessField;
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
            var metalField = new NumericMaterialParameterViewModel(ParameterType.WorkFunction)
			{
				Minimum = 0.0,
				Maximum = 10,
				StepSize = 0.1,
                Value = material.WorkFunction?.ElectronVolts ?? 0.0
			};

            metalField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.WorkFunction = Energy.FromElectronVolts(metalField.Value);
            };

            metalSection.Add(GetThicknessSection(material));
            metalSection.Add(metalField);
			return metalSection;
		}

		private static List<MaterialParameterViewModel> GetDielectricParameterSection(Dielectric material)
		{
			var dielectricSection = new List<MaterialParameterViewModel>();
            var bandGapField = new NumericMaterialParameterViewModel(ParameterType.BandGap)
			{
				Minimum = 0,
				Maximum = 10,
				StepSize = 0.1,
				Value = material.BandGap?.ElectronVolts ?? 0.0
			};

            bandGapField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.BandGap = Energy.FromElectronVolts(bandGapField.Value);
            };

            var eaField = new NumericMaterialParameterViewModel(ParameterType.ElectronAffinity)
			{
				Minimum = 0,
				Maximum = 5,
				StepSize = 0.05,
				Value = material.ElectronAffinity?.ElectronVolts ?? 0.0
			};

            eaField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.ElectronAffinity = Energy.FromElectronVolts(eaField.Value);
            };

            var dcField = new NumericMaterialParameterViewModel(ParameterType.DielectricConstant)
			{
				Minimum = .1,
				Maximum = 30,
				StepSize = 1,
				Value = material.DielectricConstant
			};

            dcField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.DielectricConstant = dcField.Value;
            };

            dielectricSection.Add(GetThicknessSection(material));
            dielectricSection.Add(bandGapField);
            dielectricSection.Add(eaField);
            dielectricSection.Add(dcField);

			return dielectricSection;
		}

		private static List<MaterialParameterViewModel> GetSemiconductorParameterSection(Semiconductor material)
		{
			var semiconductorSection = new List<MaterialParameterViewModel>();
            var bgField = new MaterialParameterViewModel<string>(ParameterType.SemiconductorBandGap)
			{
                Value = material.BandGap?.Expression
			};

            bgField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.BandGap = new MathExpression<Energy>(bgField.Value);
            };

            semiconductorSection.Add(bgField);

            var eaField = new NumericMaterialParameterViewModel(ParameterType.ElectronAffinity)
			{
				Minimum = 0,
				Maximum = 5,
				StepSize = 0.05,
                Value = material.ElectronAffinity?.ElectronVolts ?? 0.0
			};

            eaField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.ElectronAffinity = Energy.FromElectronVolts(eaField.Value);
            };

            semiconductorSection.Add(eaField);

            var dcField = new NumericMaterialParameterViewModel(ParameterType.DielectricConstant)
			{
				Minimum = 0,
				Maximum = 30,
				StepSize = 1,
				Value = material.DielectricConstant
			};

            dcField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.DielectricConstant = dcField.Value;
            };

            semiconductorSection.Add(dcField);

            var iccField = new MaterialParameterViewModel<string>(ParameterType.IntrinsicCarrierConcentration)
			{
                Value = material.IntrinsicCarrierConcentration?.Expression
			};

            iccField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.IntrinsicCarrierConcentration = new MathExpression<Concentration>(iccField.Value);
            };

            semiconductorSection.Add(iccField);

            var dtField = new MaterialParameterViewModel<DopingType>(ParameterType.DopingType)
			{
				Value = material.DopingType
			};

            dtField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.DopingType = dtField.Value;
            };

            semiconductorSection.Add(dtField);

            var dopCField = new MaterialParameterViewModel<string>(ParameterType.DopantConcentration)
			{
                Value = material.DopantConcentration?.Expression
			};

            dopCField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.DopantConcentration = new MathExpression<Concentration>(dopCField.Value);
            };

            semiconductorSection.Add(dopCField);

            var tempField = new NumericMaterialParameterViewModel(ParameterType.Temperature)
            {
                Minimum = 100.0,
                Maximum = 500.0,
                StepSize = 25.0,
                Value = material.Temperature?.Kelvin ?? 0.0
            };

            tempField.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "Value") return;
                material.Temperature = new Temperature(tempField.Value);
            };

            semiconductorSection.Add(tempField);

			return semiconductorSection;
		}

		private static List<MaterialParameterViewModel> GetPlotColorSection(Material material)
		{
			var plotColorSection = new List<MaterialParameterViewModel>();
            var plotColorField = new MaterialParameterViewModel<Color>(ParameterType.PlotColor)
            {
                Value = material.FillColor
            };

            plotColorField.PropertyChanged += (sender, e) => material.FillColor = plotColorField.Value;

            plotColorSection.Add(plotColorField);
			return plotColorSection;
		}

		private static List<MaterialParameterViewModel> GetNotesSection(Material material)
		{
			var notesSection = new List<MaterialParameterViewModel>();
            var notesField = new MaterialParameterViewModel<string>(ParameterType.Notes)
			{
				Value = material.Notes
			};

            notesField.PropertyChanged += (sender, e) => material.Notes = notesField.Value;

            notesSection.Add(notesField);
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
                case ParameterType.SemiconductorBandGap:
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

                if (ShouldDebounceValue())
                {
                    if (SetPropertyDebounced(ref valueValue, value))
                    {
                        OnValueSet(value);
                    }
                }
                else
                {
                    if (SetProperty(ref valueValue, value))
                    {
                        OnValueSet(value);
				    }
                }
			}
		}

		protected virtual bool ShouldSetValue(TValue value)
		{
			return true;
		}

        protected virtual bool ShouldDebounceValue()
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
            if (Math.Abs(value - GetRoundedValue((float)Value)) <= double.Epsilon) return false;
            if (value < Minimum) return false;

            return true;
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