using Band.Units;
using System;

namespace Band
{
	public class EvalPoint : IComparable<EvalPoint>
	{
		public Length Location { get; set; }
		public ElectricCharge Charge { get; set; }
		public ElectricField ElectricField { get; set; }
		public ElectricPotential Potential { get; set; }

		public double ElectronCharge
		{
			get
			{
				return Charge / ElectricCharge.Elementary;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var that = obj as EvalPoint;
			if (that == null)
			{
				return false;
			}

			return this.Equals(that);
		}

		public bool Equals(EvalPoint that)
		{
			if (that == null)
			{
				return false;
			}

			if (ReferenceEquals(this, that))
			{
				return true;
			}

			return this.Charge == that.Charge && this.ElectricField == that.ElectricField &&
			this.ElectronCharge == that.ElectronCharge && this.Location == that.Location
			&& this.Potential == that.Potential;
		}

		public override int GetHashCode()
		{
			return Charge.GetHashCode() ^ ElectricField.GetHashCode() ^ ElectronCharge.GetHashCode()
			^ Location.GetHashCode() ^ Potential.GetHashCode();
		}

		public int CompareTo(EvalPoint other)
		{
			if (other == null)
			{
				return 1;
			}

			if (Location < other.Location) {
				return -1;
			}

			if (Location > other.Location)
			{
				return 1;
			}
				
			return 0;
		}
	}
}
