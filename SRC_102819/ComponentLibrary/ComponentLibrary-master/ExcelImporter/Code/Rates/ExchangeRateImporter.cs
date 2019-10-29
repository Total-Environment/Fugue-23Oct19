using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Rates
{
    public class ExchangeRateImporter
    {
        private readonly TabularDataParserBuilder _exchangeRateParserbuilder;
        private readonly StringBuilder _messageBuilder;

        public ExchangeRateImporter(TabularDataParserBuilder exchangeRateParserbuilder)
        {
            _exchangeRateParserbuilder = exchangeRateParserbuilder;
            _messageBuilder = new StringBuilder();
        }

        public async Task Import(IWebClient<ExchangeRateDto> webClient, string exchangeRateWorkbookPath, string sheetName)
        {
            var content =
                _exchangeRateParserbuilder.BuildParserForExchangeRate(exchangeRateWorkbookPath).Parse(sheetName);

            AddMessage("Started importing Exchange Rates");
            foreach (Entry entry in content.Content())
            {
                try
                {
                    var cells = entry.ToCellsArray();
                    var fromCurrency = cells[0].Value();
                    AddMessage($"Started importing Item/Currency {fromCurrency}");
                    var conversionRate = cells[1].Value();
                    var currencyFluctuationConefficient = cells[2].Value();
                    var exchangeRateDto = new ExchangeRateDto
                    {
                        FromCurrency = fromCurrency,
                        ToCurrency = "INR",
                        BaseConversionRate = conversionRate,
                        CurrencyFluctuationCoefficient = currencyFluctuationConefficient,
                        AppliedFrom = DateTime.Today
                    };
                    var postedExchangeRate = await webClient.Post(exchangeRateDto, "exchange-rates");
                    if (postedExchangeRate == null || postedExchangeRate.StatusCode != HttpStatusCode.Created)
                    {
                        AddMessage($"Exchange Rate for Item/Currency {fromCurrency} failed.");
                        continue;
                    }
                    AddMessage($"Successfully posted Item/Currency {fromCurrency}");
                }
                catch (Exception ex)
                {
                    AddMessage($"Exception during upload - {ex.Message} - {ex.StackTrace}");
                }
                finally
                {
                    WriteToFile();
                }
            }
            AddMessage("Done importing Exchange Rates");
        }

        private void WriteToFile()
        {
            var filePath = "ExchangeRateUpload.txt";
            using (var streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(_messageBuilder.ToString());
                _messageBuilder.Clear();
            }
        }

        private void AddMessage(string message)
        {
            Console.WriteLine(message);
            _messageBuilder.Append(message);
            _messageBuilder.Append(Environment.NewLine);
        }
    }
}