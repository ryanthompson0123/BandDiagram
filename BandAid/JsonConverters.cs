using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Band.Units;
using System.Collections.Generic;

namespace Band
{
    public class MetalConverter : ExtendedJsonConverter<Metal>
    {
        protected override Metal Deserialize(Type objectType, JObject jObject)
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

        protected override JObject Serialize(Metal value)
        {
            var obj = new JObject();

            obj["name"] = value.Name;
            obj["notes"] = value.Notes;
            obj["fillColor"] = value.FillColor;
            obj["thickness"] = value.Thickness.Nanometers;
            obj["workFunction"] = value.WorkFunction.ElectronVolts;

            return obj;
        }
    }

    public class DielectricConverter : ExtendedJsonConverter<Dielectric>
    {
        protected override Dielectric Deserialize(Type objectType, JObject jObject)
        {
            return new Dielectric(Length.Zero)
            {
                Name = (string)jObject["name"],
                Notes = (string)jObject["notes"],
                FillColor = (string)jObject["fillColor"],
                BandGap = Energy.FromElectronVolts((double)jObject["bandGap"]),
                DielectricConstant = (double)jObject["dielectricConstant"],
                ElectronAffinity = Energy.FromElectronVolts(
                    (double)jObject["electronAffinity"])
            };
        }

        protected override JObject Serialize(Dielectric value)
        {
            var obj = new JObject();

            obj["name"] = value.Name;
            obj["notes"] = value.Notes;
            obj["fillColor"] = value.FillColor;
            obj["bandGap"] = value.BandGap.ElectronVolts;
            obj["dielectricConstant"] = value.DielectricConstant;
            obj["electronAffinity"] = value.ElectronAffinity.ElectronVolts;
            obj["thickness"] = value.Thickness.Nanometers;

            return obj;
        }
    }

    public class SemiconductorConverter : ExtendedJsonConverter<Semiconductor>
    {
        protected override Semiconductor Deserialize(Type objectType, JObject jObject)
        {
            return new Semiconductor
            {
                Name = (string)jObject["name"],
                Notes = (string)jObject["notes"],
                FillColor = (string)jObject["fillColor"],
                BandGap = Energy.FromElectronVolts((double)jObject["bandGap"]),
                DielectricConstant = (double)jObject["dielectricConstant"],
                IntrinsicCarrierConcentration = Concentration.FromPerCubicCentimeter(
                    (double)jObject["intrinsicCarrierConcentration"]),
                DopantConcentration = Concentration.FromPerCubicCentimeter(
                    (double)jObject["dopantConcentration"]),
                ElectronAffinity = Energy.FromElectronVolts((
                    double)jObject["electronAffinity"])
            };
        }

        protected override JObject Serialize(Semiconductor value)
        {
            var obj = new JObject();

            obj["name"] = value.Name;
            obj["notes"] = value.Notes;
            obj["fillColor"] = value.FillColor;
            obj["bandGap"] = value.BandGap.ElectronVolts;
            obj["dielectricConstant"] = value.DielectricConstant;
            obj["intrinsicCarrierConcentration"] = value.IntrinsicCarrierConcentration
                .PerCubicCentimeter;
            obj["dopantConcentration"] = value.DopantConcentration.PerCubicCentimeter;
            obj["electronAffinity"] = value.ElectronAffinity.ElectronVolts;
            obj["thickness"] = value.Thickness.Nanometers;

            return obj;
        }
    }

    public class StructureConverter : ExtendedJsonConverter<Structure>
    {
        protected override Structure Deserialize(Type objectType, JObject jObject)
        {
            var layerList = new List<Material>();

            foreach (var layer in jObject["layers"])
            {
                var dLayer = JsonConvert.DeserializeObject<Material>(layer.ToString(),
                                 new SemiconductorConverter(), new MetalConverter(),
                                 new DielectricConverter());

                layerList.Add(dLayer);
            }

            var structure = new Structure(layerList);
            structure.Bias = new ElectricPotential((double)jObject["bias"]);
            structure.Temperature = new Temperature((double)jObject["temperature"]);

            return structure;
        }

        protected override JObject Serialize(Structure value)
        {
            var obj = new JObject();

            obj["bias"] = value.Bias.Volts;
            obj["temperature"] = value.Temperature.Kelvin;

            var arr = new JArray();

            foreach (var layer in value.Layers)
            {
                var layerObj = JsonConvert.SerializeObject(layer, new SemiconductorConverter(),
                    new MetalConverter(), new DielectricConverter());
                arr.Add(layerObj);
            }

            obj["layers"] = arr;
            return obj;
        }
    }
        
    // From http://stackoverflow.com/a/8031283/1351938
    public abstract class ExtendedJsonConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Deserialize(Type objectType, JObject jObject);

        protected abstract JObject Serialize(T value);

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
            var jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Deserialize(objectType, jObject);

            return target;
        }

        public override void WriteJson(JsonWriter writer, 
            object value,
            JsonSerializer serializer)
        {
            var jObject = Serialize((T)value);

            jObject.WriteTo(writer);
        }
    }
}

