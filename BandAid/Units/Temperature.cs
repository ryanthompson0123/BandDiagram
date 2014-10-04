using System;

namespace Band.Units
{
	public class Temperature : IComparable<Temperature>
	{
		public readonly double Kelvin;

		public Temperature(double kelvin)
		{
			Kelvin = kelvin;
		}

		public static Temperature AbsoluteZero
		{
			get { return new Temperature(0); }
		}

		public static Temperature Room
		{
			get { return new Temperature(300); }
		}

		public double Celsius
		{
			get
			{
				return Kelvin - 273.15;
			}
		}

		public double Fahrenheit
		{
			get
			{
				return (9 / 5) * Celsius + 32;
			}
		}

		public static Temperature FromCelsius(double celsius)
		{
			return new Temperature(celsius + 273.15);
		}

		public static Temperature FromFahrenheit(double fahrenheit)
		{
			return new Temperature((fahrenheit - 32) * (5 / 9) + 273.15);
		}

		private const double boltzmannsConstant = 1.3806504E-23;

		public Energy ToEnergy()
		{
			return new Energy(Kelvin * boltzmannsConstant);
		}

		public static Temperature operator -(Temperature right)
		{
			return new Temperature(-right.Kelvin);
		}

		public static Temperature operator +(Temperature left, Temperature right)
		{
			return new Temperature(left.Kelvin + right.Kelvin);
		}

		public static Temperature operator -(Temperature left, Temperature right)
		{
			return new Temperature(left.Kelvin - right.Kelvin);
		}

		public static Temperature operator *(double left, Temperature right)
		{
			return new Temperature(left*right.Kelvin);
		}

		public static Temperature operator *(Temperature left, double right)
		{
			return new Temperature(left.Kelvin*right);
		}

		public static Temperature operator /(Temperature left, double right)
		{
			return new Temperature(left.Kelvin/right);
		}

		public static double operator /(Temperature left, Temperature right)
		{
			return left.Kelvin/right.Kelvin;
		}

		public int CompareTo(Temperature other)
		{
			return Kelvin.CompareTo(other.Kelvin);
		}

		public static bool operator <=(Temperature left, Temperature right)
		{
			return left.Kelvin <= right.Kelvin;
		}

		public static bool operator >=(Temperature left, Temperature right)
		{
			return left.Kelvin >= right.Kelvin;
		}

		public static bool operator <(Temperature left, Temperature right)
		{
			return left.Kelvin < right.Kelvin;
		}

		public static bool operator >(Temperature left, Temperature right)
		{
			return left.Kelvin > right.Kelvin;
		}

		public static bool operator ==(Temperature left, Temperature right)
		{
			return left.Kelvin == right.Kelvin;
		}

		public static bool operator !=(Temperature left, Temperature right)
		{
			return left.Kelvin != right.Kelvin;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Kelvin.Equals(((Temperature) obj).Kelvin);
		}

		public override int GetHashCode()
		{
			return Kelvin.GetHashCode();
		}
	}
}