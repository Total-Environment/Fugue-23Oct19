using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class ComponentCoefficientValidatorFactoryTests
	{
		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnMaterialCoefficientValidator_WhenMaterialComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientValidator(ComponentType.Material);

			result.Should().BeOfType<MaterialCoefficientValidator>();
		}

		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnServiceCoefficientValidator_WhenServiceComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientValidator(ComponentType.Service);

			result.Should().BeOfType<ServiceCoefficientValidator>();
		}

		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnAssetCoefficientValidator_WhenAssetComponentTypeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientValidator(ComponentType.Asset);

			result.Should().BeOfType<AssetCoefficientValidator>();
		}

		[Fact]
		public void
			GetComponentCoefficientValidator_ShouldReturnSemiFinishedGoodCoefficientValidator_WhenSemiFinishedGoodComponentTypeIsPassed
			()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockSemiFinishedRepository = new Mock<ICompositeComponentRepository>();
			var componentCoefficientValidatorFactory =
				new ComponentCoefficientValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockSemiFinishedRepository.Object);

			var result = componentCoefficientValidatorFactory.GetComponentCoefficientValidator(ComponentType.SFG);

			result.Should().BeOfType<SemiFinishedGoodCoefficientValidator>();
		}
	}
}