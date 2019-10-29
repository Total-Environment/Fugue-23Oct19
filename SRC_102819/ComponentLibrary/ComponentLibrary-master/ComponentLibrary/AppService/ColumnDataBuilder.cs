using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IColumnDataBuilder"/>
	public class ColumnDataBuilder : IColumnDataBuilder
	{
		public const string Na = "-- NA --";

		/// <summary>
		/// Builds the data.
		/// </summary>
		/// <param name="columns">The columns.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public async Task<List<IColumnData>> BuildData(List<ISimpleColumnDefinition> columns, IEnumerable<IColumnData> data)
		{
			var columnDatas = new List<IColumnData>();
			foreach (var column in columns)
			{
				var dataColumn = data.FirstOrDefault(h => h.Key == column.Key);
				if (dataColumn != null)
				{
					var columnData = await BuildColumn(column, dataColumn);
					columnDatas.Add(columnData);
				}
			}
			return columnDatas;
		}

		private async Task<IColumnData> BuildColumn(ISimpleColumnDefinition columnDefinition, IColumnData columnData)
		{
			if (columnData.Value == null)
			{
				if (columnDefinition.IsRequired)
					throw new ArgumentException($"Mandatory field: {columnDefinition.Name} cannot be null");
				return new ColumnData(columnDefinition.Name, columnDefinition.Key, null);
			}
			if (columnData.Value is string && (string)columnData.Value == "" && columnDefinition.IsRequired)
				throw new ArgumentException($"Mandatory field: {columnDefinition.Name} cannot be blank");
			if (columnData.Value is string && (string)columnData.Value == Na)
			{
				if (columnDefinition.IsRequired)
					throw new ArgumentException($"Mandatory field: {columnDefinition.Name} cannot be NA (Not Applicable)");
				return new ColumnData(columnDefinition.Name, columnDefinition.Key, Na);
			}
			object parsedColumnData;
			try
			{
				parsedColumnData = await columnDefinition.DataType.Parse(columnData.Value);
			}
			catch (FormatException e)
			{
				throw new FormatException($"Invalid format: for column {columnDefinition.Name}. ${e.Message}", e);
			}
			return new ColumnData(columnDefinition.Name, columnDefinition.Key, parsedColumnData);
		}
	}
}