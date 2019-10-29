using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests
{
    public class ImportRecordParserTests
    {
        [Fact]
        public void Parse_Should_AcceptMultipleCheckLists()
        {
            var record =
                "Id~HeaderName1:TemplateName1%CheckListId1~HeaderName2:TemplateName2%CheckListId2";

            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath("CheckListId1")).Returns("CheckListId1.xlsx");
            checkListVerifier.Setup(m => m.ParseFilePath("CheckListId2")).Returns("CheckListId2.xlsx");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);
            var records = new List<string>();
            records.Add(record);

            var importRecords = importRecordParser.Parse(records);

            importRecords["Id"][0].Template.Should().Be("templateName1");
            importRecords["Id"][0].CheckListId.Should().Be("CheckListId1");
            importRecords["Id"][0].CheckListPath.Should().Be("CheckListId1.xlsx");

            importRecords["Id"][1].Template.Should().Be("templateName2");
            importRecords["Id"][1].CheckListId.Should().Be("CheckListId2");
            importRecords["Id"][1].CheckListPath.Should().Be("CheckListId2.xlsx");
        }

        [Fact]
        public void Parse_Should_AcceptMultipleCheckListsForDifferentMaterials()
        {
            var record1 =
                "MaterialId1~HeaderName1:TemplateName1%CheckListId1,ChecklistPath1~HeaderName2:TemplateName2%CheckListId2";
            var record2 =
                "MaterialId2~HeaderName3:TemplateName3%CheckListId3,ChecklistPath3~HeaderName4:TemplateName4%CheckListId4";

            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath("CheckListId2")).Returns("CheckListId2.xlsx");
            checkListVerifier.Setup(m => m.ParseFilePath("CheckListId4")).Returns("CheckListId4.xlsx");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);
            var records = new List<string>();
            records.Add(record1);
            records.Add(record2);

            var importRecords = importRecordParser.Parse(records);

            importRecords["MaterialId1"][0].Template.Should().Be("templateName1");

            importRecords["MaterialId1"][1].Template.Should().Be("templateName2");

            importRecords["MaterialId2"][0].Template.Should().Be("templateName3");

            importRecords["MaterialId2"][1].Template.Should().Be("templateName4");
        }

        [Theory]
        [InlineData("-- NA --")]
        [InlineData("NoCount")]
        public void Parse_Should_AcceptTextChecklist(string checkListId)
        {
            var record = $"Id~HeaderName:TemplateName%{checkListId}";

            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath("-- NA --")).Returns("");
            checkListVerifier.Setup(m => m.ParseFilePath("NoCount")).Returns("");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);
            var records = new List<string>();
            records.Add(record);

            var importRecord = importRecordParser.Parse(records);

            importRecord["Id"][0].CheckListId.Should().Be(checkListId);
            importRecord["Id"][0].CheckListPath.Should().Be(string.Empty);
        }

        [Fact]
        public void Parse_Should_ClenseData()
        {
            var record = "1 ~ Header : Material CheckList % MCL0001 ";
            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath("MCL0001")).Returns("MCL0001.xlsx");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);
            var records = new List<string>();
            records.Add(record);

            var importRecord = importRecordParser.Parse(records);

            importRecord["1"][0].Header.Should().Be("header");
            importRecord["1"][0].Template.Should().Be("material CheckList");
            importRecord["1"][0].CheckListId.Should().Be("MCL0001");
            importRecord["1"][0].CheckListPath.Should().Be("MCL0001.xlsx");
        }

        [Fact]
        public void Parse_Should_ConsiderCheckListAsFileIfItExistsInTheDirectory()
        {
            var record = "1~HeaderName:Material CheckList% MCL0001";
            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath("MCL0001")).Returns("MCL0001.xlsx");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);
            var records = new List<string>();
            records.Add(record);

            var importRecord = importRecordParser.Parse(records);

            importRecord["1"][0].Template.Should().Be("material CheckList");
            importRecord["1"][0].CheckListId.Should().Be("MCL0001");
            importRecord["1"][0].CheckListPath.Should().Be("MCL0001.xlsx");
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("Id~HeaderName")]
        [InlineData("Id~HeaderName:TemplateName")]
        public void Parse_Should_ThorwExceptionWithEntrySeperation(string record)
        {
            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath(It.IsAny<String>())).Returns("");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);

            var records = new List<string> { record };

            var parsedRecords = importRecordParser.Parse(records);

            parsedRecords.Count.Should().Be(0);
        }

        [Fact]
        public void Parse_Should_ValidateEntrySeperation()
        {
            var record = "Id~HeaderName:TemplateName%CheckListId";

            var checkListVerifier = new Mock<IFileVerifier>();
            checkListVerifier.Setup(m => m.ParseFilePath("CheckListId")).Returns("CheckListId.xlsx");
            var importRecordParser = new ImportRecordParser(checkListVerifier.Object);
            var records = new List<string> { record };

            var importRecord = importRecordParser.Parse(records);

            importRecord.First().Key.Should().Be("Id");

            importRecord["Id"][0].Template.Should().Be("templateName");
            importRecord["Id"][0].CheckListId.Should().Be("CheckListId");
            importRecord["Id"][0].CheckListPath.Should().Be("CheckListId.xlsx");
            importRecord["Id"][0].Header.Should().Be("headerName");
        }
    }
}