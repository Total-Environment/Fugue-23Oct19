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
	public class CostPriceRatioBuilderFactoryTests
	{
		[Fact]
		public void GetCostPriceRatioBuilder_ShouldReturnMaterialCostPriceRatioBuilder_WhenMaterialIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			CostPriceRatioBuilderFactory costPriceRatioBuilderFactory =
				new CostPriceRatioBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRepository.Object);

			var costPriceRatioBuilder = costPriceRatioBuilderFactory.GetCostPriceRatioBuilder(ComponentType.Material);

			costPriceRatioBuilder.Should().BeOfType<MaterialCostPriceRatioBuilder>();
		}

		[Fact]
		public void GetCostPriceRatioBuilder_ShouldReturnServiceCostPriceRatioBuilder_WhenServiceIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			CostPriceRatioBuilderFactory costPriceRatioBuilderFactory =
				new CostPriceRatioBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRepository.Object);

			var costPriceRatioBuilder = costPriceRatioBuilderFactory.GetCostPriceRatioBuilder(ComponentType.Service);

			costPriceRatioBuilder.Should().BeOfType<ServiceCostPriceRatioBuilder>();
		}

		[Fact]
		public void GetCostPriceRatioBuilder_ShouldThrowNotImplementedException_WhenAssetIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			CostPriceRatioBuilderFactory costPriceRatioBuilderFactory =
				new CostPriceRatioBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRepository.Object);

			Action action = () => costPriceRatioBuilderFactory.GetCostPriceRatioBuilder(ComponentType.Asset);
			action.ShouldThrow<NotImplementedException>().WithMessage("Asset is not implemented. Try with Material.");
		}

		[Fact]
		public void GetCostPriceRatioBuilder_ShouldReturnSemiFinsihedGoodCostPriceRatioBuilder_WhenSemiFinishedGoodIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			CostPriceRatioBuilderFactory costPriceRatioBuilderFactory =
				new CostPriceRatioBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRepository.Object);

			var costPriceRatioBuilder = costPriceRatioBuilderFactory.GetCostPriceRatioBuilder(ComponentType.SFG);

			costPriceRatioBuilder.Should().BeOfType<SemiFinishedGoodCostPriceRatioBuilder>();
		}

		[Fact]
		public void GetCostPriceRatioBuilder_ShouldReturnPackageCostPriceRatioBuilder_WhenSPackageIsPassed()
		{
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRepository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			CostPriceRatioBuilderFactory costPriceRatioBuilderFactory =
				new CostPriceRatioBuilderFactory(mockMaterialRepository.Object, mockServiceRepository.Object,
					mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRepository.Object);

			var costPriceRatioBuilder = costPriceRatioBuilderFactory.GetCostPriceRatioBuilder(ComponentType.Package);

			costPriceRatioBuilder.Should().BeOfType<PackageCostPriceRatioBuilder>();
		}
	}
}