using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http.Results;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller
{
    public class ExchangeRateControllerIntegrationTest
    {
        private ExchangeRateDto GetRequestWith(string fromCurrency, string toCurrency, string baseConversionRate,
            string fluctuationCoefficient, DateTime appliedOn)
        {
            return new ExchangeRateDto
            {
                AppliedFrom = appliedOn,
                BaseConversionRate = baseConversionRate,
                CurrencyFluctuationCoefficient = fluctuationCoefficient,
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency
            };
        }

        private ExchangeRateDao GetPersistedDao(string fromCurrency, string toCurrency, string baseConversionRate,
            string fluctuationCoefficient, DateTime appliedOn)
        {
            return new ExchangeRateDao
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                AppliedFrom = appliedOn,
                BaseConversionRate = decimal.Parse(baseConversionRate),
                FluctuationCoefficient = decimal.Parse(fluctuationCoefficient)
            };
        }

        private class MongoRepoStub
        {
            private readonly Mock<IMongoCollection<ExchangeRateDao>> _repo =
                new Mock<IMongoCollection<ExchangeRateDao>>();

            private IList<ExchangeRateDao> _exchangeRateDaos = new List<ExchangeRateDao>();

            public MongoRepoStub AddReturns(ExchangeRateDao dao)
            {
                _repo.Setup(r => r.InsertOneAsync(dao, null, default(CancellationToken)));
                TestHelper.MockCollectionWithExisting(_repo, null, dao);
                return this;
            }

            public IMongoCollection<ExchangeRateDao> Stub()
            {
                return _repo.Object;
            }
        }

        [Fact]
        public async void Create_Should_ReturnReturnCreatedOk()
        {
            var appliedOn = DateTime.Now.AddDays(1);
            const string fromCurrency = "USD";
            const string toCurrency = "INR";
            const string baseConversionRate = "10";
            const string fluctuationCoefficient = ".4";
            var request = GetRequestWith(fromCurrency, toCurrency, baseConversionRate, fluctuationCoefficient, appliedOn);
            var dao = GetPersistedDao(fromCurrency, toCurrency, baseConversionRate, fluctuationCoefficient, appliedOn);
            var mongoRepo = new MongoRepoStub().AddReturns(dao).Stub();
            var exchangeRateRepo = new ExchangeRateRepository(mongoRepo);
            var sut = new ExchangeRateController(exchangeRateRepo);

            var response = await sut.Create(request);

            response.Should().BeAssignableTo<CreatedNegotiatedContentResult<ExchangeRateDto>>();
        }
    }
}