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
            TextInput.Placeholder = "Enter value";

            TextInput.EditingChanged += TextInput_EditingChanged;
		}

		public override void OnSelected()
		{
			TextInput.BecomeFirstResponder();
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();
            TextInput.EditingChanged -= TextInput_EditingChanged;

            TitleLabel.Text = "";
			TextInput.Text = "";
            TextInput.Placeholder = "";
		}

		void TextInput_EditingChanged(object sender, EventArgs e)
		{
			ViewModel.Value = TextInput.Text;
		}
	}
}