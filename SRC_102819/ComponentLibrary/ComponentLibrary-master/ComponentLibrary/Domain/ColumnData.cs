using System;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Represents the column data.
	/// </summary>
	/// <seealso cref="IColumnData"/>
	public class ColumnData : IColumnData
	{
		private string _key;

		/// <summary>
		/// Initializes a new instance of the <see cref="ColumnData"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="key"></param>
		/// <param name="value">The value.</param>
		public ColumnData(string name, string key, object value)
		{
			Name = name;
			Key = key;
			Value = value;
		}

		/// <inheritdoc/>
		public string Key
		{
			get { return _key; }
			set
			{
				if (value == null)
					throw new ArgumentException("Key is required.");
				_key = value;
			}
		}

		/// <inheritdoc/>
		public string Name { get; set; }

		/// <inheritdoc/>
		public object Value { get; set; }
	}
}