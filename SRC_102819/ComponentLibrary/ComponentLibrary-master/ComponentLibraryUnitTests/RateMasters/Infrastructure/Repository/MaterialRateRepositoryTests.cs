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
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Repository
{
	public class MaterialRateRepositoryTests
	{
		private readonly Mock<IBank> _mockBank = new Mock<IBank>();

		private readonly Mock<IMongoCollection<MaterialDao>> _mockMongoCollection =
					new Mock<IMongoCollection<MaterialDao>>();

		[Fact]
		public void Should_ImplementIMaterialRateRepository()
		{
			SystemUnderTest().Should().BeAssignableTo<IMaterialRateRepository>();
		}

		[Fact]
		public async void AddRates_ShouldGetTheCorrespondingMaterial_ToAppendRatesToIt()
		{
			var materialRate = GetMaterialRate(DateTime.UtcNow);
			var expectedRateDao = new MaterialRateDao(materialRate);
			var materialDao = new MaterialDao
			{
				Columns = new Dictionary<string, object>
				{
					{"material_code", "ALM00001"}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);

			await SystemUnderTest().AddRate(materialRate);
			((List<MaterialRateDao>)materialDao.Columns["rates"]).First().Should().Be(expectedRateDao);
		}

		[Fact]
		public void AddRates_ShouldThrowResourceNotFoundException_WhenMaterialDoesNotExist()
		{
			var materialRate = GetMaterialRate(DateTime.UtcNow);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task<IMaterialRate>> result = async () => await SystemUnderTest().AddRate(materialRate);
			result.ShouldThrow<ResourceNotFoundException>().WithMessage($"Material of {materialRate.Id} not found.");
		}

		[Fact]
		public async void AddRates_ShouldUpdateExistingMaterialWithRates_ReturnUpdatedMaterialRate()
		{
			var materialRate = GetMaterialRate(DateTime.UtcNow);
			var materialDao = new MaterialDao
			{
				Columns = new Dictionary<string, object>
				{
					{"material_code", "ALM00001"}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			await SystemUnderTest().AddRate(materialRate);

			_mockMongoCollection.Verify(
				m =>
					m.ReplaceOneAsync(
						It.Is(
							(FilterDefinition<MaterialDao> f) =>
									CheckFilterEquality(f, materialDao.Columns[MaterialDao.MaterialCode].ToString())),
						materialDao, null, default(CancellationToken)));
		}

		[Fact]
		public async void GetRate_ForAGivenAppliedDate_ShouldReturnTheLatestDate()
		{
			var appliedOn = DateTime.UtcNow;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5)));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(-5)));
			var materialDao = new MaterialDao
			{
				Columns = new Dictionary<string, object>
				{
					{"material_code", "ALM00001"},
					{"rates", new List<MaterialRateDao> {materialRate1, materialRate2}}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);

			var materialRate = await SystemUnderTest().GetRate("ALM00001", "Bangalore", appliedOn, "Import");

			materialRate.AppliedOn.Should().Be(materialRate2.AppliedOn);
			materialRate.Location.Should().Be(materialRate2.Location);
			materialRate.Id.Should().Be(materialRate2.MaterialId);
			materialRate.TypeOfPurchase.Should().Be(materialRate2.TypeOfPurchase);
		}

		[Fact]
		public async void GetRate_ForValidFilters_ShouldReturnCorrespondingMaterialRates()
		{
			var appliedOn = DateTime.UtcNow;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5)));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn));
			var materialDao = new MaterialDao
			{
				Columns = new Dictionary<string, object>
				{
					{"material_code", "ALM00001"},
					{"rates", new List<MaterialRateDao> {materialRate1, materialRate2}}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);

			var materialRate = await SystemUnderTest().GetRate("ALM00001", "Bangalore", appliedOn, "Import");

			materialRate.AppliedOn.Should().Be(materialRate2.AppliedOn);
			materialRate.Location.Should().Be(materialRate2.Location);
			materialRate.Id.Should().Be(materialRate2.MaterialId);
			materialRate.TypeOfPurchase.Should().Be(materialRate2.TypeOfPurchase);
		}

		[Fact]
		public void GetRate_ShouldThrowResourceNotFoundException_WhenMaterialDoesNotExist()
		{
			var materialRate = GetMaterialRate(DateTime.UtcNow);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task<IMaterialRate>> result =
				async () =>
					await
						SystemUnderTest()
							.GetRate(materialRate.Id, materialRate.Location, materialRate.AppliedOn,
								materialRate.TypeOfPurchase);
			result.ShouldThrow<ResourceNotFoundException>().WithMessage($"Material of {materialRate.Id} not found.");
		}

		[Fact]
		public void GetRate_WhenLocationisPassedAsNullOrWhitespace_ShouldThrowArgumentException()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRate("materialId", null, It.IsAny<DateTime>(), "typeOfPurchase");

			action.ShouldThrow<ArgumentException>().WithMessage("Location is a mandatory field.");
		}

		[Fact]
		public void GetRate_WhenMaterialCodeIsPassedAsNull_ThrowArgumentExceptionm()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRate(null, "location", It.IsAny<DateTime>(), "typeOfPurchase");

			action.ShouldThrow<ArgumentException>().WithMessage("Material Code is a mandatory field.");
		}

		[Fact]
		public void GetRate_WhenTypeOfPurchaseIsPassedAsNull_ThrowArgumentExceptionm()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRate("materialId", "location", It.IsAny<DateTime>(), null);

			action.ShouldThrow<ArgumentException>().WithMessage("Type of Purchase is a mandatory field.");
		}

		[Fact]
		public void GetRateHistory_ForAGivenMaterialWhichHasNoRates_ThrowResourceNotFoundException()
		{
			var materialDao = new MaterialDao
			{
				Columns = new Dictionary<string, object>
				{
					{"material_code", "ALM00001"}
				}
			};
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			Func<Task> action = () => SystemUnderTest().GetRateHistory("ALM00001");

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Material Rate of material ALM00001 not found.");
		}

		[Fact]
		public async void GetRateHistory_ForAGivenMaterialWhichHasRates_ReturnPaginatedListOfRates()
		{
			var appliedOn = DateTime.Now;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5), "Hyderabad"));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(-2)));
			var materialRate3 = new MaterialRateDao(GetMaterialRate(appliedOn, "Bangalore", "Inter-State"));
			var materialRate4 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(2), "Bangalore", "Inter-State"));
			var materialRatesList = new List<MaterialRateDao>
			{
				materialRate1,
				materialRate2,
				materialRate3,
				materialRate4
			};
			var materialDao = GetMaterialDaoWithRates(materialRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			var result = await SystemUnderTest().GetRateHistory("ALM00001");

			result.Items.Count().Should().Be(3);
			result.Items.ToList()[0].AppliedOn.Should().Be(materialRate4.AppliedOn);
			result.Items.ToList()[1].AppliedOn.Should().Be(materialRate1.AppliedOn);
			result.Items.ToList()[2].AppliedOn.Should().Be(materialRate3.AppliedOn);
		}

		[Fact]
		public async void GetRateHistory_ForAGivenMaterialWhichHasRates_ReturnPaginatedListOfRatesSortedByTypeOfPurchase()
		{
			var appliedOn = DateTime.Now;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5), "Hyderabad"));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(-2)));
			var materialRate3 = new MaterialRateDao(GetMaterialRate(appliedOn, typeOfPurchase: "Anter-State"));
			var materialRate4 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(2), typeOfPurchase: "Inter-State"));
			var materialRatesList = new List<MaterialRateDao> { materialRate1, materialRate2, materialRate3, materialRate4 };
			var materialDao = GetMaterialDaoWithRates(materialRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			var result = await SystemUnderTest().GetRateHistory("ALM00001", sortColumn: "TypeOfPurchase");

			result.Items.Count().Should().Be(3);
			result.Items.ToList()[0].TypeOfPurchase.Should().Be(materialRate4.TypeOfPurchase);
			result.Items.ToList()[1].TypeOfPurchase.Should().Be(materialRate2.TypeOfPurchase);
			result.Items.ToList()[2].TypeOfPurchase.Should().Be(materialRate1.TypeOfPurchase);
		}

		[Fact]
		public async void GetRateHistory_ForAGivenMaterialWhichHasRates_ReturnPaginatedListOfRatesSortedInAscendingOrder()
		{
			var appliedOn = DateTime.Now;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5), "Hyderabad"));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(-2)));
			var materialRate3 = new MaterialRateDao(GetMaterialRate(appliedOn, "Bangalore", "Inter-State"));
			var materialRate4 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(2), "Bangalore", "Inter-State"));
			var materialRatesList = new List<MaterialRateDao> { materialRate1, materialRate2, materialRate3, materialRate4 };
			var materialDao = GetMaterialDaoWithRates(materialRatesList);
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			var result = await SystemUnderTest().GetRateHistory("ALM00001", sortOrder: SortOrder.Ascending);

			result.Items.Count().Should().Be(3);
			result.Items.ToList()[0].AppliedOn.Should().Be(materialRate2.AppliedOn);
			result.Items.ToList()[1].AppliedOn.Should().Be(materialRate3.AppliedOn);
			result.Items.ToList()[2].AppliedOn.Should().Be(materialRate1.AppliedOn);
		}

		[Fact]
		public void GetRateHistory_WhenMaterialCodeIsPassedAsNull_ThrowArgumentExceptionm()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRateHistory(null);

			action.ShouldThrow<ArgumentException>().WithMessage("Material Code is a mandatory field.");
		}

		[Fact]
		public void GetRateHistory_WhenMaterialOfGivenMaterialCodeDoesNotExist_ThrowResourceNotFoundException()
		{
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task> action =
				() => SystemUnderTest().GetRateHistory("ALM00001");

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Material of ALM00001 not found.");
		}

		[Fact]
		public void GetRatesHistory_ForAGivenMaterial_ThrowResourceNotFoundExceptionIfNoRatesAreFound()
		{
			var materialDao = GetMaterialDaoWithRates(null);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			Func<Task> action =
				() => SystemUnderTest().GetRateHistory("ALM00001");

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Material Rate of material ALM00001 not found.");
		}

		[Fact]
		public async void GetRates_ForAGivenMaterialAndDate_ReturnAllRatesThatAreAppliedOnBeforeGivenDate()
		{
			var appliedOn = DateTime.Now;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5), "Hyderabad"));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(-2)));
			var materialRate3 = new MaterialRateDao(GetMaterialRate(appliedOn, typeOfPurchase: "Anter-State"));
			var materialRate4 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(2), typeOfPurchase: "Inter-State"));
			var materialRatesList = new List<MaterialRateDao> { materialRate1, materialRate2, materialRate3, materialRate4 };
			var materialDao = GetMaterialDaoWithRates(materialRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			var result = (await SystemUnderTest().GetRates("ALM00001", appliedOn)).ToList();

			result.Count.Should().Be(2);
			result[0].AppliedOn.Should().Be(materialRate2.AppliedOn);
			result[1].AppliedOn.Should().Be(materialRate3.AppliedOn);
		}

		[Fact]
		public async void GetRates_ForAGivenMaterialAndLocationAndDate_ReturnAllRatesThatAreAppliedOnBeforeGivenDateForGivenLocation()
		{
			var appliedOn = DateTime.Now;
			var materialRate1 = new MaterialRateDao(GetMaterialRate(appliedOn.AddHours(5), "Hyderabad"));
			var materialRate2 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(-2), "Hyderabad"));
			var materialRate3 = new MaterialRateDao(GetMaterialRate(appliedOn, typeOfPurchase: "Anter-State"));
			var materialRate4 = new MaterialRateDao(GetMaterialRate(appliedOn.AddDays(2), typeOfPurchase: "Inter-State"));
			var materialRatesList = new List<MaterialRateDao> { materialRate1, materialRate2, materialRate3, materialRate4 };
			var materialDao = GetMaterialDaoWithRates(materialRatesList);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			var result = (await SystemUnderTest().GetRates("ALM00001", "Hyderabad", appliedOn)).ToList();

			result.Count.Should().Be(1);
			result[0].AppliedOn.Should().Be(materialRate2.AppliedOn);
		}

		[Fact]
		public void GetRates_ForAGivenMaterialAndLocationAndDate_ThrowResourceNotFoundExceptionIfNoRatesAreFound()
		{
			var materialDao = GetMaterialDaoWithRates(null);

			TestHelper.MockCollectionWithExisting(_mockMongoCollection, materialDao);
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", DateTime.UtcNow);

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Material Rate of material ALM00001 not found.");
		}

		[Fact]
		public void GetRates_ShouldThrowArgumentException_WhenMaterialCodeGivenIsNull()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRates(null, DateTime.Now);

			action.ShouldThrow<ArgumentException>().WithMessage("Material Code is a mandatory field.");
		}

		[Fact]
		public void GetRates_ShouldThrowArgumentException_WhenMaterialCodeGivenIsNullAndLocationIsPresent()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRates(null, "Hyderabad", DateTime.Now);

			action.ShouldThrow<ArgumentException>().WithMessage("Material Code is a mandatory field.");
		}

		[Fact]
		public void GetRates_ShouldThrowArgumentException_WhenMaterialCodeIsPresentAndLocationIsNull()
		{
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", null, DateTime.Now);

			action.ShouldThrow<ArgumentException>().WithMessage("Location is a mandatory field.");
		}

		[Fact]
		public void GetRates_WhenMaterialOfGivenMaterialCodeDoesNotExist_ThrowResourceNotFoundException()
		{
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", DateTime.UtcNow);

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Material of ALM00001 not found.");
		}

		[Fact]
		public void GetRates_WhenMaterialOfGivenMaterialCodeDoesNotExistForGivenLocation_ThrowResourceNotFoundException()
		{
			TestHelper.MockCollectionWithExisting(_mockMongoCollection, null);
			Func<Task> action =
				() => SystemUnderTest().GetRates("ALM00001", "Hyderabad", DateTime.UtcNow);

			action.ShouldThrow<ResourceNotFoundException>().WithMessage("Material of ALM00001 not found.");
		}

		private static bool CheckFilterEquality(FilterDefinition<MaterialDao> value, string materialDaoId)
		{
			return GetPrivateFieldValue(GetPrivateFieldValue(value, "_field"), "_fieldName").Equals("material_code") &&
				   GetPrivateFieldValue(value, "_value").Equals(materialDaoId);
		}

		private static MaterialDao GetMaterialDaoWithRates(List<MaterialRateDao> materialRatesList)
		{
			var materialDao = new MaterialDao
			{
				Columns = new Dictionary<string, object>
				{
					{"material_code", "ALM00001"},
					{"rates", materialRatesList}
				}
			};
			return materialDao;
		}

		private static object GetPrivateFieldValue(object definition, string fieldName)
		{
			return
				definition.GetType()
					.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(definition);
		}

		private MaterialRate GetMaterialRate(DateTime appliedOn, string location = "Bangalore",
			string typeOfPurchase = "Import")
		{
			var coefficient = new PercentageCoefficient("Location variance", 10);
			var basicRate = new Money(10, "INR", _mockBank.Object);
			var materialRate = new MaterialRate(appliedOn, location, "ALM00001", basicRate, 0, 0,0,0,0,0,0, typeOfPurchase);
			return materialRate;
		}

		private MaterialRateRepository SystemUnderTest()
		{
			return new MaterialRateRepository(_mockMongoCollection.Object,_mockBank.Object);
		}
	}
}