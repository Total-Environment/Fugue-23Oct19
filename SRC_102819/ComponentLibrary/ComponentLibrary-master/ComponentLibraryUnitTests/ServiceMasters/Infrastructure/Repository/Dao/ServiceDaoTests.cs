using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure.Repository.Dao
{
    public class ServiceDaoTests
    {
        [Fact]
        public async void AddRate_ShouldCheckForDuplicateEntry_ThrowDuplicateResourceException()
        {
            var material = await GetService();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;
            materialDao.Columns["rates"] = new List<MaterialRateDao>
            {
                new MaterialRateDao {Location = "Bangalore", TypeOfPurchase = "Import", AppliedOn = appliedOn}
            };

            Action result = () => materialDao.AddRate(new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            });

            result.ShouldThrow<DuplicateResourceException>()
                .WithMessage(
                    $"This Material Rate is already defined for materialId: CLY0001, location: Bangalore, appliedOn: {appliedOn}");
        }

        private async Task<IMaterial> GetService()
        {
            var mockAssetDefinitionRepository = new Mock<IComponentDefinitionRepository<AssetDefinition>>();
            var mockBrandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            var tuple = SetupServiceRepository();
            var material = await tuple.Item1.GetDomain(tuple.Item2,
                mockAssetDefinitionRepository.Object, mockBrandDefinitionRepository.Object);
            material.AppendSearchKeywords(new List<string> { "Amar", "Akbar", "Anthony" });
            return material;
        }

        private static (MaterialDao materialDao, IComponentDefinitionRepository<IMaterialDefinition> materialRepository)
            SetupServiceRepository()
        {
            var mockDefinitionRepository = new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();
            const string columnName = "material_level_2";
            const string columnValue = "Clay";
            var amendedAt = DateTime.Parse("1989-01-01T13:23Z");
            const string id = "CLY0001";
            const string amendedBy = "Srikanth";
            const string createdBy = "TE";
            var createdAt = DateTime.Parse("1989-02-01T13:23Z");
            var materialDao = new MaterialDao
            {
                Columns = new Dictionary<string, object>
                {
                    {"group", columnValue},
                    {columnName, columnValue},
                    {"component_code", id},
                    {"material_code", id},
                    {"date_created", createdAt},
                    {"date_last_amended", amendedAt},
                    {"created_by", createdBy},
                    {"last_amended_by", amendedBy},
                    {"SearchKeywords", new List<string>()}
                }
            };

            var definition = new MaterialDefinition("Clay")
            {
                Headers = new List<IHeaderDefinition>
                {
                    new HeaderDefinition("Classification", "Classification", new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Material Level 2", "material_level_2", new ConstantDataType("Clay"))
                    }),
                    new HeaderDefinition("General", "general", new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Material Code", "material_code", new StringDataType())
                    }),
                    new HeaderDefinition("System Logs", "System Logs", new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Date Created", "date_created", new StringDataType()),
                        new ColumnDefinition("Date Last Amended", "date_last_amended", new StringDataType()),
                        new ColumnDefinition("Created By", "created_by", new StringDataType()),
                        new ColumnDefinition("Last Amended By", "last_amended_by", new StringDataType())
                    })
                }
            };

            mockDefinitionRepository.Setup(m => m.Find(columnValue)).ReturnsAsync(definition);
            return (materialDao, mockDefinitionRepository.Object);
        }

        [Fact]
        public async void AddRate_ShouldAddValidMaterialRateToMaterial_ReturnTheAddedMaterialRate()
        {
            var material = await GetService();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;

            var materialRate = new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            };
            var result = materialDao.AddRate(materialRate);

            ((List<MaterialRateDao>)materialDao.Columns["rates"]).First().Should().Be(materialRate);
            result.Location.Should().Be("Bangalore");
            result.TypeOfPurchase.Should().Be("Import");
            result.AppliedOn.Should().Be(appliedOn);
        }

        [Fact]
        public async void AddRate_ShouldAddValidMaterialRateToExistingListOfMaterials_ReturnTheAddedMaterialRate()
        {
            var material = await GetService();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;
            materialDao.Columns["rates"] = new List<MaterialRateDao>
            {
                new MaterialRateDao {Location = "Hyderabad", TypeOfPurchase = "Import", AppliedOn = appliedOn}
            };

            var materialRate = new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            };

            materialDao.AddRate(materialRate);

            ((List<MaterialRateDao>)materialDao.Columns["rates"]).Count.Should().Be(2);
        }

        [Fact]
        public async void GetRates_ShouldReturnTheListOfRatesInTheMaterial_WhenRatesExist()
        {
            var material = await GetService();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;
            materialDao.Columns["rates"] = new List<MaterialRateDao>
            {
                new MaterialRateDao {Location = "Hyderabad", TypeOfPurchase = "Import", AppliedOn = appliedOn}
            };

            var result = materialDao.GetRateDaos();

            result.Count.Should().Be(1);
        }

        [Fact]
        public async void GetRates_ShouldReturnEmptyListOfRatesInTheMaterial_WhenRatesDontExist()
        {
            var material = await GetService();
            var materialDao = new MaterialDao(material);

            var result = materialDao.GetRateDaos();

            result.Count.Should().Be(0);
        }

        [Fact]
        public void Distinct_ShouldForAListOfServicesWithSameId_ReturnOneServiceDao()
        {
            var service1 = new ServiceDao
            {
                Columns = new Dictionary<string, object>() { { ServiceDao.ServiceCode, "FLR00001" } }
            };
            var service2 = new ServiceDao
            {
                Columns = new Dictionary<string, object>() { { ServiceDao.ServiceCode, "FLR00001" } }
            };

            var serviceList = new List<ServiceDao> { service1, service2 };

            serviceList.Distinct().Count().Should().Be(1);
        }

        [Fact]
        public void Equals_ShouldForTwoServicesWithSameId_ReturnTrue()
        {
            var service1 = new ServiceDao
            {
                Columns = new Dictionary<string, object>() { { ServiceDao.ServiceCode, "FLR00001" } }
            };
            var service2 = new ServiceDao
            {
                Columns = new Dictionary<string, object>() { { ServiceDao.ServiceCode, "FLR00001" } }
            };

            service1.Should().Be(service2);
        }

        [Fact]
        public async Task GetDomain_Should_UseServiceLevel1ToGetServiceDefinition()
        {
            var serviceDao = new ServiceDao();
            serviceDao.Columns = new Dictionary<string, object>()
            {
                {"service_level_1", "Dado"},
                {"service_code", "1"},
                {"date_created", DateTime.Now},
                {"created_by", "te"},
                {"last_amended_by", "te"},
                {"date_last_amended", DateTime.Now},
                {"SearchKeywords", new List<string>()}
            };
            var columns = new List<IColumnDefinition>();
            columns.Add(new ColumnDefinition("Service Level 1", "service_level_1", new StringDataType()));
            columns.Add(new ColumnDefinition("Service Code", "service_code", new StringDataType()));
            columns.Add(new ColumnDefinition("Date Created", "date_created", new DateDataType()));
            columns.Add(new ColumnDefinition("Date Last Amended", "date_last_amended", new DateDataType()));
            columns.Add(new ColumnDefinition("Last Amended By", "last_amended_by", new StringDataType()));
            columns.Add(new ColumnDefinition("Created By", "created_by", new DateDataType()));

            var mockServiceDefinitionRepository = new Mock<IComponentDefinitionRepository<IServiceDefinition>>();
            var headers = new List<IHeaderDefinition>() { new HeaderDefinition("header", "header", columns) };
            var mockServiceDefinition = new ServiceDefinition("service");
            mockServiceDefinition.Headers = headers;
            mockServiceDefinitionRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(mockServiceDefinition);

            await serviceDao.GetDomain(mockServiceDefinitionRepository.Object);

            mockServiceDefinitionRepository.Verify(m => m.Find("Dado"), Times.Once);
        }
    }
}