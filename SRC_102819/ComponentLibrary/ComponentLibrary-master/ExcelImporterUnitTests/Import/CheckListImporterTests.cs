using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ExcelImporter.Code.Checklists;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests.Import
{
    public class CheckListImporterTests
    {
        [Fact]
        public async Task Import_CallConfigurationAndExcelReaderBasedOnTemplateName()
        {
            var importRecords = new ImportRecords().
                HavingSpecificImportRecordWith("CLAY0001", "MaterialInspection", "MCL0001", "Sample.xlsx").
                ToValue();

            var fixture = new Fixture()
                .WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser()
                .ExpectingCallToSpecificCheckList("Sample.xlsx");
            var importer = fixture.ToSystemUnderTest();

            var mockWebClient = new Mock<IWebClient<CheckList>>();
            mockWebClient.Setup(m => m.Post(It.IsAny<CheckList>(), "/check-lists"))
                .Returns(
                    Task.FromResult(new RestClientResponse<CheckList>(HttpStatusCode.Created,
                        new CheckList { Id = "123" })));

            await importer.Import(importRecords, mockWebClient.Object);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async Task Import_PostCheckListToWebApi()
        {
            var importRecords = new ImportRecords().
                HavingGenericImportRecordWithMaterialId("Material1").
                ToValue();
            var importer = new Fixture().
                WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser().
                ToSystemUnderTest();
            var mockWebClient = new Mock<IWebClient<CheckList>>();

            await importer.Import(importRecords, mockWebClient.Object);

            mockWebClient.Verify(w => w.Post(It.IsAny<CheckList>(), "/check-lists"), Times.Once);
        }

        [Fact]
        public async Task Import_Should_PostCheckListToWebApiWithEmptyContent_ForTextLikeByCount()
        {
            var importRecords = new ImportRecords().
                HavingSpecificImportRecordWith("Material1", "Template", "NoCount", "").
                ToValue();
            var importer = new Fixture().
                WithCheckListBuilderThatSuccessfullyBuildsAndReturnsANoCheckListParser().
                ToSystemUnderTest();
            var mockWebClient = new Mock<IWebClient<CheckList>>();

            await importer.Import(importRecords, mockWebClient.Object);

            var expectedCheckList = new CheckList
            {
                CheckListId = "NoCount",
                Content = new Table(),
                Template = "Template"
            };

            mockWebClient.Verify(w => w.Post(It.IsAny<CheckList>(), "/check-lists"), Times.Never);
        }

        [Fact]
        public async Task Update_Should_GetMaterialToUpload()
        {
            var importRecords = new ImportRecords().
                HavingGenericImportRecordWithMaterialId("Material1").
                GenericImportRecordWithMaterialId("Material1").
                ToValue();
            var importer = new Fixture().
                WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser().
                ToSystemUnderTest();
            var mockWebClient = new Mock<IWebClient<CheckList>>();
            var mockMaterialWebClient = new Mock<IWebClient<Dictionary<string, object>>>();
            mockWebClient.Setup(m => m.Post(It.IsAny<CheckList>(), It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new RestClientResponse<CheckList>(HttpStatusCode.Created, new CheckList { Id = "123" })));

            await importer.Import(importRecords, mockWebClient.Object);

            await importer.Update(importRecords, mockMaterialWebClient.Object, "materials", "MaterialUpload.txt");

            mockMaterialWebClient.Verify(m => m.Get("/materials/Material1"), Times.Once);
        }

        [Fact]
        public async Task Update_Should_SetCheckListIdForMaterialToUpload()
        {
            var importRecords = new ImportRecords().
                HavingSpecificImportRecordWith("componentId", "quantity Evaluation Method", "MCL0001", "checkListPath",
                    "purchase").
                ToValue();
            var importer = new Fixture().
                WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser().
                ToSystemUnderTest();

            var mockWebClient = new Mock<IWebClient<CheckList>>();
            var mockMaterialWebClient = new Mock<IWebClient<Dictionary<string, object>>>();
            mockWebClient.Setup(m => m.Post(It.IsAny<CheckList>(), It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new RestClientResponse<CheckList>(HttpStatusCode.Created, new CheckList { Id = "123" })));

            var material = new Dictionary<string, object>
            {
                {"classification",  JObject.FromObject(new Dictionary<string, object> {{"material Level 2", "Clay"}}) },
                {"purchase", JObject.FromObject(new Dictionary<string, object> {{"quantity Evaluation Method", "123"}})}
            };

            var materialDefinition = new Dictionary<string, object>
            {
                {
                    "headers", JArray.FromObject(
                        new List<JObject>
                        {
                            JObject.FromObject(new Dictionary<string, object>
                            {
                                {
                                    "columns",
                                    JArray.FromObject(new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object> {{"name", "Material Level 2"}}
                                    })
                                },
                                {"name", "Classification"}
                            }),
                             JObject.FromObject(new Dictionary<string, object>
                            {
                                {
                                    "columns",
                                    JArray.FromObject(new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object> {{"name", "Quantity Evaluation Method"}}
                                    })
                                },
                                {"name", "Purchase"}
                            }),
                        }
                    )
                }
            };

            var expected = new Dictionary<string, object>
            {
                {"Classification",  new KeyValuePair<string, object>("Material Level 2", "Clay")},
                {"Purchase", new KeyValuePair<string, object>("Quantity Evaluation Method", "MCL0001")}
            };

            mockMaterialWebClient.Setup(m => m.Get("material-definitions/Clay"))
                .Returns(
                    Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, materialDefinition)));

            mockMaterialWebClient.Setup(m => m.Get("/materials/componentId"))
                .Returns(
                    Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, material)));

            mockMaterialWebClient.Setup(m => m.Put("componentId", expected, "materials"))
                .Returns(
                    Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, expected)));


            await importer.Import(importRecords, mockWebClient.Object);

            await importer.Update(importRecords, mockMaterialWebClient.Object, "materials", "MaterialUpload.txt");

            mockMaterialWebClient.Verify(
                m =>
                    m.Put("componentId",
                        It.Is<Dictionary<string, object>>(
                            d =>
                                ((Dictionary<string, object>)d["Purchase"])["Quantity Evaluation Method"].ToString() ==
                                "MCL0001"), "materials"), Times.Once);

        }

        [Fact]
        public async Task Update_Should_SetTextAsCheckListIdWhenCheckListIsNotAvailableToUpload()
        {
            var importRecords = new ImportRecords().
                HavingSpecificImportRecordWith("componentId", "quantity Evaluation Method", "By Count", "", "purchase").
                ToValue();
            var importer = new Fixture().
                WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser().
                ToSystemUnderTest();

            var mockWebClient = new Mock<IWebClient<CheckList>>();
            var mockMaterialWebClient = new Mock<IWebClient<Dictionary<string, object>>>();

            var materialDefinition = new Dictionary<string, object>
            {
                {
                    "headers", JArray.FromObject(
                        new List<JObject>
                        {
                            JObject.FromObject(new Dictionary<string, object>
                            {
                                {
                                    "columns",
                                    JArray.FromObject(new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object> {{"name", "Material Level 2"}}
                                    })
                                },
                                {"name", "Classification"}
                            }),
                             JObject.FromObject(new Dictionary<string, object>
                            {
                                {
                                    "columns",
                                    JArray.FromObject(new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object> {{"name", "Quantity Evaluation Method"}}
                                    })
                                },
                                {"name", "Purchase"}
                            }),
                        }
                    )
                }
            };

            var material = new Dictionary<string, object>
            {
                 {"classification",  JObject.FromObject(new Dictionary<string, object> {{"material Level 2", "Clay"}}) },
                {"purchase", JObject.FromObject(new Dictionary<string, object> {{"quantity Evaluation Method", "321"}})}
            };

            mockMaterialWebClient.Setup(m => m.Get(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, material)));

            mockMaterialWebClient.Setup(m => m.Get("material-definitions/Clay"))
               .Returns(
                   Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, materialDefinition)));

            await importer.Import(importRecords, mockWebClient.Object);

            await importer.Update(importRecords, mockMaterialWebClient.Object, "materials", "MaterialUpload.txt");

            mockMaterialWebClient.Verify(
                m =>
                    m.Put("componentId",
                        It.Is<Dictionary<string, object>>(
                            d =>
                                ((Dictionary<string, object>)d["Purchase"])["Quantity Evaluation Method"].ToString() ==
                                "By Count"), "materials"), Times.Once);

        }

        [Fact]
        public async Task Update_Should_SubmitUpdatedMaterialToApi()
        {
            var importRecords = new ImportRecords().
                HavingSpecificImportRecordWith("componentId", "quantity Evaluation Method", "By Count", "", "purchase").
                ToValue();
            var importer = new Fixture().
                WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser().
                ToSystemUnderTest();

            var mockWebClient = new Mock<IWebClient<CheckList>>();
            var mockMaterialWebClient = new Mock<IWebClient<Dictionary<string, object>>>();
            var material = new Dictionary<string, object>
            {
                {"classification",  JObject.FromObject(new Dictionary<string, object> {{"material Level 2", "Clay"}}) },
                {"purchase", JObject.FromObject(new Dictionary<string, object> {{"quantity Evaluation Method", ""}})}
            };

            var materialDefinition = new Dictionary<string, object>
            {
                {
                    "headers", JArray.FromObject(
                        new List<JObject>
                        {
                            JObject.FromObject(new Dictionary<string, object>
                            {
                                {
                                    "columns",
                                    JArray.FromObject(new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object> {{"name", "Material Level 2"}}
                                    })
                                },
                                {"name", "Classification"}
                            }),
                             JObject.FromObject(new Dictionary<string, object>
                            {
                                {
                                    "columns",
                                    JArray.FromObject(new List<Dictionary<string, object>>
                                    {
                                        new Dictionary<string, object> {{"name", "Quantity Evaluation Method"}}
                                    })
                                },
                                {"name", "Purchase"}
                            }),
                        }
                    )
                }
            };

            mockMaterialWebClient.Setup(m => m.Get(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, material)));

            mockMaterialWebClient.Setup(m => m.Get("material-definitions/Clay"))
              .Returns(
                  Task.FromResult(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, materialDefinition)));

            await importer.Import(importRecords, mockWebClient.Object);

            await importer.Update(importRecords, mockMaterialWebClient.Object, "materials", "MaterialUpload.txt");

            mockMaterialWebClient.Verify(
                m =>
                    m.Put("componentId",
                        It.Is<Dictionary<string, object>>(
                            d =>
                                ((Dictionary<string, object>)d["Purchase"])["Quantity Evaluation Method"].ToString() ==
                                "By Count"), "materials"), Times.Once);
        }
    }

    public class Fixture
    {
        private readonly Mock<TabularDataParserBuilder> _checkListBuilder;
        private readonly List<Action> _expectations;
        private readonly CheckListImporter _importer;

        public Fixture()
        {
            _checkListBuilder = new Mock<TabularDataParserBuilder>();
            var mockCheckListConfigurationReader = new Mock<CheckListConigurationReader>();
            var mockTabularDataConfiguration = new Mock<TabularDataLoadConfiguration>();
            mockTabularDataConfiguration.SetupGet(m => m.DataRowIndex).Returns(10);
            mockTabularDataConfiguration.SetupGet(m => m.NullColumnReference).Returns("B");
            var headerConfiguration = new List<KeyValuePair<string, string>>();
            headerConfiguration.Add(new KeyValuePair<string, string>("B", "S.No"));
            headerConfiguration.Add(new KeyValuePair<string, string>("C", "Work Description"));
            headerConfiguration.Add(new KeyValuePair<string, string>("D", "Selected"));
            headerConfiguration.Add(new KeyValuePair<string, string>("E", "Remarks"));
            mockCheckListConfigurationReader.Setup(m => m.Read(It.IsAny<string>()))
                .Returns(mockTabularDataConfiguration.Object);

            _importer = new CheckListImporter(_checkListBuilder.Object, mockCheckListConfigurationReader.Object);
            _expectations = new List<Action>();
        }

        public CheckListImporter ToSystemUnderTest()
        {
            return _importer;
        }

        public Fixture ExpectingCallToGenericCheckList()
        {
            _expectations.Add(
                () =>
                    _checkListBuilder.Verify(
                        b => b.BuildParserForCheckList("CheckListPath", It.IsAny<TabularDataLoadConfiguration>()),
                        Times.Once));
            return this;
        }

        public Fixture ExpectingCallToSpecificCheckList(string checklistPath)
        {
            _expectations.Add(
                () =>
                    _checkListBuilder.Verify(
                        b => b.BuildParserForCheckList(checklistPath, It.IsAny<TabularDataLoadConfiguration>()),
                        Times.Once));
            return this;
        }

        public void VerifyExpectations()
        {
            _expectations.ForEach(a => a.Invoke());
        }

        public Fixture WithCheckListBuilderThatSuccessfullyBuildsAndReturnsAParser()
        {
            var checkListparser = new Mock<ITabularDataParser>();
            checkListparser.Setup(c => c.Parse()).Returns(new Mock<Table>().Object);
            _checkListBuilder.Setup(
                    m => m.BuildParserForCheckList(It.IsAny<string>(), It.IsAny<TabularDataLoadConfiguration>()))
                .Returns(checkListparser.Object);

            return this;
        }

        public Fixture WithCheckListBuilderThatSuccessfullyBuildsAndReturnsANoCheckListParser()
        {
            var checkListparser = new NoTabularDataParser();
            var mockTableDataconfiguration = new Mock<TabularDataLoadConfiguration>();
            _checkListBuilder.Setup(
                    m => m.BuildParserForCheckList(It.IsAny<string>(), mockTableDataconfiguration.Object))
                .Returns(checkListparser);

            return this;
        }
    }

    public class ImportRecords
    {
        private readonly Dictionary<string, List<ImportRecord>> _importRecords;

        public ImportRecords()
        {
            _importRecords = new Dictionary<string, List<ImportRecord>>();
        }

        public ImportRecords HavingGenericImportRecordWithMaterialId(string material)
        {
            _importRecords.Add(material, new List<ImportRecord>
            {
                new ImportRecord
                {
                    Template = "TemplateName",
                    CheckListId = "CheckListId",
                    CheckListPath = "CheckListPath",
                    Header = "GroupHeader"
                }
            });
            return this;
        }

        public ImportRecords HavingSpecificImportRecordWith(string componentId, string template, string checklistId,
            string checkListPath, string header = "")
        {
            _importRecords.Add(componentId, new List<ImportRecord>
            {
                new ImportRecord
                {
                    Template = template,
                    CheckListId = checklistId,
                    CheckListPath = checkListPath,
                    Header = header
                }
            });
            return this;
        }

        public Dictionary<string, List<ImportRecord>> ToValue()
        {
            return _importRecords;
        }

        public ImportRecords AndWith(string componentId, string template, string checkListId, string checkListPath,
            string header = "")
        {
            _importRecords[componentId].Add(
                new ImportRecord
                {
                    Template = template,
                    CheckListId = checkListId,
                    CheckListPath = checkListPath,
                    Header = header
                }
            );
            return this;
        }

        public ImportRecords GenericImportRecordWithMaterialId(string componentId)
        {
            _importRecords[componentId].Add(
                new ImportRecord
                {
                    Template = "TemplateName",
                    CheckListId = "CheckListId",
                    CheckListPath = "CheckListPath",
                    Header = "GroupHeader"
                }
            );
            return this;
        }
    }
}