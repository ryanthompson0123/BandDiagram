using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Band.Units;
using System.Collections.Generic;

namespace Band
{
    // From http://stackoverflow.com/a/8031283/1351938
    /// <summary>Base Generic JSON Converter that can help quickly define converters for specific types by automatically
    /// generating the CanConvert, ReadJson, and WriteJson methods, requiring the implementer only to define a strongly typed Create method.</summary>
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>Create an instance of objectType, based properties in the JSON object</summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">contents of JSON object that will be deserialized</param>
        protected abstract T Create(Type objectType, JObject jObject);

        /// <summary>Determines if this converted is designed to deserialization to objects of the specified type.</summary>
        /// <param name="objectType">The target type for deserialization.</param>
        /// <returns>True if the type is supported.</returns>
        public override bool CanConvert(Type objectType)
        {
            // FrameWork 4.5
            return typeof(T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        /// <summary>Parses the json to the specified type.</summary>
        /// <param name="reader">Newtonsoft.Json.JsonReader</param>
        /// <param name="objectType">Target type.</param>
        /// <param name="existingValue">Ignored</param>
        /// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
        /// <returns>Deserialized Object</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            //Create a new reader for this jObject, and set all properties to match the original reader.
            JsonReader jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            serializer.Populate(jObjectReader, target);

            return target;
        }

        /// <summary>Serializes to the specified type</summary>
        /// <param name="writer">Newtonsoft.Json.JsonWriter</param>
        /// <param name="value">Object to serialize.</param>
        /// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

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
        protected abstract T Deserialize(Type objectType, JToken jToken);

        protected abstract JToken Serialize(T value);

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
            var jToken = JToken.Load(reader);

            // Create target object based on JObject
            var target = Deserialize(objectType, jToken);

            return target;
        }

        public override void WriteJson(JsonWriter writer, 
            object value,
            JsonSerializer serializer)
        {
            var jToken = Serialize((T)value);

            jToken.WriteTo(writer);
        }
    }
}

