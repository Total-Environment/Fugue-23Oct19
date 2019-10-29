using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public class MaterialCostPriceRatioBuilderTests
	{
		[Fact]
		public async Task Create_ShouldPopulateLevels_WhenCodeIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Find(It.IsAny<string>()))
				.ReturnsAsync(
					new Material(
						new List<IHeaderData>
						{
							new HeaderData("Classification", "Classification")
							{
								Columns =
									new List<IColumnData>
									{
										new ColumnData("Material Level 1", "Material Level 1", "Primary"),
										new ColumnData("Material Level 2", "Material Level 2", "A&C"),
										new ColumnData("Material Level 3", "Material Level 3", "A")
									}
							}
						}, new MaterialDefinition("A&C")));
			MaterialCostPriceRatioBuilder materialCostPriceRatioBuilder = new MaterialCostPriceRatioBuilder(mockMaterialRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Material, null, null, null, "ALM0001", null);

			var costPriceRatio = await materialCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("Primary");
			costPriceRatio.Level2.ShouldBeEquivalentTo("A&C");
			costPriceRatio.Level3.ShouldBeEquivalentTo("A");
		}

		[Fact]
		public async Task Create_ShouldNotPopulateLevels_WhenCodeIsNotPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			MaterialCostPriceRatioBuilder materialCostPriceRatioBuilder = new MaterialCostPriceRatioBuilder(mockMaterialRepository.Object);
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Material, "Primary", "A&C", "A", null, null);

			var costPriceRatio = await materialCostPriceRatioBuilder.Build(costPriceRatioToBeCreated);
			costPriceRatio.Level1.ShouldBeEquivalentTo("Primary");
			costPriceRatio.Level2.ShouldBeEquivalentTo("A&C");
			costPriceRatio.Level3.ShouldBeEquivalentTo("A");
		}
	}
}