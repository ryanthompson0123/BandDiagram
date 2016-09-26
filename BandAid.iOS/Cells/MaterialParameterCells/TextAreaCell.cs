using System;

using Foundation;
using UIKit;
using Band;

namespace BandAid.iOS
{
	public partial class TextAreaCell : BaseParameterCell
	{
		public static readonly NSString Key = new NSString("textAreaCell");

		public MaterialParameterViewModel<string> ViewModel { get; set; }

		public UILabel TitleLabel
		{
			get { return titleLabel; }
		}

		public UITextView TextView
		{
			get { return textView; }
		}

		protected TextAreaCell(IntPtr handle) 
			: base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Initialize()
		{
			TitleLabel.Text = ViewModel.TitleText;
			TextView.Text = ViewModel.Value;

			TextView.Changed += TextView_Changed;
		}

		public override void OnSelected()
		{
			TextView.BecomeFirstResponder();
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			TextView.Changed -= TextView_Changed;

			TitleLabel.Text = "";
			TextView.Text = "";
		}

		void TextView_Changed(object sender, EventArgs e)
		{
			ViewModel.Value = TextView.Text;
		}
	}
}