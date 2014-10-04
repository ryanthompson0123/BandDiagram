using System;

namespace Band.Units
{
	public class ElectricPotential : IComparable<ElectricPotential>
	{
		public readonly double Volts;

		public ElectricPotential(double volts)
		{
			Volts = volts;
		}

		public double Megavolts
		{
			get
			{
				return Volts / 1E6;
			}
		}

		public double Millivolts
		{
			get
			{
				return Volts * 1E3;
			}
		}

		public static ElectricPotential Zero
		{
			get { return new ElectricPotential(0); }
		}

		public static ElectricPotential FromMegavolts(double megavolts)
		{
			return new ElectricPotential(megavolts * 1E6);
		}

		public static ElectricPotential FromMillivolts(double millivolts)
		{
			return new ElectricPotential(millivolts / 1E3);
		}

		public static ElectricPotential operator -(ElectricPotential right)
		{
			return new ElectricPotential(-right.Volts);
		}

		public static ElectricPotential operator +(ElectricPotential left, ElectricPotential right)
		{
			return new ElectricPotential(left.Volts + right.Volts);
		}

		public static ElectricPotential operator -(ElectricPotential left, ElectricPotential right)
		{
			return new ElectricPotential(left.Volts - right.Volts);
		}

		public static ElectricPotential operator *(double left, ElectricPotential right)
		{
			return new ElectricPotential(left*right.Volts);
		}

		public static ElectricPotential operator *(ElectricPotential left, double right)
		{
			return new ElectricPotential(left.Volts*right);
		}

		public static ElectricPotential operator /(ElectricPotential left, double right)
		{
			return new ElectricPotential(left.Volts/right);
		}

        public static Length operator /(ElectricPotential left, ElectricField right)
        {
            return new Length(left.Volts / right.VoltsPerMeter);
        }

		public static ElectricField operator /(ElectricPotential left, Length right)
		{
			return new ElectricField(left.Volts / right.Meters);
		}

		public static double operator /(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts/right.Volts;
		}

        public static ElectricPotential operator +(ElectricPotential left, Energy right)
        {
            return new ElectricPotential(left.Volts + right.ElectronVolts);
        }

        public static ElectricPotential operator -(ElectricPotential left, Energy right)
        {
            return new ElectricPotential(left.Volts - right.ElectronVolts);
        }

        public static ElectricPotential Abs(ElectricPotential potential)
        {
            return new ElectricPotential(Math.Abs(potential.Volts));
        }

		public int CompareTo(ElectricPotential other)
		{
			return Volts.CompareTo(other.Volts);
		}

		public static bool operator <=(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts <= right.Volts;
		}

		public static bool operator >=(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts >= right.Volts;
		}

		public static bool operator <(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts < right.Volts;
		}

		public static bool operator >(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts > right.Volts;
		}

		public static bool operator ==(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts == right.Volts;
		}

		public static bool operator !=(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts != right.Volts;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Volts.Equals(((ElectricPotential) obj).Volts);
		}

		public override int GetHashCode()
		{
			return Volts.GetHashCode();
		}
	}
}

