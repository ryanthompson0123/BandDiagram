using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Band.Units
{
    [JsonConverter(typeof(Energy.Converter))]
	public class Energy : IComparable<Energy>
	{
        public readonly double Joules;

		public Energy(double joules)
		{
			Joules = joules;
		}

		public static Energy Zero
		{
			get { return new Energy(0); }
		}

		public double MilliJoules
		{
			get { return Joules * 1E3; }
		}

		public double ElectronVolts
		{
			get { return Joules / 1.60217656535E-19; }
		}

		public ElectricPotential ToPotential()
		{
			return new ElectricPotential(ElectronVolts);
		}

        public static Energy FromElectronVolts(double electronVolts)
        {
            return new Energy(electronVolts * 1.60217656535E-19);
        }

		public static Energy FromMilliJoules(double milliJoules)
		{
			return new Energy(milliJoules * 1E3);
		}

		public static Energy operator -(Energy right)
		{
			return new Energy(-right.Joules);
		}

		public static Energy operator +(Energy left, Energy right)
		{
			return new Energy(left.Joules + right.Joules);
		}

        public static Energy operator +(Energy left, MathExpression<Energy> right)
        {
            return new Energy(left.Joules + right.Evaluate().Joules);
        }

		public static Energy operator -(Energy left, Energy right)
		{
			return new Energy(left.Joules - right.Joules);
		}

        public static Energy operator -(Energy left, MathExpression<Energy> right)
        {
            return new Energy(left.Joules - right.Evaluate().Joules);
        }

		public static Energy operator +(Energy left, ElectricPotential right)
		{
            return Energy.FromElectronVolts(left.ElectronVolts + right.Volts);
		}

		public static Energy operator -(Energy left, ElectricPotential right)
		{
            return Energy.FromElectronVolts(left.ElectronVolts - right.Volts);
        }         

		public static Energy operator *(double left, Energy right)
		{
			return new Energy(left*right.Joules);
		}

		public static Energy operator *(Energy left, double right)
		{
			return new Energy(left.Joules*right);
		}

		public static Energy operator /(Energy left, double right)
		{
			return new Energy(left.Joules/right);
		}

		public static ElectricPotential operator /(Energy left, ElectricCharge right)
		{
			return new ElectricPotential(left.Joules / right.Coulombs);
		}

		public static double operator /(Energy left, Energy right)
		{
			return left.Joules/right.Joules;
		}

		public int CompareTo(Energy other)
		{
			return Joules.CompareTo(other.Joules);
		}

		public static bool operator <=(Energy left, Energy right)
		{
			return left.Joules <= right.Joules;
		}

		public static bool operator >=(Energy left, Energy right)
		{
			return left.Joules >= right.Joules;
		}

		public static bool operator <(Energy left, Energy right)
		{
			return left.Joules < right.Joules;
		}

		public static bool operator >(Energy left, Energy right)
		{
			return left.Joules > right.Joules;
		}

		public static bool operator ==(Energy left, Energy right)
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

			return left.Joules == right.Joules;
		}

		public static bool operator !=(Energy left, Energy right)
		{
            return !(left == right);
		}

        public static implicit operator ElectricPotential(Energy e)
        {
            return new ElectricPotential(e.ElectronVolts);
        }

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Joules.Equals(((Energy) obj).Joules);
		}

		public override int GetHashCode()
		{
			return Joules.GetHashCode();
		}

        public class Converter : ExtendedJsonConverter<Energy>
        {
            protected override Energy Deserialize(Type objectType, JToken jToken)
            {
                if (jToken == null || jToken.Type == JTokenType.Null)
                {
                    return null;
                }

                return Energy.FromElectronVolts(jToken.ToObject<double>());
            }

            protected override JToken Serialize(Energy value)
            {
                return JToken.FromObject(value.ElectronVolts);
            }
        }
	}
}