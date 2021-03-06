﻿using System;

namespace Band.Units
{
	public class Permittivity : IComparable<Permittivity>
	{
		public readonly double FaradsPerMeter;

		public Permittivity(double faradsPerMeter)
		{
			FaradsPerMeter = faradsPerMeter;
		}

		public double FaradsPerCentimeter
		{
			get
			{
				return FaradsPerMeter / 1E2;
			}
		}

		public static Permittivity FromFaradsPerCentimeter(double faradsPerCentimeter)
		{
			return new Permittivity(faradsPerCentimeter * 1E2);
		}

        public static Permittivity Zero = new Permittivity(0);

        public static Permittivity OfFreeSpace = new Permittivity(8.8541878176E-12 );

		public static Permittivity operator -(Permittivity right)
		{
			return new Permittivity(-right.FaradsPerMeter);
		}

		public static Permittivity operator +(Permittivity left, Permittivity right)
		{
			return new Permittivity(left.FaradsPerMeter + right.FaradsPerMeter);
		}

		public static Permittivity operator -(Permittivity left, Permittivity right)
		{
			return new Permittivity(left.FaradsPerMeter - right.FaradsPerMeter);
		}

		public static Permittivity operator *(double left, Permittivity right)
		{
			return new Permittivity(left*right.FaradsPerMeter);
		}

		public static Permittivity operator *(Permittivity left, double right)
		{
			return new Permittivity(left.FaradsPerMeter*right);
		}

        public static Length operator /(Permittivity left, CapacitanceDensity right)
        {
            return new Length(left.FaradsPerMeter / right.FaradsPerSquareMeter);
        }

        public static CapacitanceDensity operator /(Permittivity left, Length right)
        {
            return new CapacitanceDensity(left.FaradsPerMeter / right.Meters);
        }

        public static Capacitance operator *(Permittivity left, Length right)
        {
            return new Capacitance(left.FaradsPerMeter * right.Meters);
        }

        public static Capacitance operator *(Length left, Permittivity right)
        {
            return new Capacitance(right.FaradsPerMeter * left.Meters);
        }

		public static Permittivity operator /(Permittivity left, double right)
		{
			return new Permittivity(left.FaradsPerMeter/right);
		}

		public static double operator /(Permittivity left, Permittivity right)
		{
			return left.FaradsPerMeter/right.FaradsPerMeter;
		}

		public int CompareTo(Permittivity other)
		{
			return FaradsPerMeter.CompareTo(other.FaradsPerMeter);
		}

		public static bool operator <=(Permittivity left, Permittivity right)
		{
			return left.FaradsPerMeter <= right.FaradsPerMeter;
		}

		public static bool operator >=(Permittivity left, Permittivity right)
		{
			return left.FaradsPerMeter >= right.FaradsPerMeter;
		}

		public static bool operator <(Permittivity left, Permittivity right)
		{
			return left.FaradsPerMeter < right.FaradsPerMeter;
		}

		public static bool operator >(Permittivity left, Permittivity right)
		{
			return left.FaradsPerMeter > right.FaradsPerMeter;
		}

		public static bool operator ==(Permittivity left, Permittivity right)
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

			return left.FaradsPerMeter == right.FaradsPerMeter;
		}

		public static bool operator !=(Permittivity left, Permittivity right)
		{
            return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return FaradsPerMeter.Equals(((Permittivity) obj).FaradsPerMeter);
		}

		public override int GetHashCode()
		{
			return FaradsPerMeter.GetHashCode();
		}
	}
}

