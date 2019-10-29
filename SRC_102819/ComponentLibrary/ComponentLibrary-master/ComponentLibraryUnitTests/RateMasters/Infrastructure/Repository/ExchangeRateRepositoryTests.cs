using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Repository
{
	public class ExchangeRateRepositoryTests
	{
		[Fact]
		public void Should_ImplementIExchangeRateRepository()
		{
			var sut = new ExchangeRateRepository(new MongoRepoStub().Stub());

			sut.Should().BeAssignableTo<IExchangeRateRepository>();
		}

		[Fact]
		public void GetExchangeRate_Should_ThrowArgumentExceptionIfFromOrToCurrencyAreNull()
		{
			var mongoRepoStub = new MongoRepoStub().Stub();
			var sut = new ExchangeRateRepository(mongoRepoStub);

			Func<Task> fromFunc = () => sut.GetExchangeRate(null, "INR", DateTime.Today);
			Func<Task> toFunc = () => sut.GetExchangeRate("USD", null, DateTime.Today);

			fromFunc.ShouldThrow<ArgumentException>();
			toFunc.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void GetHistory_Should_ReturnTwoListItems()
		{
			var exachangeRates = new List<ExchangeRateDao>
			{
				new ExchangeRateDao(new ExchangeRate("USD", "INR", 1, 1, DateTime.Today)),
				new ExchangeRateDao(new ExchangeRate("USD", "INR", 1, 1, DateTime.Today.AddDays(2))),
				new ExchangeRateDao(new ExchangeRate("INR", "USD", 1, 1,DateTime.Today.AddDays(4))),
				new ExchangeRateDao(new ExchangeRate("INR", "USD", 1, 1, DateTime.Today.AddDays(6)))
			};

			var mongoRepoStub = new MongoRepoStub().AddReturnFindAsync(exachangeRates);
			var sut = new ExchangeRateRepository(mongoRepoStub);
			var appliedFrom = DateTime.Today.AddDays(8);
			var result = sut.GetHistory(appliedFrom: appliedFrom);
			result.Result.Items.Count().ShouldBeEquivalentTo(2);
			result.Result.Items.First().FromCurrency.ShouldBeEquivalentTo("USD");
			result.Result.Items.ToArray()[1].FromCurrency.ShouldBeEquivalentTo("INR");
		}

		[Fact]
		public async void CreateExchangeRate_Should_QueryMongoWithDao()
		{
			var mongoRepoStub = new MongoRepoStub()
				.AddReturnExchangeRateDao()
				.AddAcceptingSomeDao();
			var sut = new ExchangeRateRepository(mongoRepoStub.OnlyStub());

			await sut.CreateExchangeRate(SomeExchangeRate());

			mongoRepoStub.VerifyExpectations();
		}

		private static ExchangeRate SomeExchangeRate()
		{
			return new ExchangeRate("USD", "INR", 1m, 1m, DateTime.Today);
		}

		[Fact]
		public void CreateExchangeRate_Should_ShouldThrowArgumentExceptionWhenNullIsPassed()
		{
			var mongoRepoStub = new MongoRepoStub();
			var sut = new ExchangeRateRepository(mongoRepoStub.Stub());

			Func<Task<IExchangeRate>> func = () => sut.CreateExchangeRate(null);

			func.ShouldThrow<ArgumentException>();
		}

		private static ExchangeRateDao ExchangeRateDtoStub(DateTime appliedOn)
		{
			var exchangeRate = new ExchangeRate("USD", "INR", 1m, 1m, appliedOn);
			return new ExchangeRateDao(exchangeRate);
		}

		private class MongoRepoStub
		{
			private readonly Mock<IMongoCollection<ExchangeRateDao>> _mongoRepo = new Mock<IMongoCollection<ExchangeRateDao>>();

			private readonly IList<Action> _expectations = new List<Action>();
			private readonly IList<ExchangeRateDao> _exchangeRates = new List<ExchangeRateDao>();

			public IMongoCollection<ExchangeRateDao> Stub()
			{
				TestHelper.MockCollectionWithExistingList(_mongoRepo, _exchangeRates);
				return _mongoRepo.Object;
			}

			public void VerifyExpectations()
			{
				_expectations.ForEach(e => e.Invoke());
			}

			public MongoRepoStub FindAllByReturnsNull()
			{
				// We don't have to do anything. It just returns null.
				return this;
			}

			public MongoRepoStub FindAllByReturns(ExchangeRateDao exchangeRate)
			{
				_exchangeRates.Add(exchangeRate);
				return this;
			}

			public MongoRepoStub FindByAllReturnSetValues()
			{
				TestHelper.MockCollectionWithExistingList(_mongoRepo, _exchangeRates);
				return this;
			}

			public MongoRepoStub AddReturnExchangeRateDao()
			{
				var exchangeRateDao = new Mock<ExchangeRateDao>();
				exchangeRateDao.Setup(e => e.FromCurrency).Returns("USD");
				exchangeRateDao.Setup(e => e.ToCurrency).Returns("USD");
				exchangeRateDao.Setup(e => e.FluctuationCoefficient).Returns(10);
				exchangeRateDao.Setup(e => e.BaseConversionRate).Returns(1);
				_mongoRepo.Setup(m => m.InsertOneAsync(It.IsAny<ExchangeRateDao>(), null, default(CancellationToken))).Returns(Task.CompletedTask);
				TestHelper.MockCollectionWithExisting(_mongoRepo, null, exchangeRateDao.Object);
				return this;
			}

			public IMongoCollection<ExchangeRateDao> AddReturnFindAsync(
				List<ExchangeRateDao> exchangeRates)
			{
				TestHelper.MockCollectionToReturnAllListItems(_mongoRepo, exchangeRates);
				return _mongoRepo.Object;
			}

			public MongoRepoStub AddAcceptingSomeDao()
			{
				_expectations.Add(() => _mongoRepo.Verify(m => m.InsertOneAsync(It.IsAny<ExchangeRateDao>(), null, default(CancellationToken)), Times.Once));
				return this;
			}

			public IMongoCollection<ExchangeRateDao> OnlyStub()
			{
				return _mongoRepo.Object;
			}
		}
	}
}