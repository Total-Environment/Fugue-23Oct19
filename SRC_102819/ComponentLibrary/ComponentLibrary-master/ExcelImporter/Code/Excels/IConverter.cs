using System;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public interface IConverter
    {
        event OnNewUrl UrlUpdater;
        bool CanConvert(Type objectType);
        object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);
        void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
        string WriteJsonPropertyName(JsonWriter writer, string excelPropertyName);
        JsonConverter ToJsonConverter();
    }
}