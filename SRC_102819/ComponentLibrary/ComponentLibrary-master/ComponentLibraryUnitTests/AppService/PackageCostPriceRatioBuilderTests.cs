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
	public class PackageCostPriceRatioBuilderTests
	{
		[Fact]
		public async Task Create_ShouldPopulateLevels_WhenCodeIsPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			mockPackageRepository.Setup(m => m.Find("package", It.IsAny<string>(), It.IsAny<ICompositeComponentDefinition>()))
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
										new ColumnData("Package Level 1", "Package Level 1", "HVAC"),
										new ColumnData("Package Level 2", "Package Level 2", "AVA")
									}
							}
						},
						CompositeComponentDefinition = new CompositeComponentDefinition { Code = "HVAC" }
					});
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			PackageCostPriceRatioBuilder packageCostPriceRatioBuilder =
				new PackageCostPriceRatioBuilder(mockPackageRepository.Object, mockCompositeComponentDefinitionRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Package, null, null, null, "FDP0001", null);

			var costPriceRatio = await packageCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("HVAC");
			costPriceRatio.Level2.ShouldBeEquivalentTo("AVA");
		}

		[Fact]
		public async Task Create_ShouldNotPopulateLevels_WhenCodeIsNotPassed()
		{
			Mock<ICompositeComponentRepository> mockPackageRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			PackageCostPriceRatioBuilder packageCostPriceRatioBuilder =
				new PackageCostPriceRatioBuilder(mockPackageRepository.Object, mockCompositeComponentDefinitionRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Package, "HVAC", "AVA", null, null, null);

			var costPriceRatio = await packageCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("HVAC");
			costPriceRatio.Level2.ShouldBeEquivalentTo("AVA");
		}
	}
}