using System;

namespace Band.Units
{
    public class Area : IComparable<Area>
    {
        public readonly double SquareMeters;

        public Area(double squareMeters)
        {
            SquareMeters = squareMeters;
        }

        public static Area Zero
        {
            get
            {
                return new Area(0);
            }
        }

        public double SquareCentimeters
        {
            get
            {
                return SquareMeters / 1E-04;
            }
        }

        public double SquareMicrometers
        {
            get
            {
                return SquareMeters / 1E-012;
            }
        }

        public double SquareNanometers
        {
            get
            {
                return SquareMeters / 1E-018;
            }
        }

        public static Area FromSquareCentimeters(double squareCentimeters)
        {
            return new Area(squareCentimeters / 1E4);
        }

        public static Area FromSquareMicrometers(double squareMicrometers)
        {
            return new Area(squareMicrometers / 1E12);
        }

        public static Area FromSquareNanometers(double squareNanometers)
        {
            return new Area(squareNanometers / 1E18);
        }

        public static Area operator -(Area right)
        {
            return new Area(-right.SquareMeters);
        }

        public static Area operator +(Area left, Area right)
        {
            return new Area(left.SquareMeters + right.SquareMeters);
        }

        public static Area operator -(Area left, Area right)
        {
            return new Area(left.SquareMeters - right.SquareMeters);
        }

        public static Area operator *(double left, Area right)
        {
            return new Area(left * right.SquareMeters);
        }

        public static Area operator *(Area left, double right)
        {
            return new Area(left.SquareMeters * right);
        }

        public static Volume operator *(Area left, Length right)
        {
            return new Volume(left.SquareMeters * right.Meters);
        }

        public static Volume operator *(Length left, Area right)
        {
            return new Volume(left.Meters * right.SquareMeters);
        }

        public static Area operator /(Area left, double right)
        {
            return new Area(left.SquareMeters / right);
        }

        public static double operator /(Area left, Area right)
        {
            return left.SquareMeters / right.SquareMeters;
        }

        public static Length operator /(Area left, Length right)
        {
            return new Length(left.SquareMeters / right.Meters);
        }

        public int CompareTo(Area other)
        {
            return SquareMeters.CompareTo(other.SquareMeters);
        }

        public static bool operator <=(Area left, Area right)
        {
            return left.SquareMeters <= right.SquareMeters;
        }

        public static bool operator >=(Area left, Area right)
        {
            return left.SquareMeters >= right.SquareMeters;
        }

        public static bool operator <(Area left, Area right)
        {
            return left.SquareMeters < right.SquareMeters;
        }

        public static bool operator >(Area left, Area right)
        {
            return left.SquareMeters > right.SquareMeters;
        }

        public static bool operator ==(Area left, Area right)
        {
            return left.SquareMeters == right.SquareMeters;
        }

        public static bool operator !=(Area left, Area right)
        {
            return left.SquareMeters != right.SquareMeters;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return SquareMeters.Equals(((Area)obj).SquareMeters);
        }

        public override int GetHashCode()
        {
            return SquareMeters.GetHashCode();
        }
    }
}

