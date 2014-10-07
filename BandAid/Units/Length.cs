using System;

namespace Band.Units
{
	public class Length : IComparable<Length>
	{
		public readonly double Meters;

		public Length(double meters)
		{
			Meters = meters;
		}

		public static Length Zero
		{
			get
			{
				return new Length(0);
			}
		}

		public double Centimeters
		{
			get
			{
				return Meters / 1E-02;
			}
		}

		public double Micrometers
		{
			get
			{
				return Meters / 1E-06;
			}
		}

		public double Nanometers
		{
			get
			{
				return Meters / 1E-09;
			}
		}

		public static Length FromCentimeters(double centimeters)
		{
			return new Length(centimeters / 1E2);
		}

		public static Length FromMicrometers(double micrometers)
		{
			return new Length(micrometers / 1E6);
		}

		public static Length FromNanometers(double nanometers)
		{
			return new Length(nanometers / 1E9);
		}

		public static Length operator -(Length right)
		{
			return new Length(-right.Meters);
		}

		public static Length operator +(Length left, Length right)
		{
			return new Length(left.Meters + right.Meters);
		}

		public static Length operator -(Length left, Length right)
		{
			return new Length(left.Meters - right.Meters);
		}

		public static Length operator *(double left, Length right)
		{
			return new Length(left*right.Meters);
		}

		public static Length operator *(Length left, double right)
		{
			return new Length(left.Meters*right);
		}

        public static Area operator *(Length left, Length right)
        {
            return new Area(left.Meters * right.Meters);
        }

		public static Length operator /(Length left, double right)
		{
			return new Length(left.Meters/right);
		}

		public static double operator /(Length left, Length right)
		{
			return left.Meters/right.Meters;
		}
			
		public int CompareTo(Length other)
		{
			return Meters.CompareTo(other.Meters);
		}

		public static bool operator <=(Length left, Length right)
		{
			return left.Meters <= right.Meters;
		}

		public static bool operator >=(Length left, Length right)
		{
			return left.Meters >= right.Meters;
		}

		public static bool operator <(Length left, Length right)
		{
			return left.Meters < right.Meters;
		}

		public static bool operator >(Length left, Length right)
		{
			return left.Meters > right.Meters;
		}

		public static bool operator ==(Length left, Length right)
		{
			return left.Meters == right.Meters;
		}

		public static bool operator !=(Length left, Length right)
		{
			return left.Meters != right.Meters;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Meters.Equals(((Length) obj).Meters);
		}

		public override int GetHashCode()
		{
			return Meters.GetHashCode();
		}
	}
}

