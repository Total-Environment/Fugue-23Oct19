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
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class CostPriceRatioValidatorFactoryTests
	{
		[Fact]
		public void GetCostPriceRatioValidator_ShouldReturnMaterialCostPriceRatioValidator_WhenMaterialIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			CostPriceRatioValidatorFactory costPriceRatioValidatorFactory =
				new CostPriceRatioValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockProjectDataRepository.Object, mockDependencyDefinitionRepository.Object);

			var costPriceRatioValidator = costPriceRatioValidatorFactory.GetCostPriceRatioValidator(ComponentType.Material);

			costPriceRatioValidator.Should().BeOfType<MaterialCostPriceRatioValidator>();
		}

		[Fact]
		public void GetCostPriceRatioValidator_ShouldReturnServiceCostPriceRatioValidator_WhenServiceIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			CostPriceRatioValidatorFactory costPriceRatioValidatorFactory =
				new CostPriceRatioValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockProjectDataRepository.Object, mockDependencyDefinitionRepository.Object);

			var costPriceRatioValidator = costPriceRatioValidatorFactory.GetCostPriceRatioValidator(ComponentType.Service);

			costPriceRatioValidator.Should().BeOfType<ServiceCostPriceRatioValidator>();
		}

		[Fact]
		public void GetCostPriceRatioValidator_ShouldThrowNotImplementedException_WhenAssetIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			CostPriceRatioValidatorFactory costPriceRatioValidatorFactory =
				new CostPriceRatioValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockProjectDataRepository.Object, mockDependencyDefinitionRepository.Object);

			Action action = () => costPriceRatioValidatorFactory.GetCostPriceRatioValidator(ComponentType.Asset);
			action.ShouldThrow<NotImplementedException>().WithMessage("Asset is not implemented. Try with Material.");
		}

		[Fact]
		public void GetCostPriceRatioValidator_ShouldReturnSemiFinsihedGoodCostPriceRatioValidator_WhenSemiFinishedGoodIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			CostPriceRatioValidatorFactory costPriceRatioValidatorFactory =
				new CostPriceRatioValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockProjectDataRepository.Object, mockDependencyDefinitionRepository.Object);

			var costPriceRatioValidator = costPriceRatioValidatorFactory.GetCostPriceRatioValidator(ComponentType.SFG);

			costPriceRatioValidator.Should().BeOfType<SemiFinishedGoodCostPriceRatioValidator>();
		}

		[Fact]
		public void GetCostPriceRatioValidator_ShouldReturnPackageCostPriceRatioValidator_WhenSPackageIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<IProjectRepository> mockProjectDataRepository = new Mock<IProjectRepository>();
			Mock<IDependencyDefinitionRepository> mockDependencyDefinitionRepository = new Mock<IDependencyDefinitionRepository>();
			CostPriceRatioValidatorFactory costPriceRatioValidatorFactory =
				new CostPriceRatioValidatorFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockProjectDataRepository.Object, mockDependencyDefinitionRepository.Object);

			var costPriceRatioValidator = costPriceRatioValidatorFactory.GetCostPriceRatioValidator(ComponentType.Package);

			costPriceRatioValidator.Should().BeOfType<PackageCostPriceRatioValidator>();
		}
	}
}