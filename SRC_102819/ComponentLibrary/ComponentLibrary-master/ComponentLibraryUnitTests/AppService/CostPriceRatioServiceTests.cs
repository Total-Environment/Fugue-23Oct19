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
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class CostPriceRatioServiceTests
	{
		[Fact]
		public void Create_ShouldAcceptCostPriceRatio_AndReturnCostPriceRatio()
		{
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001", string.Empty);
			Mock<ICostPriceRatioRepository> mockCostPriceRatioRepository = new Mock<ICostPriceRatioRepository>();
			mockCostPriceRatioRepository.Setup(m => m.Create(It.IsAny<CostPriceRatio>())).ReturnsAsync(costPriceRatioToBeCreated);
			Mock<ICostPriceRatioValidatorFactory> mockCostPriceRatioValidatorFactory =
				new Mock<ICostPriceRatioValidatorFactory>();
			Mock<ICostPriceRatioValidator> mockCostPriceRatioValidator = new Mock<ICostPriceRatioValidator>();
			mockCostPriceRatioValidator.Setup(m => m.Validate(It.IsAny<CostPriceRatio>()))
				.ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
			mockCostPriceRatioValidatorFactory.Setup(m => m.GetCostPriceRatioValidator(ComponentType.Material))
			.Returns(mockCostPriceRatioValidator.Object);
			Mock<ICostPriceRatioBuilderFactory> mockCostPriceRatioBuilderFactory = new Mock<ICostPriceRatioBuilderFactory>();
			Mock<ICostPriceRatioBuilder> mockCostPriceRatioBuilder = new Mock<ICostPriceRatioBuilder>();
			mockCostPriceRatioBuilder.Setup(m => m.Build(It.IsAny<CostPriceRatio>())).ReturnsAsync(costPriceRatioToBeCreated);
			mockCostPriceRatioBuilderFactory.Setup(m => m.GetCostPriceRatioBuilder(ComponentType.Material))
				.Returns(mockCostPriceRatioBuilder.Object);
			Mock<IColumnDataValidator> mockColumnDataValidator = new Mock<IColumnDataValidator>();
			mockColumnDataValidator.Setup(
					m => m.Validate(It.IsAny<List<ISimpleColumnDefinition>>(), It.IsAny<IEnumerable<IColumnData>>()))
				.Returns(new Tuple<bool, string>(true, string.Empty));
			Mock<IColumnDataBuilder> mockColumnDataBuilder = new Mock<IColumnDataBuilder>();
			mockColumnDataBuilder.Setup(
					m => m.BuildData(It.IsAny<List<ISimpleColumnDefinition>>(), It.IsAny<IEnumerable<IColumnData>>()))
				.ReturnsAsync(new List<IColumnData>());

			var mockCostPriceRatioFilterFactory = new Mock<ICostPriceRatioFilterFactory>();
			Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository =
				new Mock<ICostPriceRatioDefinitionRepository>();
			mockCostPriceRatioDefinitionRepository.Setup(m => m.FindBy(It.IsAny<string>()))
				.ReturnsAsync(new CostPriceRatioDefinition("name", new List<ISimpleColumnDefinition>()));
			CostPriceRatioService costPriceRatioService = new CostPriceRatioService(
				mockCostPriceRatioDefinitionRepository.Object, mockCostPriceRatioRepository.Object,
				mockCostPriceRatioValidatorFactory.Object, mockColumnDataValidator.Object,
				mockColumnDataBuilder.Object, mockCostPriceRatioBuilderFactory.Object,
				mockCostPriceRatioFilterFactory.Object);

			var costPriceRatio = costPriceRatioService.Create(costPriceRatioToBeCreated);

			costPriceRatio.Should().NotBeNull();
		}

		[Fact]
		public void Create_ShouldReturnArgumentException_WhenInvalidCostPriceRatioIsPassed()
		{
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001", string.Empty);
			Mock<ICostPriceRatioRepository> mockCostPriceRatioRepository = new Mock<ICostPriceRatioRepository>();
			Mock<ICostPriceRatioValidatorFactory> mockCostPriceRatioValidatorFactory =
				new Mock<ICostPriceRatioValidatorFactory>();
			Mock<ICostPriceRatioValidator> mockCostPriceRatioValidator = new Mock<ICostPriceRatioValidator>();
			mockCostPriceRatioValidator.Setup(m => m.Validate(It.IsAny<CostPriceRatio>()))
				.ReturnsAsync(new Tuple<bool, string>(false, "Error"));
			mockCostPriceRatioValidatorFactory.Setup(m => m.GetCostPriceRatioValidator(ComponentType.Material))
			.Returns(mockCostPriceRatioValidator.Object);
			Mock<ICostPriceRatioBuilderFactory> mockCostPriceRatioBuilderFactory = new Mock<ICostPriceRatioBuilderFactory>();
			Mock<IColumnDataValidator> mockColumnDataValidator = new Mock<IColumnDataValidator>();
			Mock<IColumnDataBuilder> mockColumnDataBuilder = new Mock<IColumnDataBuilder>();
			Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository =
				new Mock<ICostPriceRatioDefinitionRepository>();

			var mockCostPriceBuilderFactory = new Mock<ICostPriceRatioBuilderFactory>();
			var mockCostPriceFilterFactory = new Mock<ICostPriceRatioFilterFactory>();
			CostPriceRatioService costPriceRatioService = new CostPriceRatioService(
				mockCostPriceRatioDefinitionRepository.Object, mockCostPriceRatioRepository.Object,
				mockCostPriceRatioValidatorFactory.Object, mockColumnDataValidator.Object,
				mockColumnDataBuilder.Object, mockCostPriceBuilderFactory.Object,
				mockCostPriceFilterFactory.Object);

			Func<Task> func = async () => await costPriceRatioService.Create(costPriceRatioToBeCreated);
			func.ShouldThrow<ArgumentException>().WithMessage("Invalid cost price ratio data. Error");
		}

		[Fact]
		public void Create_ShouldReturnArgumentException_WhenInvalidCostPriceRatioCoefficientsArePassed()
		{
			CostPriceRatio costPriceRatioToBeCreated = new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
				DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001", string.Empty);
			Mock<ICostPriceRatioRepository> mockCostPriceRatioRepository = new Mock<ICostPriceRatioRepository>();
			Mock<ICostPriceRatioValidatorFactory> mockCostPriceRatioValidatorFactory =
				new Mock<ICostPriceRatioValidatorFactory>();
			Mock<ICostPriceRatioValidator> mockCostPriceRatioValidator = new Mock<ICostPriceRatioValidator>();
			mockCostPriceRatioValidator.Setup(m => m.Validate(It.IsAny<CostPriceRatio>()))
				.ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
			mockCostPriceRatioValidatorFactory.Setup(m => m.GetCostPriceRatioValidator(ComponentType.Material))
			.Returns(mockCostPriceRatioValidator.Object);
			Mock<ICostPriceRatioBuilderFactory> mockCostPriceRatioBuilderFactory = new Mock<ICostPriceRatioBuilderFactory>();
			Mock<IColumnDataValidator> mockColumnDataValidator = new Mock<IColumnDataValidator>();
			mockColumnDataValidator.Setup(
					m => m.Validate(It.IsAny<List<ISimpleColumnDefinition>>(), It.IsAny<IEnumerable<IColumnData>>()))
				.Returns(new Tuple<bool, string>(false, "Error"));
			Mock<IColumnDataBuilder> mockColumnDataBuilder = new Mock<IColumnDataBuilder>();
			Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository =
				new Mock<ICostPriceRatioDefinitionRepository>();
			mockCostPriceRatioDefinitionRepository.Setup(m => m.FindBy(It.IsAny<string>()))
				.ReturnsAsync(new CostPriceRatioDefinition("name", new List<ISimpleColumnDefinition>()));

            var mockCostPriceBuilderfactory = new Mock<ICostPriceRatioBuilderFactory>();
		    var mockCostPriceFilterfactory = new Mock<ICostPriceRatioFilterFactory>();

            CostPriceRatioService costPriceRatioService = new CostPriceRatioService(
				mockCostPriceRatioDefinitionRepository.Object, mockCostPriceRatioRepository.Object,
				mockCostPriceRatioValidatorFactory.Object, mockColumnDataValidator.Object,
				mockColumnDataBuilder.Object, mockCostPriceBuilderfactory.Object,
				mockCostPriceFilterfactory.Object);

			Func<Task> func = async () => await costPriceRatioService.Create(costPriceRatioToBeCreated);
			func.ShouldThrow<ArgumentException>().WithMessage("Invalid cost price ratio coefficients. Error");
		}

		[Fact]
		public async Task GetCostPriceRatio_ShouldReturnCPR()
		{
			var costPriceRatioDefinition = new CostPriceRatioDefinition("CostPriceRatioDefinition", new List<ISimpleColumnDefinition>());
			Mock<ICostPriceRatioRepository> mockCostPriceRatioRepository = new Mock<ICostPriceRatioRepository>();
			mockCostPriceRatioRepository.Setup(
					m =>
						m.GetCostPriceRato(DateTime.Today, ComponentType.Material, "Primary", "Aluminium and Copper", "Aluminium",
							"ALM000001", "0053", It.IsAny<CostPriceRatioDefinition>()))
				.ReturnsAsync(
					new CostPriceRatio(
						new CPRCoefficient(new List<IColumnData>
						{
							new ColumnData("coefficient1", "coefficient1", new UnitValue(10.1, "%")),
							new ColumnData("coefficient2", "coefficient2", new UnitValue(20.2, "%")),
							new ColumnData("coefficient3", "coefficient3", null),
							new ColumnData("coefficient4", "coefficient4", null)
						}), DateTime.Today, ComponentType.Material, "Primary", "Aluminium and Copper", "Aluminium", "ALM000001", "0053"));
			Mock<ICostPriceRatioValidatorFactory> mockCostPriceRatioValidatorFactory =
				new Mock<ICostPriceRatioValidatorFactory>();
			Mock<IColumnDataValidator> mockColumnDataValidator = new Mock<IColumnDataValidator>();
			Mock<IColumnDataBuilder> mockColumnDataBuilder = new Mock<IColumnDataBuilder>();
			Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository =
				new Mock<ICostPriceRatioDefinitionRepository>();

			mockCostPriceRatioDefinitionRepository.Setup(m => m.FindBy("CostPriceRatioDefinition"))
				.ReturnsAsync(costPriceRatioDefinition);
			Mock<ICostPriceRatioBuilderFactory> mockCostPriceRatioBuilderFactory = new Mock<ICostPriceRatioBuilderFactory>();
			Mock<ICostPriceRatioFilterFactory> mockCostPriceRatioFilterFactory = new Mock<ICostPriceRatioFilterFactory>();
			CostPriceRatioService costPriceRatioService = new CostPriceRatioService(
				mockCostPriceRatioDefinitionRepository.Object, mockCostPriceRatioRepository.Object,
				mockCostPriceRatioValidatorFactory.Object, mockColumnDataValidator.Object,
				mockColumnDataBuilder.Object, mockCostPriceRatioBuilderFactory.Object, mockCostPriceRatioFilterFactory.Object);

			var result = await costPriceRatioService.GetCostPriceRatio(DateTime.Today, ComponentType.Material, "Primary",
				"Aluminium and Copper", "Aluminium", "ALM000001", "0053");

			result.CprCoefficient.CPR.ShouldBeEquivalentTo(1.3);
		}

	    [Fact]
	    public void GetCostPriceRatioList_ShouldReturnCostPriceList_WhenComponentTypeIsPassed()
	    {
	        Mock<ICostPriceRatioRepository> mockCostPriceRatioRepository = new Mock<ICostPriceRatioRepository>();
	        Mock<ICostPriceRatioValidatorFactory> mockCostPriceRatioValidatorFactory =
	            new Mock<ICostPriceRatioValidatorFactory>();
	        Mock<ICostPriceRatioValidator> mockCostPriceRatioValidator = new Mock<ICostPriceRatioValidator>();
	        mockCostPriceRatioValidator.Setup(m => m.Validate(It.IsAny<CostPriceRatio>()))
	            .ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
	        mockCostPriceRatioValidatorFactory.Setup(m => m.GetCostPriceRatioValidator(ComponentType.Material))
	            .Returns(mockCostPriceRatioValidator.Object);
	        Mock<ICostPriceRatioBuilderFactory> mockCostPriceRatioBuilderFactory = new Mock<ICostPriceRatioBuilderFactory>();
	        Mock<IColumnDataValidator> mockColumnDataValidator = new Mock<IColumnDataValidator>();
	        mockColumnDataValidator.Setup(
	                m => m.Validate(It.IsAny<List<ISimpleColumnDefinition>>(), It.IsAny<IEnumerable<IColumnData>>()))
	            .Returns(new Tuple<bool, string>(false, "Error"));
	        Mock<IColumnDataBuilder> mockColumnDataBuilder = new Mock<IColumnDataBuilder>();
	        Mock<ICostPriceRatioDefinitionRepository> mockCostPriceRatioDefinitionRepository =
	            new Mock<ICostPriceRatioDefinitionRepository>();
	        mockCostPriceRatioDefinitionRepository.Setup(m => m.FindBy(It.IsAny<string>()))
	            .ReturnsAsync(new CostPriceRatioDefinition("name", new List<ISimpleColumnDefinition>()));

	        var costPriceRatioList = new CostPriceRatioList(

	            new List<CostPriceRatio>
	            {
	                new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
	                    DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001",
	                    string.Empty),
	                new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
	                    DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0002",
	                    string.Empty)

	            });

	        var mockCostPriceBuilder = new Mock<ICostPriceRatioBuilder>();

	        mockCostPriceBuilder.Setup(m => m.Build(new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
	            DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001",
	            string.Empty))).ReturnsAsync(new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
	            DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001",
	            string.Empty));

	        mockCostPriceRatioRepository.Setup(m => m.GetCostPriceRatioList(It.IsAny<DateTime>(), ComponentType.Material,
	                It.IsAny<CostPriceRatioDefinition>()))
	            .ReturnsAsync(costPriceRatioList);

	        var mockCostPriceBuilderfactory = new Mock<ICostPriceRatioBuilderFactory>();
	        mockCostPriceBuilderfactory.Setup(m => m.GetCostPriceRatioBuilder(ComponentType.Material))
	            .Returns(mockCostPriceBuilder.Object);

	        var mockCostPriceRatioFilter = new Mock<ICostPriceRatioFilter>();
	        //	        mockCostPriceRatioFilter.Setup(m => m.Filter(costPriceRatioList, null)).Returns(costPriceRatioList);
	        mockCostPriceRatioFilter.Setup(m => m.Filter(It.IsAny<CostPriceRatioList>(), null)).Returns(new CostPriceRatioList(

	            new List<CostPriceRatio>
	            {
	                new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
	                    DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0001",
	                    string.Empty),
	                new CostPriceRatio(new CPRCoefficient(new List<IColumnData>()),
	                    DateTime.Today, ComponentType.Material, string.Empty, string.Empty, string.Empty, "ALM0002",
	                    string.Empty)

	            }));
	        var mockCostPriceFilterfactory = new Mock<ICostPriceRatioFilterFactory>();
	        mockCostPriceFilterfactory.Setup(m => m.GetCostPriceRatioFilter(ComponentType.Material))
	            .Returns(mockCostPriceRatioFilter.Object);

	        CostPriceRatioService costPriceRatioService = new CostPriceRatioService(
	            mockCostPriceRatioDefinitionRepository.Object, mockCostPriceRatioRepository.Object,
	            mockCostPriceRatioValidatorFactory.Object, mockColumnDataValidator.Object,
	            mockColumnDataBuilder.Object, mockCostPriceBuilderfactory.Object,
	            mockCostPriceFilterfactory.Object);

	        var result = costPriceRatioService.GetCostPriceRatioList(DateTime.Now, ComponentType.Material, null);
	        result.Result.costPriceRatios.Count.ShouldBeEquivalentTo(2);

	    }
    }
}
	   