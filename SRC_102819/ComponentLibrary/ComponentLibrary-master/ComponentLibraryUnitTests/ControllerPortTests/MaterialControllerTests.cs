using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
    public class MaterialControllerTests
    {
        private const string SomeErrorMsg = "Some Error Msg";

        private IMaterial GetMaterialWithDefinition(string group)
        {
            var material = GetMaterial(group);
            var materialDefinition = GetMaterialDefinition(group);
            material.ComponentDefinition = materialDefinition;
            return material;
        }

        private static IMaterialDefinition GetMaterialDefinition(string group)
        {
            return new MaterialDefinition(group)
            {
                Headers = new List<IHeaderDefinition>
                {
                    new HeaderDefinition("General", "general", new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Material Name", "material_name", new StringDataType())
                    })
                }
            };
        }

        private static Material GetMaterial(string group, string id = null)
        {
            return new Material
            {
                Group = group,
                Id = id,
                Headers = new List<HeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData> {new ColumnData("Material Name", "material_name", "Murrum")}
                    }
                },
                ComponentDefinition = GetMaterialDefinition(group)
            };
        }

        private static MaterialDataDto StubMaterialDataDto(string materialGroup)
        {
            return new MaterialDataDto
            {
                Headers = new List<HeaderDto>
                {
                    new HeaderDto
                    {
                        Name = "General",
                        Key = "general",
                        Columns = new List<ColumnDto>
                        {
                            new ColumnDto
                            {
                                Name = "Material Name",
                                Value = "Murrum",
                                Key = "material_name"
                            }
                        }
                    }
                },
                Group = materialGroup
            };
        }

        private class Fixture
        {
            private readonly Mock<IMaterialService> _mockMaterialService;
            private readonly List<Action> _verifications;
            private Expression<Action<IMaterialService>> _expression;

            public Fixture()
            {
                _verifications = new List<Action>();
                _mockMaterialService = new Mock<IMaterialService>();
            }

            public Fixture ForCreate()
            {
                _expression = m => m.Create(It.IsAny<IMaterial>());
                return this;
            }

            public Fixture ForFilter()
            {
                _expression =
                    m =>
                        m.SearchWithinGroup(It.IsAny<List<FilterData>>(), It.IsAny<string>(), It.IsAny<List<string>>(),
                            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<SortOrder>());
                return this;
            }

            public MaterialController SystemUnderTest()
            {
                var sapSyncer = new Mock<ISapSyncer>();
                return new MaterialController(_mockMaterialService.Object,sapSyncer.Object);
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithServiceThatReturns(Material createdMaterial)
            {
                _mockMaterialService.Setup(m => m.Create(It.IsAny<Material>())).ReturnsAsync(createdMaterial);
                return this;
            }

            public Fixture WithServiceThatThrowsException(Exception exception)
            {
                _mockMaterialService.Setup(_expression).Throws(exception);
                return this;
            }

            public Fixture WithServiceTocreate(IMaterial material)
            {
                Action verification =
                    () =>
                        _mockMaterialService.Verify(m => m.Create(It.Is<IMaterial>(mt => mt.Group == material.Group)),
                            Times.Once);
                _verifications.Add(verification);
                return this;
            }

            public void WithServiceToFilter(List<FilterData> filterData, string group, List<string> searchkeywords)
            {
                _mockMaterialService.Setup(
                    m => m.SearchWithinGroup(filterData, group, searchkeywords, It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<string>(), It.IsAny<SortOrder>())).ReturnsAsync(new List<IMaterial>());

                _verifications.Add(
                    () =>
                        _mockMaterialService.Verify(
                            ms =>
                                ms.SearchWithinGroup(filterData, group, searchkeywords, It.IsAny<int>(), It.IsAny<int>(),
                                    It.IsAny<string>(), It.IsAny<SortOrder>())));
            }

            public Fixture FindOfMaterialServiceReturns(IMaterial material)
            {
                _mockMaterialService.Setup(m => m.Find(It.IsAny<string>())).ReturnsAsync(material);
                return this;
            }

            public Fixture FindOfMaterialServiceThrows(Exception exception)
            {
                _mockMaterialService.Setup(m => m.Find(It.IsAny<string>())).ThrowsAsync(exception);
                return this;
            }

            public Fixture GetAllRatesOfMaterialServiceReturns(IEnumerable<MaterialRateSearchResult> materialRatesList)
            {
                _mockMaterialService.Setup(m => m.GetAllRates(It.IsAny<List<FilterData>>()))
                    .ReturnsAsync(materialRatesList);
                return this;
            }

            public Fixture GetAllRatesOfMaterialServiceThrows(Exception exception)
            {
                _mockMaterialService.Setup(m => m.GetAllRates(It.IsAny<List<FilterData>>()))
                    .ThrowsAsync(exception);
                return this;
            }

            public Fixture GetMaterialHavingAttachmentColumnDataInGroupOfMaterialServiceReturns(
                List<IMaterial> materials)
            {
                _mockMaterialService.Setup(
                    m =>
                        m.GetMaterialHavingAttachmentColumnDataInGroup(It.IsAny<string>(), It.IsAny<string>(),
                            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<string>>())).ReturnsAsync(materials);
                return this;
            }

            public Fixture GetCountOfMaterialsHavingAttachmentColumnDataInGroupOfMaterialServiceReturns(long count)
            {
                _mockMaterialService.Setup(
                        m =>
                            m.GetCountOfMaterialsHavingAttachmentColumnDataInGroup(It.IsAny<string>(),
                                It.IsAny<string>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(count);
                return this;
            }

            public Fixture GetMaterialHavingAttachmentColumnDataInGroupOfMaterialServiceThrows(Exception exception)
            {
                _mockMaterialService.Setup(
                    m =>
                        m.GetMaterialHavingAttachmentColumnDataInGroup(It.IsAny<string>(), It.IsAny<string>(),
                            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<string>>())).ThrowsAsync(exception);
                return this;
            }

            public Fixture GetRecentMaterialOfMaterialServiceReturn(List<IMaterial> materials)
            {
                _mockMaterialService.Setup(m => m.GetRecentMaterials(It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(materials);
                return this;
            }

            public Fixture GetRecentMaterialOfMaterialServiceThrows(Exception exception)
            {
                _mockMaterialService.Setup(m => m.GetRecentMaterials(It.IsAny<int>(), It.IsAny<int>()))
                    .ThrowsAsync(exception);
                return this;
            }

            public Fixture GetMaterialCountShouldReturn(int count)
            {
                _mockMaterialService.Setup(m => m.GetMaterialCount()).ReturnsAsync(count);
                return this;
            }

            public Fixture UpdateOfMaterialServiceShouldReturnMaterialWhenPassedMaterialHavingMaterialId(
                string materialCode, IMaterial material)
            {
                _mockMaterialService.Setup(m => m.Update(It.Is<IMaterial>(mat => mat.Id == materialCode)))
                    .ReturnsAsync(material);
                return this;
            }

            public Fixture PutOfMaterialServiceThrows(Exception exception)
            {
                _mockMaterialService.Setup(m => m.Update(It.IsAny<IMaterial>())).ThrowsAsync(exception);
                return this;
            }

            public Fixture SearchForGroupOfMaterialServiceReturns(List<string> materialGroups)
            {
                _mockMaterialService.Setup(m => m.SearchForGroups(It.IsAny<List<string>>()))
                    .ReturnsAsync(materialGroups);
                return this;
            }

            public Fixture SearcfForGroupOfMaterialServiceThrows(Exception exception)
            {
                _mockMaterialService.Setup(m => m.SearchForGroups(It.IsAny<
                    List<string>>())).ThrowsAsync(exception);
                return this;
            }
        }

        private MaterialDataDto StubMaterialDataDtoWithEmptyMaterialCode(string materialGroup)
        {
            return new MaterialDataDto
            {
                Headers = new List<HeaderDto>
                {
                    new HeaderDto
                    {
                        Name = "General",
                        Key = "general",
                        Columns = new List<ColumnDto>
                        {
                            new ColumnDto
                            {
                                Name = "Material Name",
                                Value = "Murrum",
                                Key = "material_name"
                            },
                            new ColumnDto
                            {
                                Name = "Material code",
                                Value = null,
                                Key = "material_code"
                            }
                        }
                    }
                },
                Group = materialGroup
            };
        }

        [Fact]
        public async void Get_ShouldReturnNotFound_WhenMaterialWithGivenIdDoesnotExist()
        {
            var fixture = new Fixture().FindOfMaterialServiceThrows(new ResourceNotFoundException(""));

            var result = await fixture.SystemUnderTest().Get("materialId");

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void Get_ShouldReturnOkMaterialDataDto_WhenDataTypeIsPassedAsTrue()
        {
            var material = GetMaterialWithDefinition("CLAY");
            var fixture = new Fixture().FindOfMaterialServiceReturns(material);

            var result = await fixture.SystemUnderTest().Get("MaterialId", true);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<MaterialDataTypeDto>>();
        }

        [Fact]
        public async void Get_ShouldReturnOkResult_WhenExistingMaterialIdIsPassed()
        {
            var material = GetMaterialWithDefinition("CLAY");
            var fixture = new Fixture().FindOfMaterialServiceReturns(material);

            var result = await fixture.SystemUnderTest().Get("materialId");

            result.Should().BeAssignableTo<OkNegotiatedContentResult<MaterialDataDto>>();
        }

        [Fact]
        public async void GetAllRates_ShouldReturnNotFoundResult_WhenMaterialsWithRatesAreNotFound()
        {
            var filters = new List<FilterData>
            {
                new FilterData("AppliedOn", "2017-05-10T18:30:00.000Z")
            };

            var result = await new Fixture().GetAllRatesOfMaterialServiceThrows(new ResourceNotFoundException("Material Rates")).SystemUnderTest().GetAllRates(filters);
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void GetAllRates_ShouldReturnBadRequestResult_WhenNullReferenceExceptionIsSent()
        {
            var filters = new List<FilterData>
            {
                new FilterData("AppliedOn", "2017-05-10T18:30:00.000Z")
            };

            var result = await new Fixture().GetAllRatesOfMaterialServiceThrows(new NullReferenceException("Material Rates")).SystemUnderTest().GetAllRates(filters);
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Fact]
        public async void GetAllRates_ShouldReturnBadRequestResult_WhenArgumentExceptionIsSent()
        {
            var filters = new List<FilterData>
            {
                new FilterData("AppliedOn", "2017-05-10T18:30:00.000Z")
            };

            var result = await new Fixture().GetAllRatesOfMaterialServiceThrows(new ArgumentException("Material Rates")).SystemUnderTest().GetAllRates(filters);
            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void GetAllRates_ShouldTakeValidSearchRequest_ReturnAllMaterialRates()
        {
            var bank = new Mock<IBank>();
            bank.Setup(b => b.ConvertTo(new Money(120, "INR", bank.Object), "INR",It.IsAny<DateTime>()))
                .ReturnsAsync(new Money(120, "INR", bank.Object));
            bank.Setup(b => b.ConvertTo(new Money(0, "INR", bank.Object), "INR",It.IsAny<DateTime>()))
                .ReturnsAsync(new Money(0, "INR", bank.Object));
            bank.Setup(b => b.ConvertTo(new Money(120, "INR", bank.Object), "INR"))
                .ReturnsAsync(new Money(120, "INR", bank.Object));
            bank.Setup(b => b.ConvertTo(new Money(0, "INR", bank.Object), "INR"))
                .ReturnsAsync(new Money(0, "INR", bank.Object));
            var materialRateSearchResult = new List<MaterialRateSearchResult>
            {
                new MaterialRateSearchResult("Clay", new MaterialRate(DateTime.Now, "Hyderabad", "CLY00001", new Money(120, "INR", bank.Object),0,0,0,0,0,0,0, "Import"), "description")
            };

            var materialRatesList = new List<MaterialRateSearchResult>(materialRateSearchResult);
            var result = await new Fixture().GetAllRatesOfMaterialServiceReturns(materialRatesList).SystemUnderTest()
                .GetAllRates(new List<FilterData>
                {
                    new FilterData("AppliedOn", "2017-05-10T18:30:00.000Z")
                });

            result.Should()
                .BeAssignableTo<OkNegotiatedContentResult<List<MaterialRateSearchResultDto>>>();

            var materialRates =
                ((OkNegotiatedContentResult<List<MaterialRateSearchResultDto>>)result).Content;

            materialRates.Count().Should().Be(1);
        }

        [Fact]
        public async void
            GetMaterialInGroupHavingAttachmentInColumn_ShouldReturnBadRequest_WhenBrandDefinitionDoesnotExist()
        {
            const string errorMessage = "Some message";
            var fixture =
                new Fixture().GetMaterialHavingAttachmentColumnDataInGroupOfMaterialServiceThrows(
                    new ArgumentException(errorMessage));

            var result =
                await
                    fixture.SystemUnderTest()
                        .GetMaterialsInGroupHavingAttachmentColumn("materialGroup", "columnName", 10, 10);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be(errorMessage);
        }

        [Fact]
        public async void GetMaterialInGroupHavingAttachmentInColumn_ShouldReturnOk_WhenGroupAndColumnNameIsPassedAsync()
        {
            const string materialGroup = "Clay";
            const int pageNumber = 1;
            const int batchSize = 10;
            const string columnKey = "material_name";
            var materials = new List<IMaterial>
            {
                GetMaterial(materialGroup)
            };
            var fixture =
                new Fixture().GetMaterialHavingAttachmentColumnDataInGroupOfMaterialServiceReturns(materials);

            var result = await fixture.SystemUnderTest()
                .GetMaterialsInGroupHavingAttachmentColumn(materialGroup, columnKey, pageNumber, batchSize);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<ListDto<MaterialDocumentDto>>>();
            ((OkNegotiatedContentResult<ListDto<MaterialDocumentDto>>)result).Content.Items.Count.Should().Be(1);
        }

        [Fact]
        public async void GetBrandsInGroupHavingAttachmentColumn_PassMaterialGroupAndBrandColumnNameTo_MaterialService()
        {
            var mockMaterialService = new Mock<IMaterialService>();
            var mockSapSyncerService = new Mock<ISapSyncer>();

            var serviceResultMock = new Mock<List<Dictionary<string, object>>>();

            var materialGroup = "Aluminium and Copper";
            var brandColumnKey = "manufacturer's_specification";
            var batchSize = 10;
            long resultSize = 10;
            var pageNumber = 1;

            mockMaterialService
                .Setup(m => m.GetBrandAttachmentsByGroupAndColumnNameKeywods(materialGroup, brandColumnKey, null,
                    pageNumber, batchSize)).Returns(Task.FromResult(serviceResultMock.Object));

            mockMaterialService
                .Setup(m => m.GetCountOfBrandsHavingAttachmentColumnDataInGroup(materialGroup, brandColumnKey, null))
                .Returns(Task.FromResult(resultSize));

            MaterialController mc = new MaterialController(mockMaterialService.Object,mockSapSyncerService.Object);

            await mc.GetBrandsInGroupHavingAttachmentColumn(materialGroup, pageNumber, batchSize, brandColumnKey);

            mockMaterialService.Verify(m => m.GetCountOfBrandsHavingAttachmentColumnDataInGroup(materialGroup, brandColumnKey, null), Times.Once);
        }

        [Fact]
        public async void
            GetMaterialInGroupHavingAttachmentInColumn_ShouldReturnTotalCount_WhenGroupAndColumnNameIsPassedAsync()
        {
            const string materialGroup = "Clay";
            const int pageNumber = 1;
            const int batchSize = 10;
            const string columnKey = "material_name";
            var materials = new List<IMaterial>
            {
                GetMaterial(materialGroup)
            };
            const int totalRecords = 100;
            var fixture =
                new Fixture().GetMaterialHavingAttachmentColumnDataInGroupOfMaterialServiceReturns(materials)
                    .GetCountOfMaterialsHavingAttachmentColumnDataInGroupOfMaterialServiceReturns(totalRecords);

            var result = await fixture.SystemUnderTest()
                .GetMaterialsInGroupHavingAttachmentColumn(materialGroup, columnKey, pageNumber, batchSize);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<ListDto<MaterialDocumentDto>>>();
            var listDto = (OkNegotiatedContentResult<ListDto<MaterialDocumentDto>>)result;
            listDto.Content.RecordCount.Should().Be(totalRecords);
            listDto.Content.TotalPages.Should().Be(10);
        }

        [Fact]
        public async void GetRecentMaterials_ShouldNotFound_WhenNoRecentlyCreatedMaterialAreAvailable()
        {
            var fixture = new Fixture().GetRecentMaterialOfMaterialServiceThrows(new ResourceNotFoundException(""));

            var result = await fixture.SystemUnderTest().GetRecentMaterials(1, 10);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async void GetRecentMaterials_ShouldRecentlyCreatedMaterial_WhenPageNumberAndBatchSizeArePassed()
        {
            var fixture =
                new Fixture().GetRecentMaterialOfMaterialServiceReturn(new List<IMaterial>
                {
                    new Material {Headers = new List<IHeaderData>()}
                });

            var result = await fixture.SystemUnderTest().GetRecentMaterials(1, 10);

            result.Should().BeAssignableTo<OkNegotiatedContentResult<ListDto<MaterialDataTypeDto>>>();
            ((OkNegotiatedContentResult<ListDto<MaterialDataTypeDto>>)result).Content.Items.Should().HaveCount(1);
        }

        //[Fact]
        //public async void GetRecentMaterials_ShouldReturnBatchSizeTotalPageNumberAndCurrentPageNumber()
        //{
        //    const int totalMaterialCount = 100;
        //    const int pageNumber = 1;
        //    const int batchSize = 10;
        //    var fixture = new Fixture().GetMaterialCountShouldReturn(totalMaterialCount);

        //    var result = await fixture.SystemUnderTest().GetRecentMaterials(pageNumber, batchSize);

        //    result.Should().BeOfType<OkNegotiatedContentResult<ListDto<MaterialDataTypeDto>>>();
        //    var listDto = ((OkNegotiatedContentResult<ListDto<MaterialDataTypeDto>>)result).Content;
        //    listDto.PageNumber.Should().Be(pageNumber);
        //    listDto.BatchSize.Should().Be(batchSize);
        //    listDto.TotalPages.Should().Be(10);
        //}

        //[Fact]
        //public async void GetRecentMaterials_ShouldReturnTotalCount()
        //{
        //    const int totalMaterialCount = 100;
        //    var fixture = new Fixture().GetMaterialCountShouldReturn(totalMaterialCount);

        //    var result = await fixture.SystemUnderTest().GetRecentMaterials(1, 10);

        //    result.Should().BeOfType<OkNegotiatedContentResult<ListDto<MaterialDataTypeDto>>>();
        //    ((OkNegotiatedContentResult<ListDto<MaterialDataTypeDto>>)result).Content.RecordCount.Should()
        //        .Be(totalMaterialCount);
        //}

        [Fact]
        public async void Post_ShouldBadRequest_WhenMaterialServiceThrowArgumentException()
        {
            const string msg = "msg";
            var fixture = new Fixture().ForCreate().WithServiceThatThrowsException(new ArgumentException(msg));
            var materialController = fixture.SystemUnderTest();

            var result = await materialController.Post(StubMaterialDataDto("Clay"));

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            var badRequestErrorMessageResult = (BadRequestErrorMessageResult)result;
            badRequestErrorMessageResult.Message.Should().Be(msg);
        }

        [Fact]
        public async void Post_ShouldBuildMaterial_WhenMaterialDtoIsPassed()
        {
            const string materialGroup = "Clay";
            var materialDto = StubMaterialDataDto(materialGroup);
            var materialData = GetMaterial(materialGroup);
            var fixture = new Fixture().WithServiceTocreate(materialData)
                .WithServiceThatReturns(materialData);
            var materialController = fixture.SystemUnderTest();

            await materialController.Post(materialDto);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Post_ShouldReturnBadRequest_WhenServiceThrowFormatException()
        {
            var fixture = new Fixture().ForCreate().WithServiceThatThrowsException(new FormatException(SomeErrorMsg));

            var result = await fixture.SystemUnderTest().Post(StubMaterialDataDto("Clay"));

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            var errorMessage = ((BadRequestErrorMessageResult)result).Message;
            errorMessage.Should().Be(SomeErrorMsg);
        }

        [Fact]
        public async void Post_ShouldReturnConflict_WhenMaterialWithSameIdExist()
        {
            var fixture =
                new Fixture().ForCreate().WithServiceThatThrowsException(new DuplicateResourceException("Some message"));

            var result = await fixture.SystemUnderTest().Post(StubMaterialDataDto("CLAY"));

            result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public async void Post_ShouldReturnContentCreated_WhenMaterialDtoIsPassed()
        {
            const string materialGroup = "Clay";
            var materialDto = StubMaterialDataDto(materialGroup);
            var createdMaterialData = GetMaterial(materialGroup, "CLY0001");
            var fixture = new Fixture().WithServiceThatReturns(createdMaterialData);
            var materialController = fixture.SystemUnderTest();

            var material = await materialController.Post(materialDto);

            material.Should().BeAssignableTo<CreatedNegotiatedContentResult<MaterialDataDto>>();
            var result = (CreatedNegotiatedContentResult<MaterialDataDto>)material;
            result.Content.Id.Should().NotBeNull();
        }

        [Fact]
        public async void Put_ShouldReturnBadRequest_WhenInputMaterialHasInvalidColumn()
        {
            const string errorMessage = "Error Message";
            var fixture = new Fixture().PutOfMaterialServiceThrows(new FormatException(errorMessage));

            var result = await fixture.SystemUnderTest().Put("materialCode", StubMaterialDataDto("materialGroup"));

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be(errorMessage);
        }

        [Fact]
        public async void Put_ShouldReturnBadRequest_WhenInputMaterialIsInvalid()
        {
            const string errorMessage = "Error Message";
            var fixture = new Fixture().PutOfMaterialServiceThrows(new ArgumentException(errorMessage));

            var result = await fixture.SystemUnderTest().Put("materialCode", StubMaterialDataDto("materialGroup"));

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be(errorMessage);
        }

        [Fact]
        public async void Put_ShouldReturnBadRequest_WhenMaterialCodeIsNotPassed()
        {
            var fixture = new Fixture();

            var result = await fixture.SystemUnderTest().Put(null, StubMaterialDataDto("materialGroup"));

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be("Material code cannot be null.");
        }

        [Fact]
        public async void Put_ShouldReturnBadRequest_WhenNoMaterialWithSameIdExist()
        {
            const string errorMessage = "Not Found material with material code: materialCode.";
            var fixture = new Fixture().PutOfMaterialServiceThrows(new ResourceNotFoundException(errorMessage));

            var result = await fixture.SystemUnderTest().Put("materialCode", StubMaterialDataDto("materialGroup"));

            result.Should().BeOfType<BadRequestErrorMessageResult>();
            ((BadRequestErrorMessageResult)result).Message.Should().Be(errorMessage);
        }

        [Fact]
        public async void Put_ShouldUpdateMaterial_WhenMaterialIsPassed()
        {
            const string materialCode = "materialCode";
            const string materialgroup = "materialGroup";
            var materialRequest = StubMaterialDataDtoWithEmptyMaterialCode(materialgroup);
            var material = GetMaterial("Some new Group");
            var fixture =
                new Fixture().UpdateOfMaterialServiceShouldReturnMaterialWhenPassedMaterialHavingMaterialId(
                    materialCode, material);

            var result = await fixture.SystemUnderTest().Put(materialCode, materialRequest);

            result.Should().BeOfType<OkNegotiatedContentResult<MaterialDataTypeDto>>();
            ((OkNegotiatedContentResult<MaterialDataTypeDto>)result).Content.Group.Should().Be("Some new Group");
        }

        [Fact]
        public async Task SearchForGroup_ShouldReturnBadRequest_WhenAllKeywordsAreLessThan3Letter()
        {
            const string keyword = "ab as";
            var fixture = new Fixture();

            var searchInGroup = await fixture.SystemUnderTest().SearchForGroup(keyword);

            searchInGroup.Should().BeAssignableTo<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void SearchForGroup_ShouldShouldReturnListOfGroup_WhenKeywordsArePassed()
        {
            var materialGroups = new List<string> { "Clay material", "furniture" };
            var fixture = new Fixture().SearchForGroupOfMaterialServiceReturns(materialGroups);

            var result = await fixture.SystemUnderTest().SearchForGroup("Keyword");

            result.Should().BeOfType<OkNegotiatedContentResult<List<string>>>();
            ((OkNegotiatedContentResult<List<string>>)result).Content.Should().Equal(materialGroups);
        }

        [Fact]
        public async void SearchInGroup_ShouldReturnNotFound_WhenNoGroupGroupWithPassedKeywordsAreFound()
        {
            var fixture = new Fixture().SearcfForGroupOfMaterialServiceThrows(new ResourceNotFoundException(""));

            var result = await fixture.SystemUnderTest().SearchForGroup("Keywords");

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldAcceptFilterCriteriaWithGroupName_ReturnFilteredResults()
        {
            var filterData = new List<FilterData> { new FilterData("material_level_4", "Some") };
            new JavaScriptSerializer().Serialize(filterData);
            var fixture = new Fixture();
            const string group = "Clay";
            const string searckKeyword = "some search";

            fixture.WithServiceToFilter(filterData, @group, searckKeyword.Split(' ').ToList());
            var controller = fixture.SystemUnderTest();
            var searchWithinGroupRequest = new MaterialSearchRequest
            {
                GroupName = group,
                SearchQuery = searckKeyword,
                FilterDatas = filterData,
                PageNumber = 1,
                BatchSize = 10
            };
            await controller.SearchWithinGroup(searchWithinGroupRequest);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldReturnBadRequest_WhenFilterValueIsNotValid()
        {
            var filterData = new List<FilterData> { new FilterData("material_level_4", "some") };
            var fixture = new Fixture();
            var group = "invalid";
            var searckKeyword = "some search";

            fixture.ForFilter()
                .WithServiceThatThrowsException(new ArgumentException("Invalid int value."));
            var controller = fixture.SystemUnderTest();
            var searchWithinGroupRequest = new MaterialSearchRequest
            {
                GroupName = group,
                SearchQuery = searckKeyword,
                FilterDatas = filterData,
                PageNumber = 1,
                BatchSize = 10
            };
            var result = await controller.SearchWithinGroup(searchWithinGroupRequest);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = (BadRequestErrorMessageResult)result;
            response.Message.Should().Be("Invalid int value.");
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldReturnBadRequest_WhenGroupIsNotSpecifed()
        {
            var filterData = new List<FilterData> { new FilterData("material_level_4", "some") };
            var fixture = new Fixture();
            var group = "";
            var searckKeyword = "some search";

            var controller = fixture.SystemUnderTest();
            var materialSearchRequest = new MaterialSearchRequest
            {
                GroupName = group,
                SearchQuery = searckKeyword,
                FilterDatas = filterData,
                PageNumber = 1,
                BatchSize = 10
            };
            var result = await controller.SearchWithinGroup(materialSearchRequest);

            result.Should().BeAssignableTo<BadRequestErrorMessageResult>();
            var response = (BadRequestErrorMessageResult)result;
            response.Message.Should().Be("Material group is not specified.");
        }

        [Fact]
        public async Task SearchWithinGroup_ShouldReturnNotFound_WhenGroupSpecifiedIsNotValid()
        {
            var filterData = new List<FilterData> { new FilterData("material_level_4", "some") };
            var fixture = new Fixture();
            const string groupName = "invalid";
            const string searckKeyword = "some search";

            fixture.ForFilter()
                .WithServiceThatThrowsException(new ResourceNotFoundException("Material group not found"));
            var controller = fixture.SystemUnderTest();
            var materialSearchRequest = new MaterialSearchRequest
            {
                GroupName = groupName,
                SearchQuery = searckKeyword,
                FilterDatas = filterData,
                PageNumber = 1,
                BatchSize = 10
            };
            var result = await controller.SearchWithinGroup(materialSearchRequest);

            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}