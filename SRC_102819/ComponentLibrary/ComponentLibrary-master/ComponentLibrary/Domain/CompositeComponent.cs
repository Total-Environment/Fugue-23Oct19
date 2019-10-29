using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// Semi finished good.
	/// </summary>
	/// <seealso cref="System.Dynamic.DynamicObject"/>
	public class CompositeComponent : DynamicObject
	{
		/// <summary>
		/// Gets or sets the Sfg code.
		/// </summary>
		/// <value>The code.</value>
		public string Code { get; set; }

		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>The group.</value>
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public List<IHeaderData> Headers { get; set; }

		/// <summary>
		/// Gets or sets the semi finished good composition.
		/// </summary>
		/// <value>The semi finished good composition.</value>
		public IComponentComposition ComponentComposition { get; set; }

		/// <summary>
		/// Gets or sets the semi finished good definition.
		/// </summary>
		/// <value>The semi finished good definition.</value>
		public ICompositeComponentDefinition CompositeComponentDefinition { get; set; }

		/// <summary>
		/// Provides the implementation for operations that get member values. Classes derived from
		/// the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to
		/// specify dynamic behavior for operations such as getting a value for a property.
		/// </summary>
		/// <param name="binder">
		/// Provides information about the object that called the dynamic operation. The binder.Name
		/// property provides the name of the member on which the dynamic operation is performed. For
		/// example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where
		/// sampleObject is an instance of the class derived from the <see
		/// cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The
		/// binder.IgnoreCase property specifies whether the member name is case-sensitive.
		/// </param>
		/// <param name="result">
		/// The result of the get operation. For example, if the method is called for a property, you
		/// can assign the property value to <paramref name="result"/>.
		/// </param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the
		/// run-time binder of the language determines the behavior. (In most cases, a run-time
		/// exception is thrown.)
		/// </returns>
		/// <exception cref="ArgumentException"></exception>
		public override bool TryGetMember(GetMemberBinder binder,
			out object result)
		{
			result = Headers.FirstOrDefault(h => h.Key == binder.Name);
			if (result == null)
				throw new ArgumentException($"No header with {binder.Name} is found.");
			return true;
		}

		/// <summary>
		/// Provides the implementation for operations that set member values. Classes derived from
		/// the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to
		/// specify dynamic behavior for operations such as setting a value for a property.
		/// </summary>
		/// <param name="binder">
		/// Provides information about the object that called the dynamic operation. The binder.Name
		/// property provides the name of the member to which the value is being assigned. For
		/// example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an
		/// instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/>
		/// class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies
		/// whether the member name is case-sensitive.
		/// </param>
		/// <param name="value">
		/// The value to set to the member. For example, for sampleObject.SampleProperty = "Test",
		/// where sampleObject is an instance of the class derived from the <see
		/// cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".
		/// </param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the
		/// run-time binder of the language determines the behavior. (In most cases, a
		/// language-specific run-time exception is thrown.)
		/// </returns>
		/// <exception cref="InvalidOperationException"></exception>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			throw new InvalidOperationException();
		}

	    /// <summary>
	    /// Updates the column.
	    /// </summary>
	    /// <param name="type"></param>
	    /// <param name="columnKey">Column Key.</param>
	    /// <param name="value">The value.</param>
	    public void UpdateColumn(string type, string columnKey, Dictionary<string, object> value)
		{
			IHeaderData header;
			var headers = HeaderWithColumnKey(type, columnKey, out header);
			IColumnData column;
			var columns = ColumnWithKey(type, columnKey, header, out column);
			var headerDefinition = HeaderDefinitionWithColumnKey(columnKey);
			var columnDefinition = ColumnDefinitionWithKey(columnKey, headerDefinition);
			var parsedValue = columnDefinition.Parse(value).Result;
			column.Value = parsedValue.Value;
			header.Columns = columns;
			Headers = headers;
		}

		private List<IHeaderData> HeaderWithColumnKey(string type, string columnKey, out IHeaderData header)
		{
		    var componentType = type == "sfg" ? "SFG" : "Package";
			var headers = new List<IHeaderData>(Headers);
			header =
				headers.FirstOrDefault(
					h =>
						h != null &&
						h.Columns.Any(
							c =>
								c != null &&
								string.Equals(c.Key, columnKey, StringComparison.CurrentCultureIgnoreCase)));
			if (header == null)
				throw new ArgumentException($"{componentType} {Code} does not have column \"{columnKey}\".");
			return headers;
		}

		private List<IColumnData> ColumnWithKey(string type, string columnKey, IHeaderData header, out IColumnData column)
		{
		    var componentType = type == "sfg" ? "SFG" : "Package";
            var columns = new List<IColumnData>(header.Columns);
			column =
				columns.FirstOrDefault(c => string.Equals(c.Key, columnKey, StringComparison.CurrentCultureIgnoreCase));
			if (column == null)
				throw new ArgumentException($"{componentType} {Code} does not have column {columnKey}.");
			return columns;
		}

		private IHeaderDefinition HeaderDefinitionWithColumnKey(string columnKey)
		{
			var headerDefinition = CompositeComponentDefinition.Headers.FirstOrDefault(
				h => h.Columns.Any(c => string.Equals(c.Key, columnKey, StringComparison.CurrentCultureIgnoreCase)));
			if (headerDefinition == null)
				throw new ArgumentException($"Definition does not have column {columnKey}.");
			return headerDefinition;
		}

		private static IColumnDefinition ColumnDefinitionWithKey(string columnKey, IHeaderDefinition headerDefinition)
		{
			var columnDefinition =
				headerDefinition.Columns.FirstOrDefault(
					c => string.Equals(c.Key, columnKey, StringComparison.CurrentCultureIgnoreCase));
			if (columnDefinition == null)
				throw new ArgumentException($"Definition does not have column {columnKey}.");
			return columnDefinition;
		}

		/// <summary>
		/// Gets the <see cref="IHeaderData"/> with the specified key.
		/// </summary>
		/// <value>The <see cref="IHeaderData"/>.</value>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public IHeaderData this[string key]
		{
			get { return Headers.FirstOrDefault(h => h.Name == key); }
		}
	}
}