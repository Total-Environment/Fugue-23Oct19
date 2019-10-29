using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
    public class OldMaterialControllerTests
    {
        [Fact]
        public async void Get_ShouldReturn200OkWithMaterial_WhenCalledWithExistingId()
        {
            const string id = "CLY00001";
            var sut = new Fixture().WithExisting(id).SystemUnderTest();
            var result = await sut.Get(id);
            result.Should().BeOfType<OkNegotiatedContentResult<MaterialDto>>();
            var parsedResult = (OkNegotiatedContentResult<MaterialDto>)result;
            parsedResult.Content.Material.Id.Should().Be(id);
        }

        [Fact]
        public async void Get_ShouldReturn200OkWithMaterial_WhenCalledWithExistingIdAndDoesnotPassDataType()
        {
            const string id = "CLY00001";
            var sut = new Fixture().WithExisting(id).SystemUnderTest();
            var result = await sut.Get(id);
            result.Should().NotBeOfType<OkNegotiatedContentResult<MaterialWithDataTypeDto>>();
        }

        [Fact]
        public async void Get_ShouldReturn200OkWithMaterialContainsDataTypes_WhenCalledWithExistingIdAndDataTypeAsTrue()
        {
            const string id = "CLY00001";
            var sut = new Fixture().WithExisting(id).SystemUnderTest();
            var result = await sut.Get(id, true);
            result.Should().BeOfType<OkNegotiatedContentResult<MaterialWithDataTypeDto>>();
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
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition>
                {
                    new HeaderDefinition("General","general",
                        new List<IColumnDefinition> {new ColumnDefinition("Image", "image", new StaticFileDataType())})
                });
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var materials = new List<IMaterial>
            {
                new Material(
                    new List<IHeaderData>
                    {
                        new HeaderData("General","general") {Columns = new List<IColumnData> {new ColumnData("Image", "image", "Image")}}
                    },
                    new MaterialDefinition("Clay"){ Headers = new List<IHeaderDefinition>
                    {
                        new HeaderDefinition("General","general", new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Image","image",new StringDataType())
                        })
                    }})
            };
            var fixture =
                new Fixture().WithMockDefinition(mockDefinition.Object)
                    .WithStubbedMaterialsByGroupAndColumnName(materials);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Clay", "image", 1, 10);
            result.Should().BeAssignableTo<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var parsedResult = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<MaterialDocumentDto>)parsedResult.Content["items"];
            items.Should().HaveCount(1);
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn400BadRequest_WhenColumnNameIsNotStaticOrCheckDataType()
        {
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition>
                {
                    new HeaderDefinition("General","General",
                        new List<IColumnDefinition> {new ColumnDefinition("Image", "Image", new StringDataType())})
                });
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Clay", "image", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo(
                "image is neithter static file data type nor check list data type.");
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn400BadRequest_WhenColumnNameNotFound()
        {
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition>()) });
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Clay", "image", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo(
                "image is not valid column in the material definition of Clay group.");
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldReturn400BadRequest_WhenGroupNotFound()
        {
            var fixture = new Fixture().WithoutMockDefinition("Clay");
            var result = await fixture.SystemUnderTest().GetByGroupAndColumnName("Clay", "image", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("Clay not found.");
        }

        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWord_ShouldReturn200Ok()
        {
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition>
                {
                    new HeaderDefinition("General","General",
                        new List<IColumnDefinition> {new ColumnDefinition("Image", "Image", new StaticFileDataType())})
                });
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var materials = new List<IMaterial>
            {
                new Material(
                    new List<IHeaderData>
                    {
                        new HeaderData("General","general") {Columns = new List<IColumnData> {new ColumnData("Image", "image", "Image")}}
                    },
                    new MaterialDefinition("Clay"){ Headers = new List<IHeaderDefinition>
                    {
                        new HeaderDefinition("General","general", new List<IColumnDefinition>
                        {
                            new ColumnDefinition("Image","image",new StringDataType())
                        })
                    }}) {Id = "CLY000001"}
            };
            var fixture =
                new Fixture().WithMockDefinition(mockDefinition.Object)
                    .WithStubbedMaterialsByGroupAndColumnNameAndKeyWord(materials);
            var result = await fixture.SystemUnderTest()
                .GetByGroupAndColumnNameAndKeyWord("Clay", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var parsedResult = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<MaterialDocumentDto>)parsedResult.Content["items"];
            items.Should().HaveCount(1);
        }

        [Fact]
        public async void
            GetByGroupAndColumnNameAndKeyWord_ShouldReturn400BadRequest_WhenColumnNameIsNotStaticOrCheckDataType()
        {
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition>
                {
                    new HeaderDefinition("General","General",
                        new List<IColumnDefinition> {new ColumnDefinition("Image", "Image", new StringDataType())})
                });
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest()
                .GetByGroupAndColumnNameAndKeyWord("Clay", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo(
                "image is neithter static file data type nor check list data type.");
        }

        /// <summary>
        /// Gets the by group and column name and key word should return400 bad request when column
        /// name not found.
        /// </summary>
        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWord_ShouldReturn400BadRequest_WhenColumnNameNotFound()
        {
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Headers)
                .Returns(new List<IHeaderDefinition> { new HeaderDefinition("General", "General", new List<IColumnDefinition>()) });
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object);
            var result = await fixture.SystemUnderTest()
                .GetByGroupAndColumnNameAndKeyWord("Clay", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo(
                "image is not valid column in the material definition of Clay group.");
        }

        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWord_ShouldReturn400BadRequest_WhenGroupNotFound()
        {
            var fixture = new Fixture().WithoutMockDefinition("Clay");
            var result = await fixture.SystemUnderTest()
                .GetByGroupAndColumnNameAndKeyWord("Clay", "image", "CLY000001", 1, 10);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var parsedResult = (BadRequestErrorMessageResult)result;
            parsedResult.Message.ShouldBeEquivalentTo("Clay not found.");
        }

        [Fact]
        public async void Post_SearchWithinGroup_ShouldThrowBadRequest_WhenkeywordIsNullOrLessThan3Letter()
        {
            var fixture = new Fixture();
            const string groupname = "groupName";
            const string nullKeyword = null;
            const string twoLetterKeyword = "ab";

            var searchWithinGroupRequest = new MaterialSearchRequest
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
        public async void Post_ShouldCallUpdateCounter_WhenCounterValueIsLessThanMaterialCode()
        {
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"HSM Code", "HSM Code"},
                        {"Material Code", "CLY000010"}
                    }
                }
            };

            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var fixture = new Fixture()
                .WithMockDefinition(mockDefinition.Object)
                .WithCounterCurrentvalue(9, "Material")
                .ShouldCallUpdateOfCounterRepository(10, "Material");
            var result = await fixture.SystemUnderTest().Post(material);

            fixture.VerifyExpectations();
            result.Should().BeOfType<CreatedNegotiatedContentResult<MaterialDto>>();
        }

        [Fact]
        public async void Post_ShouldReturn201Created_WhenPassedADictionary()
        {
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"HSM Code", "HSM Code"},
                        {"Material Code", "CLY000010"}
                    }
                }
            };
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<BrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            const string id = "CLY000010";
            var fixture =
                new Fixture().WithMockDefinition(mockDefinition.Object).WithCounter(1, "Material").ShouldAddId(id);
            var result = await fixture.SystemUnderTest().Post(material);
            result.Should().BeOfType<CreatedNegotiatedContentResult<MaterialDto>>();
            var parsedResult = (CreatedNegotiatedContentResult<MaterialDto>)result;
            var parsedResultContent = parsedResult.Content.Material;
            parsedResultContent.Id.Should().Be(id);
            parsedResultContent.SearchKeywords.Should().Contain(id);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenClassificationHeaderIsNotPassed()
        {
            var material = new Dictionary<string, object>();
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Post(material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Classification was not found.");
        }

        /// <summary>
        /// Posts the should return400 bad request when given group in material code does not match
        /// with definition.
        /// </summary>
        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenGivenGroupInMaterialCodeDoesNotMatchWithDefinition()
        {
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"HSM Code", "HSM Code"},
                        {"Material Code", "SFT000010"}
                    }
                }
            };

            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result = await sut.Post(material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should()
                .Be("Group code of material Code: SFT000010 is invalid.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenGivenMaterialCodeIsNotValid()
        {
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"HSM Code", "HSM Code"},
                        {"Material Code", "CLY010"}
                    }
                }
            };

            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result = await sut.Post(material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("material Code: CLY010 is invalid.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenMaterialDefinitionIsNotFound()
        {
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                }
            };
            var sut = new Fixture().WithoutMockDefinition("Clay").SystemUnderTest();
            var result = await sut.Post(material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Clay not found.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenMaterialLevel2IsNotPassed()
        {
            var material = new Dictionary<string, object> { { "Classification", new Dictionary<string, object>() } };
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Post(material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Material Level 2 was not found.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenNullIsPassed()
        {
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Post(null);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Request is null.");
        }

        [Fact]
        public async void Post_ShouldReturn400BadRequest_WhenParseFails()
        {
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .Throws(new FormatException("Error message"));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result =
                await
                    sut.Post(new Dictionary<string, object>
                    {
                        {"Classification", new Dictionary<string, object> {{"Material Level 2", "Clay"}}},
                        {"General", new Dictionary<string, object> {{"Can be Used as an Asset", false}}}
                    });
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Error message");
        }

        [Fact]
        public async void Put_ShouldReturn200WithUpdatedMaterial_WhenPassedMaterial()
        {
            var materialId = "CLY000043";
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"Can be Used as an Asset", false}
                    }
                }
            };
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar")));
            const string id = "CLY000001";
            var fixture =
                new Fixture().WithMockDefinition(mockDefinition.Object)
                    .WithExisting(id)
                    .ShouldUpdate(new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar")));
            var result = await fixture.SystemUnderTest().Put(materialId, material);
            result.Should().BeOfType<OkNegotiatedContentResult<MaterialDto>>();
            var parsedResult = (OkNegotiatedContentResult<MaterialDto>)result;
            parsedResult.Content.Should().NotBeNull();
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenClassificationHeaderIsNotPassed()
        {
            var materialId = "CLY000043";
            var material = new Dictionary<string, object>();
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Put(materialId, material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Classification was not found.");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenMaterialDefinitionIsNotFound()
        {
            var materialId = "CLY000043";
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                }
            };
            var sut = new Fixture().WithoutMockDefinition("Clay").SystemUnderTest();
            var result = await sut.Put(materialId, material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Clay not found.");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenMaterialLevel2IsNotPassed()
        {
            var materialId = "CLY000043";
            var material = new Dictionary<string, object> { { "Classification", new Dictionary<string, object>() } };
            var sut = new Fixture().SystemUnderTest();
            var result = await sut.Put(materialId, material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Material Level 2 was not found.");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenParseFails()
        {
            var materialId = "CLY000043";
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .Throws(new FormatException("Error message"));
            var sut = new Fixture().WithMockDefinition(mockDefinition.Object).SystemUnderTest();
            var result =
                await
                    sut.Put(materialId,
                        new Dictionary<string, object>
                        {
                            {"Classification", new Dictionary<string, object> {{"Material Level 2", "Clay"}}},
                            {"General", new Dictionary<string, object> {{"Can be Used as an Asset", false}}}
                        });
            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Error message");
        }

        [Fact]
        public async void Put_ShouldReturn400BadRequest_WhenPassedNonExistingMaterial()
        {
            var materialId = "CLY000043";
            var material = new Dictionary<string, object>
            {
                {
                    "Classification", new Dictionary<string, object>
                    {
                        {"Material Level 1", "Primary"},
                        {"Material Level 2", "Clay"},
                        {"Material Level 3", "Soil"},
                        {"Material Level 4", "Filler"},
                        {"Material Level 5", "Vitrified"}
                    }
                },
                {
                    "General", new Dictionary<string, object>
                    {
                        {"Can be Used as an Asset", false}
                    }
                }
            };
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
            const string id = "CLY000001";
            var fixture = new Fixture().WithMockDefinition(mockDefinition.Object).WithoutExisting(id).WithoutUpdating();
            var result = await fixture.SystemUnderTest().Put(materialId, material);
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Search_ForInvalidMaterialLevel2_ReturnBadRequest()
        {
            var fixture = new Fixture().WithStubbedInvalidMaterialLevel2();
            var materialLevel2 = "Clay Material";
            var searchValue = "Clay";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1, materialLevel2);

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        public async void Search_ForNullParameters_ReturnBadRequest()
        {
            var fixture = new Fixture().WithStubbedInvalidMaterialLevel2();

            var result = await fixture.SystemUnderTest().Search(null, 1, null);

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Search_ForSortParameters_ReturnSortedData()
        {
            var materialList = new List<IMaterial>
            {
                new Material(new List<IHeaderData>(), new MaterialDefinition("a")),
                new Material(new List<IHeaderData>(), new MaterialDefinition("a")),
                new Material(new List<IHeaderData>(), new MaterialDefinition("a"))
            };
            const string sortKey = "Material Name";
            var sortOrder = SortOrder.Descending;
            var fixture = new Fixture().WithStubbedSearchValues(materialList, sortKey, sortOrder);
            var materialLevel2 = "Clay Material";
            var searchValue = "Clay";

            var result = await fixture.SystemUnderTest()
                .Search(searchValue, 1, materialLevel2, sortOrder: sortOrder, sortColumn: sortKey);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var resultContents = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<MaterialSearchDto>)resultContents.Content["items"];
            ((string)resultContents.Content["sortColumn"]).Should().Be(sortKey);
            ((string)resultContents.Content["sortOrder"]).Should().Be(sortOrder.ToString());
            items.Should().HaveCount(3);
        }

        [Fact]
        public async void Search_ForValidSearchParameters_ReturnNoData()
        {
            var materialList = new List<IMaterial>();
            var fixture = new Fixture().WithStubbedSearchValues(materialList);
            var materialLevel2 = "Clay Material";
            var searchValue = "Clay";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1, materialLevel2);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var resultContents = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<MaterialSearchDto>)resultContents.Content["items"];
            items.Count.Should().Be(0);
            ((long)resultContents.Content["recordCount"]).Should().Be(0);
        }

        [Fact]
        public async void Search_ForValidSearchParameters_ReturnRelevantMaterial()
        {
            const string firstMaterialId = "CLY000001";
            const string secondMaterialId = "CLY000015";
            var firstMaterial = GetMaterial(firstMaterialId);
            var secondMaterial = GetMaterial(secondMaterialId);
            var materialList = new List<IMaterial> { firstMaterial, secondMaterial };
            var fixture = new Fixture().WithStubbedSearchValues(materialList);
            var materialLevel2 = "Clay Material";
            var searchValue = "Clay";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1, materialLevel2);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
            var resultContents = (OkNegotiatedContentResult<Dictionary<string, object>>)result;
            var items = (List<MaterialSearchDto>)resultContents.Content["items"];
            items.Count.Should().Be(2);
            items[0].MaterialCode.Should().Be(firstMaterialId);
            items[1].MaterialCode.Should().Be(secondMaterialId);
        }

        [Fact]
        public async void Search_ForWithoutMaterialLevel_ReturnAcceptRequest()
        {
            var materialList = new List<IMaterial>();
            var fixture = new Fixture().WithStubbedSearchValues(materialList);
            var materialLevel2 = "Clay Material";
            var searchValue = "Clay";

            var result = await fixture.SystemUnderTest().Search(searchValue, 1);

            result.Should().BeOfType<OkNegotiatedContentResult<Dictionary<string, object>>>();
        }

        [Theory]
        [InlineData("Clay Material", "S")]
        [InlineData("Clay Material", "Sa")]
        [InlineData("Clay Material", "Sa pa")]
        public async Task Search_ShouldNotAllowSearchKeywordToBeLessThanThreeCharacters_ReturnBadRequest(
            string serviceLevel1, string searchKeyword)
        {
            var result = await new Fixture().SystemUnderTest().Search(searchKeyword, 1, serviceLevel1);

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
        public async void SearchWithinGroup_ShouldThrowNotFound_WhenRepothrowResourceNotfoundException()
        {
            var fixture = new Fixture().RepoSearchThrowResourceNotFoundException();
            const string groupname = "group";
            const string keyword = "keyword";

            var searchWithinGroupRequest = new MaterialSearchRequest
            {
                GroupName = groupname,
                SearchQuery = keyword
            };
            var result = await fixture.SystemUnderTest().Post_SearchWithinGroup(searchWithinGroupRequest);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn200WithUpdatedMaterialCode_WhenPassedRateData()
        {
            // Arrange
            const string id = "CLY000001";
            var mockDefinition = new Mock<IMaterialDefinition>();
            mockDefinition.Setup(m => m.Code).Returns("CLY");
            mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar")));
            var fixture =
                new Fixture().WithMockDefinition(mockDefinition.Object)
                    .WithExisting(id)
                    .ShouldUpdate(new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar")));
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", "INR" } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<OkNegotiatedContentResult<string>>();
            var parsedResult = (OkNegotiatedContentResult<string>)result;
            parsedResult.Content.Should()
                .BeEquivalentTo("Updated rate of material with materialCode: " + updateMaterialRateRequest.MaterialCode);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenExceptionThrownWhileUpdating()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithExisting(id).WithoutUpdating();
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", "INR" } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenPassedInvalidUpdateMaterialRateRequest()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithExisting(id);
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenPassedNonExistingMaterialCode()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithoutExisting(id);
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", "INR" } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenPassedWithoutAmount()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithExisting(id);
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", null }, { "Currency", "USD" } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenPassedWithoutCurrency()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithExisting(id);
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", null } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenPassedWithoutGeneralHeader()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithExistingWithoutGeneralHeader(id);
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", "INR" } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void UpdateRate_ShouldReturn400BadRequest_WhenPassedWithoutGeneralHeaderRateColumns()
        {
            // Arrange
            const string id = "CLY000001";
            var fixture = new Fixture().WithExistingWithoutGeneralHeaderRateColumns(id);
            var updateMaterialRateRequest =
                new UpdateMaterialRateRequest
                {
                    MaterialCode = id,
                    LastPurchaseRate = new Dictionary<string, object> { { "Amount", "100.2" }, { "Currency", "INR" } },
                    WeightedAveragePurchaseRate =
                        new Dictionary<string, object> { { "Amount", "150.36" }, { "Currency", "INR" } }
                };

            // Act
            var result = await fixture.SystemUnderTest().UpdateRate(updateMaterialRateRequest);

            // Assert
            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        private static MaterialDataDto MaterialRequestStub(string materialGroup)
        {
            return new MaterialDataDto
            {
                Headers = new List<HeaderDto>
                {
                    new HeaderDto
                    {
                        Name = "General",
                        Columns = new List<ColumnDto>
                        {
                            new ColumnDto
                            {
                                Name = "ColumnName",
                                Value = "someValue"
                            }
                        }
                    }
                },
                Group = materialGroup
            };
        }

        private Material GetMaterial(string id)
        {
            return new Material(new List<IHeaderData>
                {
                    new HeaderData("Purchase","Purchase")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Last Purchase Rate","Last Purchase Rate", "50.11"),
                            new ColumnData("Weighted Average Purchase Rate","Weighted Average Purchase Rate", "71.34")
                        }
                    },
                    new HeaderData("Classification","Classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2","Material Level 2", "Clay")
                        }
                    },
                    new HeaderData("General","General")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Code","Material Code", id)
                        }
                    }
                }, new MaterialDefinition("Clay"))
            { Id = id };
        }

        public class BrandDefinitionContext
        {
            private Dictionary<string, object> material = new Dictionary<string, object>();

            public BrandDefinitionContext()
            {
                material = new Dictionary<string, object>
                {
                    {
                        "Classification", new Dictionary<string, object>
                        {
                            {"Material Level 1", "Primary"},
                            {"Material Level 2", "Clay"},
                            {"Material Level 3", "Soil"},
                            {"Material Level 4", "Filler"},
                            {"Material Level 5", "Vitrified"}
                        }
                    },
                    {
                        "General", new Dictionary<string, object>
                        {
                            {"HSM Code", "HSM Code"},
                            {"Material Code", "CLY000010"}
                        }
                    },
                    {
                        "Purchase", new Dictionary<string, object>
                        {
                            {
                                "Approved Brands", new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        {"manufacturer_name", "Dupont"},
                                        {"series", "Corian"},
                                        {"model", "NA"}
                                    }
                                }
                            }
                        }
                    }
                };
            }

            [Fact]
            public async void Post_ShouldGetBrandDefintion_WhenBrandDataIsPassed()
            {
                var mockDefinition = new Mock<IMaterialDefinition>();
                mockDefinition.Setup(m => m.Code).Returns("CLY");
                mockDefinition.Setup(m => m.Parse<Material>(It.IsAny<Dictionary<string, object>>(), It.IsAny<IBrandDefinition>()))
                    .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")));
                var fixture =
                    new Fixture().WithMockDefinition(mockDefinition.Object)
                        .WithCounter(1, "Material")
                        .ExpectingACallToBrandDefinitionRepository();

                await fixture.SystemUnderTest().Post(material);

                fixture.VerifyExpectations();
            }

            // [Fact] public async void Post_ShouldFillBrandData_WhenBrandDataIsPassed() { var
            // brandDefinition = new Mock<IBrandDefinition>(); IColumnDefinition columnDefinition =
            // new ColumnDefinition("Manufacturer's Name", new StringDataType(), key:
            // "manufacturer_name"); brandDefinition.Setup(b => b.Columns).Returns(new
            // List<IColumnDefinition>() { columnDefinition }); brandDefinition.Setup(b =>
            // b.Parse(It.IsAny<Dictionary<string, object>>())) .ReturnsAsync(new
            // BrandColumnData("manufacturer_name", "Dupoint"));
            //
            // var mockDefinition = new Mock<IMaterialDefinition>(); mockDefinition.Setup(m =>
            // m.Code).Returns("CLY"); mockDefinition.Setup(m =>
            // m.Parse<Material>(It.IsAny<Dictionary<string, object>>())) .ReturnsAsync(new
            // Material(new List<IHeaderData>(), new MaterialDefinition("Clay"))); var fixture = new
            // Fixture().WithMockDefinition(mockDefinition.Object) .WithCounter(1, "Material")
            // .WithBrandDefinition(brandDefinition.Object).
            // ExpectingACallToMaterialRepositoryWithExpectation((MaterialData m)=> m.);
            //
            // var result = await fixture.SystemUnderTest().Post(material);
            //
            // var materialData = ((CreatedNegotiatedContentResult<MaterialDto>)result).Content;
            // materialData.Material.Headers.SingleOrDefault(h => h.Name == "Purchase") .Should() .NotBe(default(IHeaderData));
            //
            // materialData.Material.Headers.SingleOrDefault(h => h.Name == "Purchase")
            // .Columns.SingleOrDefault(c => c.Name == "Approved Brands") .Should()
            // .NotBe(default(IColumnData)); var brandData = (Dictionary<string,
            // object>)materialData.Material.Headers.SingleOrDefault(h => h.Name == "Purchase")
            // .Columns.SingleOrDefault(c => c.Name == "Approved Brands").Value;
            // brandData.Count().Should().Be(1);
            // brandData["manufacturer_name"].Should().Be("Dupont"); }
        }

        private class Fixture
        {
            private readonly Mock<ICounterRepository> _counterRepository = new Mock<ICounterRepository>();

            private readonly Mock<IBrandDefinitionRepository> _mockBrandDefinitionRepository = new Mock<IBrandDefinitionRepository>();

            private readonly Mock<IComponentDefinitionRepository<IMaterialDefinition>> _mockDefinitionRepository =
                            new Mock<IComponentDefinitionRepository<IMaterialDefinition>>();

            private readonly Mock<IFilterCriteriaBuilder> _mockFilterCriteriaBuilder = new Mock<IFilterCriteriaBuilder>();
            private readonly Mock<IMasterDataRepository> _mockMasterDataRepository = new Mock<IMasterDataRepository>();

            private readonly Mock<IMaterialRepository> _mockMaterialRepository =
                new Mock<IMaterialRepository>();

            private readonly Mock<ISapSyncer> _mockSAPSyncer = new Mock<ISapSyncer>();
            private readonly List<Action> _verifications = new List<Action>();

            public Fixture ExpectingACallToBrandDefinitionRepository()
            {
                _verifications.Add(() => _mockBrandDefinitionRepository.Verify(m => m.FindBy("Generic Brand")));
                return this;
            }

            public Fixture RepoSearchInGroupCalledOnceWith(string keyword)
            {
                Action action =
                    () => _mockMaterialRepository.Verify(m => m.SearchInGroup(new List<string> { keyword }), Times.Once);
                _verifications.Add(action);
                return this;
            }

            public Fixture RepoSearchInGroupThrowsResourceNotFoundException()
            {
                _mockMaterialRepository.Setup(m => m.SearchInGroup(It.IsAny<List<string>>()))
                    .Throws(new ResourceNotFoundException(""));
                return this;
            }

            public Fixture RepoSearchThrowResourceNotFoundException()
            {
                _mockMaterialRepository.Setup(
                        m =>
                            m.Search(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                                It.IsAny<string>(), It.IsAny<SortOrder>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));

                _mockMaterialRepository.Setup(
                        m =>
                            m.Search(It.IsAny<Dictionary<string, Tuple<string, object>>>(), It.IsAny<int>(), It.IsAny<int>(),
                                It.IsAny<string>(), It.IsAny<SortOrder>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));

                return this;
            }

            public Fixture ShouldAddId(string id)
            {
                _mockMaterialRepository.Setup(m => m.Add(It.IsAny<Material>())).Returns(Task.CompletedTask);
                _verifications.Add(() => _mockMaterialRepository.Verify(m => m.Add(It.IsAny<Material>()), Times.Once));
                _mockMaterialRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(new Material(new List<IHeaderData>(), new MaterialDefinition("Clay")) { Id = id });
                return this;
            }

            public Fixture ShouldCallUpdateOfCounterRepository(int countervalue, string key)
            {
                Action action =
                    () => _counterRepository.Verify(c => c.Update(countervalue, key), Times.Once);
                _verifications.Add(action);
                return this;
            }

            public Fixture ShouldUpdate(Material material)
            {
                _mockMaterialRepository.Setup(m => m.Update(It.IsAny<Material>())).Returns(Task.CompletedTask);
                _verifications.Add(() => _mockMaterialRepository.Verify(m => m.Update(It.IsAny<Material>()), Times.Once));
                return this;
            }

            public OldMaterialController SystemUnderTest()
            {
                return new OldMaterialController(_mockDefinitionRepository.Object, _counterRepository.Object,
                    _mockMaterialRepository.Object, _mockMasterDataRepository.Object,
                    _mockSAPSyncer.Object, new Mock<IComponentDefinitionRepository<AssetDefinition>>().Object,
                    _mockBrandDefinitionRepository.Object, _mockFilterCriteriaBuilder.Object);
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithCounter(int counter, string key)
            {
                _counterRepository.Setup(r => r.NextValue(key)).ReturnsAsync(counter);
                return this;
            }

            public Fixture WithCounterCurrentvalue(int counter, string key)
            {
                _counterRepository.Setup(r => r.CurrentValue(key)).ReturnsAsync(counter);
                return this;
            }

            public Fixture WithExisting(string id)
            {
                var material = new Material(new List<IHeaderData>
                {
                    new HeaderData("Purchase","Purchase")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Last Purchase Rate","Last Purchase Rate",  "50.11"),
                            new ColumnData("Wt. Avg. Purchase Rate", "Weighted Average Purchase Rate","71.34")
                        }
                    },
                    new HeaderData("Classification","Classification")
                    {
                        Columns = new List<IColumnData>
                        {
                            new ColumnData("Material Level 2","Material Level 2", "Clay")
                        }
                    }
                }, new MaterialDefinition("Clay"))
                {
                    Id = id
                };
                _mockMaterialRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(material);

                return this;
            }

            public Fixture WithExistingWithoutGeneralHeader(string id)
            {
                _mockMaterialRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(
                        new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar"))
                        { Id = id });

                return this;
            }

            public Fixture WithExistingWithoutGeneralHeaderRateColumns(string id)
            {
                _mockMaterialRepository.Setup(m => m.Find(id))
                    .ReturnsAsync(
                        new Material(new List<IHeaderData>
                            {
                                new HeaderData("General","General")
                            }, new MaterialDefinition("Sattar"))
                        { Id = id });

                return this;
            }

            public Fixture WithMockDefinition(IMaterialDefinition materialDefinition)
            {
                _mockDefinitionRepository.Setup(m => m.Find("Clay")).ReturnsAsync(materialDefinition);
                return this;
            }

            public Fixture WithoutExisting(string id)
            {
                _mockMaterialRepository.Setup(m => m.Find(id)).Throws(new ResourceNotFoundException("Not found"));
                return this;
            }

            public Fixture WithoutMockDefinition(string materialLevel2)
            {
                _mockDefinitionRepository.Setup(m => m.Find(materialLevel2))
                    .Throws(new ResourceNotFoundException(materialLevel2));
                return this;
            }

            public Fixture WithoutUpdating()
            {
                _mockMaterialRepository.Setup(m => m.Update(It.IsAny<Material>()))
                    .Throws(new ResourceNotFoundException("a"));
                return this;
            }

            public Fixture WithStubbedInvalidMaterialLevel2()
            {
                _mockMasterDataRepository.Setup(m => m.Exists(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(false);

                return this;
            }

            public Fixture WithStubbedMaterialsByGroupAndColumnName(List<IMaterial> materials)
            {
                _mockMaterialRepository.Setup(
                        m => m.GetTotalCountByGroupAndColumnName(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(materials.Count);

                _mockMaterialRepository.Setup(
                        m =>
                            m.GetByGroupAndColumnName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                                It.IsAny<int>()))
                    .ReturnsAsync(materials);

                return this;
            }

            public Fixture WithStubbedMaterialsByGroupAndColumnNameAndKeyWord(List<IMaterial> materials)
            {
                _mockMaterialRepository.Setup(
                        m =>
                            m.GetTotalCountByGroupAndColumnNameAndKeyWords(It.IsAny<string>(), It.IsAny<string>(),
                                It.IsAny<List<string>>()))
                    .ReturnsAsync(materials.Count);

                _mockMaterialRepository.Setup(
                        m =>
                            m.GetByGroupAndColumnNameAndKeyWords(It.IsAny<string>(), It.IsAny<string>(),
                                It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(materials);

                return this;
            }

            public Fixture WithStubbedSearchValues(List<IMaterial> material)
            {
                _mockMasterDataRepository.Setup(m => m.Exists(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(true);

                _mockMaterialRepository.Setup(
                        m =>
                            m.Search(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                                It.IsAny<string>(), It.IsAny<SortOrder>()))
                    .ReturnsAsync(material);

                _mockMaterialRepository.Setup(
                        m => m.Search(It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(material);
                return this;
            }

            public Fixture WithStubbedSearchValues(List<IMaterial> materialList, string materialName, SortOrder sortOrder)
            {
                _mockMasterDataRepository.Setup(m => m.Exists(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(true);

                _mockMaterialRepository.Setup(
                        m =>
                            m.Search(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                                materialName, sortOrder))
                    .ReturnsAsync(materialList);
                return this;
            }
        }
    }
}