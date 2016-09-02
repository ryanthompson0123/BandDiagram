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

		public MaterialParameterViewModel<Color> ViewModel { get; set; }

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
            ColorView.BackgroundColor = ViewModel.Value.ToUIColor();

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
            InvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case "Value":
                        ColorView.BackgroundColor = ViewModel.Value.ToUIColor();
                        break;
                }
            });
		}
	}
}