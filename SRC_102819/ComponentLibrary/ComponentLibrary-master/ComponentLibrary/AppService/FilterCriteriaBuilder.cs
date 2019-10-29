using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.Extensions;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// Represents a filter criteria builder
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IFilterCriteriaBuilder"/>
	public class FilterCriteriaBuilder : IFilterCriteriaBuilder
	{
		/// <inheritdoc/>
		public Dictionary<string, Tuple<string, object>> Build(IMaterialDefinition materialDefinition, IBrandDefinition brandDefinition, List<FilterData> filterData, string group, List<string> searchKeywords)

		{
			var filterCriteria = new Dictionary<string, Tuple<string, object>>();
			var hasAssetCriteria =
				filterData.Any(c => c.ColumnKey == ComponentDao.CanBeUsedAsAnAsset && c.ColumnValue == "true");
			filterCriteria.Add(ComponentDao.MaterialLevel2, new Tuple<string, object>("Regex", $"((?i){group}(?-i))"));
			filterCriteria.Add($"{ComponentDao.SearchKeywords}._v", new Tuple<string, object>("Regex", GenerateRegex(searchKeywords.ToArray())));

			var definitionColumns = materialDefinition.Headers.SelectMany(
				 h => h.Columns.Select(c => new KeyValuePair<string, IDataType>(c.Key, c.DataType))).ToList();
			var brandDefinitionColumns =
				brandDefinition.Columns.Select(c => new KeyValuePair<string, IDataType>(c.Key, c.DataType)).ToList();
			if (filterData != null)
			{
				foreach (var filterCriterion in filterData)
				{
					var keyValuePair = GenrateFilterCriteria(filterCriterion, definitionColumns, brandDefinitionColumns);
					foreach (var entry in keyValuePair)
					{
						filterCriteria.Add(entry.Key, entry.Value);
					}
				}
			}
			if (hasAssetCriteria && !filterCriteria.ContainsKey(ComponentDao.CanBeUsedAsAnAsset))
			{
				filterCriteria.Add(ComponentDao.CanBeUsedAsAnAsset, new Tuple<string, object>("Eq", true));
			}
			return filterCriteria;
		}

		public Dictionary<string, Tuple<string, object>> Build(IServiceDefinition serviceDefinition,
			List<FilterData> filterData, string @group, List<string> searchKeywords)
		{
			var filterCriteria = new Dictionary<string, Tuple<string, object>>();
			filterCriteria.Add(ComponentDao.ServiceLevel1, new Tuple<string, object>("Regex", $"((?i){group}(?-i))"));
			filterCriteria.Add($"{ComponentDao.SearchKeywords}._v",
				new Tuple<string, object>("Regex", GenerateRegex(searchKeywords.ToArray())));

			var definitionColumns = serviceDefinition.Headers.SelectMany(
				h => h.Columns.Select(c => new KeyValuePair<string, IDataType>(c.Key, c.DataType))).ToList();
			if (filterData != null)
			{
				foreach (var filterCriterion in filterData)
				{
					var keyValuePair = GenrateFilterCriteria(filterCriterion, definitionColumns, null);
					foreach (var entry in keyValuePair)
					{
						filterCriteria.Add(entry.Key, entry.Value);
					}
				}
			}
			return filterCriteria;
		}

		/// <summary>
		/// Builds the specified semi finished good definition.
		/// </summary>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <param name="filterData">The filter data.</param>
		/// <param name="compositeComponentDefinition">The semi finished good definition.</param>
		/// <returns></returns>
		public Dictionary<string, Tuple<string, object>> Build(List<string> searchKeywords,
			List<FilterData> filterData, ICompositeComponentDefinition compositeComponentDefinition)
		{
			var filterCriteria = new Dictionary<string, Tuple<string, object>>();

			if (searchKeywords.Count > 0)
			{
				filterCriteria.Add($"{ComponentDao.SearchKeywords}._v",
					new Tuple<string, object>("Regex", GenerateRegex(searchKeywords.ToArray())));
			}

			var definitionColumns = compositeComponentDefinition.Headers.SelectMany(
				h => h.Columns.Select(c => new KeyValuePair<string, IDataType>(c.Key, c.DataType))).ToList();
			if (filterData != null)
			{
				foreach (var filterCriterion in filterData)
				{
					var keyValuePair = GenrateFilterCriteria(filterCriterion, definitionColumns, null);
					foreach (var entry in keyValuePair)
					{
						filterCriteria.Add(entry.Key, entry.Value);
					}
				}
			}
			return filterCriteria;
		}

		/// <inheritdoc/>
		public
		Dictionary<string, Tuple<string, object>> BuildRateFilters(List<FilterData> filterData, string componentType)
		{
			var filterCriteria = new Dictionary<string, Tuple<string, object>>();
			foreach (var data in filterData)
			{
				var tuple = data.ColumnKey.Equals(MaterialDao.AppliedOn)
					? new Tuple<string, object>("Lte", DateTime.Parse(data.ColumnValue).InIst().Date)
					: new Tuple<string, object>("Eq", data.ColumnValue);

				filterCriteria.Add(
					data.ColumnKey.Contains($"{componentType}_level") || data.ColumnKey == $"{componentType}_status" ? data.ColumnKey : $"rates._v.{data.ColumnKey}", tuple);
			}

			return filterCriteria;
		}

		private static void ParseMoneyValue(string columnName, string columnValue, Dictionary<string, Tuple<string, object>> column)
		{
			if (columnValue.IsEmpty())
			{
				throw new ArgumentException("Invalid money value.");
			}
			var parts = columnValue.Split(' ');
			decimal decimalValue = 0;
			if (parts.Length != 2 || !Decimal.TryParse(parts[0], out decimalValue) || parts[1].IsEmpty() || parts[1].Length < 3)
			{
				throw new ArgumentException("Invalid money value.");
			}
			decimal DECIMAL_PLACE = 10000m;
			column.Add($"{columnName.ToLower()}.Amount", new Tuple<string, object>("Eq", Convert.ToInt32(decimalValue * DECIMAL_PLACE)));
			column.Add($"{columnName.ToLower()}.Currency", new Tuple<string, object>("Regex", $"((?i){parts[1]}(?-i))"));
		}

		private Dictionary<string, Tuple<string, object>> GenerateFilterKey(IDataType dataType, string columnName, string columnValue)
		{
			var column = new Dictionary<string, Tuple<string, object>>();
			if (dataType == null)
				return column;
			switch (dataType.GetType().Name)
			{
				case "MasterDataDataType":
				case "StringDataType":
					if ((columnValue == null) || (columnValue.Length < 1))
					{
						throw new ArgumentException("String should be minimum 1 letter long.");
					}
					column.Add(columnName, new Tuple<string, object>("Regex", $"((?i){columnValue}(?-i))"));
					break;

				case "AutogeneratedDataType":
					var autogeneratedDataType = dataType as AutogeneratedDataType;
					if (autogeneratedDataType.SubType == "Material Code"
						|| autogeneratedDataType.SubType.Equals("Brand Code")
						|| autogeneratedDataType.SubType.Equals("SFG Code")
						|| autogeneratedDataType.SubType.Equals("Package Code")
                        || autogeneratedDataType.SubType.Equals("Service Code"))
					{
						if ((columnValue == null) || (columnValue.Length < 1))
						{
							throw new ArgumentException("String should be minimum 1 letter long.");
						}
						column.Add(columnName, new Tuple<string, object>("Regex", $"((?i){columnValue}(?-i))"));
					}
					break;

				case "BooleanDataType":
					bool boolValue;
					if (columnValue.IsEmpty() || !bool.TryParse(columnValue, out boolValue))
					{
						throw new ArgumentException("Invalid boolean value.");
					}
					column.Add(columnName, new Tuple<string, object>("Eq", boolValue));
					break;

				case "IntDataType":
					int intValue = 0;
					if (columnValue.IsEmpty() || !int.TryParse(columnValue, out intValue))
					{
						throw new ArgumentException("Invalid int value.");
					}
					column.Add(columnName, new Tuple<string, object>("Eq", intValue));
					break;

				case "DecimalDataType":
					decimal decimalValue = 0;
					if (columnValue.IsEmpty() || !decimal.TryParse(columnValue, out decimalValue))
					{
						throw new ArgumentException("Invalid decimal value.");
					}
					decimal DECIMAL_PLACE = 10000m;
					column.Add($"{columnName}._v", new Tuple<string, object>("Eq", Convert.ToInt32(decimalValue * DECIMAL_PLACE)));
					break;

				case "MoneyDataType":
					ParseMoneyValue(columnName, columnValue, column);
					break;

				case "UnitDataType":
					double doubleValue = 0;
					if (columnValue.IsEmpty() || !double.TryParse(columnValue, out doubleValue))
					{
						throw new ArgumentException("Invalid unit value.");
					}
					column.Add($"{columnName}.Value", new Tuple<string, object>("Eq", doubleValue));
					break;

				case "ArrayDataType":
					var arrayType = dataType as ArrayDataType;
					if (arrayType.DataType is StringDataType)
					{
						if ((columnValue == null) || (columnValue.Length < 1))
						{
							throw new ArgumentException("String should be minimum 1 letter long.");
						}
						column.Add($"{columnName}._v", new Tuple<string, object>("Regex", $"/({columnValue})/i"));
					}
					break;

				case "RangeDataType":
					double doubleRangeValue = 0;
					if (columnValue.IsEmpty() || !double.TryParse(columnValue, out doubleRangeValue))
					{
						throw new ArgumentException("Invalid range value.");
					}
					column.Add($"{columnName}.From", new Tuple<string, object>("Lte", doubleRangeValue));
					column.Add($"{columnName}.To", new Tuple<string, object>("Gte", doubleRangeValue));
					break;
			}
			return column;
		}

		private string GenerateRegex(string[] keywordsList)
		{
			return $"/({string.Join("|", keywordsList)})/i";
		}

		private Dictionary<string, Tuple<string, object>> GenrateFilterCriteria(FilterData filterCriterion, List<KeyValuePair<string, IDataType>> definitionColumns, List<KeyValuePair<string, IDataType>> brandDefinitionColumns)
		{
			var key = filterCriterion.ColumnKey;
			var value = filterCriterion.ColumnValue;
			var matchingdefinition = definitionColumns.SingleOrDefault(d => string.Equals(d.Key, key, StringComparison.InvariantCultureIgnoreCase));
			if (matchingdefinition.Equals(default(KeyValuePair<string, IDataType>)))
			{
				if (brandDefinitionColumns != null)
				{
					matchingdefinition =
						brandDefinitionColumns.SingleOrDefault(
							d => string.Equals(d.Key, key, StringComparison.InvariantCultureIgnoreCase));

					return GenerateFilterKey(matchingdefinition.Value, $"approved_brands._v._v.{key}.Value", value);
				}
			}
			return GenerateFilterKey(matchingdefinition.Value, key, value);
		}
	}
}