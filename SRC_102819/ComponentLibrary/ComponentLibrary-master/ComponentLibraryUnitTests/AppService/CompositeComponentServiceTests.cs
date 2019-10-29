using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class CompositeComponentServiceTests
	{
		private readonly Mock<ICompositeComponentBuilder> _mockSemiFinishedGoodBuilder =
			new Mock<ICompositeComponentBuilder>();

		private readonly Mock<ICompositeComponentRepository> _mockSemiFinishedGoodRepository =
			new Mock<ICompositeComponentRepository>();

		private readonly Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>> _mockSfgDefinitionRepository
			= new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();

		private readonly Mock<IClassificationDefinitionBuilder> _mockClassificationDefinitionBuilder =
			new Mock<IClassificationDefinitionBuilder>();

		private readonly Mock<IClassificationDefinitionRepository> _mockClassificationDefinitionRepository =
			new Mock<IClassificationDefinitionRepository>();

		private readonly Mock<IMaterialRateService> _mockMaterialRateService = new Mock<IMaterialRateService>();
		private readonly Mock<IServiceRateService> _mockServiceRateService = new Mock<IServiceRateService>();
		private readonly Mock<IRentalRateRepository> _mockRentalRateRepository = new Mock<IRentalRateRepository>();

		private readonly Mock<IFilterCriteriaBuilder>
			mockFilterCriteriaBuilder = new Mock<IFilterCriteriaBuilder>();

	    private readonly Mock<ICompositeComponentSapSyncer> _mockCompositeComponentSapSyncer = new Mock<ICompositeComponentSapSyncer>();

	    public CompositeComponentService SystemUnderTest()
		{
			return new CompositeComponentService(_mockSemiFinishedGoodBuilder.Object,
				_mockSemiFinishedGoodRepository.Object,
				_mockSfgDefinitionRepository.Object,
				_mockClassificationDefinitionRepository.Object,
				_mockClassificationDefinitionBuilder.Object,
				_mockMaterialRateService.Object,
				_mockServiceRateService.Object,
				_mockRentalRateRepository.Object,
				mockFilterCriteriaBuilder.Object,
                _mockCompositeComponentSapSyncer.Object);
		}

		[Fact]
		public async Task CloneFromService_DelegatesCloneResponsibilityToSfgBuilder_AndSavesSFGToRepository()
		{
			var mockService = new Mock<Service>();
			var mockSemiFinishedGoodComposition = new Mock<ComponentComposition>();

			var mockSfgDefinition = new CompositeComponentDefinition();
			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSfgDefinition);

			var sfg = new CompositeComponent();
			_mockSemiFinishedGoodBuilder.Setup(
					m =>
						m.CloneFromService(mockService.Object, mockSfgDefinition,
							mockSemiFinishedGoodComposition.Object))
				.ReturnsAsync(sfg);

			_mockClassificationDefinitionRepository.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new Mock<ClassificationDefinitionDao>().Object);

			_mockClassificationDefinitionBuilder.Setup(
					m => m.BuildDao(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
				.ReturnsAsync(new Mock<ClassificationDefinitionDao>().Object);

			_mockClassificationDefinitionRepository.Setup(
					m => m.CreateClassificationDefinition(It.IsAny<ClassificationDefinitionDao>()))
				.Returns(Task.CompletedTask);

			var sut = SystemUnderTest();

			await sut.CloneFromService(mockService.Object, mockSemiFinishedGoodComposition.Object);

			_mockSemiFinishedGoodRepository.Verify(m => m.Create("sfg", sfg, mockSfgDefinition), Times.Once);
		}

		[Fact]
		public async Task Create_DelegatesCreateResponsibilityToSfgBuilder_AndSavesSFGToRepository()
		{
			var sfgData = new CompositeComponent
			{
				Headers = new List<IHeaderData>
				{
					new HeaderData("headername", "header")
					{
						Columns = new List<IColumnData>
						{
							new ColumnData("columnname", "columnkey", "columnvalue")
						}
					}
				}
			};

			var mockSFgDefinition = new CompositeComponentDefinition();

			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSFgDefinition);

			var mockSFg = new CompositeComponent();
			_mockSemiFinishedGoodBuilder.Setup(m => m.Create("sfg", mockSFgDefinition, It.IsAny<CompositeComponent>()))
				.ReturnsAsync(mockSFg);

			var sut = SystemUnderTest();
			await sut.Create("sfg", sfgData);

			_mockSemiFinishedGoodRepository.Verify(m => m.Create("sfg", mockSFg, mockSFgDefinition), Times.Once);
            _mockCompositeComponentSapSyncer.Verify(m => m.Sync(It.IsAny<CompositeComponent>(), false, "sfg"), Times.Once);
		}

		[Fact]
		public async Task Update_DelegatesUpdateResponsibilityToSfgBuilder_AndSavesSFGToRepository()
		{
			var sfgData = new CompositeComponent
			{
				Headers = new List<IHeaderData>
				{
					new HeaderData("headername", "header")
					{
						Columns = new List<IColumnData>
						{
							new ColumnData("columnname", "columnkey", "columnvalue")
						}
					}
				}
			};

			var mockSFgDefinition = new CompositeComponentDefinition();

			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSFgDefinition);

			var mockSFg = new CompositeComponent();
			_mockSemiFinishedGoodBuilder.Setup(m => m.Update("sfg", mockSFgDefinition, It.IsAny<CompositeComponent>()))
				.ReturnsAsync(mockSFg);

			var sut = SystemUnderTest();
			await sut.Update("sfg", sfgData);

			_mockSemiFinishedGoodRepository.Verify(m => m.Update("sfg", mockSFg, mockSFgDefinition), Times.Once);
		}

		[Fact]
		public async Task Get_ShouldPassDefinitionAndIdToRepository_ToFetchSfg()
		{
			var mockSfgDefinition = new CompositeComponentDefinition()
			{
				Headers = new List<IHeaderDefinition>()
				{
					new HeaderDefinition("name", "key", new List<IColumnDefinition>()
						{
							new ColumnDefinition("cname", "ckey", new StringDataType(), false, false)
						},
						null
					)
				}
			};

			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSfgDefinition);

			var sut = SystemUnderTest();
			await sut.Get("sfg", "code");

			_mockSemiFinishedGoodRepository.Verify(m => m.Find("sfg", "code", mockSfgDefinition), Times.Once);
		}

		[Fact]
		public async void GetCost_ShouldComputeCostOfMaterialsBasedOnCoefficients_ReturnTotalCost()
		{
			var mockSfgDefinition = new CompositeComponentDefinition()
			{
				Headers = new List<IHeaderDefinition>()
				{
					new HeaderDefinition("name", "key", new List<IColumnDefinition>()
						{
							new ColumnDefinition("cname", "ckey", new StringDataType(), false, false)
						},
						null
					)
				}
			};
			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSfgDefinition);

			var appliedOn = DateTime.Now;
			var sfgCode = "SFL00001";
			var componentCoefficients = new List<ComponentCoefficient>
			{
				new ComponentCoefficient("ALM0001", 20,
					new List<WastagePercentage> {new WastagePercentage("Drivel", 40)}, "cm", "Aluminium Sheet",
					ComponentType.Material),
				new ComponentCoefficient("SVC0001", 12, new List<WastagePercentage> {new WastagePercentage("", 30)}, "m",
					"Masonry & Plaster", ComponentType.Service)
			};
			var semiFinishedGood = new CompositeComponent
			{
				ComponentComposition = new ComponentComposition(componentCoefficients),
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Unit of Measure", "unit_of_measure", "Load")}
                    },
                    new HeaderData("Classification","classification")
                    {
                        Columns = new List<IColumnData> {new ColumnData("SFG Level 1","sfg_level_1","hvac")}
                    }
                }
            };
			_mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM0001", "Bangalore", appliedOn, "INR"))
				.ReturnsAsync(new Money(10, "INR"));
			_mockServiceRateService.Setup(m => m.GetAverageLandedRate("SVC0001", "Bangalore", appliedOn, "INR"))
				.ReturnsAsync(new Money(10, "INR"));
			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSfgDefinition);
			_mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", sfgCode, mockSfgDefinition))
				.ReturnsAsync(semiFinishedGood);
			var semiFinishedGoodService = SystemUnderTest();

			var result = await semiFinishedGoodService.GetCost("sfg", sfgCode, "Bangalore", appliedOn);

			result.TotalCost.Should().Be(new Money(436, "INR"));
			result.ComponentCostBreakup.Count.Should().Be(2);
			result.ComponentCostBreakup[0].Should().Be(new ComponentCost("ALM0001", new Money(280, "INR")));
			result.ComponentCostBreakup[1].Should().Be(new ComponentCost("SVC0001", new Money(156, "INR")));
		}

		[Fact]
		public void GetCost_ShouldThrowResourceNotFoundExceptionContainsAllComponentCodesWhichAreFailed_WhenRatesAreNotFoundToCorrespondingComponentTypes()
		{
			var mockSfgDefinition = new CompositeComponentDefinition()
			{
				Headers = new List<IHeaderDefinition>()
				{
					new HeaderDefinition("name", "key", new List<IColumnDefinition>()
						{
							new ColumnDefinition("cname", "ckey", new StringDataType(), false, false)
						},
						null
					)
				}
			};
			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSfgDefinition);

			var appliedOn = DateTime.Now;
			var sfgCode = "SFL00001";
			var componentCoefficients = new List<ComponentCoefficient>
			{
				new ComponentCoefficient("ALM0001", 20,
					new List<WastagePercentage> {new WastagePercentage("Drivel", 40)}, "cm", "Aluminium Sheet",
					ComponentType.Material),
				new ComponentCoefficient("SVC0001", 12, new List<WastagePercentage> {new WastagePercentage("", 30)}, "m",
					"Masonry & Plaster", ComponentType.Service)
			};
			var semiFinishedGood = new CompositeComponent
			{
				ComponentComposition = new ComponentComposition(componentCoefficients),
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Unit of Measure", "unit_of_measure", "Load")}
                    },
                    new HeaderData("Classification","classification")
                    {
                        Columns = new List<IColumnData> {new ColumnData("SFG Level 1","sfg_level_1","hvac")}
                    }
                }
			};
			_mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM0001", "Bangalore", appliedOn, "INR"))
				.Throws(new ResourceNotFoundException("Not Found"));
			_mockServiceRateService.Setup(m => m.GetAverageLandedRate("SVC0001", "Bangalore", appliedOn, "INR"))
				.ReturnsAsync(new Money(10, "INR"));
			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSfgDefinition);
			_mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", sfgCode, mockSfgDefinition))
				.ReturnsAsync(semiFinishedGood);
			var semiFinishedGoodService = SystemUnderTest();

			Func<Task> action = async () => await semiFinishedGoodService.GetCost("sfg", sfgCode, "Bangalore", appliedOn);

			action.ShouldThrow<ResourceNotFoundException>($"Rate for ALM0001 as of {appliedOn.ToShortDateString()} for Bangalore location is");
		}

		[Fact]
		public async Task UpdateRates_SavesSFGToRepository()
		{
			var sfgData = new CompositeComponent
			{
				Headers = new List<IHeaderData>
				{
					new HeaderData("headername", "header")
					{
						Columns = new List<IColumnData>
						{
							new ColumnData("columnname", "columnkey", "columnvalue")
						}
					}
				}
			};

			var mockSFgDefinition = new CompositeComponentDefinition();

			_mockSfgDefinitionRepository.Setup(m => m.Find("sfg", "Semi Finished Good")).ReturnsAsync(mockSFgDefinition);

			var sut = SystemUnderTest();
			await sut.UpdateRates("sfg", sfgData);

			_mockSemiFinishedGoodRepository.Verify(m => m.Update("sfg", sfgData, mockSFgDefinition), Times.Once);
		}

	    [Fact]
	    public async Task GetCost_ShouldComputeTotalCostOfPackagesBasedOnCoefficients_ReturnTotalCost()
	    {
	        var appliedOn = DateTime.Now;
            var sfgComponentCoefficients = new List<ComponentCoefficient>
	        {
	            new ComponentCoefficient("ALM0001", 20,
	                new List<WastagePercentage> {new WastagePercentage("Drivel", 40)}, "cm", "Aluminium Sheet",
	                ComponentType.Material),
	            new ComponentCoefficient("SVC0001", 12, new List<WastagePercentage> {new WastagePercentage("", 30)}, "m",
	                "Masonry & Plaster", ComponentType.Service),
	            new ComponentCoefficient("AST0001", 10, new List<WastagePercentage> {new WastagePercentage("", 40)}, "Daily",
	                "Crane", ComponentType.Asset),
            };
	        var semiFinishedGood = new CompositeComponent
	        {
	            ComponentComposition = new ComponentComposition(sfgComponentCoefficients),
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Unit of Measure", "unit_of_measure", "Load")}
                    },
                    new HeaderData("Classification","classification")
                    {
                        Columns = new List<IColumnData> {new ColumnData("SFG Level 1","sfg_level_1","hvac")}
                    }
                }
            };

	        var packageComponentCoefficients = new List<ComponentCoefficient>
	        {
	            new ComponentCoefficient("ALM0002", 22,
	                new List<WastagePercentage> {new WastagePercentage("Drivel", 30)}, "cm", "Aluminium Sheet",
	                ComponentType.Material),
	            new ComponentCoefficient("SVC0002", 21, new List<WastagePercentage> {new WastagePercentage("", 10)}, "m",
	                "Masonry & Plaster", ComponentType.Service),
	            new ComponentCoefficient("AST0002", 10, new List<WastagePercentage> {new WastagePercentage("", 13)}, "Daily",
	                "Crane", ComponentType.Asset),
	            new ComponentCoefficient("SFG0001", 2, new List<WastagePercentage> {new WastagePercentage("", 17)}, "Daily",
	                "sfg1", ComponentType.SFG),
            };

	        var package = new CompositeComponent
	        {
	            ComponentComposition = new ComponentComposition(packageComponentCoefficients),
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Unit of Measure", "unit_of_measure", "Load")}
                    },
                    new HeaderData("Classification","classification")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Pkg Level 1","pkg_level_1","hvac")}
                    }
                }
            };
            var rentalRateAst1 = new RentalRate("AST0001","Daily",new Money(10,"INR"),DateTime.Now);
	        _mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM0001", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockServiceRateService.Setup(m => m.GetAverageLandedRate("SVC0001", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockRentalRateRepository.Setup(m => m.Get("AST0001", "Daily", appliedOn)).ReturnsAsync(rentalRateAst1);

	        var rentalRateAst2 = new RentalRate("AST0002", "Daily", new Money(10, "INR"), DateTime.Now);
	        _mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM0002", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockServiceRateService.Setup(m => m.GetAverageLandedRate("SVC0002", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockRentalRateRepository.Setup(m => m.Get("AST0002", "Daily", appliedOn)).ReturnsAsync(rentalRateAst2);

            _mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", "SFG0001", It.IsAny<ICompositeComponentDefinition>()))
	            .ReturnsAsync(semiFinishedGood);
            _mockSemiFinishedGoodRepository.Setup(m => m.Find("package", "PKG00001", It.IsAny<ICompositeComponentDefinition>()))
	            .ReturnsAsync(package);

            var semiFinishedGoodService = SystemUnderTest();

	        var result = await semiFinishedGoodService.GetCost("package", "PKG00001", "Bangalore", appliedOn);

	        result.TotalCost.Should().Be(new Money((decimal)1977.84, "INR"));
	        result.ComponentCostBreakup.Count.Should().Be(4);
	        result.ComponentCostBreakup[0].Should().Be(new ComponentCost("ALM0002", new Money(286, "INR")));
	        result.ComponentCostBreakup[1].Should().Be(new ComponentCost("SVC0002", new Money(231, "INR")));
	        result.ComponentCostBreakup[2].Should().Be(new ComponentCost("AST0002", new Money(113, "INR")));
            result.ComponentCostBreakup[3].Should().Be(new ComponentCost("SFG0001", new Money((decimal)1347.84, "INR")));

        }

	    [Fact]
	    public async Task GetCost_ShouldComputeTotalCostOfPackagesBasedOnCoefficients_ReturnException()
	    {
	        var appliedOn = DateTime.Now;
	        var sfgComponentCoefficients = new List<ComponentCoefficient>
	        {
	            new ComponentCoefficient("ALM0001", 20,
	                new List<WastagePercentage> {new WastagePercentage("Drivel", 40)}, "cm", "Aluminium Sheet",
	                ComponentType.Material),
	            new ComponentCoefficient("SVC0001", 12, new List<WastagePercentage> {new WastagePercentage("", 30)}, "m",
	                "Masonry & Plaster", ComponentType.Service),
	            new ComponentCoefficient("AST0001", 10, new List<WastagePercentage> {new WastagePercentage("", 40)}, "Daily",
	                "Crane", ComponentType.Asset),
	        };
	        var semiFinishedGood = new CompositeComponent
	        {
	            ComponentComposition = new ComponentComposition(sfgComponentCoefficients),
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Unit of Measure", "unit_of_measure", "Load")}
                    },
                    new HeaderData("Classification","classification")
                    {
                        Columns = new List<IColumnData> {new ColumnData("SFG Level 1","sfg_level_1","hvac")}
                    }
                }
            };

	        var packageComponentCoefficients = new List<ComponentCoefficient>
	        {
	            new ComponentCoefficient("ALM0002", 22,
	                new List<WastagePercentage> {new WastagePercentage("Drivel", 30)}, "cm", "Aluminium Sheet",
	                ComponentType.Material),
	            new ComponentCoefficient("SVC0002", 21, new List<WastagePercentage> {new WastagePercentage("", 10)}, "m",
	                "Masonry & Plaster", ComponentType.Service),
	            new ComponentCoefficient("AST0002", 10, new List<WastagePercentage> {new WastagePercentage("", 13)}, "Daily",
	                "Crane", ComponentType.Asset),
	            new ComponentCoefficient("SFG0001", 2, new List<WastagePercentage> {new WastagePercentage("", 17)}, "Daily",
	                "sfg1", ComponentType.SFG),
	        };

	        var package = new CompositeComponent
	        {
	            ComponentComposition = new ComponentComposition(packageComponentCoefficients),
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Unit of Measure", "unit_of_measure", "Load")}
                    },
                    new HeaderData("Classification","classification")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Pkg Level 1","pkg_level_1","hvac")}
                    }
                }
            };
	        var rentalRateAst1 = new RentalRate("AST0001", "Daily", new Money(10, "INR"), DateTime.Now);
	        _mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM0001", "Bangalore", appliedOn, "INR"))
	            .Throws(new ResourceNotFoundException("Not Found"));
            _mockServiceRateService.Setup(m => m.GetAverageLandedRate("SVC0001", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockRentalRateRepository.Setup(m => m.Get("AST0001", "Daily", appliedOn)).ReturnsAsync(rentalRateAst1);

	        var rentalRateAst2 = new RentalRate("AST0002", "Daily", new Money(10, "INR"), DateTime.Now);
	        _mockMaterialRateService.Setup(m => m.GetAverageLandedRate("ALM0002", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockServiceRateService.Setup(m => m.GetAverageLandedRate("SVC0002", "Bangalore", appliedOn, "INR"))
	            .ReturnsAsync(new Money(10, "INR"));
	        _mockRentalRateRepository.Setup(m => m.Get("AST0002", "Daily", appliedOn)).ReturnsAsync(rentalRateAst2);

	        _mockSemiFinishedGoodRepository.Setup(m => m.Find("sfg", "SFG0001", It.IsAny<ICompositeComponentDefinition>()))
	            .ReturnsAsync(semiFinishedGood);
	        _mockSemiFinishedGoodRepository.Setup(m => m.Find("package", "PKG00001", It.IsAny<ICompositeComponentDefinition>()))
	            .ReturnsAsync(package);

	        var semiFinishedGoodService = SystemUnderTest();

	        Func<Task> action = async () =>  await semiFinishedGoodService.GetCost("package", "PKG00001", "Bangalore", appliedOn);
	        action.ShouldThrow<ResourceNotFoundException>($"Rate for ALM0001 as of {appliedOn.ToShortDateString()} for Bangalore location is");

        }

    }
}