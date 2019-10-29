using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="IColumnDataValidator"/>
	public class ColumnDataValidator : IColumnDataValidator
	{
		/// <inheritdoc/>
		public Tuple<bool, string> Validate(List<ISimpleColumnDefinition> columnDefinition, IEnumerable<IColumnData> columnData)
		{
			var invalidColumnKeys = columnData.Select(column => column.Key).Except(columnDefinition.Select(c => c.Key))
				.ToList();

			if (invalidColumnKeys.Any())
			{
				return new Tuple<bool, string>(false, $"Column(s) not found, keys did not match with definition: { string.Join(", ", invalidColumnKeys)}");
			}

			return new Tuple<bool, string>(true, string.Empty);
		}
	}
}