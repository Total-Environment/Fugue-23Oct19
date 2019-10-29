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
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure.Repository
{
    public class ServiceRepositoryTests
    {
        [Fact]
        public async void Add_ShouldSave_WhenPassedService()
        {
            var service = GetService("FDP0001");
            var fixture = new Fixture().Accepting(service).WithoutExistingId(service.Id); ;
            await fixture.SystemUnderTest().Add(service);
            fixture.VerifyExpectations();
        }

        [Fact]
        public void Add_ShouldThrowArgumentException_WhenCalledWithServiceWithoutId()
        {
            var service = GetService("CLY001");
            service.Id = null;
            var fixture = new Fixture().Accepting(service);
            Func<Task> act = async () => await fixture.SystemUnderTest().Add(service);
            act.ShouldThrow<ArgumentNullException>().WithMessage("Id is required.\r\nParameter name: service");
        }

        [Fact]
        public void Add_ShouldThrowInvalidOperationException_WhenGivenServiceIsAlreadyExist()
        {
            var service = GetService("FDP0001");
            var fixture = new Fixture().Accepting(service).WithExisting(service);
            Func<Task> action = async () => await fixture.SystemUnderTest().Add(service);
            action.ShouldThrow<InvalidOperationException>().WithMessage("service code: 1 is already exists");
        }

        [Fact]
        public async void Count_ShouldWhenKeywordsAndServiceLevel_ReturnMatchingRecordCount()
        {
            var service = GetService("CLY0001");
            var fixture = new Fixture();
            var sut = fixture.WithExisting(service).ExpectingNumberOfFindAsyncCalls(1).SystemUnderTest();
            await sut.Count(new List<string> { "Clay", "Service" }, "Clay Service");

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Find_ShouldReturnService_WhenCalledWithId()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var sut = new Fixture().WithExisting(service).SystemUnderTest();
            (await sut.Find(id)).Id.Should().Be(id);
        }

        [Fact]
        public void Find_ShouldThrowResourceNotFoundException_WhenCalledWithNonExistingId()
        {
            const string id = "CLY001";
            var sut = new Fixture().WithoutExistingId(id).SystemUnderTest();
            Func<Task> act = async () => await sut.Find(id);
            act.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void GetByGroupAndColumnName_ReturnRelevantServices()
        {
            var services = new List<ServiceDao>
            {
                new ServiceDao(GetService("CLY0001")),
                new ServiceDao(GetService("CLY0002"))
            };
            var sut = new Fixture().WhichReturns(services).SystemUnderTest();
            var expectedServices = await sut.GetByGroupAndColumnName("Clay", "Image");
            expectedServices.Should().NotBeNull();
            expectedServices.Should().HaveCount(2);
            expectedServices.First().Id.Should().Be("CLY0001");
            expectedServices.Last().Id.Should().Be("CLY0002");
        }

        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWords_ReturnRelevantServices()
        {
            var services = new List<ServiceDao>
            {
                new ServiceDao(GetService("CLY0001")),
                new ServiceDao(GetService("CLY0002"))
            };
            var sut = new Fixture().WhichReturns(services).SystemUnderTest();
            var expectedServices =
                await sut.GetByGroupAndColumnNameAndKeyWords("Clay", "Image", new List<string> { "CLY0001", "CLY0002" });
            expectedServices.Should().NotBeNull();
            expectedServices.Should().HaveCount(2);
            expectedServices.First().Id.Should().Be("CLY0001");
            expectedServices.Last().Id.Should().Be("CLY0002");
        }

        [Fact]
        public async void GetTotalCountByGroupAndColumnName_ReturnRelevantServices()
        {
            var services = new List<ServiceDao>
            {
                new ServiceDao(GetService("CLY0001")),
                new ServiceDao(GetService("CLY0002"))
            };
            var sut = new Fixture().WhichReturns(services).SystemUnderTest();
            var totalCount = await sut.GetTotalCountByGroupAndColumnName("Clay", "Image");
            totalCount.ShouldBeEquivalentTo(2);
        }

        [Fact]
        public async void GetTotalCountByGroupAndColumnNameAndKeyWords_ReturnRelevantServices()
        {
            var services = new List<ServiceDao>
            {
                new ServiceDao(GetService("CLY0001")),
                new ServiceDao(GetService("CLY0002"))
            };
            var sut = new Fixture().WhichReturns(services).SystemUnderTest();
            var totalCount =
                await
                    sut.GetTotalCountByGroupAndColumnNameAndKeyWords("Clay", "Image",
                        new List<string> { "CLY0001", "CLY0002" });
            totalCount.ShouldBeEquivalentTo(2);
        }

        [Fact]
        public void It_ShouldOfTypeIServiceRepository()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Should().BeAssignableTo<IServiceRepository>();
        }

        [Fact]
        public async void Search_ShouldForAKeywordWithASpace_WillSearchForIndividualWords()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var fixture = new Fixture().WithExisting(service).ExpectingNumberOfFindAsyncCalls(1);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" }, "Clay Service");

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Search_ShouldForASetOfKeywordsWithoutServiceLevel_ReturnRecords()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var fixture = new Fixture().WithExisting(service).ExpectingNumberOfFindAsyncCalls(1);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" });

            fixture.VerifyExpectations();
        }

        [Fact]
        public void Search_ShouldUseAListOfInvalidSearchKeywords_ReturnNoService()
        {
            const string id = "CLY0001";

            var sut = new Fixture().WithoutExistingId(id).SystemUnderTest();
            Func<Task> action = async () => await sut.Search(new List<string> { "Clay" }, "Clay Service");
            action.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void Search_ShouldUseAListOfValidSearchKeywords_ReturnRelevantService()
        {
            const string id = "CLY0001";
            var service = GetService(id);

            var sut = new Fixture().WithExisting(service).SystemUnderTest();
            var expectedService = await sut.Search(new List<string> { "Clay" }, "Clay Service");
            expectedService.Count.Should().Be(1);
            expectedService[0].Id.Should().Be(id);
        }

        [Fact]
        public async void Search_ShouldWhenBatchAndPageIsSpecified_ReturnRecordsUsingBatchInfo()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var options = new FindOptions<ServiceDao> { Limit = 50, Skip = 50 };

            var fixture = new Fixture().WithExisting(service).WithFindOptions(options);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" }, 2, 50);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Search_ShouldWhenBatchAndPageIsSpecifiedWithServiceLevel_ReturnRecordsUsingBatchInfo()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var options = new FindOptions<ServiceDao> { Limit = 50, Skip = 50 };

            var fixture = new Fixture().WithExisting(service).WithFindOptions(options);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" }, "Clay Service", 2, 50);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Update_ShouldPreserveCreatedByCreatedAt_WhenCalledUpdatingAService()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            service.CreatedAt = DateTime.Now - TimeSpan.FromDays(1);
            service.CreatedBy = "TE";

            var serviceWithOlderCreatedAtAndCreateBy = GetService(id);
            serviceWithOlderCreatedAtAndCreateBy.CreatedAt = DateTime.Now - TimeSpan.FromDays(5);
            serviceWithOlderCreatedAtAndCreateBy.CreatedBy = "Sundeep";

            var expectedService = GetService(id);
            expectedService.CreatedAt = DateTime.Now - TimeSpan.FromDays(5);
            expectedService.CreatedBy = "Sundeep";
            expectedService.AmendedAt = DateTime.Now;
            expectedService.AmendedBy = "TE";

            var sut = new Fixture().WithExisting(expectedService).Updating(expectedService);
            await sut.SystemUnderTest().Update(service);
            sut.VerifyExpectations();
        }

        [Fact]
        public void Update_ShouldThrowResourceNotFoundException_WhenCalledWithNonExistingService()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var sut = new Fixture().WithoutExistingId(id);
            Func<Task> act = async () => await sut.SystemUnderTest().Update(service);
            act.ShouldThrow<ResourceNotFoundException>();
        }

        [Fact]
        public async void Update_ShouldUpdate_WhenCalledWithExistingService()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            service.CreatedAt = DateTime.Now - TimeSpan.FromDays(1);
            service.CreatedBy = "TE";
            var sut = new Fixture().WithExisting(service).Updating(service);
            await sut.SystemUnderTest().Update(service);
            sut.VerifyExpectations();
        }

        private static Service GetService(string id)
        {
            var headerData = new List<IHeaderData>
            {
                new HeaderData("Classification", "classfication")
                {
                    Columns = new List<IColumnData> {new ColumnData("Service Level 1", "service_level_1", "Clay")}
                },
                new HeaderData("General", "general")
                {
                    Columns = new List<IColumnData>
                    {
                        new ColumnData("Service Code", "service_code", id)
                    }
                },
                new HeaderData("System Logs", "System Logs")

                {
                    Columns = new List<ColumnData>
                    {
                        new ColumnData("Date Created", "date_created", DateTime.Parse("12-12-2004")),
                        new ColumnData("Date Last Amended", "date_last_amended", null),
                        new ColumnData("Created By", "created_by", "TE TEST"),
                        new ColumnData("Last Amended By", "last_amended_by", null)
                    }
                }
            };
            var service = new Service(headerData, new ServiceDefinition("Clay Service"))
            {
                Id = id
            };
            service.AppendSearchKeywords(new List<string> { "Clay" });
            return service;
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
        public async void Search_ShouldForASetOfKeywordsWithoutServiceLevel_ReturnsRecords()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var fixture = new Fixture().WithExisting(service).ExpectingNumberOfFindAsyncCalls(1);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" });

            fixture.VerifyExpectations();
        }

        public async void Search_ShouldReturnRecordsUsingBatchInfo_WhenBatchAndPageIsSpecified()
        {
            const string id = "CLY0001";
            var service = GetService(id);
            var options = new FindOptions<ServiceDao> { Limit = 50, Skip = 50 };

            var fixture = new Fixture().WithExisting(service).WithFindOptions(options);
            var sut = fixture.SystemUnderTest();
            await sut.Search(new List<string> { "Clay", "Service" }, 2, 50);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Search_ShouldUseDefaultSortCriteria_SortCriteriaIsNotSpecified()
        {
            var services = new List<ServiceDao>
            {
                new ServiceDao(GetService("CLY0001")),
                new ServiceDao(GetService("CLY0002"))
            };

            var sut =
                new Fixture().WhichOnSortingReturns("service_code", SortOrder.Descending, services).SystemUnderTest();
            var expectedServices = await sut.Search(new List<string> { "Clay" }, "Clay Service");
            expectedServices.Should().NotBeNull();
            expectedServices.Should().HaveCount(2);
            expectedServices.First().Id.Should().Be("CLY0001");
            expectedServices.Last().Id.Should().Be("CLY0002");
        }

        private class Fixture
        {
            private readonly Mock<IMongoCollection<ServiceDao>> _mockCollection =
                new Mock<IMongoCollection<ServiceDao>>();

            private readonly Mock<IComponentDefinitionRepository<IServiceDefinition>> _mockServiceDefinitionRepository =
                new Mock<IComponentDefinitionRepository<IServiceDefinition>>();

            private readonly List<Action> _verifications = new List<Action>();
            private readonly Mock<IFilterCriteriaBuilder> _mockFilterCriteriaBuilder = new Mock<IFilterCriteriaBuilder>();

            public Fixture Accepting(Service service)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.InsertOneAsync(
                                    It.Is<ServiceDao>(inserted => (string)inserted.Columns[ServiceDao.ServiceCode] == service.Id
                                                    && Within((DateTime)inserted.Columns[ComponentDao.DateCreated],
                                                                           TimeSpan.FromMinutes(1))
                                                                       && Within((DateTime)inserted.Columns[ComponentDao.DateLastAmended],
                                                                           TimeSpan.FromMinutes(1))
                                                                           && (string)inserted.Columns[ComponentDao.CreatedBy] == "TE"
                                                    && (string)inserted.Columns[ComponentDao.LastAmendedBy] == "TE"),
                                    It.IsAny<InsertOneOptions>(),
                                    It.IsAny<CancellationToken>()), Times.Once()));
                return this;
            }

            public Fixture ExpectingNumberOfFindAsyncCalls(int numberOfInvocations)
            {
                _verifications.Add(() => _mockCollection.Verify(m => m.FindAsync(It.IsAny<FilterDefinition<ServiceDao>>(), It.IsAny<FindOptions<ServiceDao>>(),
                   default(CancellationToken)), Times.Exactly(numberOfInvocations)));
                return this;
            }

            public IServiceRepository SystemUnderTest()
            {
                var bank = new Mock<IBank>();
                return new ServiceRepository(_mockCollection.Object,
                    _mockServiceDefinitionRepository.Object,
                    bank.Object, _mockFilterCriteriaBuilder.Object);
            }

            public Fixture Updating(Service service)
            {
                _verifications.Add(
                    () => _mockCollection.Verify(m => m.ReplaceOneAsync(It.IsAny<FilterDefinition<ServiceDao>>(),
                        It.Is<ServiceDao>(updated => (string)updated.Columns[ServiceDao.ServiceCode] == service.Id
                                                    && Within((DateTime)updated.Columns[ComponentDao.DateLastAmended], TimeSpan.FromMinutes(1))
                                && (string)updated.Columns[ComponentDao.LastAmendedBy] == service.AmendedBy
                                && (DateTime)updated.Columns[ComponentDao.DateCreated] == service.CreatedAt
                            && (string)updated.Columns[ComponentDao.CreatedBy] == service.CreatedBy
                        ),
                        null, default(CancellationToken))));
                return this;
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(action => action.Invoke());
            }

            public Fixture WhichReturns(List<ServiceDao> services)
            {
                _mockServiceDefinitionRepository.Setup(m => m.Find((string)services.First().Columns[ComponentDao.ServiceLevel1])).ReturnsAsync(GetDefinition());
                var definition = default(SortDefinition<ServiceDao>);

                var mockCursor = new Mock<IAsyncCursor<ServiceDao>>();
                var enumerator = services.GetEnumerator();
                mockCursor.Setup(m => m.Current).Returns(() => new List<ServiceDao> { enumerator.Current });
                mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
                _mockCollection.Setup(
                    m =>
                        m.FindAsync(It.IsAny<FilterDefinition<ServiceDao>>(),
                            It.Is((FindOptions<ServiceDao, ServiceDao> f) => true), default(CancellationToken))).ReturnsAsync(mockCursor.Object);
                _mockCollection.Setup(
                   m =>
                       m.CountAsync(It.IsAny<FilterDefinition<ServiceDao>>(),
                           It.IsAny<CountOptions>(), default(CancellationToken))).ReturnsAsync(services.Count);

                return this;
            }

            public Fixture WithExisting(Service service)
            {
                _mockServiceDefinitionRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(GetDefinition());
                TestHelper.MockCollectionWithExisting(_mockCollection, new ServiceDao(service));
                return this;
            }

            public Fixture WithFindOptions(FindOptions<ServiceDao> options)
            {
                _verifications.Add(() => _mockCollection.Verify(m => m.FindAsync(It.IsAny<FilterDefinition<ServiceDao>>(), It.Is<FindOptions<ServiceDao>>(f => f.Limit == options.Limit && f.Skip == options.Skip),
                  default(CancellationToken))));
                return this;
            }

            public Fixture WithoutExistingId(string id)
            {
                TestHelper.MockCollectionWithExisting(_mockCollection, null);
                return this;
            }

            private static bool Within(DateTime dateTime, TimeSpan timeSpan)
            {
                var difference = DateTime.UtcNow - dateTime;
                return difference < timeSpan;
            }

            private IServiceDefinition GetDefinition()
            {
                return new ServiceDefinition("Clay")
                {
                    Headers = new List<IHeaderDefinition>
                    {
                        new HeaderDefinition("Classification", "Classification",new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Service Level 1", "service_level_1",new ConstantDataType("Clay"))
                        }),
                        new HeaderDefinition("General","General", new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Service Code","service_code",  new StringDataType())
                        }),
                        new HeaderDefinition("System Logs", "System Logs",new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Date Created","date_created",  new StringDataType()),
                            new ColumnDefinition("Date Last Amended", "date_last_amended",new StringDataType()),
                            new ColumnDefinition("Created By","created_by",  new StringDataType()),
                            new ColumnDefinition("Last Amended By","last_amended_by", new StringDataType())
                        })
                    }
                };
            }

            public Fixture ExpectingFilterOptions(Dictionary<string, Tuple<string, object>> filterOptions)
            {
                _verifications.Add(
                    () =>
                        _mockCollection.Verify(
                            m =>
                                m.FindAsync(It.Is<FilterDefinition<ServiceDao>>(f => CheckFilterOptionsEquality(f, filterOptions)),
                                    It.IsAny<FindOptions<ServiceDao>>(),
                                    default(CancellationToken)), Times.Exactly(1)));
                return this;
            }

            private bool CheckFilterOptionsEquality(FilterDefinition<ServiceDao> filterDefinition, Dictionary<string, Tuple<string, object>> filterOptions)
            {
                var equal = false;
                var givenFilterOption = filterOptions.FirstOrDefault();
                var filterDefinitions = (List<FilterDefinition<ServiceDao>>)GetPrivateFieldValue(filterDefinition, "_filters");
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

            private static object GetPrivateFieldValue(object definition, string fieldName)
            {
                return
                    definition.GetType()
                        .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(definition);
            }

            public Fixture WhichOnSortingReturns(string sortField, SortOrder sortOrder, List<ServiceDao> services)
            {
                _mockServiceDefinitionRepository.Setup(
                        m => m.Find((string)services.First().Columns[ComponentDao.ServiceLevel1]))
                    .ReturnsAsync(GetDefinition());
                var builder = Builders<ServiceDao>.Sort;
                var definition = default(SortDefinition<ServiceDao>);
                if (sortOrder == SortOrder.Descending)
                    definition = builder.Descending(new StringFieldDefinition<ServiceDao>(sortField));
                else if (sortOrder == SortOrder.Ascending)
                    definition = builder.Ascending(new StringFieldDefinition<ServiceDao>(sortField));

                var mockCursor = new Mock<IAsyncCursor<ServiceDao>>();
                var enumerator = services.GetEnumerator();
                mockCursor.Setup(m => m.Current).Returns(() => new List<ServiceDao> { enumerator.Current });
                mockCursor.Setup(m => m.MoveNext(default(CancellationToken))).Returns(() => enumerator.MoveNext());
                _mockCollection.Setup(
                    m =>
                        m.FindAsync(It.IsAny<FilterDefinition<ServiceDao>>(),
                            It.Is((FindOptions<ServiceDao, ServiceDao> f) => AreEqualFindOptions(f, definition)),
                            default(CancellationToken))).ReturnsAsync(mockCursor.Object);
                return this;
            }

            private bool AreEqualFindOptions(FindOptions<ServiceDao, ServiceDao> findOptions,
               SortDefinition<ServiceDao> definition)
            {
                var areDirectionsEqual = GetDirection(definition) == GetDirection(findOptions.Sort);
                var areSortFieldsEqual = GetSortField(definition) == GetSortField(findOptions.Sort);
                return areDirectionsEqual && areSortFieldsEqual;
            }

            private static string GetDirection(SortDefinition<ServiceDao> definition)
            {
                return GetPrivateFieldValue(definition, "_direction").ToString();
            }

            private string GetSortField(SortDefinition<ServiceDao> definition)
            {
                var field = GetPrivateFieldValue(definition, "_field");
                return GetPrivateFieldValue(field, "_fieldName").ToString();
            }
        }
    }
}