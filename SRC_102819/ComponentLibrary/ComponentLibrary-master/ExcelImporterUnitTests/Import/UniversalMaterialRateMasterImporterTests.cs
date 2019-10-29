using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.ExcelImporter.Code.Rates;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests.Import
{
    public class UniversalMaterialRateMasterImporterTests
    {
        [Fact]
        public async Task Import_Should_GetParseDataFromExcelAndCallApiWithMaterialRateDto()
        {
            var tabularDataParserBuilder = new Mock<TabularDataParserBuilder>();
            var tabularDataParser = new Mock<ITabularDataParser>();
            var table = new Mock<Table>();
            var tableContent = new List<Entry>();
            var textCells = new List<TextCell>
            {
                new TextCell("A"),
                new TextCell("B"),
                new TextCell("C"),
                new TextCell("1"),
                new TextCell("D"),
                new TextCell("2"),
                new TextCell("3"),
                new TextCell("4"),
                new TextCell("5"),
                new TextCell("6"),
                new TextCell("7"),
                new TextCell("8"),
                new TextCell("9")
            };
            var entry = new Entry(textCells);

            tableContent.Add(entry);
            tabularDataParserBuilder.Setup(
                    m => m.BuildParserForUniversalMaterialRateMaster("excels\\Rate_Master_-_Template_v-4.0.xlsx"))
                .Returns(tabularDataParser.Object);
            table.Setup(m => m.Content()).Returns(tableContent);
            tabularDataParser.Setup(m => m.Parse("Universal Material Rate Master")).Returns(table.Object);
            var universalMaterialRateMasterImporter =
                new UniversalMaterialRateMasterImporter(tabularDataParserBuilder.Object);
            var webCient = new Mock<IWebClient<MaterialRateDto>>();
            DateTime createdOn = DateTime.UtcNow;
            await
                universalMaterialRateMasterImporter.Import(webCient.Object, "excels\\Rate_Master_-_Template_v-4.0.xlsx",
                    "Universal Material Rate Master", createdOn);

            var expectedMaterialRateDto = new MaterialRateDto
            {
                ControlBaseRate = new MoneyDto { Currency = "C", Value = 1 },
                LocationVariance = 8,
                MarketFluctuation = 9,
                TaxVariance = 7,
                InsuranceCharges = 2,
                FreightCharges = 5,
                BasicCustomsDuty = 3,
                ClearanceCharges = 4,
                AppliedOn = createdOn,
                Location = "D",
                Id = "A",
                TypeOfPurchase = "B"
            };

            webCient.Verify(m => m.Post(expectedMaterialRateDto, "material-rates"), Times.Once);
        }
    }
}