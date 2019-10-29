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
    public class ExchangeRateImporterTests
    {
        [Fact]
        public async Task Import_Should_GetParseDataFromExcelAndCallApiWithExchangeRate()
        {
            var tabularDataParserBuilder = new Mock<TabularDataParserBuilder>();
            var tabularDataParser = new Mock<ITabularDataParser>();
            var table = new Mock<Table>();
            var tableContent = new List<Entry>();
            var textCells = new List<TextCell> { new TextCell("A"), new TextCell("1"), new TextCell("2") };
            var entry = new Entry(textCells);

            tableContent.Add(entry);
            tabularDataParserBuilder.Setup(
                    m => m.BuildParserForExchangeRate("excels\\Rate_Master_-_Template_v-4.0.xlsx"))
                .Returns(tabularDataParser.Object);
            table.Setup(m => m.Content()).Returns(tableContent);
            tabularDataParser.Setup(m => m.Parse("Exchange Rates")).Returns(table.Object);
            var exchangeRateImporter = new ExchangeRateImporter(tabularDataParserBuilder.Object);
            var webCient = new Mock<IWebClient<ExchangeRateDto>>();
            await exchangeRateImporter.Import(webCient.Object, "excels\\Rate_Master_-_Template_v-4.0.xlsx", "Exchange Rates");

            var expectedRate = new ExchangeRateDto
            {
                FromCurrency = "A",
                ToCurrency = "INR",
                BaseConversionRate = "1",
                CurrencyFluctuationCoefficient = "2"
            };

            webCient.Verify(m => m.Post(expectedRate, "exchange-rates"), Times.Once);
        }
    }
}