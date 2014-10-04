using System;

namespace Band.Units
{
    public class ChargeDensity : IComparable<ChargeDensity>
    {
        public readonly double CoulombsPerCubicMeter;

        public ChargeDensity(double coulombsPerCubicMeter)
        {
            CoulombsPerCubicMeter = coulombsPerCubicMeter;
        }

        public static ChargeDensity Zero
        {
            get
            {
                return new ChargeDensity(0);
            }
        }

        public static ChargeDensity FromCoulombsPerCubicCentimeter(double coulombsPerCubicCentimeter)
        {
            return new ChargeDensity(coulombsPerCubicCentimeter / 1E6);
        }

        public static ChargeDensity operator -(ChargeDensity right)
        {
            return new ChargeDensity(-right.CoulombsPerCubicMeter);
        }

        public static ChargeDensity operator +(ChargeDensity left, ChargeDensity right)
        {
            return new ChargeDensity(left.CoulombsPerCubicMeter + right.CoulombsPerCubicMeter);
        }

        public static ChargeDensity operator -(ChargeDensity left, ChargeDensity right)
        {
            return new ChargeDensity(left.CoulombsPerCubicMeter - right.CoulombsPerCubicMeter);
        }

        public static ChargeDensity operator *(double left, ChargeDensity right)
        {
            return new ChargeDensity(left * right.CoulombsPerCubicMeter);
        }

        public static ChargeDensity operator *(ChargeDensity left, double right)
        {
            return new ChargeDensity(left.CoulombsPerCubicMeter * right);
        }

        public static ChargeDensity operator /(ChargeDensity left, double right)
        {
            return new ChargeDensity(left.CoulombsPerCubicMeter / right);
        }

        public static ElectricCharge operator /(ChargeDensity left, Concentration right)
        {
            return new ElectricCharge(left.CoulombsPerCubicMeter / right.PerCubicMeter);
        }

        public static ElectricCharge operator *(ChargeDensity left, Volume right)
        {
            return new ElectricCharge(left.CoulombsPerCubicMeter * right.CubicMeters);
        }

        public static ElectricCharge operator *(Volume left, ChargeDensity right)
        {
            return new ElectricCharge(right.CoulombsPerCubicMeter * left.CubicMeters);
        }

        public static double operator /(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter / right.CoulombsPerCubicMeter;
        }

        public int CompareTo(ChargeDensity other)
        {
            return CoulombsPerCubicMeter.CompareTo(other.CoulombsPerCubicMeter);
        }

        public static bool operator <=(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter <= right.CoulombsPerCubicMeter;
        }

        public static bool operator >=(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter >= right.CoulombsPerCubicMeter;
        }

        public static bool operator <(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter < right.CoulombsPerCubicMeter;
        }

        public static bool operator >(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter > right.CoulombsPerCubicMeter;
        }

        public static bool operator ==(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter == right.CoulombsPerCubicMeter;
        }

        public static bool operator !=(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerCubicMeter != right.CoulombsPerCubicMeter;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return CoulombsPerCubicMeter.Equals(((ChargeDensity)obj).CoulombsPerCubicMeter);
        }

        public override int GetHashCode()
        {
            return CoulombsPerCubicMeter.GetHashCode();
        }
    }
}