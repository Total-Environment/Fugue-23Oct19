using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class MaterialServiceTests
    {
        [Fact]
        public async void Create_ShouldCallAssetDEfinitionRepo_WhenCanBeUsedAsAssetIsTrue()
        {
            var fixture = new Fixture().FindOfMaterialDefinitionRepoReturnMaterialWhenPassed("CLAY", GetMaterialDefinition())
                .BuildOfMaterialBuilderShouldReturn(GetMaterial("CLAY"))
                .FindOfAssetDefinitionShouldReturn(new AssetDefinition("CLAY") { Code = "CLY" })
                .FindOfAssetDefinitionShouldBeCalledWith("CLAY");

            await fixture.SystemUnderTest().Create(GetAssetMaterial("CLAY"));

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Create_ShouldCallBuildOfMaterialBuilder_WhenMaterialIsPassed()
        {
            var materialDefinition = GetMaterialDefinition();
            const string materialGroup = "CLAY";
            var material = GetMaterial(materialGroup);
            var fixture = new Fixture().FindOfMaterialDefinitionRepoReturnMaterialWhenPassed(materialGroup, materialDefinition)
                .BuildOfMaterialBuilderShouldBeCalledWith(material, materialDefinition)
                .BuildOfMaterialBuilderShouldReturn(material);

            await fixture.SystemUnderTest().Create(material);

            fixture.VerifyExpectations();
        }

        [Fact]
        public void Create_ShouldThrowArgumentException_WhenMaterialDefinitionRepoThrowsResourceNotFound()
        {
            var fixture = new Fixture().FindOfMaterialDefinitionRepoThrows(new ResourceNotFoundException(""));

            Func<Task> action = async () => await fixture.SystemUnderTest().Create(GetMaterial("CLAY"));

            action.ShouldThrow<ArgumentException>().WithMessage("Invalid material group: No definition found.");
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldLookupForGroup_ToPasstoFilterBuilder()
        {
            var group = "clay";
            var filterData = new List<FilterData>();
            var fixture = new Fixture();
            var materialService = fixture.HavingRepositoryAcceptingfindWith(group).SystemUnderTest();

            await materialService.SearchWithinGroup(filterData, group, new List<string> { "test" }, 1, 10);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldPassFilterCriteriaFromBuilderTo_ToMaterialRepository()
        {
            var group = "clay";
            var filterData = new List<FilterData> { new FilterData("Material Name", "Test Material Name") };
            var fixture = new Fixture();
            var filterCriteria = new Dictionary<string, Tuple<string, object>> { { "key", new Tuple<string, object>("Eq", "value") } };

            var materialService =
                fixture.WithFilterBuilderReturningCriteria(filterCriteria)
                    .HavingMaterialRepositorySearchAcceptingFilterCriteria(filterCriteria)
                    .SystemUnderTest();

            await materialService.SearchWithinGroup(filterData, group, new List<string> { "test" }, 1, 10);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task
            SearchWithinGroup_ShouldPassMaterialDefinitionFilterDataAndSearchKeyWordsToFilterBuilder_ForGeneratingFilterCriteria()
        {
            var group = "clay";
            var filterData = new List<FilterData> { new FilterData("Material Name", "Test Material Name") };
            var searchKeywords = new List<string> { "cla" };

            var fixture = new Fixture();

            var materialService =
                fixture.WithMaterialDefintionHavingGroup(group)
                    .WithBrandDefinition()
                    .HavingFilterCriteriaBuilderAccepting(filterData, group, searchKeywords)
                    .SystemUnderTest();

            await materialService.SearchWithinGroup(filterData, group, searchKeywords, 1, 10);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldReturnResults_FromMaterialRepository()
        {
            var group = "clay";
            var filterData = new List<FilterData> { new FilterData("Material Name", "Test Material Name") };
            var fixture = new Fixture();
            var materials = new List<IMaterial> { new Material() };

            var materialService = fixture.WithMaterialRepositoryreturning(materials).SystemUnderTest();

            var result = await materialService.SearchWithinGroup(filterData, group, new List<string> { "test" }, 1, 10);

            result.Count.Should().Be(1);
        }

        [Fact]
        public async void Find_ShouldReturnMaterial_WhenExistingIdIsPassed()
        {
            var materialId = "CLY00001";
            var fixture = new Fixture().WithExistingMaterial(materialId);

            var result = await fixture.SystemUnderTest().Find(materialId);

            result.Id.Should().Be(materialId);
        }

        [Fact]
        public async void GetMaterialHavingAttachmentColumnDataInGroup_ShouldReturmMaterialsWithAttachmentData_WhenGroupNameAndColumnNameIsPassed()
        {
            const string materialGroup = "CLAY";
            const string attachmentColumnName = "Quality Evaluation Method";
            var materials = new List<IMaterial>()
            {
                GetMaterial(materialGroup)
            };
            var materialDefinition = GetMaterialDefinitionWithGroupHavingAttachmentColumn(materialGroup, attachmentColumnName);

            var fixture = new Fixture()
                .FindOfMaterialDefinitionRepoReturnMaterialWhenPassed(materialGroup, materialDefinition)
                .GetByGroupAndColumnNameOfMaterialRepoReturnsListOfMaterials(materials);

            var result = await fixture.SystemUnderTest().GetMaterialHavingAttachmentColumnDataInGroup(materialGroup, attachmentColumnName, 1, 100);

            result.Count.Should().Be(1);
        }

        [Fact]
        public void GetMaterialHavingAttachmentColumnDataInGroup_ShouldThrowArgumentException_WhenColumnNameDoesNotExistInMaterialDefinitionAsync()
        {
            const string materialGroup = "CLAY";
            const string attachmentColumnName = "Quality Evaluation Method";
            var materialDefinition = GetMaterialDefinitionWithGroupHavingAttachmentColumn(materialGroup, attachmentColumnName);

            var fixture = new Fixture().FindOfMaterialDefinitionRepoReturnMaterialWhenPassed(materialGroup, materialDefinition);

            Func<Task> action = async () => await fixture.SystemUnderTest()
                .GetMaterialHavingAttachmentColumnDataInGroup(materialGroup, "Invalid Column Name", 10, 10);

            action.ShouldThrow<ArgumentException>()
                .WithMessage("Invalid Column Name is not valid column in the material definition.");
        }

        [Fact]
        public void GetMaterialHavingAttachmentColumnDataInGroup_ShouldThrowArgumentException_WhenColumnIsNeitherAStaticFileNorACheckList()
        {
            const string materialGroup = "CLAY";
            const string attachmentColumnName = "Quality Evaluation Method";
            var materialDefinition = GetMaterialDefinitionWithGroupNotHavingAttachmentColumn(materialGroup, attachmentColumnName);

            var fixture = new Fixture().FindOfMaterialDefinitionRepoReturnMaterialWhenPassed(materialGroup, materialDefinition);

            Func<Task> action = async () => await fixture.SystemUnderTest()
                .GetMaterialHavingAttachmentColumnDataInGroup(materialGroup, attachmentColumnName, 10, 10);

            action.ShouldThrow<ArgumentException>()
                .WithMessage("Quality Evaluation Method is neither static file data type nor check list data type.");
        }

        [Fact]
        public async void GetCountOfMaterialsHavingAttachmentColumnDataInGroup_ShouldReturnTotalNumberOfMaterial_WhenGroupAndColumnNameIsPassed()
        {
            const string materialGroup = "CLAY";
            const string attachmentColumnName = "Quality Evaluation Method";
            const int count = 100;
            var fixture = new Fixture().GetTotalCountByGroupAndColumnNameOfMaterialRepoReturns(count);

            var result = await fixture.SystemUnderTest()
                .GetCountOfMaterialsHavingAttachmentColumnDataInGroup(materialGroup, attachmentColumnName);

            result.Should().Be(count);
        }

        [Fact]
        public async void GetRecentMaterials_ShouldReturnMostRecentlyCreatedMaterial_WhenPageNumberAndBatchSizeArePassed()
        {
            var fixture = new Fixture().ListComponentOfMaterialRepoReturns(new List<IMaterial> { new Material() });

            var recentMaterials = await fixture.SystemUnderTest().GetRecentMaterials(1, 10);

            recentMaterials.Should().HaveCount(1);
        }

        [Fact]
        public async void GetMaterialCount_ShouldReturnTotalMaterial()
        {
            const int totalMaterialCount = 100;
            var fixture = new Fixture().CountOfMaterialRepoReturns(totalMaterialCount);

            var result = await fixture.SystemUnderTest().GetMaterialCount();

            result.Should().Be(totalMaterialCount);
        }

        [Fact]
        public async void Update_ShouldUpdateMaterial_WhenMaterialIsPassed()
        {
            const string materialid = "materialId";
            var fixture =
                new Fixture().BuildOfMaterialBuilderShouldReturn(GetMaterial("CLAY", materialid))
                    .UpdateOfMaterialRepoShouldBeCalledWithMaterialHavingId(materialid);

            await fixture.SystemUnderTest().Update(GetMaterial("CLAY", materialid));

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Update_ShouldReturnTheUpdatedMaterial_WhenMaterialIsPassed()
        {
            const string materialgroup = "materialGroup";
            const string materialcode = "materialCode";
            var material1 = GetMaterial(materialgroup, materialcode);
            var fixture = new Fixture().BuildOfMaterialBuilderShouldReturn(material1).FindOfMaterialRepoReturns(material1);

            var material = await fixture.SystemUnderTest().Update(GetMaterial(materialgroup));

            material.Group.Should().Be(materialgroup);
            material.Id.Should().Be(materialcode);
        }

        [Fact]
        public async void SearchInGroup_ShouldReturnMaterialInGroupWithSearchKeyword_WhenSearchKeywordArePassed()
        {
            var materialGroups = new List<string> { "Clay material", "furniture" };
            var fixture = new Fixture().SearchInGroupOfMaterialRepoReturns(materialGroups);

            var result = await fixture.SystemUnderTest().SearchForGroups(new List<string> { "keyword", "otherKeyword" });

            result.Should().BeEquivalentTo(materialGroups);
        }

        [Fact]
        public async void GetAllRates_ShouldTakeASearchRequestWithAppliedOn_ReturnSortedAndPaginatedListOfRates()
        {

            var bank = new Mock<IBank>();
            bank.Setup(b => b.ConvertTo(new Money(120, "INR", bank.Object), "INR"))
                .ReturnsAsync(new Money(120, "INR", bank.Object));
            bank.Setup(b => b.ConvertTo(new Money(0, "INR", bank.Object), "INR"))
                .ReturnsAsync(new Money(0, "INR", bank.Object));
            var appliedOn = DateTime.UtcNow;
            var filters = new List<FilterData>
            {
                new FilterData("AppliedOn", $"{appliedOn:yyyy-MM-dd}T03:00:00+5:30")
            };
            var materialRateSearchResult = new List<MaterialRateSearchResult>
            {
                new MaterialRateSearchResult(
                    "Clay",
                    new MaterialRate(
                        DateTime.Now,
                        "Hyderabad",
                        "CLY00001",
                        new Money(120m, "INR", bank.Object),
                        10,20,0,0,0,0,3,
                        "Import"
                        ),
                    "description"
                    )
            };
            var materialRatesList = new List<MaterialRateSearchResult>(materialRateSearchResult);

            var result = await new Fixture()
                .HavingMaterialRepositoryGetAllRatesAcceptingFilterCriteria(filters)
                .GetAllRatesOfMaterialRepositoryReturns(materialRatesList)
                .SystemUnderTest().GetAllRates(filters);

            result.ToList()[0].Should().BeOfType<MaterialRateSearchResult>();
        }

        private IMaterialDefinition GetMaterialDefinitionWithGroupNotHavingAttachmentColumn(string materialGroup, string attachmentColumnName)
        {
            return new MaterialDefinition(materialGroup)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("Purchase", "purchase", new List<IColumnDefinition>{
                        new ColumnDefinition(attachmentColumnName,attachmentColumnName, new StringDataType())
                    })
                }
            };
        }

        private IMaterialDefinition GetMaterialDefinitionWithGroupHavingAttachmentColumn(string materialGroup, string attachmentColumnName)
        {
            return new MaterialDefinition(materialGroup)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("Purchase", "purchase", new List<IColumnDefinition>{
                        new ColumnDefinition(attachmentColumnName,attachmentColumnName, new StaticFileDataType())
                    })
                }
            };
        }

        private static IMaterial GetMaterial(string group, string id = null, bool isAsset = false)
        {
            var columnDatas = new List<IColumnData>
            {
                new ColumnData("Material Name", "material_name", "Murrum"),
                new ColumnData("Material Code", "material_code", id)
            };
            columnDatas.Add(isAsset
                ? new ColumnData("Can be Used as an Asset", "can_be_used_as_an_asset", true)
                : new ColumnData("Can be Used as an Asset", "can_be_used_as_an_asset", false));

            var materialData = new Material
            {
                Id = id,
                Group = group,
                Headers = new List<HeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = columnDatas
                    },
                    new HeaderData("Classification", "classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2", "material_level_2", group)
                        }
                    }
                }
            };
            return materialData;
        }

        private IMaterial GetAssetMaterial(string group)
        {
            return GetMaterial(group, null, true);
        }

        private IMaterialDefinition GetMaterialDefinition()
        {
            return new MaterialDefinition("CLAY")
            {
                Code = "CLY",
                Headers = new List<IHeaderDefinition>
                {
                    new HeaderDefinition("General","General",  new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            "material level 1", "material level 1",
                            new MasterDataDataType(new MasterDataList("material_level_1", new List<MasterDataValue>
                            {
                                new MasterDataValue("Primary"),
                                new MasterDataValue("Secondry")
                            }))),
                        new ColumnDefinition(
                            "Material Code", "Material Code",
                            new StringDataType()),
                        new ColumnDefinition(
                            "Date", "Date",
                            new DateDataType()),
                        new ColumnDefinition(
                            "Image", "Image",
                            new ArrayDataType(new StringDataType())),
                        new ColumnDefinition(
                            "Material Name", "Material Name",
                            new StringDataType())
                    }),
                    new HeaderDefinition("Classification","Classification", new List<IColumnDefinition>
                    {
                        new ColumnDefinition(
                            "Material Level 2","Material Level 2",
                            new ConstantDataType("Clay"))
                    })
                }
            };
        }

        private class Fixture
        {
            private readonly Mock<IFilterCriteriaBuilder> _mockFilterBuilder = new Mock<IFilterCriteriaBuilder>();

            private readonly Mock<IComponentDefinitionRepository<IMaterialDefinition>> _mockMaterialDefinitionRepository
                = new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();

            private readonly Mock<IMaterialDefinition> _mockMaterialDefinition = new Mock<IMaterialDefinition>();
            private readonly Mock<IBrandDefinition> _mockBrandDefinition = new Mock<IBrandDefinition>();
            private readonly Mock<IComponentDefinitionRepository<AssetDefinition>> _mockAssetDefinitionRepository = new Mock<IComponentDefinitionRepository<AssetDefinition>>();
            private readonly Mock<IMaterialBuilder> _mockMaterialBuilder = new Mock<IMaterialBuilder>();
            private readonly Mock<IBrandDefinitionRepository> _mockBrandDefinitionRepository = new Mock<IBrandDefinitionRepository>();

            private readonly Mock<IMaterialRepository> _mockMaterialRepository
                = new Mock<IMaterialRepository>();

            private readonly List<Action> _verifications = new List<Action>();

            public Fixture BuildOfMaterialBuilderShouldBeCalledWith(IMaterial material,
                IMaterialDefinition materialDefinition)
            {
                _verifications.Add(() => _mockMaterialBuilder.Verify(m => m.BuildAsync(
                    It.Is<IMaterial>(mt => mt.Group == material.Group && mt.Headers.Count() == material.Headers.Count()),
                    It.Is<IMaterialDefinition>(md => md.Name == materialDefinition.Name)), Times.Once));
                return this;
            }

            public Fixture BuildOfMaterialBuilderShouldReturn(IMaterial material)
            {
                _mockMaterialBuilder.Setup(m => m.BuildAsync(
                        It.Is<IMaterial>(
                            mt => mt.Group == material.Group && mt.Headers.Count() == material.Headers.Count()),
                        It.IsAny<IMaterialDefinition>()))
                    .ReturnsAsync(material);
                return this;
            }

            public Fixture FindOfAssetDefinitionShouldBeCalledWith(string materialGroup)
            {
                _verifications.Add(() => _mockAssetDefinitionRepository.Verify(m => m.Find(materialGroup), Times.Once));
                return this;
            }

            public Fixture FindOfAssetDefinitionShouldReturn(AssetDefinition assetDefinition)
            {
                _mockAssetDefinitionRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(assetDefinition);
                return this;
            }

            public Fixture FindOfMaterialDefinitionRepoThrows(Exception exception)
            {
                _mockMaterialDefinitionRepository.Setup(m => m.Find(It.IsAny<string>())).ThrowsAsync(exception);
                return this;
            }

            public Fixture FindOfMaterialDefinitionRepoReturnMaterialWhenPassed(string materialGroup,
                IMaterialDefinition materialDefinition)
            {
                _mockMaterialDefinitionRepository.Setup(m => m.Find(materialGroup)).ReturnsAsync(materialDefinition);
                return this;
            }

            public Fixture HavingFilterCriteriaBuilderAccepting(List<FilterData> filterData, string group,
                List<string> searchKeywords)
            {
                _verifications.Add(
                    () =>
                        _mockFilterBuilder.Verify(
                            f => f.Build(_mockMaterialDefinition.Object, _mockBrandDefinition.Object, filterData, group, searchKeywords)));
                return this;
            }

            public Fixture HavingMaterialRepositorySearchAcceptingFilterCriteria(Dictionary<string, Tuple<string, object>> filterCriteria)
            {
                _verifications.Add(
                    () =>
                        _mockMaterialRepository.Verify(
                            m =>
                                m.Search(filterCriteria, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                                    It.IsAny<SortOrder>())));
                return this;
            }

            public Fixture HavingMaterialRepositoryGetAllRatesAcceptingFilterCriteria(List<FilterData> filterCriteria)
            {
                _verifications.Add(
                    () =>
                        _mockMaterialRepository.Verify(
                            m =>
                                m.GetAllRates(filterCriteria)));
                return this;
            }

            public Fixture HavingRepositoryAcceptingfindWith(string group)
            {
                _verifications.Add(() => _mockMaterialDefinitionRepository.Verify(m => m.Find(group), Times.Once));
                return this;
            }

            public MaterialService SystemUnderTest()
            {
                return new MaterialService(_mockMaterialDefinitionRepository.Object, _mockFilterBuilder.Object,
                    _mockMaterialRepository.Object, _mockMaterialBuilder.Object, _mockAssetDefinitionRepository.Object, _mockBrandDefinitionRepository.Object);
            }

            public void VerifyExpectations()
            {
                foreach (var verification in _verifications)
                    verification.Invoke();
            }

            public Fixture WithFilterBuilderReturningCriteria(Dictionary<string, Tuple<string, object>> filterCriteria)
            {
                _mockFilterBuilder.Setup(
                        f => f.Build(It.IsAny<IMaterialDefinition>(), It.IsAny<IBrandDefinition>(), It.IsAny<List<FilterData>>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                    .Returns(filterCriteria);
                return this;
            }

            public Fixture WithFilterBuilderReturningRateCriteria(Dictionary<string, Tuple<string, object>> filterCriteria)
            {
                _mockFilterBuilder.Setup(
                        f => f.BuildRateFilters(It.IsAny<List<FilterData>>(), "material")).Returns(filterCriteria);
                return this;
            }

            public Fixture WithMaterialDefintionHavingGroup(string group)
            {
                _mockMaterialDefinitionRepository.Setup(m => m.Find(group)).ReturnsAsync(_mockMaterialDefinition.Object);
                return this;
            }

            public Fixture WithMaterialRepositoryreturning(List<IMaterial> materials)
            {
                _mockMaterialRepository.Setup(m => m.Search(It.IsAny<Dictionary<string, Tuple<string, object>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SortOrder>())).ReturnsAsync(materials);
                return this;
            }

            public Fixture WithBrandDefinition()
            {
                _mockBrandDefinitionRepository.Setup(b => b.FindBy(It.IsAny<string>()))
                    .ReturnsAsync(_mockBrandDefinition.Object);

                return this;
            }

            public Fixture WithExistingMaterial(string cly00001)
            {
                _mockMaterialRepository.Setup(m => m.Find(cly00001)).ReturnsAsync(new Material { Id = cly00001 });
                return this;
            }

            public Fixture GetByGroupAndColumnNameOfMaterialRepoReturnsListOfMaterials(List<IMaterial> materials)
            {
                _mockMaterialRepository.Setup(m => m.GetByGroupAndColumnName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(materials);
                return this;
            }

            public Fixture GetTotalCountByGroupAndColumnNameOfMaterialRepoReturns(long count)
            {
                _mockMaterialRepository.Setup(m => m.GetTotalCountByGroupAndColumnName(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(count);
                return this;
            }

            public Fixture ListComponentOfMaterialRepoReturns(List<IMaterial> materials)
            {
                _mockMaterialRepository.Setup(m => m.ListComponents(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(materials);
                return this;
            }

            public Fixture CountOfMaterialRepoReturns(int totalMaterialCount)
            {
                _mockMaterialRepository.Setup(m => m.Count(It.IsAny<List<string>>(), It.IsAny<string>()))
                    .ReturnsAsync(totalMaterialCount);
                return this;
            }

            public Fixture UpdateOfMaterialRepoShouldBeCalledWithMaterialHavingId(string materialid)
            {
                _verifications.Add(() => _mockMaterialRepository.Verify(m => m.Update(It.Is<IMaterial>(mat => mat.Id == materialid)), Times.Once));
                return this;
            }

            public Fixture FindOfMaterialRepoReturns(IMaterial material)
            {
                _mockMaterialRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(material);
                return this;
            }

            public Fixture SearchInGroupOfMaterialRepoReturns(List<string> materials)
            {
                _mockMaterialRepository.Setup(m => m.SearchInGroup(It.IsAny<List<string>>())).ReturnsAsync(materials);
                return this;
            }

            public Fixture GetAllRatesOfMaterialRepositoryReturns(List<MaterialRateSearchResult> materialRatesList)
            {
                _mockMaterialRepository.Setup(
                        m => m.GetAllRates(It.IsAny<List<FilterData>>()))
                    .ReturnsAsync(materialRatesList);

                return this;
            }
        }
    }
}