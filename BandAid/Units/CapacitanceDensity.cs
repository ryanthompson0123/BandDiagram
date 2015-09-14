using System;

namespace Band.Units
{
    public class CapacitanceDensity : IComparable<CapacitanceDensity>
    {
        public readonly double FaradsPerSquareMeter;

        public CapacitanceDensity(double faradsPerSquareMeter)
        {
            FaradsPerSquareMeter = faradsPerSquareMeter;
        }

        public double FaradsPerSquareCentimeter
        {
            get { return FaradsPerSquareMeter / 1E4; }
        }

        public static CapacitanceDensity FromFaradsPerSquareCentimeter(
            double faradsPerSquareCentimeter)
        {
            return new CapacitanceDensity(faradsPerSquareCentimeter * 1E4);
        }

        public static CapacitanceDensity Zero
        {
            get
            {
                return new CapacitanceDensity(0);
            }
        }

        public static CapacitanceDensity operator -(CapacitanceDensity right)
        {
            return new CapacitanceDensity(-right.FaradsPerSquareMeter);
        }

        public static CapacitanceDensity operator +(CapacitanceDensity left, CapacitanceDensity right)
        {
            return new CapacitanceDensity(left.FaradsPerSquareMeter + right.FaradsPerSquareMeter);
        }

        public static CapacitanceDensity operator -(CapacitanceDensity left, CapacitanceDensity right)
        {
            return new CapacitanceDensity(left.FaradsPerSquareMeter - right.FaradsPerSquareMeter);
        }

        public static CapacitanceDensity operator *(double left, CapacitanceDensity right)
        {
            return new CapacitanceDensity(left * right.FaradsPerSquareMeter);
        }

        public static CapacitanceDensity operator *(CapacitanceDensity left, double right)
        {
            return new CapacitanceDensity(left.FaradsPerSquareMeter * right);
        }

        public static CapacitanceDensity operator /(CapacitanceDensity left, double right)
        {
            return new CapacitanceDensity(left.FaradsPerSquareMeter / right);
        }

        public static double operator /(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter / right.FaradsPerSquareMeter;
        }

        public static Capacitance operator *(CapacitanceDensity left, Area right)
        {
            return new Capacitance(left.FaradsPerSquareMeter * right.SquareMeters);
        }

        public static Capacitance operator *(Area left, CapacitanceDensity right)
        {
            return new Capacitance(right.FaradsPerSquareMeter * left.SquareMeters);
        }

        public int CompareTo(CapacitanceDensity other)
        {
            return FaradsPerSquareMeter.CompareTo(other.FaradsPerSquareMeter);
        }

        public static bool operator <=(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter <= right.FaradsPerSquareMeter;
        }

        public static bool operator >=(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter >= right.FaradsPerSquareMeter;
        }

        public static bool operator <(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter < right.FaradsPerSquareMeter;
        }

        public static bool operator >(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter > right.FaradsPerSquareMeter;
        }

        public static bool operator ==(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter == right.FaradsPerSquareMeter;
        }

        public static bool operator !=(CapacitanceDensity left, CapacitanceDensity right)
        {
            return left.FaradsPerSquareMeter != right.FaradsPerSquareMeter;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return FaradsPerSquareMeter.Equals(((CapacitanceDensity)obj).FaradsPerSquareMeter);
        }

        public override int GetHashCode()
        {
            return FaradsPerSquareMeter.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} F/cm\xB2");
        }
    }
}

