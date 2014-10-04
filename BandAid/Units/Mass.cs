using System;

namespace Band.Units
{
	public class Mass : IComparable<Mass>
	{
		public readonly double Kilograms;

		public Mass(double kilograms)
		{
			Kilograms = kilograms;
		}

		public static Mass Zero
		{
			get
			{
				return new Mass(0);
			}
		}

		public double Grams
		{
			get
			{
				return Kilograms * 1E3;
			}
		}

		public static Mass FromGrams(double grams)
		{
			return new Mass(grams / 1E3);
		}

		public static Mass operator -(Mass right)
		{
			return new Mass(-right.Kilograms);
		}

		public static Mass operator +(Mass left, Mass right)
		{
			return new Mass(left.Kilograms + right.Kilograms);
		}

		public static Mass operator -(Mass left, Mass right)
		{
			return new Mass(left.Kilograms - right.Kilograms);
		}

		public static Mass operator *(double left, Mass right)
		{
			return new Mass(left*right.Kilograms);
		}

		public static Mass operator *(Mass left, double right)
		{
			return new Mass(left.Kilograms*right);
		}

		public static Mass operator /(Mass left, double right)
		{
			return new Mass(left.Kilograms/right);
		}

		public static double operator /(Mass left, Mass right)
		{
			return left.Kilograms/right.Kilograms;
		}

		public int CompareTo(Mass other)
		{
			return Kilograms.CompareTo(other.Kilograms);
		}

		public static bool operator <=(Mass left, Mass right)
		{
			return left.Kilograms <= right.Kilograms;
		}

		public static bool operator >=(Mass left, Mass right)
		{
			return left.Kilograms >= right.Kilograms;
		}

		public static bool operator <(Mass left, Mass right)
		{
			return left.Kilograms < right.Kilograms;
		}

		public static bool operator >(Mass left, Mass right)
		{
			return left.Kilograms > right.Kilograms;
		}

		public static bool operator ==(Mass left, Mass right)
		{
			return left.Kilograms == right.Kilograms;
		}

		public static bool operator !=(Mass left, Mass right)
		{
			return left.Kilograms != right.Kilograms;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Kilograms.Equals(((Mass) obj).Kilograms);
		}

		public override int GetHashCode()
		{
			return Kilograms.GetHashCode();
		}
	}
}

