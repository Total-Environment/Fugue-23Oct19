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
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DataAdaptorsTests
{
    public class MaterialDaoTests
    {
        [Fact]
        public async void AddRate_ShouldCheckForDuplicateEntry_ThrowDuplicateResourceException()
        {
            var material = await GetMaterial();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;
            materialDao.Columns["rates"] = new List<MaterialRateDao> { new MaterialRateDao { Location = "Bangalore", TypeOfPurchase = "Import", AppliedOn = appliedOn } };

            Action result = () => materialDao.AddRate(new MaterialRateDao
            {
                Location = "Bangalore",
                TypeOfPurchase = "Import",
                AppliedOn = appliedOn
            });

            result.ShouldThrow<DuplicateResourceException>()
                .WithMessage($"This Material Rate is already defined for materialId: CLY0001, location: Bangalore, appliedOn: {appliedOn}");
        }

        [Fact]
        public async void AddRate_ShouldAddValidMaterialRateToMaterial_ReturnTheAddedMaterialRate()
        {
            var material = await GetMaterial();
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
            var material = await GetMaterial();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;
            materialDao.Columns["rates"] = new List<MaterialRateDao> { new MaterialRateDao { Location = "Hyderabad", TypeOfPurchase = "Import", AppliedOn = appliedOn } };

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
            var material = await GetMaterial();
            var materialDao = new MaterialDao(material);
            var appliedOn = DateTime.Now;
            materialDao.Columns["rates"] = new List<MaterialRateDao> { new MaterialRateDao { Location = "Hyderabad", TypeOfPurchase = "Import", AppliedOn = appliedOn } };

            var result = materialDao.GetRateDaos();

            result.Count.Should().Be(1);
        }

        [Fact]
        public async void GetRates_ShouldReturnEmptyListOfRatesInTheMaterial_WhenRatesDontExist()
        {
            var material = await GetMaterial();
            var materialDao = new MaterialDao(material);

            var result = materialDao.GetRateDaos();

            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetDomain_ShouldReturnMaterialWithHeaders_WhenColumnsExist()
        {
            var material = await GetMaterial();

            material.Headers.Count().Should().Be(3);
            var firstHeader = material.Headers.First();
            firstHeader.Name.Should().Be("Classification");
            firstHeader.Columns.Count().Should().Be(1);
            var columnData = material.Headers.First().Columns.First();
            columnData.Name.Should().Be("Material Level 2");
            columnData.Key.Should().Be("material_level_2");
            columnData.Value.Should().Be("Clay");
            material.Id.Should().Be("CLY0001");
            material.AmendedAt.Should().Be(DateTime.Parse("1989-01-01T13:23Z"));
            material.AmendedBy.Should().Be("Srikanth");
            material.CreatedBy.Should().Be("TE");
            material.CreatedAt.Should().Be(DateTime.Parse("1989-02-01T13:23Z"));
        }

        [Fact]
        public async Task GetDomain_ShouldReturnMaterialWithSearchKeywords_WhenSearchKeywordsExistsInColumns()
        {
            var material = await GetMaterial();

            material.SearchKeywords.Should().BeEquivalentTo(new List<string> { "CLY0001", "Amar", "Akbar", "Anthony" });
        }

        [Fact]
        public async Task GetDomainWithoutAsset_ShouldReturnMaterialWithoutAssetDefinition()
        {
            var (materialDao, materialRepository) = SetupMaterialRepository();
            var mockBrandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            var material = await materialDao.GetDomainWithoutAsset(materialRepository, mockBrandDefinitionRepository.Object);
            material.Headers.Count().Should().Be(3);
        }

        [Fact]
        public void New_ShouldReturnMaterialDaoWithColumnsFilled_WhenPassedMaterialWithColumnsInHeaders()
        {
            var materialMock = new Mock<IMaterial>();
            var headerData = new HeaderData("Classification", "classification_key")
            {
                Columns = new List<ColumnData> { new ColumnData("Material Level 1", "material_level_1", "Primary") }
            };
            materialMock.Setup(m => m.Headers).Returns(new List<IHeaderData> { headerData });
            var materialDao = new MaterialDao(materialMock.Object);
            materialDao.Columns.Should().ContainKey("material_level_1").WhichValue.Should().Be("Primary");
            materialDao.Columns.Count.Should().Be(8);
        }

        [Fact]
        public void SetDomain_ShouldFillColumns_WhenPassedMaterialWithColumnsInHeaders()
        {
            var materialMock = new Mock<IMaterial>();
            var headerData = new HeaderData("Classification", "classification_key")
            {
                Columns = new List<ColumnData> { new ColumnData("Material Level 1", "material_level_1", "Primary") }
            };
            materialMock.Setup(m => m.Headers).Returns(new List<IHeaderData> { headerData });
            var materialDao = new MaterialDao();
            materialDao.SetDomain(materialMock.Object);
            materialDao.Columns.Should().ContainKey("material_level_1").WhichValue.Should().Be("Primary");
            materialDao.Columns.Count.Should().Be(8);
        }

        [Fact]
        public void SetDomain_ShouldSetSearchKeywords_WhenMaterialWithSearchKeywordsIsPassed()
        {
            var materialMock = new Mock<IMaterial>();
            var headerData = new HeaderData("Classification", "Classification")
            {
                Columns = new List<ColumnData> { new ColumnData("Material Level 1", "Material Level 1", "Primary") }
            };
            materialMock.Setup(m => m.Headers).Returns(new List<IHeaderData> { headerData });
            var expected = new List<string> { "Eena", "Meena", "Deeka" };
            materialMock.Setup(m => m.SearchKeywords).Returns(expected);
            var materialDao = new MaterialDao();

            materialDao.SetDomain(materialMock.Object);

            materialDao.Columns["SearchKeywords"].As<IEnumerable<string>>().Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SetDomain_ShouldUpdateAutogeneratedProperties_WhenPassedMaterial()
        {
            var materialMock = new Mock<IMaterial>();
            const string materialCode = "CLY0001";
            var amendedAt = DateTime.Now;
            var createdAt = DateTime.Today;
            const string amendedBy = "TE";
            const string createdBy = "Anil";
            var headerData = new HeaderData("Classification", "Classification")
            {
                Columns = new List<ColumnData>
                {
                    new ColumnData("Material Level 2","material_level_2", "Clay"),
                    new ColumnData("Material Code","material_code", null),
                    new ColumnData("Date Created","date_created", null),
                    new ColumnData("Date Last Amended","date_last_amended", null),
                    new ColumnData("Created By","created_by", null),
                    new ColumnData("Amended By","amended_by", null)
                }
            };
            materialMock.Setup(m => m.Headers).Returns(new List<IHeaderData> { headerData });
            materialMock.Setup(m => m.Id).Returns(materialCode);
            materialMock.Setup(m => m.AmendedAt).Returns(amendedAt);
            materialMock.Setup(m => m.CreatedAt).Returns(createdAt);
            materialMock.Setup(m => m.CreatedBy).Returns(createdBy);
            materialMock.Setup(m => m.AmendedBy).Returns(amendedBy);
            materialMock.Setup(m => m.Group).Returns("Clay");

            var materialDao = new MaterialDao();
            materialDao.SetDomain(materialMock.Object);

            materialDao.Columns["material_code"].Should().Be(materialCode);
            materialDao.Columns["created_by"].Should().Be(createdBy);
            materialDao.Columns["date_created"].Should().Be(createdAt);
            materialDao.Columns["date_last_amended"].Should().Be(amendedAt);
            materialDao.Columns["last_amended_by"].Should().Be(amendedBy);
            materialDao.Columns["group"].Should().Be("Clay");
        }

        private async Task<Material> GetMaterial()
        {
            var mockAssetDefinitionRepository = new Mock<IComponentDefinitionRepository<AssetDefinition>>();
            var mockBrandDefinitionRepository = new Mock<IBrandDefinitionRepository>();
            var tuple = SetupMaterialRepository();
            var material = await tuple.Item1.GetDomain(tuple.Item2,
                mockAssetDefinitionRepository.Object, mockBrandDefinitionRepository.Object);
            material.AppendSearchKeywords(new List<string> { "Amar", "Akbar", "Anthony" });
            return material;
        }

        private (MaterialDao materialDao, IComponentDefinitionRepository<IMaterialDefinition> materialRepository)
            SetupMaterialRepository()
        {
            var mockDefinitionRepository = new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();
            var columnName = "material_level_2";
            var columnValue = "Clay";
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
                    new HeaderDefinition("Classification", "Classification",new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Material Level 2","material_level_2", new ConstantDataType("Clay"))
                    }),
                    new HeaderDefinition("General", "general", new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Material Code","material_code", new StringDataType())
                    }),
                    new HeaderDefinition("System Logs", "System Logs",new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Date Created","date_created",  new StringDataType()),
                        new ColumnDefinition("Date Last Amended","date_last_amended", new StringDataType()),
                        new ColumnDefinition("Created By","created_by", new StringDataType()),
                        new ColumnDefinition("Last Amended By", "last_amended_by",new StringDataType())
                    })
                }
            };

            mockDefinitionRepository.Setup(m => m.Find(columnValue)).ReturnsAsync(definition);
            return (materialDao, mockDefinitionRepository.Object);
        }
    }
}