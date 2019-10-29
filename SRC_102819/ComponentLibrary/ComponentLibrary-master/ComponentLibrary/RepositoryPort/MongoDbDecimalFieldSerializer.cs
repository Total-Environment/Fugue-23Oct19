using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// The MongoDB Decimal Field Serialiser
    /// </summary>
    /// <seealso>
    ///     <cref>SerializerBase{decimal}</cref>
    /// </seealso>
    [BsonSerializer(typeof(MongoDBDecimalFieldSerializer))]
    public class MongoDBDecimalFieldSerializer : SerializerBase<decimal>
    {
        private const decimal DecimalPlace = 10000m;

        /// <inheritdoc/>
        public override decimal Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // Remove this if data is imported
            if (context.Reader.CurrentBsonType == BsonType.String)
            {
                return decimal.Parse(context.Reader.ReadString());
            }
            var dbData = context.Reader.ReadInt64();
            return Math.Round(dbData/DecimalPlace, 4);
        }

        /// <inheritdoc/>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, decimal value)
        {
            var realValue = value;
            context.Writer.WriteInt64(Convert.ToInt64(realValue*DecimalPlace));
        }
    }
}