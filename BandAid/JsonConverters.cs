using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Band.Units;

namespace Band
{
    public class MetalConverter : JsonCreationConverter<Metal>
    {
        protected override Metal Create(Type objectType, JObject jObject)
        {
            var metal = new Metal(Length.Zero)
            {
                Name = (string)jObject["name"],
                Notes = (string)jObject["notes"],
                FillColor = (string)jObject["fillColor"]
            };

            metal.SetWorkFunction(Energy.FromElectronVolts((double)jObject["workFunction"]));
            return metal;
        }
    }

    public class DielectricConverter : JsonCreationConverter<Dielectric>
    {
        protected override Dielectric Create(Type objectType, JObject jObject)
        {
            return new Dielectric(Length.Zero)
            {
                Name = (string)jObject["name"],
                Notes = (string)jObject["notes"],
                FillColor = (string)jObject["fillColor"],
                BandGap = Energy.FromElectronVolts((double)jObject["bandGap"]),
                DielectricConstant = (double)jObject["dielectricConstant"],
                ElectronAffinity = Energy.FromElectronVolts((double)jObject["electronAffinity"])
            };
        }
    }

    public class SemiconductorConverter : JsonCreationConverter<Semiconductor>
    {
        protected override Semiconductor Create(Type objectType, JObject jObject)
        {
            return new Semiconductor()
            {
                Name = (string)jObject["name"],
                Notes = (string)jObject["notes"],
                FillColor = (string)jObject["fillColor"],
                BandGap = Energy.FromElectronVolts((double)jObject["bandGap"]),
                DielectricConstant = (double)jObject["dielectricConstant"],
                IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter((double)jObject["intrinsicCarrierConcentration"]),
                DopantConcentration = Concentration.FromPerCubicCentimeter((double)jObject["dopantConcentration"]),
                ElectronAffinity = Energy.FromElectronVolts((double)jObject["electronAffinity"])
            };
        }
    }
        
    // From http://stackoverflow.com/a/8031283/1351938
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, 
            Type objectType, 
            object existingValue, 
            JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            //serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, 
            object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

