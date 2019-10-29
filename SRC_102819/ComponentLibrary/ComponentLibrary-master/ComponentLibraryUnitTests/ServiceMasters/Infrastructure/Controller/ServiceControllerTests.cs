using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure.Controller
{
    public class ServiceControllerTests
    {
        [Fact]
        public async void Get_ShouldReturn200OkWithService_WhenCalledWithExistingId()
        {
            const string id = "CLY00001";
            var sut = new Fixture().WithExisting(id).SystemUnderTest();
            var result = await sut.Get(id, false);
            result.Should().BeOfType<OkNegotiatedContentResult<ServiceDto>>();
            var parsedResult = (OkNegotiatedContentResult<ServiceDto>)result;
            parsedResult.Content.Id.Should().Be(id);
        }

        [Fact]
        public async void Get_ShouldReturn200OkWithServiceWithDataType_WhenCalledWithExistingId()
        {
            const string id = "CLY00001";
            var sut = new Fixture().WithExisting(id).SystemUnderTest();
            var result = await sut.Get(id, true);
            result.Should().BeOfType<OkNegotiatedContentResult<ServiceWithDataTypeDto>>();
            var parsedResult = (OkNegotiatedContentResult<ServiceWithDataTypeDto>)result;
            parsedResult.Content.Id.Should().Be(id);
        }

        [Fact]
        public async void Get_ShouldReturn404NotFound_WhenCalledWithNonExistingId()
        {
            const string id = "CLY00001";
            var sut = new Fixture().WithoutExisting(id).SystemUnderTest();
            var result = await sut.Get(id);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn200Ok()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("Primary");
            mockDefinition.Setup(m => m.Headers).Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition> { new ColumnDefinition("Image", "Image", new StaticFileDataType()) }) });
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            var services = new List<Service>
            {
                new Service(new List<IHeaderData> {new HeaderData("General", "General") {Columns = new List<IColumnData> {new ColumnData("Image", "Image", "Image")} } }, new ServiceDefinition("Clay"))
            };
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object).WithStubbedServicesByGroupAndColumnName(services);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Primary", "image", 1, 10);
            result.Should().BeAssignableTo<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var parsedResult = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<ServiceDocumentDto>)parsedResult.Content["items"];
            items.Should().HaveCount(1);
        }

        /// <summary>
        /// Gets the type of the by group and column name should return400 bad request when column
        /// name is not static or check data.
        /// </summary>
        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn400BadRequest_WhenColumnNameIsNotStaticOrCheckDataType()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("Primary");
            mockDefinition.Setup(m => m.Headers).Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition> { new ColumnDefinition("Image", "Image", new StringDataType()) }) });
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Primary", "image", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("image is neithter static file data type nor check list data type.");
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn400BadRequest_WhenColumnNameNotFound()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("Primary");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition> { }) });
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Primary", "image", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("image is not valid column in the service definition of Primary group.");
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn400BadRequest_WhenGroupNotFound()
        {
            var fixture = new Fixture().WithoutMockDefinition("Primary");
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Primary", "image", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("Primary not found.");
        }

        [Fact]
        public async void GetByGroupAndColumnKeyAndKeyWord_ShouldReturn200Ok()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("Primary");
            mockDefinition.Setup(m => m.Headers).Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition> { new ColumnDefinition("Image", "Image", new StaticFileDataType()) }) });
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            var services = new List<Service>
            {
                new Service(
                    new List<IHeaderData>
                    {
                        new HeaderData("General", "General") {Columns = new List<IColumnData> {new ColumnData("Image", "Image", "Image")}}
                    },
                    new ServiceDefinition("Primary")) {Id = "CLY000001"}
            };
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object).WithStubbedServicesByGroupAndColumnNameAndKeyWord(services);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnKeyAndKeyWord("Primary", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var parsedResult = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<ServiceDocumentDto>)parsedResult.Content["items"];
            items.Should().HaveCount(1);
        }

        [Fact]
        public async void GetByGroupAndColumnKeyAndKeyWord_ShouldReturn400BadRequest_WhenColumnNameIsNotStaticOrCheckDataType()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("Primary");
            mockDefinition.Setup(m => m.Headers).Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition> { new ColumnDefinition("Image", "Image", new StringDataType()) }) });
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnKeyAndKeyWord("Primary", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("image is neithter static file data type nor check list data type.");
        }

        [Fact]
        public async void GetByGroupAndColumnKeyAndKeyWord_ShouldReturn400BadRequest_WhenColumnNameNotFound()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("Primary");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition> { new ColumnDefinition("Method of Measurement", "method_of_measurement", new StringDataType()) }) });
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnKeyAndKeyWord("Primary", "Method of Measurement", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("method of measurement is not valid column in the service definition of Primary group.");
        }
        
        [Fact]
        public async void GetByGroupAndColumnKeyAndKeyWord_ShouldReturn400BadRequest_WhenGroupNotFound()
        {
            var fixture = new Fixture().WithoutMockDefinition("Primary");
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnKeyAndKeyWord("Primary", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("Primary not found.");
        }

        [Fact]
        public async void Post_SearchWithinGroup_ShouldThrowBadRequest_WhenkeywordIsNullOrLessThan3Letter()
        {
            var fixture = new Fixture();
            const string groupname = "groupName";
            const string nullKeyword = null;
            const string twoLetterKeyword = "ab";

            var searchWithinGroupRequest = new ServiceSearchRequest
            {
                GroupName = groupname,
                SearchQuery = nullKeyword
            };
            var nullKeywordResult = await fixture.SystemUnderTest().Post_SearchWithinGroup(searchWithinGroupRequest);
            var toLetterKeywordResult = await fixture.SystemUnderTest()
                .Post_SearchWithinGroup(searchWithinGroupRequest);

            nullKeywordResult.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            toLetterKeywordResult.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Post_SearchWithinGroup_ShouldThrowNotFound_WhenRepothrowResourceNotfoundException()
        {
            var fixture = new Fixture().RepoSearchThrowResourceNotFoundException();
            const string groupname = "group";
            const string keyword = "keyword";

            var searchWithinGroupRequest = new ServiceSearchRequest
            {
                GroupName = groupname,
                SearchQuery = keyword
            };
            var result = await fixture.SystemUnderTest().Post_SearchWithinGroup(searchWithinGroupRequest);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void Post_ShouldCallUpdateCounter_WhenCounterValueIsLessThanServiceCode()
        {
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"Service Code", "FDP0010"}
                    }
                }
            };

            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("FDP");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
               .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("FLooring")));
            var fixture = new Fixture()
                .WithMockDefinition(mockDefinition.Object)
                .WithMockServiceRepositoryForId("FDP0010")
                .WithCounterCurrentvalue(9)
                .ShouldCallUpdateOfCounterRepository(10);
            var result = await fixture.SystemUnderTest().PostOld(service);

            fixture.VerifyExpectations();
            result.Should().BeOfType<CreatedNegotiatedContentResult<ServiceDto>>();
        }

        [Fact]
        public async void Post_ShouldReturn201Created_WhenPassedADictionary()
        {
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"Service Code", "FDP0100"}
                    }
                }
            };
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("FDP");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Flooring")));
            const string id = "FDP0100";
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object).WithCounter(1).ShouldAddId(id);
            var result = await fixture.SystemUnderTest().PostOld(service);
            result.Should().BeOfType<CreatedNegotiatedContentResult<ServiceDto>>();
            var parsedResult = (CreatedNegotiatedContentResult<ServiceDto>)result;
            var serviceResult = parsedResult.Content;
            serviceResult.Id.Should().Be(id);
           // serviceResult.SearchKeywords.Should().Contain(id);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenClassificationHeaderIsNotPassed()
        {
            var service = new Dictionary<string, object>();
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.PostOld(service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Classification was not found.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenGivenGroupInServiceCodeDoesNotMatchWithDefinition()
        {
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"Service Code", "FDP0100"}
                    }
                }
            };

            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("MAP");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
               .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Flooring")));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result = await sut.PostOld(service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Group code of service Code: FDP0100 is invalid.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenGivenServiceCodeIsNotValid()
        {
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"Service Code", "FDP100"}
                    }
                }
            };

            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("FDP");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
               .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Flooring")));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result = await sut.PostOld(service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("service Code: FDP100 is invalid.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenNullIsPassed()
        {
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.PostOld(null);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("JSON is malformed.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenParseFails()
        {
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .Throws(new FormatException("Error message"));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result =
                await
                    sut.PostOld(new Dictionary<string, object>
                    {
                        {"Classification", new Dictionary<string, object> {{"Service Level 1", "Primary"}}}
                    });
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Error message");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenServiceDefinitionIsNotFound()
        {
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                }
            };
            var sut = new Fixture().WithoutMockDefinition("Primary").SystemUnderTest();
            var result = await sut.PostOld(service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Primary not found.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenServiceLevel1IsNotPassed()
        {
            var service = new Dictionary<string, object> { { "Classification", new Dictionary<string, object>() } };
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.PostOld(service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Service Level 1 was not found.");
        }

        [Fact]
        public async void Put_ShouldReturn200WithUpdatedService_WhenPassedService()
        {
            var serviceId = "CLY000043";
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                }
            };
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Sattar")));
            const string id = "CLY000001";
            var fixture =
                new Fixture().WithMockDefinition(mockDefinition.Object)
                    .WithExisting(id)
                    .WithMockServiceRepositoryForId("CLY000043")
                    .ShouldUpdate(new Service(new List<IHeaderData>(), new ServiceDefinition("Sattar")));
            var result = await fixture.SystemUnderTest().PutOld(serviceId, service);
            result.Should().BeOfType<OkNegotiatedContentResult<ServiceDto>>();
            var parsedResult = (OkNegotiatedContentResult<ServiceDto>)result;
            parsedResult.Content.Should().NotBeNull();
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenClassificationHeaderIsNotPassed()
        {
            var serviceId = "CLY000043";
            var service = new Dictionary<string, object>();
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.PutOld(serviceId, service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Classification was not found.");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenParseFails()
        {
            var serviceId = "CLY000043";
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .Throws(new FormatException("Error message"));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result =
                await
                    sut.PutOld(serviceId,
                        new Dictionary<string, object>
                        {
                            {"Classification", new Dictionary<string, object> {{"Service Level 1", "Primary"}}}
                        });
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Error message");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenPassedNonExistingService()
        {
            var serviceId = "CLY000043";
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                }
            };
            var mockDefinition = new Mock<IServiceDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Service>(It.IsAny<Dictionary<string, object>>(), null))
                .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Primary")));
            const string id = "CLY000001";
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object).WithoutExisting(id).WithoutUpdating();
            var result = await fixture.SystemUnderTest().PutOld(serviceId, service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenServiceDefinitionIsNotFound()
        {
            var serviceId = "CLY000043";
            var service = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Service Level 1", "Primary"},
                        {"Service Level 2", "Clay"},
                        {"Service Level 3", "Soil"},
                        {"Service Level 4", "Filler"},
                        {"Service Level 5", "Vitrified"}
                    }
                }
            };
            var sut = new Fixture().WithoutMockDefinition("Primary").SystemUnderTest();
            var result = await sut.PutOld(serviceId, service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Primary not found.");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenServiceLevel1IsNotPassed()
        {
            var serviceId = "CLY000043";
            var service = new Dictionary<string, object> { { "Classification", new Dictionary<string, object>() } };
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.PutOld(serviceId, service);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Service Level 1 was not found.");
        }

        [Fact]
        public async void Search_ForInvalidServiceLevel1_ReturnBadRequest()
        {
            var fixture = new Fixture().WithStubbedInvalidServiceLevel1();
            var serviceLevel1 = "Clay Service";
            var searchValue = "Clay";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1, serviceLevel1);

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Search_ForNullParameters_ReturnBadRequest()
        {
            var fixture = new Fixture().WithStubbedInvalidServiceLevel1();

            var result = await fixture.SystemUnderTest().Search(null, 1, null);

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Search_ForValidSearchParameters_ReturnNoData()
        {
            var serviceList = new List<Service>();
            var fixture = new Fixture().WithStubbedSearchValues(serviceList);
            var serviceLevel1 = "Civil Works";
            var searchValue = "Masonry";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1, serviceLevel1);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var resultContents = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<ServiceDto>)resultContents.Content["items"];
            items.Count.Should().Be(0);
            ((long)resultContents.Content["recordCount"]).Should().Be(0);
        }

        [Fact]
        public async void Search_ForValidSearchParameters_ReturnRelevantService()
        {
            const string firstServiceId = "CW000001";
            const string secondServiceId = "CW000015";
            var firstService = GetService(firstServiceId);
            var secondService = GetService(secondServiceId);
            var serviceList = new List<Service> { firstService, secondService };
            var fixture = new Fixture().WithStubbedSearchValues(serviceList);
            var serviceLevel1 = "Civil Works";
            var searchValue = "Masonry";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1, serviceLevel1);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var resultContents = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<ServiceDto>)resultContents.Content["items"];
            items.Count.Should().Be(2);

            items[0].Id.Should().Be(firstServiceId);
            items[1].Id.Should().Be(secondServiceId);
        }

        [Fact]
        public async void Search_ForWithoutServiceLevel_ReturnAcceptRequest()
        {
            const string firstServiceId = "CW000001";
            const string secondServiceId = "CW000015";
            var firstService = GetService(firstServiceId);
            var secondService = GetService(secondServiceId);
            var serviceList = new List<Service> { firstService, secondService };
            var fixture = new Fixture().WithStubbedSearchValues(serviceList);
            var serviceLevel1 = "Civil Works";
            var searchValue = "Masonry";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
        }

        [Theory]
        [InlineData("Masonry & Plaster", "S")]
        [InlineData("Masonry & Plaster", "Sa")]
        [InlineData("Masonry & Plaster", "Sa pa")]
        public async Task Search_ShouldNotAllowSearchKeywordToBeLessThanThreeCharacters_ReturnBadRequest(
            string serviceLevel1, string searchKeyword)
        {
            var result = await new Fixture().SystemUnderTest().Search(searchKeyword, 0, serviceLevel1);

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            var resultContents = (BadRequestErrorMessageResult)result;
            resultContents.Message.Should().Be("The search keyword should be greater than 3 characters");
        }

        [Fact]
        public async void SearchInGroup_ShouldQuerySearchInGroupOfRepo_WhenKeywordMoreThan3CharacterLongInPassed()
        {
            var keyword = "keyword";
            var fixture = new Fixture().RepoSearchInGroupCalledOnceWith(keyword);

            await fixture.SystemUnderTest().SearchInGroup(keyword);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task SearchInGroup_ShouldReturnBadRequest_WhenAllKeywordsAreLessThan3Letter()
        {
            const string keyword = "ab as";
            var fixture = new Fixture();

            var searchInGroup = await fixture.SystemUnderTest().SearchInGroup(keyword);

            searchInGroup.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void SearchInGroup_ShouldReturnBadRequest_WhenKeywordIsLessThan3Letter()
        {
            var fixture = new Fixture();
            const string invalidKeyword = "ab";
            const string validKeyword = "abc";

            var result = await fixture.SystemUnderTest().SearchInGroup(invalidKeyword);
            var otherResult = await fixture.SystemUnderTest().SearchInGroup(validKeyword);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            otherResult.Should().NotBeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void SearchInGroup_ShouldReturnNotFound_WhenRepoThrowsResourceNotFoundException()
        {
            var fixture = new Fixture().RepoSearchInGroupThrowsResourceNotFoundException();
            var result = await fixture.SystemUnderTest().SearchInGroup("keyword");
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturnNotFound_WhenRepositoryThrowResourceNotFoundException()
        {
            var sut = new Fixture().FindOfServiceRepoThrowsResourceNotFoundException().SystemUnderTest();
            var serviceRateDtoStub = new ServiceRateDtoStub()
                .HavingSomeLastPurchaseRate()
                .HavingSomeAvergaePurchaseRate();
            var result = await sut.UpdateRates("", serviceRateDtoStub.Stub());
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldThrowBadRequest_WhenAveragePurchaseRateIsNotPassed()
        {
            var sut = new Fixture().SystemUnderTest();
            var serviceRateDtoStub = new ServiceRateDtoStub().HavingAveragePurcaheRateAsNull();
            var result = await sut.UpdateRates("", serviceRateDtoStub.Stub());
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldThrowBadRequest_WhenLastPurchaseRateIsNotPassed()
        {
            var sut = new Fixture().SystemUnderTest();
            var serviceRateDtoStub = new ServiceRateDtoStub().HavingLastPurchaseRateAsNull();
            var result = await sut.UpdateRates("", serviceRateDtoStub.Stub());
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        private Service GetService(string id)
        {
            return new Service(new List<IHeaderData>
                {
                    new HeaderData("Classification", "Classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Service Level 1", "Service Level 1", "Civil Works")
                        }
                    },
                    new HeaderData("General", "General")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Service Code", "Service Code", id)
                        }
                    }
                }, new ServiceDefinition("Civil Works"))
            { Id = id };
        }

        private class Fixture
        {
            private readonly Mock<ICounterRepository> _counterRepository = new Mock<ICounterRepository>();

            private readonly Mock<IComponentDefinitionRepository<IServiceDefinition>> _mockDefinitionRepository =
                new Mock<IComponentDefinitionRepository<IServiceDefinition>>();

            private readonly Mock<IFilterCriteriaBuilder> _mockFilterCriterialBuilder = new Mock<IFilterCriteriaBuilder>();
            private readonly Mock<IMasterDataRepository> _mockMasterDataRepository = new Mock<IMasterDataRepository>();

            private readonly Mock<IClassificationDefinitionRepository>
                _mockServiceClassificationDefinitionRepository = new Mock<IClassificationDefinitionRepository>();

            private readonly Mock<IServiceRepository> _mockServiceRepository =
                            new Mock<IServiceRepository>();

            private readonly Mock<IServiceSapSyncer> _mockServiceSapSyncer = new Mock<IServiceSapSyncer>();
            private readonly List<Action> _verifications = new List<Action>();

            public Fixture FindOfServiceRepoThrowsResourceNotFoundException()
            {
                _mockServiceRepository.Setup(m => m.Find(It.IsAny<string>())).Throws(new ResourceNotFoundException(""));
                return this;
            }

            public Fixture RepoSearchInGroupCalledOnceWith(string keyword)
            {
                Action action =
                    () => _mockServiceRepository.Verify(m => m.SearchInGroup(new List<string> { keyword }), Times.Once);
                _verifications.Add(action);
                return this;
            }

            public Fixture RepoSearchInGroupThrowsResourceNotFoundException()
            {
                _mockServiceRepository.Setup(m => m.SearchInGroup(It.IsAny<List<string>>()))
                    .Throws(new ResourceNotFoundException(""));
                return this;
            }

            public Fixture RepoSearchThrowResourceNotFoundException()
            {
                _mockServiceRepository.Setup(
                        m => m.Search(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SortOrder>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));

                _mockServiceRepository.Setup(
                        m =>
                            m.Search(It.IsAny<Dictionary<string, Tuple<string, object>>>(), It.IsAny<int>(), It.IsAny<int>(),
                                It.IsAny<string>(), It.IsAny<SortOrder>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));

                return this;
            }

            public Fixture ShouldAddId(string id)
            {
                _mockServiceRepository.Setup(m => m.Add(It.IsAny<Service>())).Returns(Task.CompletedTask);
                _verifications.Add(() => _mockServiceRepository.Verify(m => m.Add(It.IsAny<Service>()), Times.Once));
                _mockServiceRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("Clay")) { Id = id });
                return this;
            }

            public Fixture ShouldCallUpdateOfCounterRepository(int counterValue)
            {
                Action action = () => _counterRepository.Verify(c => c.Update(counterValue, "Service"), Times.Once);
                _verifications.Add(action);
                return this;
            }

            public Fixture ShouldUpdate(Service service)
            {
                _mockServiceRepository.Setup(m => m.Update(It.IsAny<Service>())).Returns(Task.CompletedTask);
                _verifications.Add(() => _mockServiceRepository.Verify(m => m.Update(It.IsAny<Service>()), Times.Once));
                return this;
            }

            public ServiceController SystemUnderTest()
            {
                return new ServiceController(_mockDefinitionRepository.Object, _counterRepository.Object,
                    _mockServiceRepository.Object, _mockMasterDataRepository.Object,
                    _mockServiceSapSyncer.Object, _mockServiceClassificationDefinitionRepository.Object,
                    _mockFilterCriterialBuilder.Object);
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithCounter(int counter)
            {
                _counterRepository.Setup(r => r.NextValue("Service")).ReturnsAsync(counter);
                return this;
            }

            public Fixture WithCounterCurrentvalue(int counter)
            {
                _counterRepository.Setup(r => r.CurrentValue("Service")).ReturnsAsync(counter);
                return this;
            }

            public Fixture WithExisting(string id)
            {
                var service = new Service(new List<IHeaderData>
                {
                    new HeaderData("Purchase", "Purchase")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Last Purchase Rate", "Last Purchase Rate", "50.11"),
                            new ColumnData("Weighted Average Purchase Rate", "Weighted Average Purchase Rate", "71.34")
                        }
                    },
                    new HeaderData("Classification", "Classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Service Level 1", "Service Level 1", "Clay")
                        }
                    }
                }, new ServiceDefinition("Clay"))
                { Id = id };
                _mockServiceRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(service);

                _mockServiceClassificationDefinitionRepository.Setup(m => m.Find(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(
                        new ClassificationDefinitionDao(new ClassificationDefinition("Clay",
                            "Clay description", null)));

                return this;
            }

            public Fixture WithExistingWithoutGeneralHeader(string id)
            {
                _mockServiceRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(
                        new Service(new List<IHeaderData>(), new ServiceDefinition("Sattar"))
                        { Id = id });

                return this;
            }

            public Fixture WithExistingWithoutGeneralHeaderRateColumns(string id)
            {
                _mockServiceRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(
                        new Service(new List<IHeaderData>
                            {
                                new HeaderData("General", "General")
                            }, new ServiceDefinition("Sattar"))
                        { Id = id });

                return this;
            }

            public Fixture WithMockDefinition(IServiceDefinition serviceDefinition)
            {
                _mockDefinitionRepository.Setup(m => m.Find("Primary")).ReturnsAsync(serviceDefinition);
                return this;
            }

            public Fixture WithoutExisting(string id)
            {
                _mockServiceRepository.Setup(m => m.Find(id)).Throws(new ResourceNotFoundException("Not found"));
                return this;
            }

            public Fixture WithoutMockDefinition(string group)
            {
                _mockDefinitionRepository.Setup(m => m.Find(group))
                    .Throws(new ResourceNotFoundException(group));
                return this;
            }

            public Fixture WithoutUpdating()
            {
                _mockServiceRepository.Setup(m => m.Update(It.IsAny<Service>()))
                    .Throws(new ResourceNotFoundException("a"));
                return this;
            }

            public Fixture WithStubbedInvalidServiceLevel1()
            {
                _mockMasterDataRepository.Setup(m => m.Exists(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(false);

                return this;
            }

            public Fixture WithStubbedSearchValues(List<Service> service)
            {
                _mockMasterDataRepository.Setup(m => m.Exists(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(true);

                _mockServiceRepository.Setup(
                        m =>
                            m.Search(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<Int32>(),
                                It.IsAny<Int32>(), It.IsAny<string>(), It.IsAny<SortOrder>()))
                    .ReturnsAsync(service);

                _mockServiceRepository.Setup(
                        m => m.Search(It.IsAny<List<string>>(), It.IsAny<Int32>(), It.IsAny<Int32>()))
                    .ReturnsAsync(service);
                return this;
            }

            public Fixture WithStubbedServicesByGroupAndColumnName(List<Service> services)
            {
                _mockServiceRepository.Setup(
                       m => m.GetTotalCountByGroupAndColumnName(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(services.Count);

                _mockServiceRepository.Setup(
                        m => m.GetByGroupAndColumnName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(services);

                return this;
            }

            public Fixture WithStubbedServicesByGroupAndColumnNameAndKeyWord(List<Service> services)
            {
                _mockServiceRepository.Setup(
                       m => m.GetTotalCountByGroupAndColumnNameAndKeyWords(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                   .ReturnsAsync(services.Count);

                _mockServiceRepository.Setup(
                        m => m.GetByGroupAndColumnNameAndKeyWords(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(services);

                return this;
            }

            public Fixture WithMockServiceRepositoryForId(string code)
            {
                _mockServiceRepository.Setup(m => m.Find(code))
                    .ReturnsAsync(new Service(new List<IHeaderData>(), new ServiceDefinition("FDP")));
                return this;
            }
        }

        private class ServiceRateDtoStub
        {
            private readonly RateDto _stub = new RateDto();

            public ServiceRateDtoStub HavingAveragePurcaheRateAsNull()
            {
                _stub.WeightedAveragePurchaseRate = null;
                return this;
            }

            public ServiceRateDtoStub HavingLastPurchaseRateAsNull()
            {
                _stub.LastPurchaseRate = null;
                return this;
            }

            public ServiceRateDtoStub HavingSomeAvergaePurchaseRate()
            {
                _stub.WeightedAveragePurchaseRate = new MoneyDto("INR", 5);
                return this;
            }

            public ServiceRateDtoStub HavingSomeLastPurchaseRate()
            {
                _stub.LastPurchaseRate = new MoneyDto("INR", 5);
                return this;
            }

            public RateDto Stub()
            {
                return _stub;
            }
        }
    }
}