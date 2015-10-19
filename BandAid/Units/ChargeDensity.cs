using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Band.Units
{
    [JsonConverter(typeof(ChargeDensity.Converter))]
    public class ChargeDensity : IComparable<ChargeDensity>
    {
        public readonly double CoulombsPerSquareMeter;

        public ChargeDensity(double coulombsPerSquareMeter)
        {
            CoulombsPerSquareMeter = coulombsPerSquareMeter;
        }

        public double CoulombsPerSquareCentimeter
        {
            get { return CoulombsPerSquareMeter / 1E4; }
        }

        public double MicroCoulombsPerSquareCentimeter
        {
            get { return CoulombsPerSquareCentimeter * 1E6; }
        }

        public double PointOneGigaCoulombsPerSquareCentimeter
        {
            get { return CoulombsPerSquareMeter / 1E13; }
        }

        public double ElectronsPerSquareMeter
        {
            get { return CoulombsPerSquareMeter / ElectricCharge.Elementary.Coulombs; }
        }

        public static ChargeDensity Zero = new ChargeDensity(0);

        public static ChargeDensity FromCoulombsPerSquareCentimeter(double coulombsPerSquareCentimeter)
        {
            return new ChargeDensity(coulombsPerSquareCentimeter * 1E4);
        }

        public static ChargeDensity operator -(ChargeDensity right)
        {
            return new ChargeDensity(-right.CoulombsPerSquareMeter);
        }

        public static ChargeDensity operator +(ChargeDensity left, ChargeDensity right)
        {
            return new ChargeDensity(left.CoulombsPerSquareMeter + right.CoulombsPerSquareMeter);
        }

        public static ChargeDensity operator -(ChargeDensity left, ChargeDensity right)
        {
            return new ChargeDensity(left.CoulombsPerSquareMeter - right.CoulombsPerSquareMeter);
        }

        public static ChargeDensity operator *(double left, ChargeDensity right)
        {
            return new ChargeDensity(left * right.CoulombsPerSquareMeter);
        }

        public static ChargeDensity operator *(ChargeDensity left, double right)
        {
            return new ChargeDensity(left.CoulombsPerSquareMeter * right);
        }

        public static ChargeDensity operator /(ChargeDensity left, double right)
        {
            return new ChargeDensity(left.CoulombsPerSquareMeter / right);
        }

        public static ElectricField operator /(ChargeDensity left, Permittivity right)
        {
            return new ElectricField(left.CoulombsPerSquareMeter / right.FaradsPerMeter);
        }

        public static Length operator /(ChargeDensity left, ChargeConcentration right)
        {
            return new Length(Math.Abs(left.CoulombsPerSquareMeter / right.CoulombsPerCubicMeter));
        }

        public static double operator /(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerSquareMeter / right.CoulombsPerSquareMeter;
        }

        public int CompareTo(ChargeDensity other)
        {
            return CoulombsPerSquareMeter.CompareTo(other.CoulombsPerSquareMeter);
        }

        public static bool operator <=(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerSquareMeter <= right.CoulombsPerSquareMeter;
        }

        public static bool operator >=(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerSquareMeter >= right.CoulombsPerSquareMeter;
        }

        public static bool operator <(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerSquareMeter < right.CoulombsPerSquareMeter;
        }

        public static bool operator >(ChargeDensity left, ChargeDensity right)
        {
            return left.CoulombsPerSquareMeter > right.CoulombsPerSquareMeter;
        }

        public static bool operator ==(ChargeDensity left, ChargeDensity right)
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

            return left.CoulombsPerSquareMeter == right.CoulombsPerSquareMeter;

        }

        public static bool operator !=(ChargeDensity left, ChargeDensity right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return CoulombsPerSquareMeter.Equals(((ChargeDensity)obj).CoulombsPerSquareMeter);
        }

        public override int GetHashCode()
        {
            return CoulombsPerSquareMeter.GetHashCode();
        }

        public class Converter : ExtendedJsonConverter<ChargeDensity>
        {
            protected override ChargeDensity Deserialize(Type objectType, JToken jToken)
            {
                return new ChargeDensity(jToken.ToObject<double>());
            }

            protected override JToken Serialize(ChargeDensity value)
            {
                if (value == null)
                {
                    return JToken.FromObject(ChargeDensity.Zero);
                }

                return JToken.FromObject(value.CoulombsPerSquareMeter);
            }
        }
    }
}