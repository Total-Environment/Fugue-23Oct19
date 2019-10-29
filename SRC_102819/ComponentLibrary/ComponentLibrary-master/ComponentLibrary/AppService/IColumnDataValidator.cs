using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface IColumnDataValidator
	{
		/// <summary>
		/// Validates the specified column definition.
		/// </summary>
		/// <param name="columnDefinition">The column definition.</param>
		/// <param name="columnData">The column data.</param>
		/// <returns></returns>
		Tuple<bool, string> Validate(List<ISimpleColumnDefinition> columnDefinition, IEnumerable<IColumnData> columnData);
	}
}