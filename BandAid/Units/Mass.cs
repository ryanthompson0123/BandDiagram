using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Band.Units
{
    [JsonConverter(typeof(Mass.Converter))]
	public class Mass : IComparable<Mass>
	{
		public readonly double Kilograms;

		public Mass(double kilograms)
		{
			Kilograms = kilograms;
		}

        public static Mass Zero = new Mass(0);

		public double Grams
		{
			get
			{
				return Kilograms * 1E3;
			}
		}

		public static Mass FromGrams(double grams)
		{
			return new Mass(grams / 1E3);
		}

		public static Mass operator -(Mass right)
		{
			return new Mass(-right.Kilograms);
		}

		public static Mass operator +(Mass left, Mass right)
		{
			return new Mass(left.Kilograms + right.Kilograms);
		}

		public static Mass operator -(Mass left, Mass right)
		{
			return new Mass(left.Kilograms - right.Kilograms);
		}

		public static Mass operator *(double left, Mass right)
		{
			return new Mass(left*right.Kilograms);
		}

		public static Mass operator *(Mass left, double right)
		{
			return new Mass(left.Kilograms*right);
		}

		public static Mass operator /(Mass left, double right)
		{
			return new Mass(left.Kilograms/right);
		}

		public static double operator /(Mass left, Mass right)
		{
			return left.Kilograms/right.Kilograms;
		}

		public int CompareTo(Mass other)
		{
			return Kilograms.CompareTo(other.Kilograms);
		}

		public static bool operator <=(Mass left, Mass right)
		{
			return left.Kilograms <= right.Kilograms;
		}

		public static bool operator >=(Mass left, Mass right)
		{
			return left.Kilograms >= right.Kilograms;
		}

		public static bool operator <(Mass left, Mass right)
		{
			return left.Kilograms < right.Kilograms;
		}

		public static bool operator >(Mass left, Mass right)
		{
			return left.Kilograms > right.Kilograms;
		}

		public static bool operator ==(Mass left, Mass right)
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

			return left.Kilograms == right.Kilograms;
		}

		public static bool operator !=(Mass left, Mass right)
		{
            return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Kilograms.Equals(((Mass) obj).Kilograms);
		}

		public override int GetHashCode()
		{
			return Kilograms.GetHashCode();
		}

        public class Converter : ExtendedJsonConverter<Mass>
        {
            protected override Mass Deserialize(Type objectType, JToken jToken)
            {
                return new Mass(jToken.ToObject<double>());
            }

            protected override JToken Serialize(Mass value)
            {
                return JToken.FromObject(value.Kilograms);
            }
        }
	}
}

