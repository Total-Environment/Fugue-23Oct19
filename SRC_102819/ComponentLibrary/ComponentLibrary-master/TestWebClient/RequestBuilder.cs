using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TE.ComponentLibrary.TestWebClient
{
    public static class RequestBuilder<T>
    {
        public static T BuildPostRequest(string fileName, string nullField = null, string nullValue = null)
        {
            var body = File.ReadAllText($"RequestJsons/{fileName}");
            body = NullifyField(nullField, body, nullValue);
            return JsonConvert.DeserializeObject<T>(body, new StringEnumConverter());
        }

        private static string NullifyField(string nullField, string body, string nullvalue)
        {
            if (nullField == null) return body;
            var resource = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
            if (nullvalue == null)
                resource.Remove(nullField);
            else
                resource[nullField] = nullvalue;
            body = JsonConvert.SerializeObject(resource);
            return body;
        }
    }
}
