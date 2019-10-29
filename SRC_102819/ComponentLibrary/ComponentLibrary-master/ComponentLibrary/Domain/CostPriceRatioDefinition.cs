using System;
using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	public class CostPriceRatioDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDefinition"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="columns">The columns.</param>
		/// <exception cref="System.ArgumentNullException">name or columns</exception>
		public CostPriceRatioDefinition(string name, List<ISimpleColumnDefinition> columns)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			Name = name;
			if (columns == null)
				throw new ArgumentNullException(nameof(columns));
			Columns = columns;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; }

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public List<ISimpleColumnDefinition> Columns { get; }
	}
}