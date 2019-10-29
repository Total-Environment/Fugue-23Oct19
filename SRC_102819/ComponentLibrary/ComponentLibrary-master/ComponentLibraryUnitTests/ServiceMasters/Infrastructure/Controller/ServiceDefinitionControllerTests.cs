using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Castle.Core.Internal;
using FluentAssertions;
using MongoDB.Bson;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure.Controller
{
    public class ServiceDefinitionControllerTests
    {
        private const string ServiceDefinitionCode = "code";

        [Fact]
        public async void Get_ShouldReturnNotFound_WhenRepoThrowsResourceNotFoundException()
        {
            var fixture = new Fixture().ServiceDefinitionRepo_Find_ShouldThrowResourceNotFound();

            var result = await fixture.SystemUnderTest().Get("code");

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Get_ShouldReturnOKWithServiceDefinitionDto_WhenPassedValidId()
        {
            var fixture = new Fixture().Accepting(GetServiceDefinition()).SystemUnderTest();

            var result = await fixture.Get("Clay");

            result.Should().BeOfType<OkNegotiatedContentResult<ServiceDefinitionDao>>();

            var castResult = (OkNegotiatedContentResult<ServiceDefinitionDao>)result;
            castResult.Content.Name.Should().Be("Clay");
        }

        [Fact]
        public async void Patch_ShouldReturnBadRequest_WhenServiceDtoDoesnotMatchWithGenericService()
        {
            var dataType = new DataTypeDtoStub()
                .WithNameAs("String");

            const string columnName = "Column1";

            var column = new ColumnDefinitionDtoStub()
                .WithNameAs(columnName)
                .WithDataTypeAs(dataType.Stub());

            const string headerName = "Header1";
            var header = new HeaderDefinitionDtoStub()
                .WithNameAs(headerName)
                .WithColumnAs(column.Stub());

            var input = new ServiceDefinitionDtoStub()
                .WithObjectIdAs(ObjectId.GenerateNewId(DateTime.Today))
                .WithHeaderAs(header.Stub())
                .WithNameAs(ServiceDefinitionCode);
            var columnDifinition = new ColumnDefinitionStub()
                .HavingNameAs("column2");
            var headerDefinition = new HeaderDefinitionStub()
                .WithNameAs(headerName)
                .WithColumnAs(columnDifinition.Stub());
            var serviceDefinition = new ServiceDefinitionStub()
                .HavingHeader(headerDefinition.Stub());
            var fixture = new Fixture();
            var mockRepo = fixture.DataTypeRepoConstructingDataType().GetServiceDefinitionRepository();
            mockRepo.Setup(m => m.Find(ServiceDefinitionCode)).ReturnsAsync(serviceDefinition.Stub());

            var result = await fixture.Accepting(GetGenericService()).SystemUnderTest().Patch(input.Stub());

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Post_ShouldReturn201CreatedWithServiceDefinition_WhenPassedService()
        {
            var input = GetInput();
            var genericService = GetGenericService();
            var sut = new Fixture().Accepting(genericService).Accepting(GetServiceDefinition()).SystemUnderTest();

            var result = await sut.Post(input);
            result.Should().BeOfType<CreatedNegotiatedContentResult<ServiceDefinitionDao>>();
            var castResult = (CreatedNegotiatedContentResult<ServiceDefinitionDao>)result;
            castResult.Content.Name.Should().Be("Clay");
        }

        [Fact]
        public async void Post_ShouldReturnBadRequest_WhenInputServiceDoesNotMatchWithGenericServiceHeaderColumnMapping()
        {
            var input = GetInputWhichDoesNotMatchColumnMappingWithGenericService();
            var sut = new Fixture().Accepting(GetGenericService()).SystemUnderTest();
            var result = await sut.Post(input);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Post_ShouldThrow409Conflict_WhenMongoRepositoryThrowsArgumentException()
        {
            var input = new ServiceDefinition("Clay");
            var genericService = GetGenericService();
            var sut = new Fixture().Accepting(genericService).WithExisting(input).SystemUnderTest();
            var result = await sut.Post(GetInput());
            result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async void Put_ShouldReturnNotFound_WhenServiceDefinitionRepoThrowResourceNotFound()
        {
            var input = new ServiceDefinitionDtoStub().WithNameAs(ServiceDefinitionCode).Stub();
            var genericService = GetGenericService();
            var fixture = new Fixture().Accepting(genericService).ServiceDefinitionRepo_Find_ShouldThrowResourceNotFound();

            var result = await fixture.SystemUnderTest().Patch(input);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void Put_ShouldReturnOk_ServiceDefinitionIsPassedWithExtraColumn()
        {
            var input = GetInput();
            input.Headers[0].Columns.Add(new ColumnDefinitionDto
            {
                Name = "SomeColumn",
                DataType = new DataTypeDto { Name = "String", SubType = null }
            });

            var genericService = GetGenericService();
            var sut = new Fixture()
                .Accepting(genericService)
                .DataTypeRepoConstructingDataType()
                .ServiceDefinitionRepoHavingDefinition("Clay", "columnName")
                .SystemUnderTest();

            var result = await sut.Patch(input);

            result.Should().BeAssignableTo<OkResult>();
        }

        [Fact]
        public async void Put_ShouldThrowBadRequest_WhenServiceDtoHasInfoWhichIsAlreadyExist()
        {
            const string headerName = "Header1";
            var header = new HeaderDefinitionDtoStub()
                .WithNameAs(headerName);
            var input = new ServiceDefinitionDtoStub()
                .WithObjectIdAs(ObjectId.GenerateNewId(DateTime.Today))
                .WithHeaderAs(header.Stub())
                .WithNameAs(ServiceDefinitionCode);
            var headerDefinition = new HeaderDefinitionStub()
                .WithNameAs(headerName);
            var serviceDefinitionStub = new ServiceDefinitionStub()
                .HavingHeader(headerDefinition.Stub());

            var fixture = new Fixture().DataTypeRepoConstructingDataType()
                .FindOfServiceDefinitionRepoReturns(ServiceDefinitionCode, serviceDefinitionStub.Stub());

            var result = await fixture.SystemUnderTest().Patch(input.Stub());

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async Task Put_ShouldUpdateService_WhenPassedDtoWithNewHeader()
        {
            var dataType = new DataTypeDtoStub()
                .WithNameAs("String");
            const string columnName = "Column1";
            var column = new ColumnDefinitionDtoStub()
                .WithNameAs(columnName)
                .WithDataTypeAs(dataType.Stub());
            const string headerName = "Header1";
            var header = new HeaderDefinitionDtoStub()
                .WithNameAs(headerName)
                .WithColumnAs(column.Stub());
            var input = new ServiceDefinitionDtoStub()
                .WithObjectIdAs(ObjectId.GenerateNewId(DateTime.Today))
                .WithHeaderAs(header.Stub())
                .WithNameAs(ServiceDefinitionCode);
            var columnDifinition = new ColumnDefinitionStub()
                .HavingNameAs("column2");
            var headerDefinition = new HeaderDefinitionStub()
                .WithNameAs(headerName)
                .WithColumnAs(columnDifinition.Stub());
            var serviceDefinition = new ServiceDefinitionStub()
                .HavingHeader(headerDefinition.Stub());
            var fixture = new Fixture();
            var mockRepo = fixture.DataTypeRepoConstructingDataType().GetServiceDefinitionRepository();
            mockRepo.Setup(m => m.Find(ServiceDefinitionCode)).ReturnsAsync(serviceDefinition.Stub());

            var genericService = GetGenericService();
            genericService.Headers.Add(new HeaderDefinition("Header1", "Header1",
                new List<ColumnDefinition>
                {
                    new ColumnDefinition("column1","column1", new ConstantDataType("Primary")),
                    new ColumnDefinition("column2", "column2",new ConstantDataType("Primary"))
                }));

            await fixture.Accepting(genericService).SystemUnderTest().Patch(input.Stub());

            mockRepo.Verify(
                m =>
                    m.Update(
                        It.Is<IServiceDefinition>(
                            d => d.Headers.First(h => h.Name == headerName).Columns.Any(c => c.Name == "column2"))));
        }

        private static ServiceDefinitionDao GetInput()
        {
            return new ServiceDefinitionDao
            {
                Name = "Clay",
                Code = "CLY",
                Headers = new List<HeaderDefinitionDto>
                {
                    new HeaderDefinitionDto
                    {
                        Name = "Classification",
                        Columns = new List<ColumnDefinitionDto>
                        {
                            new ColumnDefinitionDto
                            {
                                Name = "Service Level 1",
                                DataType = new DataTypeDto
                                {
                                    Name = "Constant",
                                    SubType = "primary"
                                }
                            },
                            new ColumnDefinitionDto
                            {
                                Name = "Service Level 3",
                                DataType = new DataTypeDto
                                {
                                    Name = "MasterData",
                                    SubType = "Clay_Service_Level_3"
                                }
                            }
                        }
                    }
                }
            };
        }

        private static IServiceDefinition ServiceDefinitionWithOneHeader(string serviceCode, string headerName)
        {
            return new ServiceDefinitionStub()
                .HavingHeader(new HeaderDefinitionStub().WithNameAs(headerName).Stub())
                .HavingCode(serviceCode).Stub();
        }

        private ServiceDefinition GetGenericService()
        {
            return new ServiceDefinition("Generic Service")
            {
                Code = "GNR",
                Headers = new List<IHeaderDefinition>
                {
                    new HeaderDefinition("Classification","Classification", new List<ColumnDefinition>
                        {
                            new ColumnDefinition("Service Level 1","Service Level 1",new ConstantDataType("Primary")),
                            new ColumnDefinition("Service Level 3","Service Level 3",new ConstantDataType("Primary")),
                            new ColumnDefinition("SomeColumn","SomeColumn",new ConstantDataType("Primary"))
                        })
                }
            };
        }

        private ServiceDefinitionDao GetInputWhichDoesNotMatchColumnMappingWithGenericService()
        {
            return new ServiceDefinitionDao
            {
                Name = "Clay",
                Code = "CLY",
                Headers = new List<HeaderDefinitionDto>
                {
                    new HeaderDefinitionDto
                    {
                        Name = "Deepika",
                        Columns = new List<ColumnDefinitionDto>
                        {
                            new ColumnDefinitionDto
                            {
                                Name = "Service Level 1",
                                DataType = new DataTypeDto
                                {
                                    Name = "Constant",
                                    SubType = "primary"
                                }
                            }
                        }
                    }
                }
            };
        }

        private ServiceDefinition GetServiceDefinition()
        {
            return new ServiceDefinition("Clay")
            {
                Code = "GNR",
                Headers = new List<IHeaderDefinition>
                {
                    new HeaderDefinition("Classification","Classification",  new List<ColumnDefinition>
                        {
                            new ColumnDefinition("Service Level 1","Service Level 1",new ConstantDataType("Primary")),
                            new ColumnDefinition("Service Level 3","Service Level 3",new ConstantDataType("Primary")),
                            new ColumnDefinition("SomeColumn","SomeColumn",new ConstantDataType("Primary"))
                        })
                }
            };
        }

        private class ColumnDefinitionDtoStub
        {
            private readonly ColumnDefinitionDto _stub = new ColumnDefinitionDto();

            public ColumnDefinitionDto Stub()
            {
                return _stub;
            }

            public ColumnDefinitionDtoStub WithDataTypeAs(DataTypeDto dataTypeDto)
            {
                _stub.DataType = dataTypeDto;
                return this;
            }

            public ColumnDefinitionDtoStub WithNameAs(string name)
            {
                _stub.Name = name;
                return this;
            }
        }

        private class ColumnDefinitionStub
        {
            private readonly Mock<IColumnDefinition> _stub = new Mock<IColumnDefinition>();

            public ColumnDefinitionStub HavingNameAs(string name)
            {
                _stub.Setup(s => s.Name).Returns(name);
                return this;
            }

            public IColumnDefinition Stub()
            {
                return _stub.Object;
            }
        }

        private class DataTypeDtoStub
        {
            private readonly DataTypeDto _stub = new DataTypeDto();

            public DataTypeDto Stub()
            {
                return _stub;
            }

            public DataTypeDtoStub WithNameAs(string name)
            {
                _stub.Name = name;
                return this;
            }
        }

        private class Fixture
        {
            private readonly IList<Action> _expectation = new List<Action>();
            private readonly MasterDataList _masterDataList;
            private readonly Mock<IMasterDataRepository> _masterDataRepository = new Mock<IMasterDataRepository>();
            private readonly Mock<IDataTypeFactory> _mockDataTypeFactory = new Mock<IDataTypeFactory>();

            private readonly Mock<IComponentDefinitionRepository<IServiceDefinition>> _repositoryMock =
                new Mock<IComponentDefinitionRepository<IServiceDefinition>>();

            public Fixture()
            {
                _masterDataList = new MasterDataList("Clay_Service_Level_3", new List<MasterDataValue>());
                _mockDataTypeFactory.Setup(m => m.Construct("Constant", "primary"))
                    .ReturnsAsync(new ConstantDataType("primary"));
                _mockDataTypeFactory.Setup(m => m.Construct("MasterData", "Clay_Service_Level_3"))
                    .ReturnsAsync(new ConstantDataType("primary"));
            }

            public Fixture Accepting(ServiceDefinition input)
            {
                _masterDataRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(_masterDataList);

                _repositoryMock.Setup(m => m.Add(It.IsAny<ServiceDefinition>())).Returns(Task.CompletedTask);
                _repositoryMock.Setup(m => m.Find(input.Name))
                    .ReturnsAsync(input);
                return this;
            }

            public Fixture DataTypeRepoConstructingDataType()
            {
                _mockDataTypeFactory.Setup(m => m.Construct(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new Mock<IDataType>().Object);
                return this;
            }

            public Fixture FindOfServiceDefinitionRepoReturns(string code, IServiceDefinition definition)
            {
                _repositoryMock.Setup(r => r.Find(code)).ReturnsAsync(definition);
                return this;
            }

            public Mock<IComponentDefinitionRepository<IServiceDefinition>> GetServiceDefinitionRepository()
            {
                return _repositoryMock;
            }

            public Fixture ServiceDefinitionRepo_Find_ShouldThrowResourceNotFound()
            {
                _repositoryMock.Setup(m => m.Find(It.IsAny<string>())).ThrowsAsync(new ResourceNotFoundException(""));
                return this;
            }

            public Fixture ServiceDefinitionRepoHavingDefinition(string serviceCode, string headerCode)
            {
                _repositoryMock.Setup(m => m.Find(serviceCode))
                    .ReturnsAsync(ServiceDefinitionWithOneHeader(serviceCode, headerCode));
                return this;
            }

            public Fixture ServiceDefinitionRepoPatchShouldBeCalledWith(string servicedefinitionCode)
            {
                _expectation.Add(() => _repositoryMock.Verify(r => r.Find(servicedefinitionCode), Times.Once));
                return this;
            }

            public ServiceDefinitionController SystemUnderTest()
            {
                return new ServiceDefinitionController(_repositoryMock.Object, _mockDataTypeFactory.Object,
                    new Mock<IDependencyDefinitionRepository>().Object);
            }

            public void VerifyExpectations()
            {
                _expectation.ForEach(e => e.Invoke());
            }

            public Fixture WithExisting(IComponentDefinition input)
            {
                _repositoryMock.Setup(m => m.Add(It.Is<ServiceDefinition>(k => k.Name == input.Name)))
                    .Throws(new DuplicateResourceException(input.Name));

                return this;
            }
        }

        private class HeaderDefinitionDtoStub
        {
            private readonly List<ColumnDefinitionDto> _columns = new List<ColumnDefinitionDto>();
            private readonly HeaderDefinitionDto _stub = new HeaderDefinitionDto();

            public HeaderDefinitionDto Stub()
            {
                _stub.Columns = _columns;
                return _stub;
            }

            public HeaderDefinitionDtoStub WithColumnAs(ColumnDefinitionDto column)
            {
                _columns.Add(column);
                return this;
            }

            public HeaderDefinitionDtoStub WithNameAs(string name)
            {
                _stub.Name = name;
                return this;
            }
        }

        private class HeaderDefinitionStub
        {
            private readonly List<IColumnDefinition> _columns = new List<IColumnDefinition>();
            private readonly Mock<IHeaderDefinition> _stub = new Mock<IHeaderDefinition>();

            public IHeaderDefinition Stub()
            {
                _stub.Setup(s => s.Columns).Returns(_columns);
                return _stub.Object;
            }

            public HeaderDefinitionStub WithColumnAs(IColumnDefinition columnDefinition)
            {
                _columns.Add(columnDefinition);
                return this;
            }

            public HeaderDefinitionStub WithNameAs(string name)
            {
                _stub.Setup(s => s.Name).Returns(name);
                return this;
            }
        }

        private class ServiceDefinitionDtoStub
        {
            private readonly List<HeaderDefinitionDto> _header = new List<HeaderDefinitionDto>();
            private readonly ServiceDefinitionDao _stub = new ServiceDefinitionDao();
            private string _name;

            public ServiceDefinitionDao Stub()
            {
                _stub.Headers = _header;
                _stub.Name = _name;
                return _stub;
            }

            public ServiceDefinitionDtoStub WithHeaderAs(HeaderDefinitionDto stub)
            {
                _header.Add(stub);
                return this;
            }

            public ServiceDefinitionDtoStub WithNameAs(string name)
            {
                _name = name;
                return this;
            }

            public ServiceDefinitionDtoStub WithObjectIdAs(ObjectId objectId)
            {
                _stub.ObjectId = objectId;
                return this;
            }
        }

        private class ServiceDefinitionStub
        {
            private readonly List<IHeaderDefinition> _headers = new List<IHeaderDefinition>();
            private readonly Mock<IServiceDefinition> _stub = new Mock<IServiceDefinition>();

            public ServiceDefinitionStub HavingCode(string serviceCode)
            {
                _stub.Setup(s => s.Code).Returns(serviceCode);
                return this;
            }

            public ServiceDefinitionStub HavingHeader(IHeaderDefinition header)
            {
                _headers.Add(header);
                return this;
            }

            public IServiceDefinition Stub()
            {
                _stub.Setup(s => s.Headers).Returns(_headers);
                return _stub.Object;
            }
        }
    }
}