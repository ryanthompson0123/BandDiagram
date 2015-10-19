using System;

namespace Band.Units
{
	public class ElectricField : IComparable<ElectricField>
	{
		public readonly double VoltsPerMeter;

		public ElectricField(double voltsPerMeter)
		{
			VoltsPerMeter = voltsPerMeter;
		}

        public static ElectricField Zero = new ElectricField(0);

		public double VoltsPerCentimeter
		{
			get
			{
				return VoltsPerMeter / 1E2;
			}
		}

		public double MegavoltsPerCentimeter
		{
			get
			{
				return VoltsPerMeter / 1E8;
			}
		}

		public static ElectricField FromVoltsPerCentimeter(double voltsPerCentimeter)
		{
			return new ElectricField(voltsPerCentimeter * 1E2);
		}

		public static ElectricField FromMegavoltsPerCentimeter(double megavoltsPerCentimeter)
		{
			return new ElectricField(megavoltsPerCentimeter * 1E8);
		}

		public static ElectricField operator -(ElectricField right)
		{
			return new ElectricField(-right.VoltsPerMeter);
		}

		public static ElectricField operator +(ElectricField left, ElectricField right)
		{
			return new ElectricField(left.VoltsPerMeter + right.VoltsPerMeter);
		}

		public static ElectricField operator -(ElectricField left, ElectricField right)
		{
			return new ElectricField(left.VoltsPerMeter - right.VoltsPerMeter);
		}

		public static ElectricField operator *(double left, ElectricField right)
		{
			return new ElectricField(left*right.VoltsPerMeter);
		}

		public static ElectricField operator *(ElectricField left, double right)
		{
			return new ElectricField(left.VoltsPerMeter*right);
		}

		public static ElectricPotential operator *(ElectricField left, Length right)
		{
			return new ElectricPotential(left.VoltsPerMeter * right.Meters);
		}

		public static ElectricPotential operator *(Length left, ElectricField right)
		{
			return new ElectricPotential(left.Meters * right.VoltsPerMeter);
		}

		public static ElectricField operator /(ElectricField left, double right)
		{
			return new ElectricField(left.VoltsPerMeter/right);
		}

		public static double operator /(ElectricField left, ElectricField right)
		{
			return left.VoltsPerMeter/right.VoltsPerMeter;
		}

		public int CompareTo(ElectricField other)
		{
			return VoltsPerMeter.CompareTo(other.VoltsPerMeter);
		}

		public static bool operator <=(ElectricField left, ElectricField right)
		{
			return left.VoltsPerMeter <= right.VoltsPerMeter;
		}

		public static bool operator >=(ElectricField left, ElectricField right)
		{
			return left.VoltsPerMeter >= right.VoltsPerMeter;
		}

		public static bool operator <(ElectricField left, ElectricField right)
		{
			return left.VoltsPerMeter < right.VoltsPerMeter;
		}

		public static bool operator >(ElectricField left, ElectricField right)
		{
			return left.VoltsPerMeter > right.VoltsPerMeter;
		}

		public static bool operator ==(ElectricField left, ElectricField right)
		{
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

			return left.VoltsPerMeter == right.VoltsPerMeter;
		}

		public static bool operator !=(ElectricField left, ElectricField right)
		{
            return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return VoltsPerMeter.Equals(((ElectricField) obj).VoltsPerMeter);
		}

		public override int GetHashCode()
		{
			return VoltsPerMeter.GetHashCode();
		}
	}
}

