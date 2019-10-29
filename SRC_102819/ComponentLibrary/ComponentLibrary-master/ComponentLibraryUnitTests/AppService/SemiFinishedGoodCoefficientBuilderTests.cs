using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class SemiFinishedGoodCoefficientBuilderTests
	{
		[Fact]
		public async Task BuildData_ShouldSetNameAndUnitOfMeasureProperties_WhenValidComponentCoefficientIsPassed()
		{
			var mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", "code", It.IsAny<ICompositeComponentDefinition>()))
				.ReturnsAsync(new CompositeComponent
				{
					Headers =
						new List<IHeaderData>
						{
							new HeaderData("General", "General")
							{
								Columns =
									new List<IColumnData>
									{
										new ColumnData("Short Description", "Short Description", "test"),
										new ColumnData("Unit Of Measure", "Unit Of Measure", "uom")
									}
							}
						}
				});
			var mockSemiFinishedGoodDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			mockSemiFinishedGoodDefinitionRepository.Setup(m => m.Find("sfg", Keys.Sfg.SfgDefinitionGroup))
				.ReturnsAsync(new CompositeComponentDefinition
				{
					Headers =
						new List<IHeaderDefinition>
						{
							new HeaderDefinition("General", "General",
								new List<IColumnDefinition>
								{
									new ColumnDefinition("Short Description", "Short Description", new StringDataType()),
									new ColumnDefinition("Unit of Measure", "Unit Of Measure", new StringDataType())
								})
						}
				});
			var semiFinishedGoodCoefficientBuilder = new SemiFinishedGoodCoefficientBuilder(
				mockSemiFinishedGoodRepository.Object,
				mockSemiFinishedGoodDefinitionRepository.Object);

			var result = await semiFinishedGoodCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
				"", ComponentType.SFG));

			result.Name.Should().BeEquivalentTo("test");
			result.UnitOfMeasure.Should().BeEquivalentTo("uom");
		}

		[Fact]
		public void BuildData_ShouldThrowArgumentException_WhenComponentCoefficientWithNonExistingServiceIsPassed()
		{
			var mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", "code", It.IsAny<ICompositeComponentDefinition>()))
				.ReturnsAsync(null);
			var semiFinishedGoodCoefficientBuilder = new SemiFinishedGoodCoefficientBuilder(
				mockSemiFinishedGoodRepository.Object,
				new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>().Object);

			Func<Task> func = async () => await semiFinishedGoodCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
				"", ComponentType.SFG));

			func.ShouldThrow<ArgumentException>().WithMessage("Invalid semi finished good code. code");
		}
	}
}