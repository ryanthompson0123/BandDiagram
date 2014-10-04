using System;

namespace Band.Units
{
	public class ElectricCharge : IComparable<ElectricCharge>
	{
		public readonly double Coulombs;

		public ElectricCharge(double coulombs)
		{
			Coulombs = coulombs;
		}

		public static ElectricCharge Zero
		{
			get
			{
				return new ElectricCharge(0);
			}
		}

		public static ElectricCharge Elementary
		{
			get
			{
				return new ElectricCharge(1.602176487E-19);
			}
		}

		public static ElectricCharge operator -(ElectricCharge right)
		{
			return new ElectricCharge(-right.Coulombs);
		}

		public static ElectricCharge operator +(ElectricCharge left, ElectricCharge right)
		{
			return new ElectricCharge(left.Coulombs + right.Coulombs);
		}

		public static ElectricCharge operator -(ElectricCharge left, ElectricCharge right)
		{
			return new ElectricCharge(left.Coulombs - right.Coulombs);
		}

		public static ElectricCharge operator *(double left, ElectricCharge right)
		{
			return new ElectricCharge(left*right.Coulombs);
		}

		public static ElectricCharge operator *(ElectricCharge left, double right)
		{
			return new ElectricCharge(left.Coulombs*right);
		}

		public static ElectricCharge operator /(ElectricCharge left, double right)
		{
			return new ElectricCharge(left.Coulombs/right);
		}

        public static ChargeDensity operator *(ElectricCharge left, Concentration right)
        {
            return new ChargeDensity(left.Coulombs * right.PerCubicMeter);
        }

        public static ChargeDensity operator *(Concentration left, ElectricCharge right)
        {
            return new ChargeDensity(right.Coulombs * left.PerCubicMeter);
        }

		public static double operator /(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs/right.Coulombs;
		}

		public int CompareTo(ElectricCharge other)
		{
			return Coulombs.CompareTo(other.Coulombs);
		}

		public static bool operator <=(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs <= right.Coulombs;
		}

		public static bool operator >=(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs >= right.Coulombs;
		}

		public static bool operator <(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs < right.Coulombs;
		}

		public static bool operator >(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs > right.Coulombs;
		}

		public static bool operator ==(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs == right.Coulombs;
		}

		public static bool operator !=(ElectricCharge left, ElectricCharge right)
		{
			return left.Coulombs != right.Coulombs;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Coulombs.Equals(((ElectricCharge) obj).Coulombs);
		}

		public override int GetHashCode()
		{
			return Coulombs.GetHashCode();
		}
	}
}

