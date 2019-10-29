using FluentAssertions;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Repository
{
	public class ServiceRateRepositoryTests
	{
		private readonly Mock<IBank> _mockBank = new Mock<IBank>();

		private readonly Mock<IMongoCollection<ServiceDao>> _mockMongoCollection =
			new Mock<IMongoCollection<ServiceDao>>();

		[Fact]
		public void Should_ImplementIServiceRateRepository()
		{
			SystemUnderTest().Should().BeAssignableTo<IServiceRateRepository>();
		}

		[Fact]
		public async void AddRates_ShouldGetTheCorrespondingService_ToAppendRatesToIt()
		{
			var serviceRate = GetServiceRate(DateTime.UtcNow);
			var expectedRateDao = new ServiceRateDao(serviceRate);
			var serviceDao = new ServiceDao
			{
				Columns = new Dictionary<string, object>
				{
					{"service_code", "ALM00001"}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);

			await SystemUnderTest().AddRate(serviceRate);
			((List<ServiceRateDao>)serviceDao.Columns["rates"]).First().Should().Be(expectedRateDao);
		}

		[Fact]
		public void AddRates_ShouldThrowResourceNotFoundException_WhenServiceDoesNotExist()
		{
			var serviceRate = GetServiceRate(DateTime.UtcNow);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task<IServiceRate>> result = async () => await SystemUnderTest().AddRate(serviceRate);
			result.ShouldThrow<ResourceNotFoundException>().WithMessage($"Service of {serviceRate.Id} not found.");
		}

		[Fact]
		public async void AddRates_ShouldUpdateExistingServiceWithRates_ReturnUpdatedServiceRate()
		{
			var serviceRate = GetServiceRate(DateTime.UtcNow);
			var serviceDao = new ServiceDao
			{
				Columns = new Dictionary<string, object>
				{
					{"service_code", "ALM00001"}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			await SystemUnderTest().AddRate(serviceRate);

			_mockMongoCollection.Verify(
				m =>
					m.ReplaceOneAsync(
						It.Is(
							(FilterDefinition<ServiceDao> f) =>
								CheckFilterEquality(f, serviceDao.Columns[ServiceDao.ServiceCode].ToString())),
						serviceDao, null, default(CancellationToken)));
		}

		[Fact]
		public async void GetRate_ForAGivenAppliedDate_ShouldReturnTheLatestDate()
		{
			var appliedOn = DateTime.UtcNow;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5)));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(-5)));
			var serviceDao = new ServiceDao
			{
				Columns = new Dictionary<string, object>
				{
					{"service_code", "ALM00001"},
					{"rates", new List<ServiceRateDao> {serviceRate1, serviceRate2}}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);

			var serviceRate = await SystemUnderTest().GetRate("ALM00001", "Bangalore", appliedOn, "Import");

			serviceRate.AppliedOn.Should().Be(serviceRate2.AppliedOn);
			serviceRate.Location.Should().Be(serviceRate2.Location);
			serviceRate.Id.Should().Be(serviceRate2.ServiceId);
			serviceRate.TypeOfPurchase.Should().Be(serviceRate2.TypeOfPurchase);
		}

		[Fact]
		public async void GetRate_ForValidFilters_ShouldReturnCorrespondingServiceRates()
		{
			var appliedOn = DateTime.UtcNow;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5)));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn));
			var serviceDao = new ServiceDao
			{
				Columns = new Dictionary<string, object>
				{
					{"service_code", "ALM00001"},
					{"rates", new List<ServiceRateDao> {serviceRate1, serviceRate2}}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);

			var serviceRate = await SystemUnderTest().GetRate("ALM00001", "Bangalore", appliedOn, "Import");

			serviceRate.AppliedOn.Should().Be(serviceRate2.AppliedOn);
			serviceRate.Location.Should().Be(serviceRate2.Location);
			serviceRate.Id.Should().Be(serviceRate2.ServiceId);
			serviceRate.TypeOfPurchase.Should().Be(serviceRate2.TypeOfPurchase);
		}

		[Fact]
		public void GetRate_ShouldThrowResourceNotFoundException_WhenServiceDoesNotExist()
		{
			var serviceRate = GetServiceRate(DateTime.UtcNow);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task<IServiceRate>> result =
				async () =>
					await
						SystemUnderTest()
							.GetRate(serviceRate.Id, serviceRate.Location, serviceRate.AppliedOn,
								serviceRate.TypeOfPurchase);
			result.ShouldThrow<ResourceNotFoundException>().WithMessage($"Service of {serviceRate.Id} not found.");
		}

		[Fact]
		public void GetRate_WhenLocationisPassedAsNullOrWhitespace_ShouldThrowArgumentException()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRate("serviceId", null, It.IsAny<DateTime>(), "typeOfPurchase");

			action.ShouldThrow<ArgumentException>().WithMessage("Location is a mandatory field.");
		}

		[Fact]
		public void GetRate_WhenServiceCodeIsPassedAsNull_ThrowArgumentExceptionm()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRate(null, "location", It.IsAny<DateTime>(), "typeOfPurchase");

			action.ShouldThrow<ArgumentException>().WithMessage("Service Code is a mandatory field.");
		}

		[Fact]
		public void GetRate_WhenTypeOfPurchaseIsPassedAsNull_ThrowArgumentExceptionm()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRate("serviceId", "location", It.IsAny<DateTime>(), null);

			action.ShouldThrow<ArgumentException>().WithMessage("Type of Purchase is a mandatory field.");
		}

		[Fact]
		public void GetRateHistory_ForAGivenServiceWhichHasNoRates_ThrowResourceNotFoundException()
		{
			var serviceDao = new ServiceDao
			{
				Columns = new Dictionary<string, object>
				{
					{"service_code", "ALM00001"}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			Func<Task> action = () => SystemUnderTest().GetRateHistory("ALM00001");

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Service Rate of service ALM00001 not found.");
		}

		[Fact]
		public async void GetRateHistory_ForAGivenServiceWhichHasRates_ReturnPaginatedListOfRates()
		{
			var appliedOn = DateTime.Now;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5), "Hyderabad"));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(-2)));
			var serviceRate3 = new ServiceRateDao(GetServiceRate(appliedOn, "Bangalore", "Inter-State"));
			var serviceRate4 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(2), "Bangalore", "Inter-State"));
			var serviceRatesList = new List<ServiceRateDao>
			{
				serviceRate1,
				serviceRate2,
				serviceRate3,
				serviceRate4
			};
			var serviceDao = GetServiceDaoWithRates(serviceRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			var result = await SystemUnderTest().GetRateHistory("ALM00001");

			result.Items.Count().Should().Be(3);
			result.Items.ToList()[0].AppliedOn.Should().Be(serviceRate4.AppliedOn);
			result.Items.ToList()[1].AppliedOn.Should().Be(serviceRate1.AppliedOn);
			result.Items.ToList()[2].AppliedOn.Should().Be(serviceRate3.AppliedOn);
		}

		[Fact]
		public async void GetRateHistory_ForAGivenServiceWhichHasRates_ReturnPaginatedListOfRatesSortedByTypeOfPurchase()
		{
			var appliedOn = DateTime.Now;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5), "Hyderabad"));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(-2)));
			var serviceRate3 = new ServiceRateDao(GetServiceRate(appliedOn, typeOfPurchase: "Anter-State"));
			var serviceRate4 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(2), typeOfPurchase: "Inter-State"));
			var serviceRatesList = new List<ServiceRateDao> { serviceRate1, serviceRate2, serviceRate3, serviceRate4 };
			var serviceDao = GetServiceDaoWithRates(serviceRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			var result = await SystemUnderTest().GetRateHistory("ALM00001", sortColumn: "TypeOfPurchase");

			result.Items.Count().Should().Be(3);
			result.Items.ToList()[0].TypeOfPurchase.Should().Be(serviceRate4.TypeOfPurchase);
			result.Items.ToList()[1].TypeOfPurchase.Should().Be(serviceRate2.TypeOfPurchase);
			result.Items.ToList()[2].TypeOfPurchase.Should().Be(serviceRate1.TypeOfPurchase);
		}

		[Fact]
		public async void GetRateHistory_ForAGivenServiceWhichHasRates_ReturnPaginatedListOfRatesSortedInAscendingOrder()
		{
			var appliedOn = DateTime.Now;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5), "Hyderabad"));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(-2)));
			var serviceRate3 = new ServiceRateDao(GetServiceRate(appliedOn, "Bangalore", "Inter-State"));
			var serviceRate4 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(2), "Bangalore", "Inter-State"));
			var serviceRatesList = new List<ServiceRateDao> { serviceRate1, serviceRate2, serviceRate3, serviceRate4 };
			var serviceDao = GetServiceDaoWithRates(serviceRatesList);
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			var result = await SystemUnderTest().GetRateHistory("ALM00001", sortOrder: SortOrder.Ascending);

			result.Items.Count().Should().Be(3);
			result.Items.ToList()[0].AppliedOn.Should().Be(serviceRate2.AppliedOn);
			result.Items.ToList()[1].AppliedOn.Should().Be(serviceRate3.AppliedOn);
			result.Items.ToList()[2].AppliedOn.Should().Be(serviceRate1.AppliedOn);
		}

		[Fact]
		public void GetRateHistory_WhenServiceCodeIsPassedAsNull_ThrowArgumentExceptionm()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRateHistory(null);

			action.ShouldThrow<ArgumentException>().WithMessage("Service Code is a mandatory field.");
		}

		[Fact]
		public void GetRateHistory_WhenServiceOfGivenServiceCodeDoesNotExist_ThrowResourceNotFoundException()
		{
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task> action =
				() => SystemUnderTest().GetRateHistory("ALM00001");

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Service of ALM00001 not found.");
		}

		[Fact]
		public void GetRatesHistory_ForAGivenService_ThrowResourceNotFoundExceptionIfNoRatesAreFound()
		{
			var serviceDao = GetServiceDaoWithRates(null);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			Func<Task> action =
				() => SystemUnderTest().GetRateHistory("ALM00001");

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Service Rate of service ALM00001 not found.");
		}

		[Fact]
		public async void GetRates_ForAGivenServiceAndDate_ReturnAllRatesThatAreAppliedOnBeforeGivenDate()
		{
			var appliedOn = DateTime.Now;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5), "Hyderabad"));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(-2)));
			var serviceRate3 = new ServiceRateDao(GetServiceRate(appliedOn, typeOfPurchase: "Anter-State"));
			var serviceRate4 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(2), typeOfPurchase: "Inter-State"));
			var serviceRatesList = new List<ServiceRateDao> { serviceRate1, serviceRate2, serviceRate3, serviceRate4 };
			var serviceDao = GetServiceDaoWithRates(serviceRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			var result = (await SystemUnderTest().GetRates("ALM00001", appliedOn)).ToList();

			result.Count.Should().Be(2);
			result[0].AppliedOn.Should().Be(serviceRate2.AppliedOn);
			result[1].AppliedOn.Should().Be(serviceRate3.AppliedOn);
		}

		[Fact]
		public async void GetRates_ForAGivenServiceAndLocationAndDate_ReturnAllRatesThatAreAppliedOnBeforeGivenDateForGivenLocation()
		{
			var appliedOn = DateTime.Now;
			var serviceRate1 = new ServiceRateDao(GetServiceRate(appliedOn.AddHours(5), "Hyderabad"));
			var serviceRate2 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(-2), "Hyderabad"));
			var serviceRate3 = new ServiceRateDao(GetServiceRate(appliedOn, typeOfPurchase: "Anter-State"));
			var serviceRate4 = new ServiceRateDao(GetServiceRate(appliedOn.AddDays(2), typeOfPurchase: "Inter-State"));
			var serviceRatesList = new List<ServiceRateDao> { serviceRate1, serviceRate2, serviceRate3, serviceRate4 };
			var serviceDao = GetServiceDaoWithRates(serviceRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			var result = (await SystemUnderTest().GetRates("ALM00001", "Hyderabad", appliedOn)).ToList();

			result.Count.Should().Be(1);
			result[0].AppliedOn.Should().Be(serviceRate2.AppliedOn);
		}

		[Fact]
		public void GetRates_ForAGivenServiceAndLocationAndDate_ThrowResourceNotFoundExceptionIfNoRatesAreFound()
		{
			var serviceDao = GetServiceDaoWithRates(null);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, serviceDao);
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", DateTime.UtcNow);

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Service Rate of service ALM00001 not found.");
		}

		[Fact]
		public void GetRates_ShouldThrowArgumentException_WhenServiceCodeGivenIsNull()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRates(null, DateTime.Now);

			action.ShouldThrow<ArgumentException>().WithMessage("Service Code is a mandatory field.");
		}

		[Fact]
		public void GetRates_ShouldThrowArgumentException_WhenServiceCodeGivenIsNullAndLocationIsPresent()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRates(null, "Hyderabad", DateTime.Now);

			action.ShouldThrow<ArgumentException>().WithMessage("Service Code is a mandatory field.");
		}

		[Fact]
		public void GetRates_ShouldThrowArgumentException_WhenServiceCodeIsPresentAndLocationIsNull()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", null, DateTime.Now);

			action.ShouldThrow<ArgumentException>().WithMessage("Location is a mandatory field.");
		}

		[Fact]
		public void GetRates_WhenServiceOfGivenServiceCodeDoesNotExist_ThrowResourceNotFoundException()
		{
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", DateTime.UtcNow);

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Service of ALM00001 not found.");
		}

		[Fact]
		public void GetRates_WhenServiceOfGivenServiceCodeDoesNotExistForGivenLocation_ThrowResourceNotFoundException()
		{
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", "Hyderabad", DateTime.UtcNow);

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Service of ALM00001 not found.");
		}

		private static bool CheckFilterEquality(FilterDefinition<ServiceDao> value, string serviceDaoId)
		{
			return GetPrivateFieldValue(GetPrivateFieldValue(value, "_field"), "_fieldName").Equals("service_code") &&
				   GetPrivateFieldValue(value, "_value").Equals(serviceDaoId);
		}

		private static ServiceDao GetServiceDaoWithRates(List<ServiceRateDao> serviceRatesList)
		{
			var serviceDao = new ServiceDao
			{
				Columns = new Dictionary<string, object>
				{
					{"service_code", "ALM00001"},
					{"rates", serviceRatesList}
				}
			};
			return serviceDao;
		}

		private static object GetPrivateFieldValue(object definition, string fieldName)
		{
			return
				definition.GetType()
					.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(definition);
		}

		private ServiceRate GetServiceRate(DateTime appliedOn, string location = "Bangalore",
			string typeOfPurchase = "Import")
		{
			var coefficient = new PercentageCoefficient("Location variance", 10);
			var basicRate = new Money(10, "INR", _mockBank.Object);
			var serviceRate = new ServiceRate(
                appliedOn,
                location,
                "ALM00001",
                basicRate,
                10,
                0,
                typeOfPurchase);
			return serviceRate;
		}

		private ServiceRateRepository SystemUnderTest()
		{
			return new ServiceRateRepository(_mockMongoCollection.Object, _mockBank.Object);
		}
	}
}