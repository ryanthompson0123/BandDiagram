using System;

namespace Band.Units
{
    public class Capacitance : IComparable<Capacitance>
    {
        public readonly double Farads;

        public Capacitance(double farads)
        {
            Farads = farads;
        }

        public double MicroFarads
        {
            get { return Farads / 1E-6; }
        }

        public static Capacitance FromMicroFarads(double microFarads)
        {
            return new Capacitance(microFarads * 1E-6);
        }

        public static Capacitance Zero
        {
            get
            {
                return new Capacitance(0);
            }
        }

        public static Capacitance operator -(Capacitance right)
        {
            return new Capacitance(-right.Farads);
        }

        public static Capacitance operator +(Capacitance left, Capacitance right)
        {
            return new Capacitance(left.Farads + right.Farads);
        }

        public static Capacitance operator -(Capacitance left, Capacitance right)
        {
            return new Capacitance(left.Farads - right.Farads);
        }

        public static Capacitance operator *(double left, Capacitance right)
        {
            return new Capacitance(left * right.Farads);
        }

        public static Capacitance operator *(Capacitance left, double right)
        {
            return new Capacitance(left.Farads * right);
        }

        public static Capacitance operator /(Capacitance left, double right)
        {
            return new Capacitance(left.Farads / right);
        }

        public static double operator /(Capacitance left, Capacitance right)
        {
            return left.Farads / right.Farads;
        }

        public static Permittivity operator /(Capacitance left, Length right)
        {
            return new Permittivity(left.Farads / right.Meters);
        }

        public int CompareTo(Capacitance other)
        {
            return Farads.CompareTo(other.Farads);
        }

        public static bool operator <=(Capacitance left, Capacitance right)
        {
            return left.Farads <= right.Farads;
        }

        public static bool operator >=(Capacitance left, Capacitance right)
        {
            return left.Farads >= right.Farads;
        }

        public static bool operator <(Capacitance left, Capacitance right)
        {
            return left.Farads < right.Farads;
        }

        public static bool operator >(Capacitance left, Capacitance right)
        {
            return left.Farads > right.Farads;
        }

        public static bool operator ==(Capacitance left, Capacitance right)
        {
            return left.Farads == right.Farads;
        }

        public static bool operator !=(Capacitance left, Capacitance right)
        {
            return left.Farads != right.Farads;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Farads.Equals(((Capacitance)obj).Farads);
        }

        public override int GetHashCode()
        {
            return Farads.GetHashCode();
        }
    }
}

