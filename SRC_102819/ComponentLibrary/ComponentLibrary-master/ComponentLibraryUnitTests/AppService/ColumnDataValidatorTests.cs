using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class ColumnDataValidatorTests
	{
		[Fact]
		public void Validate_ShouldRetunsSuccessValidationResult_WhenDataColumnsArePartOfColumnDefinitions()
		{
			ColumnDataValidator columnDataValidator = new ColumnDataValidator();

			List<ISimpleColumnDefinition> columnDefinitions = new List<ISimpleColumnDefinition> { new SimpleColumnDefinition("name", "key", new UnitDataType("%")) };
			IEnumerable<IColumnData> columnDatas = new List<IColumnData> { new ColumnData("name", "key", new Dictionary<string, object> { { "type", "%" }, { "value", "10.4" } }) };
			var result = columnDataValidator.Validate(columnDefinitions, columnDatas);

			result.Item1.ShouldBeEquivalentTo(true);
			result.Item2.ShouldBeEquivalentTo("");
		}

		[Fact]
		public void Validate_ShouldRetunsSuccessValidationResult_WhenDataColumnsAreNotPartOfColumnDefinitions()
		{
			ColumnDataValidator columnDataValidator = new ColumnDataValidator();

			List<ISimpleColumnDefinition> columnDefinitions = new List<ISimpleColumnDefinition> { new SimpleColumnDefinition("name", "key", new UnitDataType("%")) };
			IEnumerable<IColumnData> columnDatas = new List<IColumnData> { new ColumnData("name1", "key1", new Dictionary<string, object> { { "type", "%" }, { "value", "10.4" } }) };
			var result = columnDataValidator.Validate(columnDefinitions, columnDatas);

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Column(s) not found, keys did not match with definition: key1");
		}
	}
}