using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	public class HeaderColumnDataBuilder
	{
		public const string Na = "-- NA --";

		public async Task<List<IHeaderData>> BuildData(IEnumerable<IHeaderDefinition> headers, IEnumerable<IHeaderData> data)
		{
			var headerDatas = new List<IHeaderData>();
			foreach (var materialDefinitionHeader in headers)
			{
				var materialDataHeader = data.FirstOrDefault(h => h.Key == materialDefinitionHeader.Key);
				if (materialDataHeader != null)
				{
					var headerData = await BuildHeader(materialDefinitionHeader, materialDataHeader);
					headerDatas.Add(headerData);
				}
			}
			return headerDatas;
		}

		private async Task<IColumnData> BuildColumn(IColumnDefinition columnDefinition, IColumnData columnData)
		{
			if (columnData == null || columnData.Value == null)
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

		private async Task<IHeaderData> BuildHeader(IHeaderDefinition headerDefinition, IHeaderData headerData)
		{
			var columnDatas = new List<IColumnData>();
			foreach (var columnDefinition in headerDefinition.Columns)
			{
				var rawColumnData = headerData.Columns.FirstOrDefault(c => c.Key == columnDefinition.Key);
			    var columnData = await BuildColumn(columnDefinition, rawColumnData);        
                if (columnData != null)
				{
				    columnDatas.Add(columnData);
				}
			}
			return new HeaderData(headerData.Name, headerData.Key)
			{
				Columns = columnDatas
			};
		}
	}
}