using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
	public class CostPriceRatioDefinitionControllerTests
	{
		[Fact]
		public async Task Add_ShouldAdd()
		{
			var costPriceRatioDefinition = new CostPriceRatioDefinition("Cost Price Ratio", new List<ISimpleColumnDefinition>());
			CostPriceRatioDefinitionDto costPriceRatioDefinitionDto =
				new CostPriceRatioDefinitionDto(costPriceRatioDefinition);
			Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository = new Mock<ICostPriceRatioDefinitionRepository>();
			mockCostPriceRatioDefinitionRepository.Setup(m => m.Add(It.IsAny<CostPriceRatioDefinition>()))
				.Returns(Task.CompletedTask);
			mockCostPriceRatioDefinitionRepository.Setup(m => m.FindBy(It.IsAny<string>())).ReturnsAsync(costPriceRatioDefinition);
			Mock<ISimpleDataTypeFactory> mockDataTypeFactory = new Mock<ISimpleDataTypeFactory>();
			CostPriceRatioDefinitionController costPriceRatioDefinitionController =
				new CostPriceRatioDefinitionController(mockCostPriceRatioDefinitionRepository.Object, mockDataTypeFactory.Object);

			var result = await costPriceRatioDefinitionController.Add(costPriceRatioDefinitionDto);

			result.Should().BeOfType<CreatedNegotiatedContentResult<CostPriceRatioDefinitionDto>>();
		}

		[Fact]
		public async Task Get_ShouldGet()
		{
			var costPriceRatioDefinition = new CostPriceRatioDefinition("Cost Price Ratio", new List<ISimpleColumnDefinition>());
			Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository = new Mock<ICostPriceRatioDefinitionRepository>();
			mockCostPriceRatioDefinitionRepository.Setup(m => m.FindBy(It.IsAny<string>())).ReturnsAsync(costPriceRatioDefinition);
			Mock<ISimpleDataTypeFactory> mockDataTypeFactory = new Mock<ISimpleDataTypeFactory>();
			CostPriceRatioDefinitionController costPriceRatioDefinitionController =
				new CostPriceRatioDefinitionController(mockCostPriceRatioDefinitionRepository.Object, mockDataTypeFactory.Object);

			var result = await costPriceRatioDefinitionController.Get();

			result.Should().BeOfType<OkNegotiatedContentResult<CostPriceRatioDefinitionDto>>();
		}
	}
}