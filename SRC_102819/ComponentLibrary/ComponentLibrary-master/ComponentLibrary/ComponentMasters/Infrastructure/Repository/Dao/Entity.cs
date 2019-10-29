using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Used to generate Object ID for DAOs that require it
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        /// <value>
        /// The object identifier.
        /// </value>
        [BsonId]
        [JsonIgnore]
        public ObjectId ObjectId { get; set; }

        /// <summary>
        /// Sets the object identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static ObjectId SetObjectId(string id)
        {
            return id != null ? ObjectId.Parse(id) : ObjectId.GenerateNewId();
        }

    }
}