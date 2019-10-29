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
	[BsonSerializer(typeof(MongoDBCPRCoefficientSerializer))]
	public class MongoDBCPRCoefficientSerializer : SerializerBase<CPRCoefficient>
	{
		/// <inheritdoc/>
		public override CPRCoefficient Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			var columnsDictionary = BsonSerializer.Deserialize<Dictionary<string, object>>(context.Reader);
			return new CPRCoefficient(columnsDictionary?.Select(kv => new ColumnData(null, kv.Key, kv.Value)));
		}

		/// <inheritdoc/>
		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CPRCoefficient value)
		{
			BsonSerializer.Serialize(context.Writer, value.Columns.ToDictionary(c => c.Key, c => c.Value));
		}
	}
}