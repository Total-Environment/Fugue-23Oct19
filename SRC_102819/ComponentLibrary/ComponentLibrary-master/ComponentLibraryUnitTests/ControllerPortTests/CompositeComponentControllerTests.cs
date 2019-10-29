using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
	public class CompositeComponentControllerTests
	{
		[Fact]
		public async Task CloneFromService_GetsTheServiceAndDelegatesSFGCreationUsingServiceTo_SFGService()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var mockService = new Mock<IService>();
			var mockSemiFinishedGoodComposition = new Mock<ComponentComposition>();

			mockServiceRepository.Setup(m => m.Find("ServiceCode")).Returns(Task.FromResult(mockService.Object));

			var sut = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);
			await sut.CloneFromService("ServiceCode", new ComponentCompositionDto(mockSemiFinishedGoodComposition.Object));

			mockSFGService.Verify(m => m.CloneFromService(mockService.Object, It.IsAny<ComponentComposition>()), Times.Once);
		}

		[Fact]
		public async Task CloneFromService_Returns400_WhenAnyInvalidDataIsSent()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			mockServiceRepository.Setup(m => m.Find("ServiceCode")).Throws(new InvalidOperationException());
			var mockSemiFinishedGoodComposition = new Mock<ComponentComposition>();
			var sut = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);

			var createdSFG = await sut.CloneFromService("ServiceCode",
				new ComponentCompositionDto(mockSemiFinishedGoodComposition.Object));

			createdSFG.Should().BeAssignableTo(typeof(BadRequestErrorMessageResult));
		}

		[Fact]
		public async Task CloneFromService_Returns404_WhenServiceCodeisNotValid()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			mockServiceRepository.Setup(m => m.Find("ServiceCode")).Throws(new ResourceNotFoundException("Service code not found."));
			var mockSemiFinishedGoodComposition = new Mock<ComponentComposition>();
			var sut = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);

			var createdSFG = await sut.CloneFromService("ServiceCode",
				new ComponentCompositionDto(mockSemiFinishedGoodComposition.Object));

			createdSFG.Should().BeAssignableTo(typeof(NotFoundResult));
		}

		[Fact]
		public async Task CloneFromService_Returns404_WhenSfgCompositionIsNull()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sut =
				new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);
			var createdSFG = await sut.CloneFromService("ServiceCode", null);
			createdSFG.Should().BeAssignableTo(typeof(BadRequestErrorMessageResult));
			var obj = (BadRequestErrorMessageResult)createdSFG;
			obj.Message.ShouldBeEquivalentTo("ComponentComposition is required.");
		}

		[Fact]
		public async Task CreateSfg_Returns404_WhenSfgCompositionIsNull()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sut =
				new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);
			var createdSFG = await sut.CreateCompositeComponent("sfg", new CompositeComponentDto());
			createdSFG.Should().BeAssignableTo(typeof(BadRequestErrorMessageResult));
			var obj = (BadRequestErrorMessageResult)createdSFG;
			obj.Message.ShouldBeEquivalentTo("ComponentComposition is required.");
		}

		[Fact]
		public async Task CreateSFG_DelegeatesCreateResponsibiityWithRecievedData_ToSfgService()
		{
			var mockSfgService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sut = new CompositeComponentController(mockSfgService.Object,
				mockServiceRepository.Object);
			await sut.CreateCompositeComponent("sfg", new CompositeComponentDto(new CompositeComponent()
			{
				Headers = new List<IHeaderData>(),
				ComponentComposition = new ComponentComposition
				(
					new List<ComponentCoefficient>()
				)
			}));
			mockSfgService.Verify(m => m.Create("sfg", It.IsAny<CompositeComponent>()), Times.Once);
		}

		[Fact]
		public async Task UpdateRates_DelegeatesUpdateResponsibiityWithRecievedData_ToSfgService()
		{
			var mockSfgService = new Mock<ICompositeComponentService>();
			mockSfgService.Setup(m => m.Get("sfg", It.IsAny<string>()))
				.ReturnsAsync(new CompositeComponent
				{
					Headers =
						new List<IHeaderData>
						{
							new HeaderData("purchase", "purchase")
							{
								Columns =
									new List<IColumnData>
									{
										new ColumnData("last purchase rate", "last_purchase_rate", null),
										new ColumnData("weighted average purchase rate",
											"weighted_average_purchase_rate", null)
									}
							}
						},
					CompositeComponentDefinition =
						new CompositeComponentDefinition
						{
							Headers =
								new List<IHeaderDefinition>
								{
									new HeaderDefinition("purchase", "purchase",
										new List<IColumnDefinition>
										{
											new ColumnDefinition("last purchase rate", "last_purchase_rate",
												new MoneyDataType()),
											new ColumnDefinition("weighted average purchase rate",
												"weighted_average_purchase_rate",
												new MoneyDataType())
										})
								}
						}
				});
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sut = new CompositeComponentController(mockSfgService.Object,
				mockServiceRepository.Object);
			var result = await sut.UpdateRates("sfg", "sfgCode",
				new RateDto
				{
					LastPurchaseRate = new MoneyDto { Amount = 100, Currency = "INR" },
					WeightedAveragePurchaseRate = new MoneyDto { Amount = 200, Currency = "INR" }
				});
			mockSfgService.Verify(m => m.UpdateRates("sfg", It.IsAny<CompositeComponent>()), Times.Once);
			result.Should().BeOfType(typeof(OkNegotiatedContentResult<string>));
			((OkNegotiatedContentResult<string>)result).Content.Should()
				.Be("Updated rates of sfg with code: sfgCode.");
		}

		[Fact]
		public async Task GetSFG_DelegeatesGetResponsibiityWithRecievedData_ToSfgService()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var data = "SFGCode";
			var sut = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);

			await sut.GetCompositeComponent("sfg", data);

			mockSFGService.Verify(m => m.Get("sfg", data), Times.Once);
		}

		[Fact]
		public async Task GetSfg_Returns404_WhenAnyInvalidIdSent()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			mockServiceRepository.Setup(m => m.Find("ServiceCode")).Throws(new ResourceNotFoundException(""));
			var mockComponentComposition = new Mock<ComponentComposition>();
			var sut = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);

			var createdSFG = await sut.CloneFromService("ServiceCode",
				new ComponentCompositionDto(mockComponentComposition.Object));

			createdSFG.Should().BeAssignableTo(typeof(NotFoundResult));
		}

		[Fact]
		public async void GetSfgCost_ShouldComputeTheRatesFromItsCoefficients_Return200OKResult()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sfgController = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);
			var appliedOn = DateTime.Now;
			var sfgCost = new CompositeComponentCost { TotalCost = new Money(280, "INR") };
			sfgCost.ComponentCostBreakup.Add(new ComponentCost("ALM00001", new Money(280, "INR")));
			mockSFGService.Setup(m => m.GetCost("sfg", "SFG40001", "Hyderabad", appliedOn.InIst().Date)).ReturnsAsync(sfgCost);
			var result = await sfgController.GetCost("sfg", "SFG40001", "Hyderabad", appliedOn);

			result.Should().BeAssignableTo(typeof(OkNegotiatedContentResult<CompositeComponentCost>));
			((OkNegotiatedContentResult<CompositeComponentCost>)result).Content.TotalCost.Should().Be(new Money(280, "INR"));
		}

        [Fact]
	    public async void GetPackageCost_ShouldComputeTheRatesFromItsCoefficients_Return200OKResult()
	    {
	        var mockSFGService = new Mock<ICompositeComponentService>();
	        var mockServiceRepository = new Mock<IServiceRepository>();
	        var compositeComponentController = new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);
	        var appliedOn = DateTime.Now;
	        var compositeComponentCost = new CompositeComponentCost { TotalCost = new Money(280, "INR") };
	        compositeComponentCost.ComponentCostBreakup.Add(new ComponentCost("ALM00001", new Money(280, "INR")));
	        mockSFGService.Setup(m => m.GetCost("package", "PKG0001", "Hyderabad", appliedOn.InIst().Date)).ReturnsAsync(compositeComponentCost);
	        var result = await compositeComponentController.GetCost("package", "PKG0001", "Hyderabad", appliedOn);

	        result.Should().BeAssignableTo(typeof(OkNegotiatedContentResult<CompositeComponentCost>));
	        ((OkNegotiatedContentResult<CompositeComponentCost>)result).Content.TotalCost.Should().Be(new Money(280, "INR"));
	    }

        [Fact]
		public async Task UpdateSfg_Returns404_WhenSfgCompositionIsNull()
		{
			var mockSFGService = new Mock<ICompositeComponentService>();
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sut =
				new CompositeComponentController(mockSFGService.Object, mockServiceRepository.Object);
			var updatedSFG = await sut.UpdateCompositeComponent("sfg", "sfgCode", new CompositeComponentDto());
			updatedSFG.Should().BeAssignableTo(typeof(BadRequestErrorMessageResult));
			var obj = (BadRequestErrorMessageResult)updatedSFG;
			obj.Message.ShouldBeEquivalentTo("ComponentComposition is required.");
		}

		[Fact]
		public async Task UpdateSFG_DelegeatesCreateResponsibiityWithRecievedData_ToSfgService()
		{
			var semiFinishedGood = new CompositeComponent()
			{
				Headers =
					new List<IHeaderData>
					{
						new HeaderData("general", "general")
						{
							Columns = new List<IColumnData> {new ColumnData("sfg_code", "sfg_code", "sfg_code")}
						}
					},
				ComponentComposition = new ComponentComposition
				(
					new List<ComponentCoefficient>()
				),
				CompositeComponentDefinition =
					new CompositeComponentDefinition
					{
						Headers =
							new List<IHeaderDefinition>
							{
								new HeaderDefinition("general", "general",
									new List<IColumnDefinition>
									{
										new ColumnDefinition("sfg_code", "sfg_code", new AutogeneratedDataType("SFG Code"))
									})
							}
					}
			};
			var mockSfgService = new Mock<ICompositeComponentService>();
			mockSfgService.Setup(m => m.Update("sfg", It.IsAny<CompositeComponent>())).ReturnsAsync(semiFinishedGood);
			var mockServiceRepository = new Mock<IServiceRepository>();
			var sut = new CompositeComponentController(mockSfgService.Object,
				mockServiceRepository.Object);

			await sut.UpdateCompositeComponent("sfg", "sfgCode", new CompositeComponentDto(semiFinishedGood));
			mockSfgService.Verify(m => m.Update("sfg", It.IsAny<CompositeComponent>()), Times.Once);
		}

		[Fact]
		public async Task Count_ShouldReturnCorrectCountValue()
		{
			Mock<ICompositeComponentBuilder> mockSemiFinishedGoodBuilder = new Mock<ICompositeComponentBuilder>();
			Mock<ICompositeComponentRepository> mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(m => m.Count("sfg", It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(1);
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>> mockSfgDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			mockSfgDefinitionRepository.Setup(m => m.Find("sfg", It.IsAny<string>())).ReturnsAsync(new CompositeComponentDefinition());
			Mock<IClassificationDefinitionRepository> mockClassicDefinitionRepository = new Mock<IClassificationDefinitionRepository>();
			Mock<IClassificationDefinitionBuilder> mockClassificationDefinitionBuilder = new Mock<IClassificationDefinitionBuilder>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<IRentalRateRepository> mockRentalRateRepository = new Mock<IRentalRateRepository>();
			Mock<IFilterCriteriaBuilder> mockFilterCriteriaBuilder = new Mock<IFilterCriteriaBuilder>();
			mockFilterCriteriaBuilder.Setup(
					m => m.Build(It.IsAny<List<string>>(), It.IsAny<List<FilterData>>(), It.IsAny<CompositeComponentDefinition>()))
				.Returns(new Dictionary<string, Tuple<string, object>>());
		    var mockCompositeComponentSapSyncer = new Mock<ICompositeComponentSapSyncer>();
		    CompositeComponentService compositeComponentService = new CompositeComponentService(mockSemiFinishedGoodBuilder.Object,
				mockSemiFinishedGoodRepository.Object, mockSfgDefinitionRepository.Object, mockClassicDefinitionRepository.Object,
				mockClassificationDefinitionBuilder.Object, mockMaterialRateService.Object, mockServiceRateService.Object,
				mockRentalRateRepository.Object, mockFilterCriteriaBuilder.Object, mockCompositeComponentSapSyncer.Object);

			var result = await compositeComponentService.Count("sfg", new List<string>(), new List<FilterData>());

			result.ShouldBeEquivalentTo(1);
		}

		[Fact]
		public async Task Find_ShouldReturnListOfSemiFinishedGoods()
		{
			Mock<ICompositeComponentBuilder> mockSemiFinishedGoodBuilder = new Mock<ICompositeComponentBuilder>();
			Mock<ICompositeComponentRepository> mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(
					m => m.Find("sfg", new Dictionary<string, Tuple<string, object>>(), string.Empty, SortOrder.Ascending, 1, 1))
				.ReturnsAsync(new List<CompositeComponent> { new CompositeComponent() });
			Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>> mockSfgDefinitionRepository = new Mock<ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>>();
			mockSfgDefinitionRepository.Setup(m => m.Find("sfg", It.IsAny<string>())).ReturnsAsync(new CompositeComponentDefinition());
			Mock<IClassificationDefinitionRepository> mockClassicDefinitionRepository = new Mock<IClassificationDefinitionRepository>();
			Mock<IClassificationDefinitionBuilder> mockClassificationDefinitionBuilder = new Mock<IClassificationDefinitionBuilder>();
			Mock<IMaterialRateService> mockMaterialRateService = new Mock<IMaterialRateService>();
			Mock<IServiceRateService> mockServiceRateService = new Mock<IServiceRateService>();
			Mock<IRentalRateRepository> mockRentalRateRepository = new Mock<IRentalRateRepository>();
			Mock<IFilterCriteriaBuilder> mockFilterCriteriaBuilder = new Mock<IFilterCriteriaBuilder>();
		    var mockCompositeComponentSapSyncer = new Mock<ICompositeComponentSapSyncer>();
            mockFilterCriteriaBuilder.Setup(
					m => m.Build(It.IsAny<List<string>>(), It.IsAny<List<FilterData>>(), It.IsAny<CompositeComponentDefinition>()))
				.Returns(new Dictionary<string, Tuple<string, object>>());
			CompositeComponentService compositeComponentService = new CompositeComponentService(mockSemiFinishedGoodBuilder.Object,
				mockSemiFinishedGoodRepository.Object, mockSfgDefinitionRepository.Object, mockClassicDefinitionRepository.Object,
				mockClassificationDefinitionBuilder.Object, mockMaterialRateService.Object, mockServiceRateService.Object,
				mockRentalRateRepository.Object, mockFilterCriteriaBuilder.Object, mockCompositeComponentSapSyncer.Object);

			var result = await compositeComponentService.Find("sfg", new List<string>(), new List<FilterData>(), string.Empty, SortOrder.Ascending, 1, 1);

			result.Count.ShouldBeEquivalentTo(1);
		}

		[Fact]
		public async Task GetRecentSemiFinishedGoods_ShouldReturnListOfSemiFinishedGoods()
		{
			Mock<ICompositeComponentService> mockSfgService = new Mock<ICompositeComponentService>();
			mockSfgService.Setup(
					m => m.Find("sfg", It.IsAny<List<string>>(), It.IsAny<List<FilterData>>(), ComponentDao.DateCreated, SortOrder.Descending, 1, 1))
				.ReturnsAsync(new List<CompositeComponent> { new CompositeComponent { Headers = new List<IHeaderData>() } });
			mockSfgService.Setup(m => m.Count("sfg", It.IsAny<List<string>>(), It.IsAny<List<FilterData>>())).ReturnsAsync(1);
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			CompositeComponentController compositeComponentController = new CompositeComponentController(mockSfgService.Object,
				mockServiceRepository.Object);

			var result = await compositeComponentController.GetRecentCompositeComponents("sfg", pageNumber: 1, batchSize: 1);

			result.Should().BeOfType(typeof(OkNegotiatedContentResult<ListDto<CompositeComponentDto>>));
			((OkNegotiatedContentResult<ListDto<CompositeComponentDto>>)result).Content.Items.Count.ShouldBeEquivalentTo(1);
		}

		[Fact]
		public async Task Search_ShouldReturnListOfSemiFinishedGoods()
		{
			Mock<ICompositeComponentService> mockSfgService = new Mock<ICompositeComponentService>();
			mockSfgService.Setup(
					m => m.Find("sfg", It.IsAny<List<string>>(), It.IsAny<List<FilterData>>(), It.IsAny<string>(), It.IsAny<SortOrder>(), 1, 1))
				.ReturnsAsync(new List<CompositeComponent> { new CompositeComponent { Headers = new List<IHeaderData>() } });
			mockSfgService.Setup(m => m.Count("sfg", It.IsAny<List<string>>(), It.IsAny<List<FilterData>>())).ReturnsAsync(1);
			Mock<IServiceRepository> mockServiceRepository = new Mock<IServiceRepository>();
			CompositeComponentController compositeComponentController = new CompositeComponentController(mockSfgService.Object,
				mockServiceRepository.Object);

			var result =
				await compositeComponentController.Search("sfg", new CompositeComponentSearchRequest
				{
					BatchSize = 1,
					FilterDatas = new List<FilterData>(),
					IgnoreSearchQuery = false,
					PageNumber = 1,
					SearchQuery = "Search Query",
					SortColumn = string.Empty,
					SortOrder = SortOrder.Ascending
				});

			result.Should().BeOfType(typeof(OkNegotiatedContentResult<ListDto<CompositeComponentDto>>));
			((OkNegotiatedContentResult<ListDto<CompositeComponentDto>>)result).Content.Items.Count.ShouldBeEquivalentTo(1);
		}
	}
}