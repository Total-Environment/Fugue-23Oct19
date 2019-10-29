using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The Serialiser for Service DAO
    /// </summary>
    public class ServiceDaoSerializer : IBsonSerializer<ServiceDao>
    {
        /// <inheritdoc />
        public Type ValueType { get; } = typeof(MaterialDao);

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        /// <inheritdoc />
        public ServiceDao Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new ServiceDao
            {
                Columns = BsonSerializer.Deserialize<Dictionary<string, object>>(context.Reader)
            };
        }

        /// <inheritdoc/>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ServiceDao value)
        {
            BsonSerializer.Serialize(context.Writer, value.Columns);
        }

        /// <inheritdoc/>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (ServiceDao)value);
        }
    }
}