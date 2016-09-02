using System;
using System.Collections.Generic;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YAMP;

namespace Band
{
    public class MathExpression<TUnit>
    {
        private Dictionary<string, Value> variables;

        private TUnit cachedComputedValue;
        private bool cacheIsDirty;

        public readonly string Expression;

        private Func<double, TUnit> customConstructor;
        public Func<double, TUnit> CustomConstructor
        {
            get { return customConstructor; }
            set
            {
                customConstructor = value;
                cacheIsDirty = true;
            }
        }

        public bool IsValid
        {
            get
            {
                try
                {
                    Parser.Parse(Expression);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public MathExpression(string expression)
        {
            Expression = expression;
            variables = new Dictionary<string, Value>();
            cacheIsDirty = true;
        }

        public TUnit Evaluate()
        {
            if (cacheIsDirty)
            {
                var parser = Parser.Parse(Expression);
                var scalar = (ScalarValue)parser.Execute(variables);

                cachedComputedValue = ConstructUnit(scalar.Value);
                cacheIsDirty = false;
            }

            return cachedComputedValue;
        }

        public void SetVariable(string name, double value)
        {
            variables[name] = new ScalarValue(value);
            cacheIsDirty = true;
        }

        private TUnit ConstructUnit(double value)
        {
            if (CustomConstructor != null)
            {
                return CustomConstructor(value);
            }

            return (TUnit)Activator.CreateInstance(typeof(TUnit), value);
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            MathExpression<TUnit> p = obj as MathExpression<TUnit>;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Expression == p.Expression;
        }

        public bool Equals(MathExpression<TUnit> p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return Expression == p.Expression;
        }

        public static MathExpression<TUnit> operator *(double left, MathExpression<TUnit> right)
        {
            var result = new MathExpression<TUnit>(string.Format("{0}*({1})", left, right.Expression));

            right.variables
                .ForEach(kvp => result.variables[kvp.Key] = kvp.Value);

            result.CustomConstructor = right.CustomConstructor;

            return result;
        }

        public static MathExpression<TUnit> operator *(MathExpression<TUnit> left, MathExpression<TUnit> right)
        {
            var result = new MathExpression<TUnit>(string.Format("({0})*({1})", left.Expression, right.Expression));

            left.variables
                .ForEach(kvp => result.variables[kvp.Key] = kvp.Value);

            right.variables
                 .ForEach(kvp => result.variables[kvp.Key] = kvp.Value);

            result.CustomConstructor = right.CustomConstructor;

            return result;
        }

        public static MathExpression<TUnit> operator /(MathExpression<TUnit> left, double right)
        {
            var result = new MathExpression<TUnit>(string.Format("({0})/{1}", left.Expression, right));

            left.variables
                .ForEach(kvp => result.variables[kvp.Key] = kvp.Value);

            result.CustomConstructor = left.CustomConstructor;

            return result;
        }

        public static MathExpression<TUnit> operator /(MathExpression<TUnit> left, MathExpression<TUnit> right)
        {
            var result = new MathExpression<TUnit>(string.Format("({0})/({1})", left.Expression, right.Expression));

            left.variables
                .ForEach(kvp => result.variables[kvp.Key] = kvp.Value);

            right.variables
                 .ForEach(kvp => result.variables[kvp.Key] = kvp.Value);

            result.CustomConstructor = right.CustomConstructor;
            return result;
        }

        public class Converter : ExtendedJsonConverter<MathExpression<TUnit>>
        {
            protected override MathExpression<TUnit> Deserialize(Type objectType, JToken jToken)
            {
                if (jToken == null || jToken.Type == JTokenType.Null)
                {
                    return null;
                }

                return (MathExpression<TUnit>)Activator.CreateInstance(objectType, jToken.ToObject<string>());
                //return new MathExpression<TUnit>(jToken.ToObject<string>());
            }

            protected override JToken Serialize(MathExpression<TUnit> value)
            {
                if (value == null)
                {
                    return JToken.FromObject("");
                }

                return JToken.FromObject(value.Expression);
            }
        }
    }
}

