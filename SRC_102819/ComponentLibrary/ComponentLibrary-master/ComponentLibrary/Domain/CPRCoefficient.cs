using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	public class CPRCoefficient
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CPRCoefficient"/> class.
		/// </summary>
		/// <param name="columns">The columns.</param>
		public CPRCoefficient(IEnumerable<IColumnData> columns)
		{
			Columns = columns;
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public IEnumerable<IColumnData> Columns { get; }

		/// <summary>
		/// Gets the CPR.
		/// </summary>
		/// <value>The CPR.</value>
		public double CPR
		{
			get
			{
				double sum = Columns
					.Where(c => c.Value != null)
					.Select(c => ((UnitValue)c.Value).Value)
					.Aggregate<double, double>(0, (current, value) => current + value);
				return Math.Round((1 + sum / 100), 2);
			}
		}
	}
}