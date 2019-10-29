using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace TE.ComponentLibrary.ComponentLibrary.DataAdaptors
{
    /// <summary>
    /// Serialiser for Component DAO
    /// </summary>
    public class ComponentDaoSerializer : IBsonSerializer<MaterialDao>
    {
        /// <inheritdoc />
        public Type ValueType { get; } = typeof(MaterialDao);

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        /// <inheritdoc />
        public MaterialDao Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new MaterialDao
            {
                Columns = BsonSerializer.Deserialize<Dictionary<string, object>>(context.Reader)
            };
        }

        /// <inheritdoc/>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MaterialDao value)
        {
            BsonSerializer.Serialize(context.Writer, value.Columns);
        }

        /// <inheritdoc/>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (MaterialDao) value);
        }
    }
}