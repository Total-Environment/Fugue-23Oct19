using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Castle.Core.Internal;
using FluentAssertions;
using MongoDB.Bson;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
	public class MaterialDefinitionControllerTests
	{
		private const string MaterialDefinitionCode = "code";

		[Fact]
		public async void Get_ShouldReturnNotFound_WhenRepoThrowsResourceNotFoundException()
		{
			var fixture = new Fixture().MaterialDefinitionRepo_Find_ShouldThrowResourceNotFound();

			var result = await fixture.SystemUnderTest().Get("code");

			result.Should().BeOfType<NotFoundResult>();
		}

		[Fact]
		public async void Get_ShouldReturnOKWithMaterialDefinitionDto_WhenPassedValidId()
		{
			var fixture = new Fixture().Accepting(GetMaterialDefinition()).SystemUnderTest();

			var result = await fixture.Get("Clay");

			result.Should().BeOfType<OkNegotiatedContentResult<MaterialDefinitionDao>>();

			var castResult = (OkNegotiatedContentResult<MaterialDefinitionDao>)result;
			castResult.Content.Name.Should().Be("Clay");
		}

		[Fact]
		public async void Patch_ShouldReturnBadRequest_WhenMaterialDtoDoesnotMatchWithGenericMaterial()
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

			var input = new MaterialDefinitionDtoStub()
				.WithObjectIdAs(ObjectId.GenerateNewId(DateTime.Today))
				.WithHeaderAs(header.Stub())
				.WithNameAs(MaterialDefinitionCode);
			var columnDifinition = new ColumnDefinitionStub()
				.HavingNameAs("column2");
			var headerDefinition = new HeaderDefinitionStub()
				.WithNameAs(headerName)
				.WithColumnAs(columnDifinition.Stub());
			var materialDefinition = new MaterialDefinitionStub()
				.HavingHeader(headerDefinition.Stub());
			var fixture = new Fixture();
			var mockRepo = fixture.DataTypeRepoConstructingDataType().GetMaterialDefinitionRepository();
			mockRepo.Setup(m => m.Find(MaterialDefinitionCode)).ReturnsAsync(materialDefinition.Stub());

			var result = await fixture.Accepting(GetGenericMaterial()).SystemUnderTest().Patch(input.Stub());

			result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
		}

		[Fact]
		public async void Post_ShouldReturn201CreatedWithMaterialDefinition_WhenPassedMaterial()
		{
			var input = GetInput();
			var genericMaterial = GetGenericMaterial();
			var sut = new Fixture().Accepting(genericMaterial).Accepting(GetMaterialDefinition()).SystemUnderTest();

			var result = await sut.Post(input);
			result.Should().BeOfType<CreatedNegotiatedContentResult<MaterialDefinitionDao>>();
			var castResult = (CreatedNegotiatedContentResult<MaterialDefinitionDao>)result;
			castResult.Content.Name.Should().Be("Clay");
		}

		[Fact]
		public async void Post_ShouldReturn400BadRequest_WhenPassedMaterialWithMasterDataIdThatDoesntExist()
		{
			var input = new MaterialDefinitionDao
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
								Name = "Material Level 3",
								DataType = new DataTypeDto
								{
									Name = "MasterData",
									SubType = "Clay_Material_Level_3"
								}
							}
						}
					}
				}
			};
			var dataTypeFactory = new Mock<IDataTypeFactory>();
			dataTypeFactory.Setup(d => d.Construct("MasterData", "Clay_Material_Level_3"))
				.Throws(new ResourceNotFoundException("Clay_Material_Level_3"));
			var controller =
				new MaterialDefinitionController(
					new Mock<IComponentDefinitionRepository<IMaterialDefinition>>().Object, dataTypeFactory.Object,
					new Mock<IDependencyDefinitionRepository>().Object, new Mock<ICodePrefixTypeMappingRepository>().Object);
			var result = await controller.Post(input);
			result.Should().BeOfType<BadRequestErrorMessageResult>();
			var castResult = (BadRequestErrorMessageResult)result;
			castResult.Message.Should().Be("Clay_Material_Level_3 not found.");
		}

		[Fact]
		public async void
			Post_ShouldReturnBadRequest_WhenInputMaterialDoesNotMatchWithGenericMaterialHeaderColumnMapping()
		{
			var input = GetInputWhichDoesNotMatchColumnMappingWithGenericMaterial();
			var sut = new Fixture().Accepting(GetGenericMaterial()).SystemUnderTest();
			var result = await sut.Post(input);
			result.Should().BeOfType<BadRequestErrorMessageResult>();
		}

		[Fact]
		public async void Post_ShouldReturnCreatedWithMaterialDefinitionWithKeys_WhenPassedMaterial()
		{
			var input = GetInput();
			var genericMaterial = GetGenericMaterial();
			var sut = new Fixture().Accepting(genericMaterial).Accepting(GetMaterialDefinition()).SystemUnderTest();

			var result = await sut.Post(input);
			result.Should().BeOfType<CreatedNegotiatedContentResult<MaterialDefinitionDao>>();
			var castResult = (CreatedNegotiatedContentResult<MaterialDefinitionDao>)result;
			castResult.Content.Headers.Any(h => h.Name == null || h.Key == null).Should().BeFalse();
			castResult.Content.Headers.Any(h => h.Columns.Any(c => c.Key == null || c.Name == null)).Should().BeFalse();
		}

		[Fact]
		public async void Post_ShouldThrow409Conflict_WhenMongoRepositoryThrowsArgumentException()
		{
			var input = new MaterialDefinition("Clay");
			var genericMaterial = GetGenericMaterial();
			var sut = new Fixture().Accepting(genericMaterial).WithExisting(input).SystemUnderTest();
			var result = await sut.Post(GetInput());
			result.Should().BeOfType<ConflictResult>();
		}

		[Fact]
		public async void Put_ShouldReturnNotFound_WhenMaterialDefinitionRepoThrowResourceNotFound()
		{
			var input = new MaterialDefinitionDtoStub().WithNameAs(MaterialDefinitionCode).Stub();
			var genericMaterial = GetGenericMaterial();
			var fixture =
				new Fixture().Accepting(genericMaterial).MaterialDefinitionRepo_Find_ShouldThrowResourceNotFound();

			var result = await fixture.SystemUnderTest().Patch(input);

			result.Should().BeAssignableTo<NotFoundResult>();
		}

		[Fact]
		public async void Put_ShouldReturnOk_MaterialDefinitionIsPassedWithExtraColumn()
		{
			var input = GetInput();
			input.Headers[0].Columns.Add(new ColumnDefinitionDto
			{
				Name = "SomeColumn",
				DataType = new DataTypeDto { Name = "String", SubType = null }
			});

			var genericMaterial = GetGenericMaterial();
			var sut = new Fixture()
				.Accepting(genericMaterial)
				.DataTypeRepoConstructingDataType()
				.MaterialDefinitionRepoHavingDefinition("Clay", "columnName")
				.SystemUnderTest();

			var result = await sut.Patch(input);

			result.Should().BeAssignableTo<OkResult>();
		}

		[Fact]
		public async void Put_ShouldThrowBadRequest_WhenMaterialDtoHasInfoWhichIsAlreadyExist()
		{
			const string headerName = "Header1";
			var header = new HeaderDefinitionDtoStub()
				.WithNameAs(headerName);
			var input = new MaterialDefinitionDtoStub()
				.WithObjectIdAs(ObjectId.GenerateNewId(DateTime.Today))
				.WithHeaderAs(header.Stub())
				.WithNameAs(MaterialDefinitionCode);
			var headerDefinition = new HeaderDefinitionStub()
				.WithNameAs(headerName);
			var materialDefinitionStub = new MaterialDefinitionStub()
				.HavingHeader(headerDefinition.Stub());

			var fixture = new Fixture().DataTypeRepoConstructingDataType()
				.FindOfMaterialDefinitionRepoReturns(MaterialDefinitionCode, materialDefinitionStub.Stub());

			var result = await fixture.SystemUnderTest().Patch(input.Stub());

			result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
		}

		[Fact]
		public async Task Put_ShouldUpdateMaterial_WhenPassedDtoWithNewHeader()
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
			var input = new MaterialDefinitionDtoStub()
				.WithObjectIdAs(ObjectId.GenerateNewId(DateTime.Today))
				.WithHeaderAs(header.Stub())
				.WithNameAs(MaterialDefinitionCode);
			var columnDifinition = new ColumnDefinitionStub()
				.HavingNameAs("column2");
			var headerDefinition = new HeaderDefinitionStub()
				.WithNameAs(headerName)
				.WithColumnAs(columnDifinition.Stub());
			var materialDefinition = new MaterialDefinitionStub()
				.HavingHeader(headerDefinition.Stub());
			var fixture = new Fixture();
			var mockRepo = fixture.DataTypeRepoConstructingDataType().GetMaterialDefinitionRepository();
			mockRepo.Setup(m => m.Find(MaterialDefinitionCode)).ReturnsAsync(materialDefinition.Stub());

			var genericMaterial = GetGenericMaterial();
			genericMaterial.Headers.Add(new HeaderDefinition("Header1", "Header1",
				new List<ColumnDefinition>
				{
					new ColumnDefinition("column1","column1", new ConstantDataType("Primary")),
					new ColumnDefinition("column2","column1", new ConstantDataType("Primary"))
				}));

			await fixture.Accepting(genericMaterial).SystemUnderTest().Patch(input.Stub());

			mockRepo.Verify(
				m =>
					m.Update(
						It.Is<IMaterialDefinition>(
							d => d.Headers.First(h => h.Name == headerName).Columns.Any(c => c.Name == "column2"))));
		}

		private static MaterialDefinitionDao GetInput()
		{
			return new MaterialDefinitionDao
			{
				Name = "Clay",
				Code = "CLY",
				Headers = new List<HeaderDefinitionDto>
				{
					new HeaderDefinitionDto
					{
						Name = "Classification",
						Key = "classification",
						Columns = new List<ColumnDefinitionDto>
						{
							new ColumnDefinitionDto
							{
								Name = "Material Level 1",
								Key = "Material Level 1",
								DataType = new DataTypeDto
								{
									Name = "Constant",
									SubType = "primary"
								}
							},
							new ColumnDefinitionDto
							{
								Key = "Material Level 3",
								Name = "Material Level 3",
								DataType = new DataTypeDto
								{
									Name = "MasterData",
									SubType = "Clay_Material_Level_3"
								}
							}
						}
					}
				}
			};
		}

		private static IMaterialDefinition MaterialDefinitionWithOneHeader(string materialCode, string headerName)
		{
			return new MaterialDefinitionStub()
				.HavingHeader(new HeaderDefinitionStub().WithNameAs(headerName).Stub())
				.HavingCode(materialCode).Stub();
		}

		private MaterialDefinition GetGenericMaterial()
		{
			return new MaterialDefinition("Generic Material")
			{
				Code = "GNR",
				Headers = new List<IHeaderDefinition>
				{
					new HeaderDefinition("Classification", "classification", new List<ColumnDefinition>
					{
						new ColumnDefinition("Material Level 1", "Material Level 1", new ConstantDataType("Primary")),
						new ColumnDefinition("Material Level 3", "Material Level 3",new ConstantDataType("Primary")),
						new ColumnDefinition("SomeColumn", "SomeColumn",new ConstantDataType("Primary"))
					}),
					new HeaderDefinition("Purchase","Purchase", new List<IColumnDefinition>
					{
						new ColumnDefinition("Approved Brands","Approved Brands",
							new BrandDataType(new BrandDefinition("Generic Brand", new List<ISimpleColumnDefinition>()), "BCY",
							new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object))
					})
				}
			};
		}

		private MaterialDefinitionDao GetInputWhichDoesNotMatchColumnMappingWithGenericMaterial()
		{
			return new MaterialDefinitionDao
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
								Name = "Material Level 1",
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

		private MaterialDefinition GetMaterialDefinition()
		{
			return new MaterialDefinition("Clay")
			{
				Code = "GNR",
				Headers = new List<IHeaderDefinition>
				{
					new HeaderDefinition("Classification","Classification", new List<ColumnDefinition>
					{
						new ColumnDefinition("Material Level 1","Material Level 1", new ConstantDataType("Primary")),
						new ColumnDefinition("Material Level 3","Material Level 3", new ConstantDataType("Primary")),
						new ColumnDefinition("SomeColumn","SomeColumn", new ConstantDataType("Primary"))
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

			private readonly Mock<IComponentDefinitionRepository<IMaterialDefinition>> _repositoryMock =
				new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();

			public Fixture()
			{
				_masterDataList = new MasterDataList("Clay_Material_Level_3", new List<MasterDataValue>());
				_mockDataTypeFactory.Setup(m => m.Construct("Constant", "primary"))
					.ReturnsAsync(new ConstantDataType("primary"));
				_mockDataTypeFactory.Setup(m => m.Construct("MasterData", "Clay_Material_Level_3"))
					.ReturnsAsync(new ConstantDataType("primary"));
			}

			public Fixture Accepting(MaterialDefinition input)
			{
				_masterDataRepository.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(_masterDataList);

				_repositoryMock.Setup(m => m.Add(It.IsAny<MaterialDefinition>())).Returns(Task.CompletedTask);
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

			public Fixture FindOfMaterialDefinitionRepoReturns(string code, IMaterialDefinition definition)
			{
				_repositoryMock.Setup(r => r.Find(code)).ReturnsAsync(definition);
				return this;
			}

			public Mock<IComponentDefinitionRepository<IMaterialDefinition>> GetMaterialDefinitionRepository()
			{
				return _repositoryMock;
			}

			public Fixture MaterialDefinitionRepo_Find_ShouldThrowResourceNotFound()
			{
				_repositoryMock.Setup(m => m.Find(It.IsAny<string>())).ThrowsAsync(new ResourceNotFoundException(""));
				return this;
			}

			public Fixture MaterialDefinitionRepoHavingDefinition(string materialCode, string headerCode)
			{
				_repositoryMock.Setup(m => m.Find(materialCode))
					.ReturnsAsync(MaterialDefinitionWithOneHeader(materialCode, headerCode));
				return this;
			}

			public Fixture MaterialDefinitionRepoPatchShouldBeCalledWith(string materialdefinitionCode)
			{
				_expectation.Add(() => _repositoryMock.Verify(r => r.Find(materialdefinitionCode), Times.Once));
				return this;
			}

			public MaterialDefinitionController SystemUnderTest()
			{
				return new MaterialDefinitionController(_repositoryMock.Object, _mockDataTypeFactory.Object,
					new Mock<IDependencyDefinitionRepository>().Object, new Mock<ICodePrefixTypeMappingRepository>().Object);
			}

			public void VerifyExpectations()
			{
				_expectation.ForEach(e => e.Invoke());
			}

			public Fixture WithExisting(IComponentDefinition input)
			{
				_repositoryMock.Setup(m => m.Add(It.Is<MaterialDefinition>(k => k.Name == input.Name)))
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

		private class MaterialDefinitionDtoStub
		{
			private readonly List<HeaderDefinitionDto> _header = new List<HeaderDefinitionDto>();
			private readonly MaterialDefinitionDao _stub = new MaterialDefinitionDao();
			private string _name;

			public MaterialDefinitionDao Stub()
			{
				_stub.Headers = _header;
				_stub.Name = _name;
				return _stub;
			}

			public MaterialDefinitionDtoStub WithHeaderAs(HeaderDefinitionDto stub)
			{
				_header.Add(stub);
				return this;
			}

			public MaterialDefinitionDtoStub WithNameAs(string name)
			{
				_name = name;
				return this;
			}

			public MaterialDefinitionDtoStub WithObjectIdAs(ObjectId objectId)
			{
				_stub.ObjectId = objectId;
				return this;
			}
		}

		private class MaterialDefinitionStub
		{
			private readonly List<IHeaderDefinition> _headers = new List<IHeaderDefinition>();
			private readonly Mock<IMaterialDefinition> _stub = new Mock<IMaterialDefinition>();

			public MaterialDefinitionStub HavingCode(string materialCode)
			{
				_stub.Setup(s => s.Code).Returns(materialCode);
				return this;
			}

			public MaterialDefinitionStub HavingHeader(IHeaderDefinition header)
			{
				_headers.Add(header);
				return this;
			}

			public IMaterialDefinition Stub()
			{
				_stub.Setup(s => s.Headers).Returns(_headers);
				return _stub.Object;
			}
		}
	}
}