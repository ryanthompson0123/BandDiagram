using System;

using Foundation;
using UIKit;
using Band;

namespace BandAid.iOS
{
	public partial class MultiButtonCell : BaseParameterCell
	{
		public static readonly NSString Key = new NSString("multiButtonCell");

		public MaterialParameterViewModel<DopingType> ViewModel { get; set; }

		public UILabel TitleLabel
		{
			get { return titleLabel; }
		}

		public UISegmentedControl ValueSegment
		{
			get { return this.valueSegment; }
		}

		protected MultiButtonCell(IntPtr handle) 
			: base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void Initialize()
		{
			TitleLabel.Text = ViewModel.TitleText;
			ValueSegment.SelectedSegment = ViewModel.Value == DopingType.N ? 0 : 1;

			ValueSegment.ValueChanged += ValueSegment_ValueChanged;
		}

		public override void PrepareForReuse()
		{
			base.PrepareForReuse();

			ValueSegment.ValueChanged -= ValueSegment_ValueChanged;

			TitleLabel.Text = "";
			ValueSegment.SelectedSegment = 0;
		}

		void ValueSegment_ValueChanged(object sender, EventArgs e)
		{
			ViewModel.Value = ValueSegment.SelectedSegment == 0 ? DopingType.N : DopingType.P;
		}
	}
}
