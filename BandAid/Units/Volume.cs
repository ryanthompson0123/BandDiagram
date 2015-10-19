using System;

namespace Band.Units
{
    public class Volume : IComparable<Volume>
    {
        public readonly double CubicMeters;

        public Volume(double cubicMeters)
        {
            CubicMeters = cubicMeters;
        }

        public static Volume Zero = new Volume(0);

        public double CubicCentimeters
        {
            get
            {
                return CubicMeters / 1E-06;
            }
        }

        public double CubicMicrometers
        {
            get
            {
                return CubicMeters / 1E-18;
            }
        }

        public double CubicNanometers
        {
            get
            {
                return CubicMeters / 1E-27;
            }
        }

        public static Volume FromCubicCentimeters(double CubicCentimeters)
        {
            return new Volume(CubicCentimeters / 1E6);
        }

        public static Volume FromCubicMicrometers(double CubicMicrometers)
        {
            return new Volume(CubicMicrometers / 1E18);
        }

        public static Volume FromCubicNanometers(double CubicNanometers)
        {
            return new Volume(CubicNanometers / 1E27);
        }

        public static Volume operator -(Volume right)
        {
            return new Volume(-right.CubicMeters);
        }

        public static Volume operator +(Volume left, Volume right)
        {
            return new Volume(left.CubicMeters + right.CubicMeters);
        }

        public static Volume operator -(Volume left, Volume right)
        {
            return new Volume(left.CubicMeters - right.CubicMeters);
        }

        public static Volume operator *(double left, Volume right)
        {
            return new Volume(left * right.CubicMeters);
        }

        public static Volume operator *(Volume left, double right)
        {
            return new Volume(left.CubicMeters * right);
        }

        public static Volume operator /(Volume left, double right)
        {
            return new Volume(left.CubicMeters / right);
        }

        public static double operator /(Volume left, Volume right)
        {
            return left.CubicMeters / right.CubicMeters;
        }

        public static Area operator /(Volume left, Length right)
        {
            return new Area(left.CubicMeters / right.Meters);
        }

        public static Length operator /(Volume left, Area right)
        {
            return new Length(left.CubicMeters / right.SquareMeters);
        }

        public int CompareTo(Volume other)
        {
            return CubicMeters.CompareTo(other.CubicMeters);
        }

        public static bool operator <=(Volume left, Volume right)
        {
            return left.CubicMeters <= right.CubicMeters;
        }

        public static bool operator >=(Volume left, Volume right)
        {
            return left.CubicMeters >= right.CubicMeters;
        }

        public static bool operator <(Volume left, Volume right)
        {
            return left.CubicMeters < right.CubicMeters;
        }

        public static bool operator >(Volume left, Volume right)
        {
            return left.CubicMeters > right.CubicMeters;
        }

        public static bool operator ==(Volume left, Volume right)
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

            return left.CubicMeters == right.CubicMeters;
        }

        public static bool operator !=(Volume left, Volume right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return CubicMeters.Equals(((Volume)obj).CubicMeters);
        }

        public override int GetHashCode()
        {
            return CubicMeters.GetHashCode();
        }
    }
}

