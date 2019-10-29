using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// </summary>
    [BsonSerializer(typeof(MongoDBBrandSerializer))]
    public class MongoDBBrandSerializer : SerializerBase<Brand>
    {
        /// <inheritdoc />
        public override Brand Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var columnsDictionary =
                BsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(context.Reader);
            return
                new Brand(
                    columnsDictionary?.Select(kv => new ColumnData((string) kv.Value["Name"], kv.Key, kv.Value["Value"])),
                    null);
        }

        /// <inheritdoc />
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Brand value)
        {
            BsonSerializer.Serialize(context.Writer,
                value.Columns.ToDictionary(c => c.Key,
                    c => new Dictionary<string, object> {{"Name", c.Name}, {"Value", c.Value}}));
        }
    }
}