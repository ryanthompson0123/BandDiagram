using System;

using Foundation;
using UIKit;
using Band;

namespace BandAid.iOS
{
	public partial class TextInputCell : BaseParameterCell
	{
		public static readonly NSString Key = new NSString("textInputCell");

		public MaterialParameterViewModel<string> ViewModel { get; set; }
		public UILabel TitleLabel
		{
			get { return titleLabel; }
		}

		public UITextField TextInput
		{
			get { return textInput; }
		}

		protected TextInputCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Initialize()
		{
			TitleLabel.Text = ViewModel.TitleText;
			TextInput.Text = ViewModel.Value;

			TextInput.ValueChanged += TextInput_ValueChanged;
		}

		public override void OnSelected()
		{
			TextInput.BecomeFirstResponder();
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			TextInput.ValueChanged -= TextInput_ValueChanged;

			TitleLabel.Text = "";
			TextInput.Text = "";
		}

		void TextInput_ValueChanged(object sender, EventArgs e)
		{
			ViewModel.Value = TextInput.Text;
		}
	}
}