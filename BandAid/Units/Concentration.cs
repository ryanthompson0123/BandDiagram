using System;

namespace Band.Units
{
	public class Concentration : IComparable<Concentration>
	{
		public readonly double PerCubicMeter;

		public Concentration(double perCubicMeter)
		{
			PerCubicMeter = perCubicMeter;
		}

        public double PerCubicCentimeter
        {
            get { return PerCubicMeter * 1E6; }
        }

		public static Concentration Zero
		{
			get
			{
				return new Concentration(0);
			}
		}

		public static Concentration FromPerCubicCentimeter(double perCubicCentimeter)
		{
			return new Concentration(perCubicCentimeter / 1E6);
		}

		public static Concentration operator -(Concentration right)
		{
			return new Concentration(-right.PerCubicMeter);
		}

		public static Concentration operator +(Concentration left, Concentration right)
		{
			return new Concentration(left.PerCubicMeter + right.PerCubicMeter);
		}

		public static Concentration operator -(Concentration left, Concentration right)
		{
			return new Concentration(left.PerCubicMeter - right.PerCubicMeter);
		}

		public static Concentration operator *(double left, Concentration right)
		{
			return new Concentration(left*right.PerCubicMeter);
		}

		public static Concentration operator *(Concentration left, double right)
		{
			return new Concentration(left.PerCubicMeter*right);
		}

        public static double operator *(Volume left, Concentration right)
        {
            return left.CubicMeters * right.PerCubicMeter;
        }

        public static double operator *(Concentration left, Volume right)
        {
            return right.CubicMeters * left.PerCubicMeter;
        }

		public static Concentration operator /(Concentration left, double right)
		{
			return new Concentration(left.PerCubicMeter/right);
		}

		public static double operator /(Concentration left, Concentration right)
		{
			return left.PerCubicMeter/right.PerCubicMeter;
		}

		public int CompareTo(Concentration other)
		{
			return PerCubicMeter.CompareTo(other.PerCubicMeter);
		}

		public static bool operator <=(Concentration left, Concentration right)
		{
			return left.PerCubicMeter <= right.PerCubicMeter;
		}

		public static bool operator >=(Concentration left, Concentration right)
		{
			return left.PerCubicMeter >= right.PerCubicMeter;
		}

		public static bool operator <(Concentration left, Concentration right)
		{
			return left.PerCubicMeter < right.PerCubicMeter;
		}

		public static bool operator >(Concentration left, Concentration right)
		{
			return left.PerCubicMeter > right.PerCubicMeter;
		}

		public static bool operator ==(Concentration left, Concentration right)
		{
			return left.PerCubicMeter == right.PerCubicMeter;
		}

		public static bool operator !=(Concentration left, Concentration right)
		{
			return left.PerCubicMeter != right.PerCubicMeter;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return PerCubicMeter.Equals(((Concentration) obj).PerCubicMeter);
		}

		public override int GetHashCode()
		{
			return PerCubicMeter.GetHashCode();
		}
	}
}