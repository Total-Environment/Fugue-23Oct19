using System.Collections.Generic;
using System.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelImporter.Importing.Infrastructure;
using Moq;
using FluentAssertions;

namespace ExcelReaderUnitTests
{
    public class ExcelReaderTests
    {
        [Fact]
        public void ReadRowWhenPassedARowShouldReturnKeyValuePairs()
        {
            var excelReader = new Mock<ExcelReader>("temp") { CallBase = true };

            excelReader.Setup(ex => ex.Initialize());
            var row = new Row();
            var cell1 = new Cell()
            {
                CellValue = new CellValue("test1"),
                CellReference = "A5"
            };
            var cell2 = new Cell()
            {
                CellValue = new CellValue("test2"),
                CellReference = "B5"
            };
            var cell3 = new Cell()
            {
                CellValue = new CellValue("test3"),
                CellReference = "C5"
            };

            row.AppendChild<Cell>(cell1);
            row.AppendChild<Cell>(cell2);
            row.AppendChild<Cell>(cell3);

            var expectedValues = new List<CustomCell>
            {
                new CustomCell(cell1, new SharedStringTable()),
                new CustomCell(cell2, new SharedStringTable()),
                new CustomCell(cell3, new SharedStringTable())
            };

            var expectedKeys = new List<string>
            {
                "test1",
                "test2",
                "test3"
            };
            var keyValuePairs = excelReader.Object.ReadRow(row);
            keyValuePairs.Keys.Should().HaveCount(3, "because we put 3 cells");
            keyValuePairs.Values.ShouldBeEquivalentTo(expectedValues);
            //keyValuePairs.Keys.ShouldBeEquivalentTo(expectedKeys);
        }

       // [Fact]
        public void GetHeaderWhenPassedACellReferenceValueShouldReturnTheValueInIt()
        {
            var excelReader = new Mock<ExcelReader>("temp") { CallBase = true };
            excelReader.Setup(ex => ex.Initialize());

            var cell = new Cell();
            cell.CellReference = "A5";
            cell.CellValue = new CellValue("I am a dummy cell");

            var sheet = new Mock<Worksheet>();
            
            //var headerValue = excelReader.Object.GetColumnName(cell.CellReference);

            //headerValue.Should().Be(cell.CellValue.InnerText);
        }
    }
}
