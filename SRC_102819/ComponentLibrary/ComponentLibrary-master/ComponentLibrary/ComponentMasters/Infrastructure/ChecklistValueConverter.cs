using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure
{
    public class ChecklistValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var uiRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
            var id = ((CheckListValue)value).Id;
            var dict = new Dictionary<string, object>
            {
                {"ui", $"{uiRoot}/check-lists/{id}"},
                {"data", $"/check-lists/{id}"},
                {"id", id}
            };
            serializer.Serialize(writer, dict);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CheckListValue);
        }
    }
}