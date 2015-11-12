using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Band.Units
{
    [JsonConverter(typeof(Length.Converter))]
	public class Length : IComparable<Length>
	{
		public readonly double Meters;

		public Length(double meters)
		{
			Meters = meters;
		}

        public static Length Zero = new Length(0);

		public double Centimeters
		{
			get
			{
				return Meters / 1E-02;
			}
		}

		public double Micrometers
		{
			get
			{
				return Meters / 1E-06;
			}
		}

		public double Nanometers
		{
			get
			{
				return Meters / 1E-09;
			}
		}

		public static Length FromCentimeters(double centimeters)
		{
			return new Length(centimeters / 1E2);
		}

		public static Length FromMicrometers(double micrometers)
		{
			return new Length(micrometers / 1E6);
		}

		public static Length FromNanometers(double nanometers)
		{
			return new Length(nanometers / 1E9);
		}

		public static Length operator -(Length right)
		{
			return new Length(-right.Meters);
		}

		public static Length operator +(Length left, Length right)
		{
			return new Length(left.Meters + right.Meters);
		}

		public static Length operator -(Length left, Length right)
		{
			return new Length(left.Meters - right.Meters);
		}

		public static Length operator *(double left, Length right)
		{
			return new Length(left*right.Meters);
		}

		public static Length operator *(Length left, double right)
		{
			return new Length(left.Meters*right);
		}

        public static Area operator *(Length left, Length right)
        {
            return new Area(left.Meters * right.Meters);
        }

		public static Length operator /(Length left, double right)
		{
			return new Length(left.Meters/right);
		}

		public static double operator /(Length left, Length right)
		{
			return left.Meters/right.Meters;
		}
			
		public int CompareTo(Length other)
		{
			return Meters.CompareTo(other.Meters);
		}

		public static bool operator <=(Length left, Length right)
		{
			return left.Meters <= right.Meters;
		}

		public static bool operator >=(Length left, Length right)
		{
			return left.Meters >= right.Meters;
		}

		public static bool operator <(Length left, Length right)
		{
			return left.Meters < right.Meters;
		}

		public static bool operator >(Length left, Length right)
		{
			return left.Meters > right.Meters;
		}

		public static bool operator ==(Length left, Length right)
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

			return left.Meters == right.Meters;
		}

		public static bool operator !=(Length left, Length right)
		{
            return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Meters.Equals(((Length) obj).Meters);
		}

		public override int GetHashCode()
		{
			return Meters.GetHashCode();
		}

        public override string ToString()
        {
            return string.Format("{0:F1} m", Meters);
        }

        public string ToString(string format)
        {
            return string.Format(format, Meters);
        }

        public string NanometersToString()
        {
            return string.Format("{0:F1} nm", Nanometers);
        }

        public string NanometersToString(string format)
        {
            return string.Format(format, Nanometers);
        }

        public class Converter : ExtendedJsonConverter<Length>
        {
            protected override Length Deserialize(Type objectType, JToken jToken)
            {
                if (jToken == null || jToken.Type == JTokenType.Null)
                {
                    return null;
                }

                return new Length(jToken.ToObject<double>());
            }

            protected override JToken Serialize(Length value)
            {
                return JToken.FromObject(value.Meters);
            }
        }
	}
}

