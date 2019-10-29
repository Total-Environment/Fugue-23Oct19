using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class ComponentCoefficientBuilderFactoryTests
	{
		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnMaterialCoefficientValidator_WhenMaterialComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var mockSemiFinishedGoodDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object, mockSemiFinishedGoodDefinitionRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientBuilder(ComponentType.Material);

			result.Should().BeOfType<MaterialCoefficientBuilder>();
		}

		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnServiceCoefficientValidator_WhenServiceComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var mockSemiFinishedGoodDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object, mockSemiFinishedGoodDefinitionRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientBuilder(ComponentType.Service);

			result.Should().BeOfType<ServiceCoefficientBuilder>();
		}

		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnAssetCoefficientValidator_WhenAssetComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var mockSemiFinishedGoodDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object, mockSemiFinishedGoodDefinitionRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientBuilder(ComponentType.Asset);
			result.Should().BeOfType<AssetCoefficientBuilder>();
		}

		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnSemiFinishedGoodCoefficientValidator_WhenSemiFinishedGoodComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var mockSemiFinishedGoodDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object, mockSemiFinishedGoodDefinitionRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientBuilder(ComponentType.SFG);
			result.Should().BeOfType<SemiFinishedGoodCoefficientBuilder>();
		}
	}
}