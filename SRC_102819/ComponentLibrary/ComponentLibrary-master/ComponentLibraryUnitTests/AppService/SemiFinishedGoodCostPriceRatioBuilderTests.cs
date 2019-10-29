using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class SemiFinishedGoodCostPriceRatioBuilderTests
	{
		[Fact]
		public async Task Create_ShouldPopulateLevels_WhenCodeIsPassed()
		{
			Mock<ICompositeComponentRepository> mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", It.IsAny<string>(), It.IsAny<ICompositeComponentDefinition>()))
				.ReturnsAsync(
					new CompositeComponent()
					{
						Headers = new List<IHeaderData>
						{
							new HeaderData("Classification", "Classification")
							{
								Columns =
									new List<IColumnData>
									{
										new ColumnData("SFG Level 1", "SFG Level 1", "HVAC"),
										new ColumnData("SFG Level 2", "SFG Level 2", "AVA")
									}
							}
						},
						CompositeComponentDefinition = new CompositeComponentDefinition { Code = "HVAC" }
					});
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			SemiFinishedGoodCostPriceRatioBuilder semiFinishedGoodCostPriceRatioBuilder =
				new SemiFinishedGoodCostPriceRatioBuilder(mockSemiFinishedGoodRepository.Object, mockCompositeComponentDefinitionRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.SFG, null, null, null, "FDP0001", null);

			var costPriceRatio = await semiFinishedGoodCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("HVAC");
			costPriceRatio.Level2.ShouldBeEquivalentTo("AVA");
		}

		[Fact]
		public async Task Create_ShouldNotPopulateLevels_WhenCodeIsNotPassed()
		{
			Mock<ICompositeComponentRepository> mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			SemiFinishedGoodCostPriceRatioBuilder semiFinishedGoodCostPriceRatioBuilder =
				new SemiFinishedGoodCostPriceRatioBuilder(mockSemiFinishedGoodRepository.Object, mockCompositeComponentDefinitionRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.SFG, "HVAC", "AVA", null, null, null);

			var costPriceRatio = await semiFinishedGoodCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("HVAC");
			costPriceRatio.Level2.ShouldBeEquivalentTo("AVA");
		}
	}
}