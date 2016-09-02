using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Band.Units
{
    [JsonConverter(typeof(ElectricPotential.Converter))]
	public class ElectricPotential : IComparable<ElectricPotential>
	{
		public readonly double Volts;

		public ElectricPotential(double volts)
		{
			Volts = volts;
		}

		public double MegaVolts
		{
			get
			{
				return Volts / 1E6;
			}
		}

		public double MilliVolts
		{
			get
			{
				return Volts * 1E3;
			}
		}

        public int RoundMilliVolts
        {
            get
            {
                var mvInt = (int)MilliVolts;
                return ((int)Math.Round(mvInt / 10.0)) * 10;
            }
        }

        public static ElectricPotential Zero = new ElectricPotential(0);

		public static ElectricPotential FromMegavolts(double megavolts)
		{
			return new ElectricPotential(megavolts * 1E6);
		}

		public static ElectricPotential FromMillivolts(double millivolts)
		{
			return new ElectricPotential(millivolts / 1E3);
		}

		public static ElectricPotential operator -(ElectricPotential right)
		{
			return new ElectricPotential(-right.Volts);
		}

		public static ElectricPotential operator +(ElectricPotential left, ElectricPotential right)
		{
			return new ElectricPotential(left.Volts + right.Volts);
		}

		public static ElectricPotential operator -(ElectricPotential left, ElectricPotential right)
		{
			return new ElectricPotential(left.Volts - right.Volts);
		}

		public static ElectricPotential operator *(double left, ElectricPotential right)
		{
			return new ElectricPotential(left*right.Volts);
		}

		public static ElectricPotential operator *(ElectricPotential left, double right)
		{
			return new ElectricPotential(left.Volts*right);
		}

		public static ElectricPotential operator /(ElectricPotential left, double right)
		{
			return new ElectricPotential(left.Volts/right);
		}

        public static Length operator /(ElectricPotential left, ElectricField right)
        {
            return new Length(left.Volts / right.VoltsPerMeter);
        }

		public static ElectricField operator /(ElectricPotential left, Length right)
		{
		return new ElectricField(left.Volts / right.Meters);
		}

		public static double operator /(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts/right.Volts;
		}
            
        public static ElectricPotential operator +(ElectricPotential left, Energy right)
        {
            return new ElectricPotential(left.Volts + right.ElectronVolts);
        }

        public static ElectricPotential operator -(ElectricPotential left, Energy right)
        {
            return new ElectricPotential(left.Volts - right.ElectronVolts);
        }

        public static ElectricPotential operator -(ElectricPotential left, MathExpression<Energy> right)
        {
            return new ElectricPotential(left.Volts - right.Evaluate().ElectronVolts);
        }

        public static ElectricPotential Abs(ElectricPotential potential)
        {
            return new ElectricPotential(Math.Abs(potential.Volts));
        }

		public int CompareTo(ElectricPotential other)
		{
			return Volts.CompareTo(other.Volts);
		}

		public static bool operator <=(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts <= right.Volts;
		}

		public static bool operator >=(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts >= right.Volts;
		}

		public static bool operator <(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts < right.Volts;
		}

		public static bool operator >(ElectricPotential left, ElectricPotential right)
		{
			return left.Volts > right.Volts;
		}

		public static bool operator ==(ElectricPotential left, ElectricPotential right)
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

            return Math.Abs(left.Volts - right.Volts) < 0.0001;
		}

		public static bool operator !=(ElectricPotential left, ElectricPotential right)
		{
            return !(left == right);
		}

        public static implicit operator Energy(ElectricPotential p)
        {
            return Energy.FromElectronVolts(p.Volts);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Volts.Equals(((ElectricPotential) obj).Volts);
		}

		public override int GetHashCode()
		{
			return Volts.GetHashCode();
		}

        public override string ToString()
        {
            return string.Format("{0:F1} V", Volts);
        }

        public string ToString(string format)
        {
            return string.Format(format, Volts);
        }

        public string MilliVoltsToString()
        {
            return string.Format("{0:F1} mV", MilliVolts);
        }

        public string MilliVoltsToString(string format)
        {
            return string.Format(format, MilliVolts);
        }

        public class Converter : ExtendedJsonConverter<ElectricPotential>
        {
            protected override ElectricPotential Deserialize(Type objectType, JToken jToken)
            {
                if (jToken == null || jToken.Type == JTokenType.Null)
                {
                    return null;
                }

                return new ElectricPotential(jToken.ToObject<double>());
            }

            protected override JToken Serialize(ElectricPotential value)
            {
                return JToken.FromObject(value.Volts);
            }
        }
    }
}

