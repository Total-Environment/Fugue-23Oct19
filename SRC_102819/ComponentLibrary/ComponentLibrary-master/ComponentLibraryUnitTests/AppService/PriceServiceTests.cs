using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class PriceServiceTests
	{
		[Fact]
		public async Task GetPrice_ShouldReturnPrice_WhenMaterialCodeIsPassed()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM000001", "Hyderabad", DateTime.Today, "INR"))
				.ReturnsAsync(new Money((decimal)100.25, "INR"));
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			mockCostPriceRateService.Setup(
					m =>
						m.GetCostPriceRatio(DateTime.Today, ComponentType.Material, "Primary", "Aluminium and Copper", "Aluminium",
							"ALM000001", "0053"))
				.ReturnsAsync(
					new CostPriceRatio(
						new CPRCoefficient(new List<IColumnData>
						{
							new ColumnData("coefficient1", "coefficient1", new UnitValue(23, "%"))
						}), DateTime.Today,
						ComponentType.Material, "Primary", "Aluminium and Copper", "Aluminium", "ALM000001", "0053"));
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Find("ALM000001"))
				.ReturnsAsync(new Material
				{
					Headers =
						new List<IHeaderData>
						{
							new HeaderData("Classification", "classification")
							{
								Columns = new List<IColumnData>
								{
									new ColumnData("Material Level 1", "Material Level 1", "Primary"),
									new ColumnData("Material Level 2", "Material Level 2", "Aluminium and Copper"),
									new ColumnData("Material Level 3", "Material Level 3", "Aluminium")
								}
							}
						}
				});
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("ALM"))
				.ReturnsAsync(new CodePrefixTypeMapping("ALM", ComponentType.Material));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			var result = await priceService.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Price.Value.ShouldBeEquivalentTo((decimal)123.31);
		}

		[Fact]
		public async Task GetPrice_ShouldReturnPrice_WhenServiceCodeIsPassed()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			mockServiceRateService.Setup(m => m.GetAverageLandedRate("FDP0001", "Hyderabad", DateTime.Today, "INR"))
				.ReturnsAsync(new Money((decimal)100.25, "INR"));
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			mockCostPriceRateService.Setup(
					m =>
						m.GetCostPriceRatio(DateTime.Today, ComponentType.Service, "FLOORING | DADO | PAVIOUR", "Flooring", null, "FDP0001",
							"0053"))
				.ReturnsAsync(
					new CostPriceRatio(
						new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(23, "%")) }),
						DateTime.Today, ComponentType.Service, "FLOORING | DADO | PAVIOUR", "Flooring", null, "FDP0001", "0053"));
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();

			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			mockServiceRepository.Setup(m => m.Find("FDP0001"))
				.ReturnsAsync(new Service
				{
					Headers =
						new List<IHeaderData>
						{
							new HeaderData("Classification", "classification")
							{
								Columns = new List<IColumnData>
								{
									new ColumnData("Service Level 1", "Service Level 1", "FLOORING | DADO | PAVIOUR"),
									new ColumnData("Service Level 2", "Service Level 2", "Flooring")
								}
							},
                            new HeaderData("General", "general")
                            {
                                Columns = new List<IColumnData>
                                {
                                    new ColumnData("Unit of Measure", "unit_of_measure", "m")
                                }
                            }
                        }
				});
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("FDP"))
				.ReturnsAsync(new CodePrefixTypeMapping("FDP", ComponentType.Service));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			var result = await priceService.GetPrice("FDP0001", "Hyderabad", DateTime.Today, "0053");

			result.Price.Value.ShouldBeEquivalentTo((decimal)123.31);
            result.UnitOfMeasure.ShouldBeEquivalentTo("m");
		}

		[Fact]
		public void GetPrice_ShouldThrowNotSupportedException_WhenAssetCodeIsPassed()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("ALM"))
				.ReturnsAsync(new CodePrefixTypeMapping("ALM", ComponentType.Asset));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			Func<Task> func = async () => await priceService.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053");

			func.ShouldThrow<NotSupportedException>().WithMessage("Asset is not supported. Try with Material.");
		}

		[Fact]
		public async Task GetPrice_ShouldReturnPrice_WhenSemiFinishedGoodCodeIsPassed()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			mockCompositeComponentService.Setup(m => m.GetCost("sfg", "FLR000001", "Hyderabad", DateTime.Today))
				.ReturnsAsync(new CompositeComponentCost { TotalCost = new Money((decimal)100.25, "INR") });
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			mockCostPriceRateService.Setup(
				m =>
					m.GetCostPriceRatio(DateTime.Today, ComponentType.SFG, "FLOORING | DADO | PAVIOUR", "Flooring", null,
						"FLR000001", "0053")).ReturnsAsync(new CostPriceRatio(
						new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(23, "%")) }),
						DateTime.Today, ComponentType.SFG, "FLOORING | DADO | PAVIOUR", "Flooring", null, "FLR000001", "0053"));
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			mockCompositeComponentRepository.Setup(m => m.Find("sfg", "FLR000001", It.IsAny<CompositeComponentDefinition>()))
				.ReturnsAsync(new CompositeComponent
				{
					Headers = new List<IHeaderData>
						{
							new HeaderData("Classification", "classification")
							{
								Columns = new List<IColumnData>
								{
									new ColumnData("SFG Level 1", "SFG Level 1", "FLOORING | DADO | PAVIOUR"),
									new ColumnData("SFG Level 2", "SFG Level 2", "Flooring")
								}
							}
						}
				});
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("FLR"))
				.ReturnsAsync(new CodePrefixTypeMapping("FLR", ComponentType.SFG));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			var result = await priceService.GetPrice("FLR000001", "Hyderabad", DateTime.Today, "0053");

			result.Price.Value.ShouldBeEquivalentTo((decimal)123.31);
		}

		[Fact]
		public async Task GetPrice_ShouldReturnPrice_WhenPackageCodeIsPassed()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			mockCompositeComponentService.Setup(m => m.GetCost("package", "PHV0001", "Hyderabad", DateTime.Today))
				.ReturnsAsync(new CompositeComponentCost { TotalCost = new Money((decimal)100.25, "INR") });
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			mockCostPriceRateService.Setup(
				m =>
					m.GetCostPriceRatio(DateTime.Today, ComponentType.Package, "HVAC", "VRV", null,
						"PHV0001", "0053")).ReturnsAsync(new CostPriceRatio(
						new CPRCoefficient(new List<IColumnData> { new ColumnData("coefficient1", "coefficient1", new UnitValue(23, "%")) }),
						DateTime.Today, ComponentType.Package, "HVAC", "VRV", null, "PHV0001", "0053"));
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			mockCompositeComponentRepository.Setup(m => m.Find("package", "PHV0001", It.IsAny<CompositeComponentDefinition>()))
				.ReturnsAsync(new CompositeComponent
				{
					Headers = new List<IHeaderData>
						{
							new HeaderData("Classification", "classification")
							{
								Columns = new List<IColumnData>
								{
									new ColumnData("Package Level 1", "Package Level 1", "HVAC"),
									new ColumnData("Package Level 2", "Package Level 2", "VRV")
								}
							}
						}
				});
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("PHV"))
				.ReturnsAsync(new CodePrefixTypeMapping("PHV", ComponentType.Package));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			var result = await priceService.GetPrice("PHV0001", "Hyderabad", DateTime.Today, "0053");

			result.Price.Value.ShouldBeEquivalentTo((decimal)123.31);
		}

		[Fact]
		public void GetPrice_ShouldThrowArgumentException_WhenInvalidLocationIsPassed()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad1")).ReturnsAsync(false);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("ALM"))
				.ReturnsAsync(new CodePrefixTypeMapping("ALM", ComponentType.Material));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			Func<Task> func = async () => await priceService.GetPrice("ALM000001", "Hyderabad1", DateTime.Today, "0053");

			func.ShouldThrow<ArgumentException>().WithMessage("Invalid location.");
		}

		[Fact]
		public void GetPrice_ShouldThrowArgumentException_WhenCPRIsNotSetUp()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			mockCostPriceRateService.Setup(
				m =>
					m.GetCostPriceRatio(It.IsAny<DateTime>(), It.IsAny<ComponentType>(), It.IsAny<string>(), It.IsAny<string>(),
						It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new ResourceNotFoundException("Resource"));
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Find("ALM000001"))
				.ReturnsAsync(new Material
				{
					Headers =
						new List<IHeaderData>
						{
							new HeaderData("Classification", "classification")
							{
								Columns = new List<IColumnData>
								{
									new ColumnData("Material Level 1", "Material Level 1", "Primary"),
									new ColumnData("Material Level 2", "Material Level 2", "Aluminium and Copper"),
									new ColumnData("Material Level 3", "Material Level 3", "Aluminium")
								}
							}
						}
				});
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("ALM"))
				.ReturnsAsync(new CodePrefixTypeMapping("ALM", ComponentType.Material));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			Func<Task> func = async () => await priceService.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053");
			func.ShouldThrow<ArgumentException>().WithMessage($"Price cannot be derived as CPR is not setup for the given ALM000001 as of {DateTime.Today:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'}.");
		}

		[Fact]
		public void GetPrice_ShouldThrowArgumentException_WhenCostIsNotAvailable()
		{
			Mock<IMasterDataRepository> mockMasterDataRepository = new Mock<IMasterDataRepository>();
			mockMasterDataRepository.Setup(m => m.Exists("location", "Hyderabad")).ReturnsAsync(true);
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM000001", "Hyderabad", DateTime.Today, "INR"))
				.Throws(new ResourceNotFoundException("Resource"));
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<ICompositeComponentService> mockCompositeComponentService = new Mock<ICompositeComponentService>();
			Mock<ICostPriceRatioService> mockCostPriceRateService = new Mock<ICostPriceRatioService>();
			mockCostPriceRateService.Setup(
					m =>
						m.GetCostPriceRatio(DateTime.Today, ComponentType.Material, "Primary", "Aluminium and Copper", "Aluminium",
							"ALM000001", "0053"))
				.ReturnsAsync(
					new CostPriceRatio(
						new CPRCoefficient(new List<IColumnData>
						{
							new ColumnData("coefficient1", "coefficient1", new UnitValue(23, "%"))
						}), DateTime.Today,
						ComponentType.Material, "Primary", "Aluminium and Copper", "Aluminium", "ALM000001", "0053"));
			Mock<IMaterialRepository> mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Find("ALM000001"))
				.ReturnsAsync(new Material
				{
					Headers =
						new List<IHeaderData>
						{
							new HeaderData("Classification", "classification")
							{
								Columns = new List<IColumnData>
								{
									new ColumnData("Material Level 1", "Material Level 1", "Primary"),
									new ColumnData("Material Level 2", "Material Level 2", "Aluminium and Copper"),
									new ColumnData("Material Level 3", "Material Level 3", "Aluminium")
								}
							}
						}
				});
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			Mock<ICompositeComponentRepository> mockCompositeComponentRepository = new Mock<ICompositeComponentRepository>();
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>
				mockCompositeComponentDefinitionRespository =
					new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			Mock<ICodePrefixTypeMappingRepository> mockCodePrefixTypeMappingRepository =
				new Mock<ICodePrefixTypeMappingRepository>();
			mockCodePrefixTypeMappingRepository.Setup(m => m.Get("ALM"))
				.ReturnsAsync(new CodePrefixTypeMapping("ALM", ComponentType.Material));
			PriceService priceService = new PriceService(mockMasterDataRepository.Object, mockProjectRepository.Object,
				mockMaterialRateService.Object, mockServiceRateService.Object, mockCompositeComponentService.Object,
				mockCostPriceRateService.Object, mockMaterialRepository.Object, mockServiceRepository.Object,
				mockCompositeComponentRepository.Object, mockCompositeComponentDefinitionRespository.Object,
				mockCodePrefixTypeMappingRepository.Object);

			Func<Task> func = async () => await priceService.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053");
			func.ShouldThrow<ArgumentException>().WithMessage($"Price cannot be derived as Cost for the ALM000001 is not available.");
		}
	}
}