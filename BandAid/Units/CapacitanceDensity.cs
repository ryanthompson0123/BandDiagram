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

        public double MicroFaradsPerSquareCentimeter
        {
            get { return FaradsPerSquareMeter * 1E2; }
        }

        public static CapacitanceDensity FromFaradsPerSquareCentimeter(
            double faradsPerSquareCentimeter)
        {
            return new CapacitanceDensity(faradsPerSquareCentimeter * 1E4);
        }

        public static CapacitanceDensity Zero = new CapacitanceDensity(0);

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
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.FaradsPerSquareMeter == right.FaradsPerSquareMeter;
        }

        public static bool operator !=(CapacitanceDensity left, CapacitanceDensity right)
        {
            return !(left == right);
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
            return string.Format("{0:F1} F/m\xB2", FaradsPerSquareMeter);
        }

        public string ToString(string format)
        {
            return string.Format(format, FaradsPerSquareMeter);
        }

        public string FaradsPerSquareCentimeterToString()
        {
            return string.Format("{0:F1} F/cm\xB2", FaradsPerSquareCentimeter);
        }

        public string FaradsPerSquareCentimeterToString(string format)
        {
            return string.Format(format, FaradsPerSquareCentimeter);
        }

        public string MicroFaradsPerSquareCentimeterToString()
        {
            return string.Format("{0:F1} μF/cm\xB2", MicroFaradsPerSquareCentimeter);
        }

        public string MicroFaradsPerSquareCentimeterToString(string format)
        {
            return string.Format(format, MicroFaradsPerSquareCentimeter);
        }
    }
}

