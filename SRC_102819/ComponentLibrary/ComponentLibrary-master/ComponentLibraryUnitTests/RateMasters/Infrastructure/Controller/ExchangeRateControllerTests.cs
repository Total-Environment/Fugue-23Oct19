using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Castle.Core.Internal;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller
{
    public class ExchangeRateControllerTests
    {
        [Fact]
        public void Should_ImplementApiController()
        {
            var sut = new ExchangeRateController(new Mock<IExchangeRateRepository>().Object);

            sut.Should().BeAssignableTo<ApiController>();
        }

        [Fact]
        public async void Create_Should_QueryExchangeRateRepository()
        {
            var exchangeRateRepo = ExchangeRateRepoWithCreateReturningExchangeRate();
            var sut = new ExchangeRateController(exchangeRateRepo.Object);
            var exchangeRateStub = new Mock<ExchangeRateDto>();
            exchangeRateStub.Setup(m => m.AppliedFrom).Returns(DateTime.Now.AddDays(1));
            exchangeRateStub.Setup(e => e.Domain()).Returns(new Mock<IExchangeRate>().Object);

            await sut.Create(exchangeRateStub.Object);

            exchangeRateRepo.Verify(e => e.CreateExchangeRate(It.IsAny<IExchangeRate>()), Times.Once);
        }

        [Fact]
        public async void Create_Should_ReturnCreatedIfRepoReturnObject()
        {
            var exchangeRateRepo = ExchangeRateRepoWithCreateReturningExchangeRate();
            var sut = new ExchangeRateController(exchangeRateRepo.Object);

            var mockExchangeRateDto = new Mock<ExchangeRateDto>();
            mockExchangeRateDto.Setup(m => m.AppliedFrom).Returns(DateTime.Now.AddDays(1));
            var result = await sut.Create(mockExchangeRateDto.Object);

            result.Should().BeAssignableTo<CreatedNegotiatedContentResult<ExchangeRateDto>>();
        }

        [Fact]
        public void Get_Should_QureryRepoWithFromCurrencyToCurrencyAndOn()
        {
            const string fromCurrency = "INR";
            const string toCurrency = "USD";
            var on = DateTime.Today;
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRateFromInrToUsd()
                .GetAcceptingLocationAndOn(fromCurrency, toCurrency, on);
            var sut = new ExchangeRateController(repoStub.Stub());

            sut.Get(fromCurrency, on, toCurrency);

            repoStub.VerifyExpectations();
        }

        [Fact]
        public void Get_Should_QureryRepoWithFromCurrencyToDefaultCurrencyAndOn()
        {
            const string fromCurrency = "USD";
            var on = DateTime.Today;
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRate()
                .GetAcceptingLocationAndOnAndDefaultCurrencyAsInr(fromCurrency, on);
            var sut = new ExchangeRateController(repoStub.Stub());

            sut.Get(fromCurrency, on);

            repoStub.VerifyExpectations();
        }

        [Fact]
        public async Task Get_Should_ReturnOkWithExchangeRateWhenRepoGetReturnExchangeRate()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRate();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get("USD", DateTime.Today);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<ExchangeRateDto>>();
        }

        [Fact]
        public async Task Get_Should_ReturnNotFoundWhenRepoThrowResourceNotFound()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetThrowResourceNotFoundException();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get("USD", DateTime.Today);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Get_Should_ReturnBadRequestWehnRepoThrowArgumentException()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetThrowArgumentException();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get("USD", DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public void GetLatest_Should_QureryRepoWithOn()
        {
            var on = DateTime.Today;
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRateFromInrToUsd()
                .GetAcceptingOn(on);
            var sut = new ExchangeRateController(repoStub.Stub());

            sut.Get(on);

            repoStub.VerifyExpectations();
        }

        [Fact]
        public async Task GetLatest_Should_ReturnOkWithExchangeRatesWhenRepoGetReturnExchangeRates()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRates();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get(DateTime.Today);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<List<ExchangeRateDto>>>();
        }

        [Fact]
        public async Task GetLatest_Should_ReturnNotFoundWhenRepoThrowResourceNotFound()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetThrowResourceNotFoundException();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get(DateTime.Today);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetLatest_Should_ReturnBadRequestWehnRepoThrowArgumentException()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetThrowArgumentException();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get(DateTime.Today);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public void GetHistory_Should_QureryRepoWithOn()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRateFromInrToUsd()
                .GetAccepting();
            var sut = new ExchangeRateController(repoStub.Stub());

            sut.Get("USD", null);

            repoStub.VerifyExpectations();
        }

        [Fact]
        public async Task GetHistory_Should_ReturnOkWithExchangeRatesWhenRepoGetReturnExchangeRates()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetReturnExchangeRates();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get("USD", null);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<PaginatedAndSortedListDto<ExchangeRateDto>>>();
        }

        [Fact]
        public async Task GetHistory_Should_ReturnNotFoundWhenRepoThrowResourceNotFound()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetThrowResourceNotFoundException();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get("USD", null);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetHistory_Should_ReturnBadRequestWehnRepoThrowArgumentException()
        {
            var repoStub = new ExchangeRateRepoStub()
                .GetThrowArgumentException();
            var sut = new ExchangeRateController(repoStub.Stub());

            var result = await sut.Get("USD", null);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        private Mock<IExchangeRateRepository> ExchangeRateRepoWithCreateReturningExchangeRate()
        {
            var exchangeRateRepo = ExchangeRateRepo();
            exchangeRateRepo.Setup(e => e.CreateExchangeRate(It.IsAny<IExchangeRate>()))
                .ReturnsAsync(new ExchangeRate("USD", "INR", 1m, 1m, DateTime.Today));
            return exchangeRateRepo;
        }

        private static Mock<IExchangeRateRepository> ExchangeRateRepo()
        {
            return new Mock<IExchangeRateRepository>();
        }

        private class ExchangeRateRepoStub
        {
            private readonly Mock<IExchangeRateRepository> _repo = new Mock<IExchangeRateRepository>();
            private readonly IList<Action> _expectations = new List<Action>();

            public IExchangeRateRepository Stub()
            {
                return _repo.Object;
            }

            public ExchangeRateRepoStub CreateShouldReturnSomeExchangeRate()
            {
                throw new NotImplementedException();
            }

            public ExchangeRateRepoStub GetAcceptingLocationAndOn(string fromCurrency, string toCurrency, DateTime on)
            {
                _expectations.Add(() => _repo.Verify(r => r.GetExchangeRate(fromCurrency, toCurrency, @on), Times.Once));
                return this;
            }

            public ExchangeRateRepoStub GetAcceptingLocationAndOnAndDefaultCurrencyAsInr(string fromCurrency, DateTime on)
            {
                _expectations.Add(() => _repo.Verify(r => r.GetExchangeRate(fromCurrency, "INR", @on), Times.Once));
                return this;
            }

            public ExchangeRateRepoStub GetAcceptingOn(DateTime on)
            {
                _expectations.Add(() => _repo.Verify(r => r.GetAll(@on), Times.Once));
                return this;
            }

            public ExchangeRateRepoStub GetAccepting()
            {
                _expectations.Add(() => _repo.Verify(r => r.GetHistory("USD", null, 1, "AppliedFrom", SortOrder.Descending), Times.Once));
                return this;
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e => e.Invoke());
            }

            public ExchangeRateRepoStub GetReturnExchangeRate()
            {
                _repo.Setup(r => r.GetExchangeRate(It.IsAny<string>(), It.IsAny<string>(), DateTime.Today))
                    .ReturnsAsync(new ExchangeRate("USD", "INR", 1m, 1m, DateTime.Today));
                return this;
            }

            public ExchangeRateRepoStub GetReturnExchangeRates()
            {
                _repo.Setup(r => r.GetAll(DateTime.Today))
                    .ReturnsAsync(new List<IExchangeRate> { new ExchangeRate("USD", "INR", 1m, 1m, DateTime.Today) });
                _repo.Setup(r => r.GetHistory("USD", null, 1, "AppliedFrom", SortOrder.Descending))
                    .ReturnsAsync(new PaginatedAndSortedList<IExchangeRate>(new List<IExchangeRate> { new ExchangeRate("USD", "INR", 1m, 1m, DateTime.Today) }, 1, 1, 20, "AppliedFrom", SortOrder.Ascending));
                return this;
            }

            public ExchangeRateRepoStub GetReturnExchangeRateFromInrToUsd()
            {
                _repo.Setup(r => r.GetExchangeRate(It.IsAny<string>(), It.IsAny<string>(), DateTime.Today))
                    .ReturnsAsync(new ExchangeRate("INR", "USD", 1m, 1m, DateTime.Today));
                return this;
            }

            public ExchangeRateRepoStub GetThrowResourceNotFoundException()
            {
                GetShouldThrow(new ResourceNotFoundException(""));
                return this;
            }

            private void GetShouldThrow(Exception exception)
            {
                _repo.Setup(r => r.GetExchangeRate(It.IsAny<string>(), It.IsAny<string>(), DateTime.Today))
                    .Throws(exception);

                _repo.Setup(r => r.GetAll(DateTime.Today))
                    .Throws(exception);

                _repo.Setup(r => r.GetHistory("USD", null, 1, "AppliedFrom", SortOrder.Descending))
                    .Throws(exception);
            }

            public ExchangeRateRepoStub GetThrowArgumentException()
            {
                GetShouldThrow(new ArgumentException());
                return this;
            }
        }
    }
}