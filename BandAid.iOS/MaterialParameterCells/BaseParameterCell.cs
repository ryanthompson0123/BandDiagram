using System;
using UIKit;

namespace BandAid.iOS
{
	public abstract class BaseParameterCell : UITableViewCell
	{
		protected BaseParameterCell(IntPtr handle)
			: base(handle)
		{
		}

		public virtual void Initialize()
		{
		}

		public virtual void OnSelected()
		{
		}
	}
}

