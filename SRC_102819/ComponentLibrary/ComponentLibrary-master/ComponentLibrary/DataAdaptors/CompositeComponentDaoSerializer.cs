using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.DataAdaptors
{
	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <seealso cref="MongoDB.Bson.Serialization.IBsonSerializer{T}"/>
	public class CompositeComponentDaoSerializer<T> : IBsonSerializer<T> where T : CompositeComponentDao, new()
	{
		/// <summary>
		/// Gets the type of the value.
		/// </summary>
		public Type ValueType { get; } = typeof(T);

		object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return Deserialize(context, args);
		}

		/// <summary>
		/// Deserializes a value.
		/// </summary>
		/// <param name="context">The deserialization context.</param>
		/// <param name="args">The deserialization args.</param>
		/// <returns>A deserialized value.</returns>
		public T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return new T()
			{
				Columns = BsonSerializer.Deserialize<Dictionary<string, object>>(context.Reader)
			};
		}

		/// <summary>
		/// Serializes a value.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		/// <param name="args">The serialization args.</param>
		/// <param name="value">The value.</param>
		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
		{
			BsonSerializer.Serialize(context.Writer, value.Columns);
		}

		/// <summary>
		/// Serializes a value.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		/// <param name="args">The serialization args.</param>
		/// <param name="value">The value.</param>
		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			Serialize(context, args, (T)value);
		}
	}
}