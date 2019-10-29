using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
	public class PriceControllerTests
	{
		[Fact]
		public async Task Get_ShouldReturnOk_WhenPriceServiceRetunsPrice()
		{
			Mock<IPriceService> mockPriceService = new Mock<IPriceService>();
			mockPriceService.Setup(m => m.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053")).ReturnsAsync(new ComponentPrice(new Money((decimal)100.25, "INR"), "Load"));
			PriceController priceController = new PriceController(mockPriceService.Object);

			var result = await priceController.Get("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Should().BeOfType(typeof(OkNegotiatedContentResult<ComponentPrice>));
			((OkNegotiatedContentResult<ComponentPrice>)result).Content.Price.Value.ShouldBeEquivalentTo((decimal)100.25);
		}

		[Fact]
		public async Task Get_ShouldReturnBadRequest_WhenPriceServiceThrowsNotSupportedException()
		{
			Mock<IPriceService> mockPriceService = new Mock<IPriceService>();
			mockPriceService.Setup(m => m.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053"))
				.Throws(new NotSupportedException("NotSupported"));
			PriceController priceController = new PriceController(mockPriceService.Object);

			var result = await priceController.Get("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Should().BeOfType(typeof(BadRequestErrorMessageResult));
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("NotSupported");
		}

		[Fact]
		public async Task Get_ShouldReturnBadRequest_WhenPriceServiceThrowsNotImplementedException()
		{
			Mock<IPriceService> mockPriceService = new Mock<IPriceService>();
			mockPriceService.Setup(m => m.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053"))
				.Throws(new NotImplementedException("NotImplemented"));
			PriceController priceController = new PriceController(mockPriceService.Object);

			var result = await priceController.Get("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Should().BeOfType(typeof(BadRequestErrorMessageResult));
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("NotImplemented");
		}

		[Fact]
		public async Task Get_ShouldReturnBadRequest_WhenPriceServiceThrowsArgumentException()
		{
			Mock<IPriceService> mockPriceService = new Mock<IPriceService>();
			mockPriceService.Setup(m => m.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053"))
				.Throws(new ArgumentException("Invalid"));
			PriceController priceController = new PriceController(mockPriceService.Object);

			var result = await priceController.Get("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Should().BeOfType(typeof(BadRequestErrorMessageResult));
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Invalid");
		}

		[Fact]
		public async Task Get_ShouldReturnBadRequest_WhenPriceServiceThrowsResourceNotFoundException()
		{
			Mock<IPriceService> mockPriceService = new Mock<IPriceService>();
			mockPriceService.Setup(m => m.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053"))
				.Throws(new ResourceNotFoundException("resource"));
			PriceController priceController = new PriceController(mockPriceService.Object);

			var result = await priceController.Get("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Should().BeOfType(typeof(BadRequestErrorMessageResult));
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("resource not found.");
		}

		[Fact]
		public async Task Get_ShouldReturnBadRequest_WhenPriceServiceThrowsException()
		{
			Mock<IPriceService> mockPriceService = new Mock<IPriceService>();
			mockPriceService.Setup(m => m.GetPrice("ALM000001", "Hyderabad", DateTime.Today, "0053"))
				.Throws(new Exception("exception"));
			PriceController priceController = new PriceController(mockPriceService.Object);

			var result = await priceController.Get("ALM000001", "Hyderabad", DateTime.Today, "0053");

			result.Should().BeOfType(typeof(ExceptionResult));
		}
	}
}