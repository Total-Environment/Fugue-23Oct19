using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure
{
    public class StaticFileConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var uiRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
            var staticFile = (StaticFile)value;
            var name = staticFile.Name;
            var dict = new Dictionary<string, object>
            {
                {"url", $"{uiRoot}/static-files/{name}"},
                {"name", name},
                {"id", staticFile.Id}
            };
            serializer.Serialize(writer, dict);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<string, string>>(reader);
            return new StaticFile(dict["Id"], dict["Name"]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StaticFile);
        }
    }
}