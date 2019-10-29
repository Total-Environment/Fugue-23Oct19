using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerAdaptersTests
{
	public class CompositeComponentDtoTests
	{
		[Fact]
		public void Create_ShouldCreateDto_FromSfgData()
		{
			var headerData = new HeaderData("name", "key");
			headerData.AddColumns(new ColumnData("cname", "ckey", "cvalue"));

			CompositeComponentDto sut = new CompositeComponentDto(new CompositeComponent()
			{
				Code = "code",
				Group = "group",
				Headers = new List<IHeaderData>() { headerData },
				ComponentComposition = new ComponentComposition(new List<ComponentCoefficient>()),
				CompositeComponentDefinition = new CompositeComponentDefinition()
				{
					Headers = new List<IHeaderDefinition>()
					{
						new HeaderDefinition("name","key", new List<IColumnDefinition>()
						{
							new ColumnDefinition("cname","ckey",new StringDataType(),false,false)
						},
						null
						)
					}
				}
			});
			sut.Code.Should().Be("code");
			sut.Group.Should().Be("group");
			sut.Headers.Count.Should().Be(1);
			sut.Headers.FirstOrDefault().Key.Should().Be("key");
			sut.Headers.FirstOrDefault().Name.Should().Be("name");
			sut.Headers.FirstOrDefault().Columns.Count().Should().Be(1);
			sut.Headers.FirstOrDefault().Columns.FirstOrDefault().Key.Should().Be("ckey");
			sut.Headers.FirstOrDefault().Columns.FirstOrDefault().Name.Should().Be("cname");
			sut.Headers.FirstOrDefault().Columns.FirstOrDefault().Value.Should().Be("cvalue");
			sut.Headers.FirstOrDefault().Columns.FirstOrDefault().DataType.Name.ShouldBeEquivalentTo("String");
		}
	}
}