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
	public interface IColumnDataBuilder
	{
		/// <summary>
		/// Builds the data.
		/// </summary>
		/// <param name="columns">The columns.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		Task<List<IColumnData>> BuildData(List<ISimpleColumnDefinition> columns, IEnumerable<IColumnData> data);
	}
}