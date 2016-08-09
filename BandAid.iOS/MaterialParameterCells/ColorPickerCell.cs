using System;

using Foundation;
using UIKit;
using Band;
using System.ComponentModel;

namespace BandAid.iOS
{
	public partial class ColorPickerCell : BaseParameterCell
	{
		public static readonly NSString Key = new NSString("colorPickerCell");

		public MaterialParameterViewModel<string> ViewModel { get; set; }

		public UILabel TitleLabel
		{
			get { return titleLabel; }
		}

		public UIView ColorView
		{
			get { return colorDisplay; }
		}

		protected ColorPickerCell(IntPtr handle) 
			: base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Initialize()
		{
			TitleLabel.Text = ViewModel.TitleText;
			ColorView.BackgroundColor = CustomUIColor.FromHexString(ViewModel.Value);

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

			TitleLabel.Text = "";
			ColorView.BackgroundColor = UIColor.Clear;
		}

		void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Value":
					ColorView.BackgroundColor = CustomUIColor.FromHexString(ViewModel.Value);
					break;
			}
		}
	}
}