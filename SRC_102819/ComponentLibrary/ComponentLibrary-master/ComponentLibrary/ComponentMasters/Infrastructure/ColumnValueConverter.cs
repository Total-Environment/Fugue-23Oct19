using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure
{
    /// <summary>
    /// Custom Converter for the columnValues
    /// </summary>
    public class ColumnValueConverter : JsonConverter
    {
        private static readonly List<Type> Types = new List<Type>
            {
                typeof(bool),
                typeof(DateTime),
                typeof(string),
                typeof(Dictionary<string, object>),
                typeof(List<object>)
            };

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return Types.Contains(objectType);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jToken = JToken.Load(reader);
            return Convert(jToken);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private object Convert(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    var dictionary = new Dictionary<string, object>();
                    var jObject = ((JObject)jToken);
                    jObject.Properties()
                        .ToList()
                        .ForEach(property => dictionary.Add(property.Name, Convert(property.Value)));
                    return dictionary;

                case JTokenType.Array:
                    var list = new List<object>();
                    var jArray = (JArray)jToken;
                    jArray.ToList().ForEach(token => list.Add(Convert(token)));
                    return list;

                case JTokenType.Integer:
                    return jToken.ToObject<string>();
                //                    return ((JObject) jToken).ToObject<int>();
                case JTokenType.Float:
                    return jToken.ToObject<string>();

                case JTokenType.String:
                    return jToken.ToObject<string>();

                case JTokenType.Boolean:
                    return jToken.ToObject<string>();

                case JTokenType.Null:
                    return null;

                case JTokenType.Date:
                    return jToken.ToObject<string>();

                default:
                    return jToken.Value<string>();
            }
        }
    }
}