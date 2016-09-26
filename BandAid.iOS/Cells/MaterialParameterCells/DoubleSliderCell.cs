using System;

using Foundation;
using UIKit;
using Band;
using System.ComponentModel;

namespace BandAid.iOS
{
	public partial class DoubleSliderCell : BaseParameterCell
	{
		public static readonly NSString Key = new NSString("doubleSliderCell");

		public NumericMaterialParameterViewModel ViewModel { get; set; }

		public UILabel TitleLabel
		{
			get { return titleLabel; }
		}

		public UITextField ValueField
		{
			get { return valueField; }
		}

		public UISlider ValueSlider
		{
			get { return valueSlider; }
		}

		protected DoubleSliderCell(IntPtr handle) 
			: base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Initialize()
		{
			TitleLabel.Text = ViewModel.TitleText;
			ValueField.Text = ViewModel.TextInputValue;
			ValueSlider.MinValue = (float)ViewModel.Minimum;
			ValueSlider.MaxValue = (float)ViewModel.Maximum;
			ValueSlider.Value = (float)ViewModel.Value;

			ValueField.ShouldChangeCharacters += ValueField_ShouldChangeCharacters;
			ValueField.EditingChanged += ValueField_EditingChanged;
			ValueSlider.ValueChanged += ValueSlider_ValueChanged;
			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		public override void OnSelected()
		{
			ValueField.BecomeFirstResponder();
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			ValueField.ShouldChangeCharacters -= ValueField_ShouldChangeCharacters;
			ValueField.ValueChanged -= ValueField_ValueChanged;
			ValueSlider.ValueChanged -= ValueSlider_ValueChanged;
			ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

			TitleLabel.Text = "";
			ValueField.Text = "0.0";
			ValueSlider.Value = 0.0f;
		}

		void ValueSlider_ValueChanged(object sender, EventArgs e)
		{
			ViewModel.SliderValue = ValueSlider.Value;
		}

		bool ValueField_ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
		{
			if (replacementString == "") return true;
			if (textField.Text == "" && replacementString == "-") return true;

			if (replacementString == "." && !textField.Text.Contains(".")) return true;

			double plug;

			if (range.Location == textField.Text.Length)	
			{
				return double.TryParse(textField.Text + replacementString, out plug);
			}
			else
			{
				var potentialString = 
					textField.Text
					.Remove((int)range.Location, (int)range.Length)
					.Insert((int)range.Location, replacementString);

				return double.TryParse(potentialString, out plug);
			}
		}

		void ValueField_ValueChanged(object sender, EventArgs e)
		{
			ViewModel.TextInputValue = ValueField.Text;
		}

		void ValueField_EditingChanged(object sender, EventArgs e)
		{
			ViewModel.TextInputValue = ValueField.Text;
		}

		void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "SliderValue":
					ValueSlider.Value = ViewModel.SliderValue;
					break;
				case "TextInputValue":
					ValueField.Text = ViewModel.TextInputValue;
					break;
			}
		}
	}
}