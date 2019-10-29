using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RepositoryPortTests
{
    public class MaterialRepositoryTests
    {
        [Fact]
        public async void Add_ShouldSave_WhenPassedMaterial()
        {
            var material = GetMaterial("CLY0001");
            var fixture = new Fixture().Accepting(material).WithEmptyCollection().WithMockCollectionReturningEmptyList(material);
            await fixture.SystemUnderTest().Add(material);
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowArgumentException_WhenCalledWithMaterialWithoutId()
        {
            var material = GetMaterial("CLY001");
            material.Id = null;
            var fixture = new Fixture().Accepting(material);
            Func<Task> act = async () => await fixture.SystemUnderTest().Add(material);
            act.ShouldThrow<ArgumentNullException>().WithMessage("Id is required.\r\nParameter name: material");
        }

        [Fact]
        public void Add_ShouldThrowDuplicateResourceException_WhenGivenMaterialIsAlreadyExist()
        {
            var material = GetMaterial("CLY0001");
            var fixture = new Fixture().Accepting(material).WithExistingMaterialWithMultipleFindAsyncCalls(material);
            Func<Task> action = async () => await fixture.SystemUnderTest().Add(material);
            action.ShouldThrow<DuplicateResourceException>().WithMessage("material codE: 1 is already exists");
        }

        [Fact]
        public async void Count_ShouldWhenKeywordsAndMaterial_ReturnMatchingRecordCount()
        {
            var material = GetMaterial("CLY0001");
            var fixture = new Fixture();
            var sut = fixture.WithExisting(material).ExpectingNumberOfFindAsyncCalls(1).SystemUnderTest();
            await sut.Count(new List<string> { "Clay", "Service" }, "Clay Service");

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Find_ShouldReturnMaterial_WhenCalledWithId()
        {
            const string id = "CLY0001";
            var material = GetMaterial(id);
            var sut = new Fixture().WithExisting(material).SystemUnderTest();
            (await sut.Find(id)).Id.Should().Be(id);
        }

        [Fact]
        public void Find_ShouldThrowResourceNotFoundException_WhenCalledWithNonExistingId()
        {
            const string id = "CLY001";
            var sut = new Fixture().WithEmptyCollection().SystemUnderTest();
            Func<Task> act = async () => await sut.Find(id);
            act.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void GetByGroupAndColumnName_ReturnRelevantMaterials()
        {
            var materials = new List<MaterialDao>
            {
                new MaterialDao(GetMaterial("CLY0001")),
                new MaterialDao(GetMaterial("CLY0002"))
            };
            var sut = new Fixture().NoAssetDefinition().WhichReturns(materials).SystemUnderTest();
            var expectedMaterials = await sut.GetByGroupAndColumnName("Clay", "Image");
            expectedMaterials.Should().NotBeNull();
            expectedMaterials.Should().HaveCount(2);
            expectedMaterials.First().Id.Should().Be("CLY0001");
            expectedMaterials.Last().Id.Should().Be("CLY0002");
        }

        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWords_ReturnRelevantMaterials()
        {
            var materials = new List<MaterialDao>
            {
                new MaterialDao(GetMaterial("CLY0001")),
                new MaterialDao(GetMaterial("CLY0002"))
            };
            var sut = new Fixture().NoAssetDefinition().WhichReturns(materials).SystemUnderTest();
            var expectedMaterials =
                await sut.GetByGroupAndColumnNameAndKeyWords("Clay", "Image", new List<string> { "CLY0001", "CLY0002" });
            expectedMaterials.Should().NotBeNull();
            expectedMaterials.Should().HaveCount(2);
            expectedMaterials.First().Id.Should().Be("CLY0001");
            expectedMaterials.Last().Id.Should().Be("CLY0002");
        }

        [Fact]
        public async void GetTotalCountByGroupAndColumnName_ReturnTotalCount()
        {
            var materials = new List<MaterialDao>
            {
                new MaterialDao(GetMaterial("CLY0001")),
                new MaterialDao(GetMaterial("CLY0002"))
            };
            var sut = new Fixture().NoAssetDefinition().WhichReturns(materials).SystemUnderTest();
            var totalCount = await sut.GetTotalCountByGroupAndColumnName("Clay", "Image");
            totalCount.ShouldBeEquivalentTo(2);
        }

        [Fact]
        public async void GetTotalCountByGroupAndColumnNameAndKeyWords_ReturnRelevantMaterials()
        {
            var materials = new List<MaterialDao>
            {
                new MaterialDao(GetMaterial("CLY0001")),
                new MaterialDao(GetMaterial("CLY0002"))
            };
            var sut = new Fixture().NoAssetDefinition().WhichReturns(materials).SystemUnderTest();
            var totalCount =
                await
                    sut.GetTotalCountByGroupAndColumnNameAndKeyWords("Clay", "Image",
                        new List<string> { "CLY0001", "CLY0002" });
            totalCount.ShouldBeEquivalentTo(2);
        }

        [Fact]
        public void It_ShouldOfTypeIMaterialRepository()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Should().BeAssignableTo<IMaterialRepository>();
        }

        [Fact]
        public void Search_ShouldApplyFiltersPassedOnMongoCollection()
        {
            var filterOptions = new Dictionary<string, Tuple<string, object>> { { "key", new Tuple<string, object>("Regex", "value") } };
            var fixture = new Fixture().ExpectingFilterOptions(filterOptions);
            var sut = fixture.SystemUnderTest();
            sut.Search(filterOptions, 1, 10);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Search_ShouldForAKeywordWithASpace_WillSearchForIndividualWords()
        {
            const string id = "CLY0001";
            var material = GetAsset(id);
            var fixture = new Fixture().WithExisting(material).ExpectingNumberOfFindAsyncCalls(1);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Material" }, "Clay Material");

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Search_ShouldForASetOfKeywordsWithoutMaterialLevel_ReturnsRecords()
        {
            const string id = "CLY0001";
            var material = GetMaterial(id);
            var fixture = new Fixture().WithExisting(material).ExpectingNumberOfFindAsyncCalls(1);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Material" });

            fixture.VerifyExpectations();
        }

        public async void Search_ShouldReturnRecordsUsingBatchInfo_WhenBatchAndPageIsSpecified()
        {
            const string id = "CLY0001";
            var service = GetMaterial(id);
            var options = new FindOptions<MaterialDao> { Limit = 50, Skip = 50 };

            var fixture = new Fixture().WithExisting(service).WithFindOptions(options);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" }, 2, 50);

            fixture.VerifyExpectations();
        }

        [Fact]
        public void Search_ShouldUseAListOfInvalidSearchKeywords_ReturnNoMaterial()
        {
            const string id = "CLY0001";

            var sut = new Fixture().WithEmptyCollection().SystemUnderTest();
            Func<Task> action = async () => await sut.Search(new List<string> { "Clay" }, "Clay Material");
            action.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void Search_ShouldUseAListOfValidSearchKeywords_ReturnRelevantMaterial()
        {
            const string id = "CLY0001";
            var material = GetAsset(id);

            var sut = new Fixture().WithExisting(material).SystemUnderTest();
            var expectedMaterial = await sut.Search(new List<string> { "Clay" }, "Clay Material");
            expectedMaterial.Count.Should().Be(1);
            expectedMaterial[0].Id.Should().Be(id);
        }

        [Fact]
        public async void Search_ShouldUseDefaultSortCriteria_SortCriteriaIsNotSpecified()
        {
            var materials = new List<MaterialDao>
            {
                new MaterialDao(GetAsset("CLY0001")),
                new MaterialDao(GetAsset("CLY0002"))
            };

            var sut =
                new Fixture().WhichOnSortingReturns("material_name", SortOrder.Descending, materials).SystemUnderTest();
            var expectedMaterials = await sut.Search(new List<string> { "Clay" }, "Clay Material");
            expectedMaterials.Should().NotBeNull();
            expectedMaterials.Should().HaveCount(2);
            expectedMaterials.First().Id.Should().Be("CLY0001");
            expectedMaterials.Last().Id.Should().Be("CLY0002");
        }

        [Fact]
        public async void Search_ShouldUseSortCriteria_ReturnRelevantMaterial()
        {
            var materials = new List<MaterialDao>
            {
                new MaterialDao(GetAsset("CLY0001")),
                new MaterialDao(GetAsset("CLY0002"))
            };

            var sut =
                new Fixture().WithAssetDefinition()
                    .WhichOnSortingReturns("material name", SortOrder.Ascending, materials)
                    .SystemUnderTest();
            var expectedMaterials = await sut.Search(new List<string> { "Clay" }, "Clay Material",
                sortCriteria: "Material Name", sortOrder: SortOrder.Ascending);
            expectedMaterials.Should().NotBeNull();
            expectedMaterials.Should().HaveCount(2);
            expectedMaterials.First().Headers.Should().HaveCount(3);
            expectedMaterials.First().Id.Should().Be("CLY0001");
            expectedMaterials.Last().Id.Should().Be("CLY0002");
        }

        [Fact]
        public async void Search_ShouldWhenBatchAndPageIsSpecifiedWithMaterialLevel_ReturnRecordsUsingBatchInfo()
        {
            const string id = "CLY0001";
            var service = GetAsset(id);
            var options = new FindOptions<MaterialDao> { Limit = 50, Skip = 50 };

            var fixture = new Fixture().WithExisting(service).WithFindOptions(options);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" }, "Clay Service", 2, 50);

            fixture.VerifyExpectations();
        }

        [Fact]
        public void Update_ShouldThrowResourceNotFoundException_WhenCalledWithNonExistingMaterial()
        {
            const string id = "CLY0001";
            var material = GetMaterial(id);
            var sut = new Fixture().WithEmptyCollection();
            Func<Task> act = async () => await sut.SystemUnderTest().Update(material);
            act.ShouldThrow<ResourceNotFoundException>();
        }

        private static Material GetAsset(string id)
        {
            var headerData = new List<IHeaderData>
            {
                new HeaderData("Classification","Classification")
                {
                    Columns = new List<IColumnData> {new ColumnData("Material Level 2", "material_level_2", "Clay")}
                },
                new HeaderData("General","General")
                {
                    Columns = new List<IColumnData>
                    {
                        new ColumnData("Can be Used as an Asset","can_be_used_as_an_asset", true),
                        new ColumnData("Material Code","material_code", id)
                    }
                },
                new HeaderData("System Logs","System Logs")
                {
                    Columns = new List<ColumnData>
                    {
                        new ColumnData("Date Created","date_created", DateTime.Parse("12-12-2004")),
                        new ColumnData("Date Last Amended","date_last_amended", null),
                        new ColumnData("Created By", "created_by", "TE TEST"),
                        new ColumnData("Last Amended By","last_amended_by",  null)
                    }
                }
            };
            var material = new Material(headerData, new MaterialDefinition("Clay Material"))
            {
                Id = id,
                Group = "Clay"
            };
            material.AppendSearchKeywords(new List<string> { "Clay" });
            return material;
        }

        private static Material GetMaterial(string id)
        {
            var headerData = new List<IHeaderData>
            {
                new HeaderData("Classification","Classification")
                {
                    Columns = new List<IColumnData> {new ColumnData("Material Level 2", "material_level_2", "Clay")}
                },
                new HeaderData("General","General")
                {
                    Columns = new List<IColumnData>
                    {
                        new ColumnData("Can be Used as an Asset","can_be_used_as_an_asset", false),
                        new ColumnData("Material Code","material_code", id)
                    }
                },
                new HeaderData("Purchase", "purchase")
                {
                    Columns =  new List<IColumnData>
                    {
                        new ColumnData("Approved Brands", "approved_brands", new List<object>
                        {
                           new Brand{Columns = new List<IColumnData>{new ColumnData("Brand Code", "brand_code", "BAC0001")}}
                        }.ToArray())
                    }
                },
                new HeaderData("System Logs","System Logs")
                {
                    Columns = new List<ColumnData>
                    {
                        new ColumnData("Date Created","date_created", DateTime.Parse("12-12-2004")),
                        new ColumnData("Date Last Amended","date_last_amended", null),
                        new ColumnData("Created By","created_by", "TE TEST"),
                        new ColumnData("Last Amended By","last_amended_by", null)
                    }
                }
            };
            var material = new Material(headerData, new MaterialDefinition("Clay Material"))
            {
                Id = id,
                Group = "Clay"
            };
            material.AppendSearchKeywords(new List<string> { "Clay" });
            return material;
        }

        private class Fixture
        {
            private readonly Mock<IComponentDefinitionRepository<AssetDefinition>> _mockAssetDefinitionRepository
                = new Mock<IComponentDefinitionRepository<AssetDefinition>>();

            private readonly Mock<IMongoCollection<MaterialDao>> _mockCollection =
                new Mock<IMongoCollection<MaterialDao>>();

            private readonly Mock<IComponentDefinitionRepository<IMaterialDefinition>> _mockMaterialDefinitionRepository
                =
                new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();

            private readonly Mock<IBrandDefinitionRepository> _mockBrandDefinitionRepository =
                new Mock<IBrandDefinitionRepository>();

            private readonly List<Action> _verifications = new List<Action>();
            private readonly Mock<IFilterCriteriaBuilder> _mockFilterCriteriaBuilder = new Mock<IFilterCriteriaBuilder>();

            public Fixture Accepting(Material material)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.InsertOneAsync(
                                    It.Is<MaterialDao>(
                                        inserted => (string)inserted.Columns[MaterialDao.MaterialCode] == material.Id
                                                    &&
                                                    Within((DateTime)inserted.Columns[ComponentDao.DateCreated],
                                                        TimeSpan.FromMinutes(1))
                                                    &&
                                                    Within((DateTime)inserted.Columns[ComponentDao.DateLastAmended],
                                                        TimeSpan.FromMinutes(1))
                                                    && (string)inserted.Columns[ComponentDao.CreatedBy] == "TE"
                                                    && (string)inserted.Columns[ComponentDao.LastAmendedBy] == "TE"),
                                    It.IsAny<InsertOneOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));
                return this;
            }

            public Fixture ExpectingFilterOptions(Dictionary<string, Tuple<string, object>> filterOptions)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.FindAsync(It.Is<FilterDefinition<MaterialDao>>(f => CheckFilterOptionsEquality(f, filterOptions)),
                                    It.IsAny<FindOptions<MaterialDao>>(),
                                    default(CancellationToken)), Times.Exactly(1)));
                return this;
            }

            public Fixture ExpectingNumberOfFindAsyncCalls(int numberOfInvocations)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.FindAsync(It.IsAny<FilterDefinition<MaterialDao>>(),
                                    It.IsAny<FindOptions<MaterialDao>>(),
                                    default(CancellationToken)), Times.Exactly(numberOfInvocations)));
                return this;
            }

            public Fixture NoAssetDefinition()
            {
                _mockAssetDefinitionRepository.Setup(c => c.Find(It.IsAny<string>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));
                return this;
            }

            public IMaterialRepository SystemUnderTest()
            {
                var bank = new Mock<IBank>();
                return new MaterialRepository(_mockCollection.Object,
                    _mockMaterialDefinitionRepository.Object,
                    _mockAssetDefinitionRepository.Object,
                    _mockBrandDefinitionRepository.Object,
                    bank.Object,
                    _mockFilterCriteriaBuilder.Object);
            }

            public Fixture Updating(IMaterial material)
            {
                _verifications.Add(
                    () => _mockCollection.Verify(m => m.ReplaceOneAsync(It.IsAny<FilterDefinition<MaterialDao>>(),
                        It.Is<MaterialDao>(updated =>
                            Within((DateTime)updated.Columns[ComponentDao.DateLastAmended], TimeSpan.FromMinutes(1))
                            && (string)updated.Columns[ComponentDao.LastAmendedBy] == "TE"
                            && (DateTime)updated.Columns[ComponentDao.DateCreated] == material.CreatedAt
                            && (string)updated.Columns[ComponentDao.CreatedBy] == material.CreatedBy
                        ),
                        null, default(CancellationToken))));
                return this;
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(action => action.Invoke());
            }

            public Fixture WhichOnSortingReturns(string sortField, SortOrder sortOrder, List<MaterialDao> materials)
            {
                _mockMaterialDefinitionRepository.Setup(
                        m => m.Find((string)materials.First().Columns[ComponentDao.MaterialLevel2]))
                    .ReturnsAsync(GetDefinition());
                var builder = Builders<MaterialDao>.Sort;
                var definition = default(SortDefinition<MaterialDao>);
                if (sortOrder == SortOrder.Descending)
                    definition = builder.Descending(new StringFieldDefinition<MaterialDao>(sortField));
                else if (sortOrder == SortOrder.Ascending)
                    definition = builder.Ascending(new StringFieldDefinition<MaterialDao>(sortField));

                var mockCursor = new Mock<IAsyncCursor<MaterialDao>>();
                var enumerator = materials.GetEnumerator();
                mockCursor.Setup(m => m.Current).Returns(() => new List<MaterialDao> { enumerator.Current });
                mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
                _mockCollection.Setup(
                    m =>
                        m.FindAsync(It.IsAny<FilterDefinition<MaterialDao>>(),
                            It.Is((FindOptions<MaterialDao, MaterialDao> f) => AreEqualFindOptions(f, definition)),
                            default(CancellationToken))).ReturnsAsync(mockCursor.Object);
                return this;
            }

            public Fixture WhichReturns(List<MaterialDao> materials)
            {
                _mockMaterialDefinitionRepository.Setup(
                        m => m.Find((string)materials.First().Columns[ComponentDao.MaterialLevel2]))
                    .ReturnsAsync(GetDefinition());
                var definition = default(SortDefinition<MaterialDao>);

                var mockCursor = new Mock<IAsyncCursor<MaterialDao>>();
                var enumerator = materials.GetEnumerator();
                mockCursor.Setup(m => m.Current).Returns(() => new List<MaterialDao> { enumerator.Current });
                mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
                _mockCollection.Setup(
                        m =>
                            m.FindAsync(It.IsAny<FilterDefinition<MaterialDao>>(),
                                It.Is((FindOptions<MaterialDao, MaterialDao> f) => true), default(CancellationToken)))
                    .ReturnsAsync(mockCursor.Object);

                _mockCollection.Setup(
                    m =>
                        m.CountAsync(It.IsAny<FilterDefinition<MaterialDao>>(),
                            It.IsAny<CountOptions>(), default(CancellationToken))).ReturnsAsync(materials.Count);
                return this;
            }

            public Fixture WithAssetDefinition()
            {
                _mockAssetDefinitionRepository.Setup(c => c.Find(It.IsAny<string>()))
                    .ReturnsAsync(GetAssetDefinition());
                return this;
            }

            public Fixture WithExisting(Material material)
            {
                _mockMaterialDefinitionRepository.Setup(m => m.Find(material.Group)).ReturnsAsync(GetDefinition());
                TestHelper.MockCollectionWithExisting(_mockCollection, new MaterialDao(material));
                return this;
            }

            public Fixture WithExistingDaos(List<MaterialDao> materialDao)
            {
                TestHelper.MockCollectionWithExistingList(_mockCollection, materialDao);
                return this;
            }

            public Fixture WithExistingMaterialWithMultipleFindAsyncCalls(Material material)
            {
                _mockMaterialDefinitionRepository.Setup(m => m.Find(material.Group)).ReturnsAsync(GetDefinition());
                TestHelper.MockCollectionWithMultipleResponseForFind(_mockCollection, new MaterialDao(material), new List<MaterialDao> { new MaterialDao(material) });
                return this;
            }

            public Fixture WithFindOptions(FindOptions<MaterialDao> options)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.FindAsync(It.IsAny<FilterDefinition<MaterialDao>>(),
                                    It.Is<FindOptions<MaterialDao>>(
                                        f => f.Limit == options.Limit && f.Skip == options.Skip),
                                    default(CancellationToken))));
                return this;
            }

            public Fixture WithEmptyCollection()
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                return this;
            }

            private static string GetDirection(SortDefinition<MaterialDao> definition)
            {
                return GetPrivateFieldValue(definition, "_direction").ToString();
            }

            private static object GetPrivateFieldValue(object definition, string fieldName)
            {
                return
                    definition.GetType()
                        .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(definition);
            }

            private static bool Within(DateTime dateTime, TimeSpan timeSpan)
            {
                var difference = DateTime.UtcNow - dateTime;
                return difference < timeSpan;
            }

            private bool AreEqualFindOptions(FindOptions<MaterialDao, MaterialDao> findOptions,
                SortDefinition<MaterialDao> definition)
            {
                var areDirectionsEqual = GetDirection(definition) == GetDirection(findOptions.Sort);
                var areSortFieldsEqual = GetSortField(definition) == GetSortField(findOptions.Sort);
                return areDirectionsEqual && areSortFieldsEqual;
            }

            private bool CheckFilterOptionsEquality(FilterDefinition<MaterialDao> filterDefinition, Dictionary<string, Tuple<string, object>> filterOptions)
            {
                var equal = false;
                var givenFilterOption = filterOptions.FirstOrDefault();
                var filterDefinitions = (List<FilterDefinition<MaterialDao>>)GetPrivateFieldValue(filterDefinition, "_filters");
                equal =
                    filterDefinitions.Count() ==
                    1;

                var filterOption = filterDefinitions.FirstOrDefault();
                if (filterOption == null)
                {
                    return false;
                }
                var field = GetPrivateFieldValue(filterOption, "_field");

                if (field == null)
                {
                    return false;
                }
                var fieldName = GetPrivateFieldValue(field, "_fieldName");
                if (fieldName == null)
                {
                    return false;
                }
                equal = equal && fieldName.ToString() == givenFilterOption.Key;

                var value = ((MongoDB.Bson.BsonRegularExpression)GetPrivateFieldValue(filterOption, "_value"));
                if (value == null)
                {
                    return false;
                }

                equal = equal && value.Pattern == givenFilterOption.Value.Item2;
                return equal;
            }

            private AssetDefinition GetAssetDefinition()
            {
                return new AssetDefinition("Clay")
                {
                    Headers = new List<IHeaderDefinition>
                    {
                        new HeaderDefinition("Maintenance","Maintenance", new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Expected Life","Expected Life", new ConstantDataType("1 day"))
                        })
                    }
                };
            }

            private IMaterialDefinition GetDefinition()
            {
                return new MaterialDefinition("Clay")
                {
                    Headers = new List<IHeaderDefinition>
                    {
                        new HeaderDefinition("Classification","Classification", new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Material Level 2","material_level_2",  new ConstantDataType("Clay"))
                        }),
                        new HeaderDefinition("General", "General",new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Can be Used as an Asset","can_be_used_as_an_asset",  new BooleanDataType()),
                            new ColumnDefinition("Material Code", "material_code", new StringDataType())
                        }),
                        new HeaderDefinition("System Logs", "System Logs",new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Date Created","date_created",  new StringDataType()),
                            new ColumnDefinition("Date Last Amended", "date_last_amended",new StringDataType()),
                            new ColumnDefinition("Created By","created_by", new StringDataType()),
                            new ColumnDefinition("Last Amended By","last_amended_by", new StringDataType())
                        })
                    }
                };
            }

            private string GetSortField(SortDefinition<MaterialDao> definition)
            {
                var field = GetPrivateFieldValue(definition, "_field");
                return GetPrivateFieldValue(field, "_fieldName").ToString();
            }

            public Fixture WithMockCollectionReturningEmptyList(Material material)
            {
                TestHelper.MockCollectionWithExistingAndFindReturnEmptyList(_mockCollection, new MaterialDao(material));
                return this;
            }
        }
    }
}