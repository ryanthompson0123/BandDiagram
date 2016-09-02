using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Band.Units
{
    [JsonConverter(typeof(Concentration.Converter))]
	public class Concentration : IComparable<Concentration>
	{
		public readonly double PerCubicMeter;

		public Concentration(double perCubicMeter)
		{
			PerCubicMeter = perCubicMeter;
		}

        public double PerCubicCentimeter
        {
            get { return PerCubicMeter * 1E6; }
        }

        public static Concentration Zero = new Concentration(0);

		public static Concentration FromPerCubicCentimeter(double perCubicCentimeter)
		{
			return new Concentration(perCubicCentimeter / 1E6);
		}

		public static Concentration operator -(Concentration right)
		{
			return new Concentration(-right.PerCubicMeter);
		}

		public static Concentration operator +(Concentration left, Concentration right)
		{
			return new Concentration(left.PerCubicMeter + right.PerCubicMeter);
		}

		public static Concentration operator -(Concentration left, Concentration right)
		{
			return new Concentration(left.PerCubicMeter - right.PerCubicMeter);
		}

        public static Concentration operator -(Concentration left, MathExpression<Concentration> right)
        {
            return new Concentration(left.PerCubicMeter - right.Evaluate().PerCubicMeter);
        }

		public static Concentration operator *(double left, Concentration right)
		{
			return new Concentration(left*right.PerCubicMeter);
		}

		public static Concentration operator *(Concentration left, double right)
		{
			return new Concentration(left.PerCubicMeter*right);
		}

        public static double operator *(Volume left, Concentration right)
        {
            return left.CubicMeters * right.PerCubicMeter;
        }

        public static double operator *(Concentration left, Volume right)
        {
            return right.CubicMeters * left.PerCubicMeter;
        }

		public static Concentration operator /(Concentration left, double right)
		{
			return new Concentration(left.PerCubicMeter/right);
		}

		public static double operator /(Concentration left, Concentration right)
		{
			return left.PerCubicMeter/right.PerCubicMeter;
		}

		public int CompareTo(Concentration other)
		{
			return PerCubicMeter.CompareTo(other.PerCubicMeter);
		}

		public static bool operator <=(Concentration left, Concentration right)
		{
			return left.PerCubicMeter <= right.PerCubicMeter;
		}

		public static bool operator >=(Concentration left, Concentration right)
		{
			return left.PerCubicMeter >= right.PerCubicMeter;
		}

		public static bool operator <(Concentration left, Concentration right)
		{
			return left.PerCubicMeter < right.PerCubicMeter;
		}

		public static bool operator >(Concentration left, Concentration right)
		{
			return left.PerCubicMeter > right.PerCubicMeter;
		}

		public static bool operator ==(Concentration left, Concentration right)
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

			return left.PerCubicMeter == right.PerCubicMeter;
		}

		public static bool operator !=(Concentration left, Concentration right)
		{
            return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return PerCubicMeter.Equals(((Concentration) obj).PerCubicMeter);
		}

		public override int GetHashCode()
		{
			return PerCubicMeter.GetHashCode();
		}

        public class Converter : ExtendedJsonConverter<Concentration>
        {
            protected override Concentration Deserialize(Type objectType, JToken jToken)
            {
                if (jToken == null || jToken.Type == JTokenType.Null)
                {
                    return null;
                }

                return Concentration.FromPerCubicCentimeter(jToken.ToObject<double>());
            }

            protected override JToken Serialize(Concentration value)
            {
                return JToken.FromObject(value.PerCubicCentimeter);
            }
        }
	}
}