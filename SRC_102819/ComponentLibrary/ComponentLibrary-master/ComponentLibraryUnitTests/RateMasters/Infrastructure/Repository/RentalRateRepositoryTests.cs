using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Repository
{
	public class RentalRateRepositoryTests
	{
		#region Add Tests

		[Fact]
		public async Task Add_Should_CallAddOnMongoRepositoryWithRentalRateDao()
		{
			var appliedFrom = DateTime.Now;
			var rentalRate = new RentalRate("1", "Daily", new Money(10, "INR"), appliedFrom);
			var mongoMockRepository = EmptyMongoMockRepository();
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			await rentalRateRepository.Add(rentalRate);
			var storedDate = rentalRate.AppliedFrom.Add(rentalRate.AppliedFrom.AdditionalTimeSinceMidnightIst());
			var storedRentalRate = new RentalRate("1", "Daily", new Money(10, "INR"), storedDate);
			var somedao = new RentalRateDao(storedRentalRate);

			mongoMockRepository.Verify(m => m.InsertOneAsync(somedao, null, default(CancellationToken)), Times.Once);
		}

		[Fact]
		public async Task Add_Should_ThrowsDuplicateResourceExceptionWhenCallAddOnMongoRepositoryWithDuplicateRentalRateDao()
		{
			var rentalRate = new RentalRate("1", "Daily", new Money(10, "INR"), DateTime.Now);
			var mongoMockRepository = MongoMockRepositoryWithAnEntry(rentalRate);
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			Func<Task> act = async () => await rentalRateRepository.Add(rentalRate);

			act.ShouldThrow<DuplicateResourceException>("Rental rate already exists.");
		}

		#endregion Add Tests

		#region Get Tests

		[Fact]
		public async Task Get_should_Return_MatchingResultsWhenResultsAreAvailable()
		{
			var appliedFrom = DateTime.Now;
			var money = new Money(10, "INR");
			var rentalRate = new RentalRate("1", "Daily", money, appliedFrom);
			var mongoMockRepository = MongoMockRepositoryWithAnEntry(rentalRate);
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			var returnedRentalRate = await rentalRateRepository.Get("1", "Daily", appliedFrom.AddDays(1));

			returnedRentalRate.Value.Should().Be(money);
		}

		[Fact]
		public async Task Get_should_Return_ResourceNotFoundException_When_ResultsAreNotAvailable()
		{
			var rentalRateRepository = new RentalRateRepository(EmptyMongoMockRepository().Object);

			Func<Task> result = () => rentalRateRepository.Get("1", "Daily", DateTime.Now);

			result.ShouldThrow<ResourceNotFoundException>("Rental rate not found.");
		}

		#endregion Get Tests

		#region GetLatest Tests

		[Fact]
		public async Task GetLatest_should_Return_LatestForEachOfUnitOfMeasure()
		{
			var appliedFrom = DateTime.Now;
			var money = new Money(10, "INR");
			var rentalRateWithDaily = new RentalRate("1", "Daily", money, appliedFrom);
			var anotheRentalRatWithDialy = new RentalRate("1", "Daily", money, appliedFrom.AddDays(-1));
			var rentalRateWithHourly = new RentalRate("1", "Hourly", money, appliedFrom.AddDays(-2));
			var anotherRentalRateWithHourly = new RentalRate("1", "Hourly", money, appliedFrom.AddDays(-3));
			var mongoMockRepository =
				MongoMockRepositoryWithEntries(new List<RentalRate> { rentalRateWithDaily, anotheRentalRatWithDialy, rentalRateWithHourly, anotherRentalRateWithHourly });
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			var returnedRentalRate = await rentalRateRepository.GetLatest("1", appliedFrom.AddDays(1));

			returnedRentalRate.Count().Should().Be(2);
		}

		[Fact]
		public void GetallRates_Should_ReturnListWithTwoItems()
		{
			var money = new Money(10, "INR");
			var rentalRates = new List<RentalRate>
			{
				new RentalRate("1","monthly",money,DateTime.Today),
				new RentalRate("1","yearly",money,DateTime.Today.AddDays(2)),
				new RentalRate("1","monthly",money,DateTime.Today.AddDays(4)),
				new RentalRate("1","yearly",money,DateTime.Today.AddDays(6))
			};
			var mongoMockRepository = MongoMockRepositoryWithReturningMultipleEntries(rentalRates);
			var sut = new RentalRateRepository(mongoMockRepository.Object);
			var appliedFrom = DateTime.Today.AddDays(8);
			var result = sut.GetAll("1", appliedFrom: appliedFrom);
			result.Result.Items.Count().ShouldBeEquivalentTo(2);
			result.Result.Items.First().UnitOfMeasure.ShouldBeEquivalentTo("monthly");
			result.Result.Items.ToArray()[1].UnitOfMeasure.ShouldBeEquivalentTo("yearly");
		}

		[Fact]
		public async Task GetLatest_should_Return_LatestPastMatchingResultsWhenResultsAreAvailable()
		{
			var appliedFrom = DateTime.Now;
			var money = new Money(10, "INR");
			var rentalRate = new RentalRate("1", "Daily", money, appliedFrom);
			var anotherrentalRate = new RentalRate("2", "Daily", money, appliedFrom.AddDays(-2));
			var mongoMockRepository =
				MongoMockRepositoryWithEntries(new List<RentalRate>() { rentalRate, anotherrentalRate });
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			var returnedRentalRate = await rentalRateRepository.GetLatest("1", appliedFrom);

			returnedRentalRate.Count().Should().Be(1);
			returnedRentalRate.FirstOrDefault().Value.Should().Be(money);
		}

		[Fact]
		public async Task GetLatest_should_Return_ResourceNotFoundException_When_ResultsAreNotAvailable()
		{
			var rentalRateRepository = new RentalRateRepository(EmptyMongoMockRepository().Object);

			Func<Task> result = () => rentalRateRepository.GetLatest("1", DateTime.Now);

			result.ShouldThrow<ResourceNotFoundException>("Rental rate not found.");
		}

		#endregion GetLatest Tests

		#region GetAll Tests

		[Fact]
		public async Task GetAll_should_Return_AllMatchingResultsWhenResultsAreAvailable()
		{
			var appliedFrom = DateTime.Now;
			var money = new Money(10, "INR");
			var rentalRate = new RentalRate("1", "Daily", money, appliedFrom);
			var rentalRateWithSameId = new RentalRate("1", "Daily", money, appliedFrom.AddDays(-1));
			var anotherRentalRate = new RentalRate("2", "Daily", money, appliedFrom.AddDays(-2));
			var mongoMockRepository =
				MongoMockRepositoryWithEntries(new List<RentalRate> { rentalRate, rentalRateWithSameId, anotherRentalRate });
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			var returnedRentalRate = await rentalRateRepository.GetAll("1");

			returnedRentalRate.TotalRecords.Should().Be(3);
		}

		[Fact]
		public async Task GetAll_should_Return_ResourceNotFoundException_When_ResultsAreNotAvailable()
		{
			var mongoMockRepository = EmptyMongoMockRepository();
			var rentalRateRepository = new RentalRateRepository(mongoMockRepository.Object);

			Func<Task> result = () => rentalRateRepository.GetAll("1");

			result.ShouldThrow<ResourceNotFoundException>("Rental rate not found.");
		}

		#endregion GetAll Tests

		private static Mock<IMongoCollection<RentalRateDao>> EmptyMongoMockRepository()
		{
			var emptyMongoMockRepository = new Mock<IMongoCollection<RentalRateDao>>();
			TestHelper.MockCollectionWithExisting(emptyMongoMockRepository, null);
			return emptyMongoMockRepository;
		}

		private static Mock<IMongoCollection<RentalRateDao>> MongoMockRepositoryWithAnEntry(RentalRate rentalRate)
		{
			var mockMongoCollection = new Mock<IMongoCollection<RentalRateDao>>();
			TestHelper.MockCollectionWithExisting(mockMongoCollection, new RentalRateDao(rentalRate));
			return mockMongoCollection;
		}

		private Mock<IMongoCollection<RentalRateDao>> MongoMockRepositoryWithEntries(List<RentalRate> rentalRates)
		{
			var mongoMockRepository = new Mock<IMongoCollection<RentalRateDao>>();
			var rentalRateDaos = rentalRates.Select(r => new RentalRateDao(r)).ToList();
			var mockMongoCollection = new Mock<IMongoCollection<RentalRateDao>>();
			TestHelper.MockCollectionWithExistingList(mockMongoCollection, rentalRateDaos);
			return mockMongoCollection;
		}

		private Mock<IMongoCollection<RentalRateDao>> MongoMockRepositoryWithReturningMultipleEntries(List<RentalRate> rentalRates)
		{
			var mongoMockRepository = new Mock<IMongoCollection<RentalRateDao>>();
			var rentalRateDaos = rentalRates.Select(r => new RentalRateDao(r)).ToList();
			var mockMongoCollection = new Mock<IMongoCollection<RentalRateDao>>();
			TestHelper.MockCollectionToReturnAllListItems(mockMongoCollection, rentalRateDaos);
			return mockMongoCollection;
		}
	}
}