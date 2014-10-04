using System;

namespace Band.Units
{
	public class Energy : IComparable<Energy>
	{
        public readonly double Joules;

		public Energy(double joules)
		{
			Joules = joules;
		}

		public static Energy Zero
		{
			get { return new Energy(0); }
		}

		public double MilliJoules
		{
			get { return Joules * 1E3; }
		}

		public double ElectronVolts
		{
			get { return Joules / 1.60217656535E-19; }
		}

		public ElectricPotential ToPotential()
		{
			return new ElectricPotential(ElectronVolts);
		}

        public static Energy FromElectronVolts(double electronVolts)
        {
            return new Energy(electronVolts * 1.60217656535E-19);
        }

		public static Energy FromMilliJoules(double milliJoules)
		{
			return new Energy(milliJoules * 1E3);
		}

		public static Energy operator -(Energy right)
		{
			return new Energy(-right.Joules);
		}

		public static Energy operator +(Energy left, Energy right)
		{
			return new Energy(left.Joules + right.Joules);
		}

		public static Energy operator -(Energy left, Energy right)
		{
			return new Energy(left.Joules - right.Joules);
		}

		public static Energy operator +(Energy left, ElectricPotential right)
		{
			return new Energy(left.ElectronVolts + right.Volts);
		}

		public static Energy operator -(Energy left, ElectricPotential right)
		{
			return new Energy(left.ElectronVolts - right.Volts);
		}

		public static Energy operator *(double left, Energy right)
		{
			return new Energy(left*right.Joules);
		}

		public static Energy operator *(Energy left, double right)
		{
			return new Energy(left.Joules*right);
		}

		public static Energy operator /(Energy left, double right)
		{
			return new Energy(left.Joules/right);
		}

		public static ElectricPotential operator /(Energy left, ElectricCharge right)
		{
			return new ElectricPotential(left.Joules / right.Coulombs);
		}

		public static double operator /(Energy left, Energy right)
		{
			return left.Joules/right.Joules;
		}

		public int CompareTo(Energy other)
		{
			return Joules.CompareTo(other.Joules);
		}

		public static bool operator <=(Energy left, Energy right)
		{
			return left.Joules <= right.Joules;
		}

		public static bool operator >=(Energy left, Energy right)
		{
			return left.Joules >= right.Joules;
		}

		public static bool operator <(Energy left, Energy right)
		{
			return left.Joules < right.Joules;
		}

		public static bool operator >(Energy left, Energy right)
		{
			return left.Joules > right.Joules;
		}

		public static bool operator ==(Energy left, Energy right)
		{
			return left.Joules == right.Joules;
		}

		public static bool operator !=(Energy left, Energy right)
		{
			return left.Joules != right.Joules;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Joules.Equals(((Energy) obj).Joules);
		}

		public override int GetHashCode()
		{
			return Joules.GetHashCode();
		}
	}
}