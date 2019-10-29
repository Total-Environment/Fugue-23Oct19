using System.IO;
using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.IntegrationTests.Import.Infrastructure
{
    public class ExcelReaderTests
    {
        private static ExcelReader ExcelReader()
        {
            return new ExcelReader(Path.GetFullPath(@"excels\\Checklist.xlsx"));
        }

        [Fact]
        public void CellLink_GetTheHyperLinkOfSpecificCell_HyperLink()
        {
            using (var excelReader = ExcelReader())
            {
                var result = excelReader.GetCellLink("Straight Rebar - 32mm ", "D12");
                Assert.Equal("Sheet1!A1", result);
            }
        }

        [Fact]
        public void Constructor_ExcelIsNotFoundInSpecifiedPath_FileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => new ExcelReader(@"..\..\notfile.xlsx"));
        }

        [Fact]
        public void Constructor_FileIsNotAnExcelFile_InvalidFileException()
        {
            Assert.Throws<FileFormatException>(() => new ExcelReader(@"ExcelImporterIntegrationTests.dll"));
        }

        [Fact]
        public void GetImage_GetPathOfImageForSpecificSheet_Path()
        {
            using (var excelReader = ExcelReader())
            {
                var sheet = excelReader.GetSheet("Sheet1");
                var workSheetPart = excelReader.GetWorksheetPart(sheet.Id);
                var result = excelReader.ImageExtractor.GetImage(workSheetPart, "");
                Assert.NotNull(result);
            }
        }

        [Fact]
        public void GetImage_ImageIsNotFoundInSpecifiedSheet_ImageNotFoundExceptio()
        {
            using (var excelReader = ExcelReader())
            {
                var sheet = excelReader.GetSheet("PO Terms - Rebars");
                var workSheetPart = excelReader.GetWorksheetPart(sheet.Id);
                Assert.Throws<FileNotFoundException>(() => excelReader.ImageExtractor.GetImage(workSheetPart, ""));
            }
        }

        [Fact]
        public void GetRowBlock_SheetAndInitialCell_GridOfCells()
        {
            using (var excelReader = ExcelReader())
            {
                var content = excelReader.GetContiguousRowBlock(10, "B");
                content.Count().Should().Be(6);
            }
        }

        [Fact]
        public void GetSheet_SheetWithSpecifedNameIsNotFound_SheetNotFoundException()
        {
            using (var excelReader = ExcelReader())
            {
                Assert.Throws<FileNotFoundException>(() => excelReader.GetSheet("No Sheet"));
            }
        }

        [Fact]
        public void ReadRowWhenPassedARowIndexShouldReturnKeyValuePairs()
        {
            using (var excelReader = ExcelReader())
            {
                var keyValuePairs = excelReader.ReadRow("Storage - Rebars", 8);
                keyValuePairs.Keys.Should().HaveCount(8);
            }
        }
    }
}