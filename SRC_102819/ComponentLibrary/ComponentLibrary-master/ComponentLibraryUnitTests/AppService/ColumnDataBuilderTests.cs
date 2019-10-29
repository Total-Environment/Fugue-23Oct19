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
	public class ColumnDataBuilderTests
	{
		[Fact]
		public async Task BuildData_ShouldAcceptSimpleColumnDefinitionsAndColumnDatas_AndReturnColumnDatas()
		{
			ColumnDataBuilder columnDataBuilder = new ColumnDataBuilder();
			List<ISimpleColumnDefinition> columns = new List<ISimpleColumnDefinition> { new SimpleColumnDefinition("name", "key", new UnitDataType("%")) };
			IEnumerable<IColumnData> data = new List<IColumnData>
			{
				new ColumnData("name", "key", new Dictionary<string, object> {{"type", "%"}, {"value", "10.4"}})
			};

			var builtData = await columnDataBuilder.BuildData(columns, data);

			builtData[0].Key.ShouldBeEquivalentTo("key");
			builtData[0].Name.ShouldBeEquivalentTo("name");
			builtData[0].Value.GetType().ShouldBeEquivalentTo(typeof(UnitValue));
			((UnitValue)builtData[0].Value).Type.ShouldBeEquivalentTo("%");
			((UnitValue)builtData[0].Value).Value.ShouldBeEquivalentTo(10.4);
		}
	}
}