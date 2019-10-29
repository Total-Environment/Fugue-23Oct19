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
    public class UniversalServiceRateMasterImporterTests
    {
        [Fact]
        public async Task Import_Should_GetParseDataFromExcelAndCallApiWithServiceRateDto()
        {
            var tabularDataParserBuilder = new Mock<TabularDataParserBuilder>();
            var tabularDataParser = new Mock<ITabularDataParser>();
            var table = new Mock<Table>();
            var tableContent = new List<Entry>();
            var textCells = new List<TextCell>
            {
                new TextCell("MSN00001"),
                new TextCell("Import"),
                new TextCell("USD"),
                new TextCell("10"),
                new TextCell("Bangalore"),
                new TextCell("5"),
                new TextCell("2")
            };
            var entry = new Entry(textCells);

            tableContent.Add(entry);
            tabularDataParserBuilder.Setup(
                    m => m.BuildParserForUniversalServiceRateMaster("excels\\ServiceRateMasterData.xlsx"))
                .Returns(tabularDataParser.Object);
            table.Setup(m => m.Content()).Returns(tableContent);
            tabularDataParser.Setup(m => m.Parse("Universal Service Rate Master")).Returns(table.Object);
            var universalServiceRateMasterImporter =
                new UniversalServiceRateMasterImporter(tabularDataParserBuilder.Object);
            var webCient = new Mock<IWebClient<ServiceRateDto>>();
            DateTime createdOn = DateTime.Now.AddDays(2).Date;
            await
                universalServiceRateMasterImporter.Import(webCient.Object, "excels\\ServiceRateMasterData.xlsx",
                    "Universal Service Rate Master", createdOn);

            var expectedServiceRateDto = new ServiceRateDto
            {
                ControlBaseRate = new MoneyDto { Currency = "USD", Value = 10 },
                LocationVariance = 5,
                MarketFluctuation = 2,
                AppliedOn = createdOn,
                Location = "Bangalore",
                Id = "MSN00001",
                TypeOfPurchase = "Import"
            };

            webCient.Verify(m => m.Post(expectedServiceRateDto, "service-rates"), Times.Once);
        }
    }
}