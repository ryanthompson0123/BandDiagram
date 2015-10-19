using Band.Units;
using System;

namespace Band
{
	public class EvalPoint : IComparable<EvalPoint>
	{
		public Length Location { get; set; }
        public ChargeDensity ChargeDensity { get; set; }
		public ElectricField ElectricField { get; set; }
		public ElectricPotential Potential { get; set; }

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

			return this.ChargeDensity == that.ChargeDensity
                && this.ElectricField == that.ElectricField
                && this.Location == that.Location
			    && this.Potential == that.Potential;
		}

        public EvalPoint()
        {
            Location = Length.Zero;
            ChargeDensity = ChargeDensity.Zero;
            ElectricField = ElectricField.Zero;
            Potential = ElectricPotential.Zero;
        }

        public EvalPoint DeepClone()
        {
            return new EvalPoint
            {
                Location = Location,
                ChargeDensity = ChargeDensity,
                ElectricField = ElectricField,
                Potential = Potential
            };
        }

		public override int GetHashCode()
		{
            return ChargeDensity.GetHashCode() 
                ^ ElectricField.GetHashCode()
			    ^ Location.GetHashCode()
                ^ Potential.GetHashCode();
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