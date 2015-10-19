using System;

namespace Band.Units
{
    public class ChargeConcentration : IComparable<ChargeConcentration>
    {
        public readonly double CoulombsPerCubicMeter;

        public ChargeConcentration(double coulombsPerCubicMeter)
        {
            CoulombsPerCubicMeter = coulombsPerCubicMeter;
        }

        public double CoulombsPerCubicCentimeter
        {
            get { return CoulombsPerCubicMeter * 1E6; }
        }

        public double ElectronsPerCubicMeter
        {
            get { return CoulombsPerCubicMeter / ElectricCharge.Elementary.Coulombs; }
        }

        public static ChargeConcentration Zero = new ChargeConcentration(0);

        public static ChargeConcentration FromCoulombsPerCubicCentimeter(double coulombsPerCubicCentimeter)
        {
            return new ChargeConcentration(coulombsPerCubicCentimeter / 1E6);
        }

        public static ChargeConcentration operator -(ChargeConcentration right)
        {
            return new ChargeConcentration(-right.CoulombsPerCubicMeter);
        }

        public static ChargeConcentration operator +(ChargeConcentration left, ChargeConcentration right)
        {
            return new ChargeConcentration(left.CoulombsPerCubicMeter + right.CoulombsPerCubicMeter);
        }

        public static ChargeConcentration operator -(ChargeConcentration left, ChargeConcentration right)
        {
            return new ChargeConcentration(left.CoulombsPerCubicMeter - right.CoulombsPerCubicMeter);
        }

        public static ChargeConcentration operator *(double left, ChargeConcentration right)
        {
            return new ChargeConcentration(left * right.CoulombsPerCubicMeter);
        }

        public static ChargeConcentration operator *(ChargeConcentration left, double right)
        {
            return new ChargeConcentration(left.CoulombsPerCubicMeter * right);
        }

        public static ChargeConcentration operator /(ChargeConcentration left, double right)
        {
            return new ChargeConcentration(left.CoulombsPerCubicMeter / right);
        }

        public static ElectricCharge operator /(ChargeConcentration left, Concentration right)
        {
            return new ElectricCharge(left.CoulombsPerCubicMeter / right.PerCubicMeter);
        }

        public static ElectricCharge operator *(ChargeConcentration left, Volume right)
        {
            return new ElectricCharge(left.CoulombsPerCubicMeter * right.CubicMeters);
        }

        public static ElectricCharge operator *(Volume left, ChargeConcentration right)
        {
            return new ElectricCharge(right.CoulombsPerCubicMeter * left.CubicMeters);
        }

        public static double operator /(ChargeConcentration left, ChargeConcentration right)
        {
            return left.CoulombsPerCubicMeter / right.CoulombsPerCubicMeter;
        }

        public int CompareTo(ChargeConcentration other)
        {
            return CoulombsPerCubicMeter.CompareTo(other.CoulombsPerCubicMeter);
        }

        public static bool operator <=(ChargeConcentration left, ChargeConcentration right)
        {
            return left.CoulombsPerCubicMeter <= right.CoulombsPerCubicMeter;
        }

        public static bool operator >=(ChargeConcentration left, ChargeConcentration right)
        {
            return left.CoulombsPerCubicMeter >= right.CoulombsPerCubicMeter;
        }

        public static bool operator <(ChargeConcentration left, ChargeConcentration right)
        {
            return left.CoulombsPerCubicMeter < right.CoulombsPerCubicMeter;
        }

        public static bool operator >(ChargeConcentration left, ChargeConcentration right)
        {
            return left.CoulombsPerCubicMeter > right.CoulombsPerCubicMeter;
        }

        public static bool operator ==(ChargeConcentration left, ChargeConcentration right)
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

            return left.CoulombsPerCubicMeter == right.CoulombsPerCubicMeter;
        }

        public static bool operator !=(ChargeConcentration left, ChargeConcentration right)
        {
            return !(right == left);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return CoulombsPerCubicMeter.Equals(((ChargeConcentration)obj).CoulombsPerCubicMeter);
        }

        public override int GetHashCode()
        {
            return CoulombsPerCubicMeter.GetHashCode();
        }
    }
}