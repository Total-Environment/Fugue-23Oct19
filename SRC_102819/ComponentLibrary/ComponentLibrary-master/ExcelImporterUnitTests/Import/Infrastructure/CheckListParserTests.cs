using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests.Import.Infrastructure
{
    public class CheckListParserTests
    {
        private void SetupHeader(Mock<IExcelReader> excelReader)
        {
            var header = new Dictionary<string, ICustomCell>();
            AddTextCell("S.No", "B", header);
            AddTextCell("Work Description", "C", header);
            AddEmptyCell("D", header);
            AddTextCell("Remarks", "E", header);
            AddEmptyCell("F", header);
            AddEmptyCell("G", header);
            AddEmptyCell("H", header);
            excelReader.Setup(e => e.ReadRow(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(header);
        }

        private void SetupRow(Mock<IExcelReader> excelReader)
        {
            var row = new Dictionary<string, ICustomCell>();
            AddTextCell("1", "B", row);
            AddTextCell("Storage area is dry and waterproof", "C", row);
            AddEmptyCell("D", row);
            AddEmptyCell("E", row);
            AddEmptyCell("F", row);
            AddEmptyCell("G", row);
            AddEmptyCell("H", row);
            var rowBlock = new List<Dictionary<string, ICustomCell>> {row};
            excelReader.Setup(e => e.GetContiguousRowBlock(10, "B"))
                .Returns(rowBlock);
        }

        private void AddTextCell(string text, string columnReference, Dictionary<string, ICustomCell> header)
        {
            var textCustomCell = new Mock<ICustomCell>();
            header.Add(columnReference, textCustomCell.Object);
            textCustomCell.Setup(t => t.Value).Returns(text);
        }

        private void AddEmptyCell(string columnReference, Dictionary<string, ICustomCell> header)
        {
            var emptyCustomCell = new Mock<ICustomCell>();
            emptyCustomCell.Setup(e => e.Value).Returns(string.Empty);
            header.Add(columnReference, emptyCustomCell.Object);
        }

        private static TabularDataLoadConfiguration CheckListLoadConfiguration()
        {
            var configuration = new TabularDataLoadConfiguration();
            configuration.DataRowIndex = 10;
            configuration.NullColumnReference = "B";
            configuration.HeaderConfiguration = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("B", "S.No"),
                new KeyValuePair<string, string>("C", "Work Description"),
                new KeyValuePair<string, string>("D", "Selected"),
                new KeyValuePair<string, string>("E", "Remarks")
            };
            return configuration;
        }

        [Fact]
        public void Parse_ParseHeaderDataInCheckList()
        {
            var templateName = "Material_Inspection";
            var excelReader = new Mock<IExcelReader>();
            SetupHeader(excelReader);
            SetupRow(excelReader);
            var configuration = CheckListLoadConfiguration();

            var checkListImporter = new TabularDataParser(excelReader.Object, configuration);

            var content = checkListImporter.Parse();

            var headerEntry = content.Content().FirstOrDefault();
            headerEntry.Should().NotBeNull();
            headerEntry.Count.Should().Be(4);
        }

        [Fact]
        public void Parse_ParseRowDataAndMatchCorrespondingHeader()
        {
            var templateName = "Material_Inspection";
            var excelReader = new Mock<IExcelReader>();
            SetupHeader(excelReader);
            SetupRow(excelReader);
            var configuration = CheckListLoadConfiguration();
            var checkListImporter = new TabularDataParser(excelReader.Object, configuration);
            var expectedRow =
                new Entry(new List<TextCell>
                {
                    new TextCell("1"),
                    new TextCell("Storage area is dry and waterproof"),
                    new TextCell(string.Empty),
                    new TextCell(string.Empty)
                });
            var content = checkListImporter.Parse();

            content.Content().Count().Should().Be(1);
            var actualRow = content.Content().LastOrDefault();
            actualRow.Should().NotBeNull();
            actualRow.Count.Should().Be(4);
            actualRow.Should().Be(expectedRow);
        }
    }
}