using System.Collections.Generic;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Represents an interface for JSON converter.
    /// </summary>
    [JsonConverter(typeof(JsonSerializableConverter))]
    public interface IJsonSerializable
    {
        /// <summary>
        /// To the json.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> ToJson();
    }
}