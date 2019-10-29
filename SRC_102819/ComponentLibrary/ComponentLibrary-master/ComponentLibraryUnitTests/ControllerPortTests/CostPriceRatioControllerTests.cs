using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
	public class CostPriceRatioControllerTests
	{
		[Fact]
		public void CostPriceRatioController_ShouldTakeCPRService()
		{
			var cprService = new Mock<ICostPriceRatioService>();
			var cprController = new CostPriceRatioController(cprService.Object);
			cprController.Should().NotBeNull();
		}

		[Fact]
		public async Task Post_ShouldReturn201StatusCode_WhenGivenProperCostPricRatio()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).ReturnsAsync(costPriceRatio);
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<CreatedNegotiatedContentResult<CostPriceRatioDto>>();
		}

		[Fact]
		public async Task Post_ShouldReturn400StatusCode_WhenDuplicateResourceExceptionOccured()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).Throws(new DuplicateResourceException("Duplicate"));
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<BadRequestErrorMessageResult>();
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Duplicate");
		}

		[Fact]
		public async Task Post_ShouldReturnNotFound_WhenDResourceNotFoundExceptionOccured()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).Throws(new ResourceNotFoundException("Not Found"));
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<NotFoundResult>();
		}

		[Fact]
		public async Task Post_ShouldReturn400StatusCode_WhenInvalidOperationExceptionOccured()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).Throws(new InvalidOperationException("Invalid"));
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<BadRequestErrorMessageResult>();
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Invalid");
		}

		[Fact]
		public async Task Post_ShouldReturn400StatusCode_WhenArgumentExceptionOccured()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).Throws(new ArgumentException("Invalid argument"));
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<BadRequestErrorMessageResult>();
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Invalid argument");
		}

		[Fact]
		public async Task Post_ShouldReturn400StatusCode_WhenFormatExceptionOccured()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).Throws(new FormatException("Invalid format"));
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<BadRequestErrorMessageResult>();
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Invalid format");
		}

		[Fact]
		public async Task Post_ShouldReturn500StatusCode_WhenExceptionOccured()
		{
			var costPriceRatio =
				new CostPriceRatio(new CPRCoefficient(new List<IColumnData> { new ColumnData("Profit", "Profit", new UnitValue(10.4, "%")) }),
					DateTime.Today,
					ComponentType.Material, String.Empty, string.Empty, string.Empty, "ALM0001", String.Empty);
			var cprService = new Mock<ICostPriceRatioService>();
			cprService.Setup(c => c.Create(It.IsAny<CostPriceRatio>())).Throws(new Exception("exception"));
			var cprController = new CostPriceRatioController(cprService.Object);
			var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
			var result = await cprController.CreateCostPriceRatio(costPriceRatioDto);
			result.Should().BeOfType<ExceptionResult>();
		}

	    [Fact]
	    public async Task Get_ShouldReturnCostPriceListDto_WhenValidArgsArePassed()
	    {
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
	        var cprService = new Mock<ICostPriceRatioService>();
	        cprService.Setup(m => m.GetCostPriceRatioList(It.IsAny<DateTime>(), ComponentType.Material, "c1"))
	            .ReturnsAsync(costPriceRatioList);

	        var cprController = new CostPriceRatioController(cprService.Object);
	        var result = await cprController.GetCostPriceRatioList(DateTime.Now, "Material", "c1");
	        var parsedResult = (OkNegotiatedContentResult<CostPriceListDto>)result;
            parsedResult.Content.costPriceRatioDtos.Count.ShouldBeEquivalentTo(2);
        }

        [Fact]
        public async Task Get_ShouldReturn400StatusCode_WhenInvalidComponentTypeIsPassed()
        {
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
            var cprService = new Mock<ICostPriceRatioService>();
            cprService.Setup(m => m.GetCostPriceRatioList(It.IsAny<DateTime>(), ComponentType.Material, "c1"))
                .ReturnsAsync(costPriceRatioList);

            var cprController = new CostPriceRatioController(cprService.Object);
            var result = await cprController.GetCostPriceRatioList(DateTime.Now, "Mater", "c1");
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Invalid component type.");
        }

    }
}