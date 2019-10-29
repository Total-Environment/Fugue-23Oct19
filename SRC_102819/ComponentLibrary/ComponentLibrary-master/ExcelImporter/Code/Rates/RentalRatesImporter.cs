using System;
using System.Collections.Generic;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;
using TE.Diagnostics.Logging;

namespace TE.ComponentLibrary.ExcelImporter.Code.Rates
{
    public class RentalRatesImporter
    {
        private readonly IWebClient<RentalRateDto> _webClient;
        private readonly Logger _logger;

        public RentalRatesImporter(IWebClient<RentalRateDto> webClient)
        {
            _webClient = webClient;
            _logger = new Logger(typeof(ExcelExporter));
        }

        public void Import(string excelPath)
        {
            var reader = new ExcelReader(excelPath);
            var block = reader.GetContiguousRowBlock("Rates", 2, "A");
            int i = 2;
            foreach (var row in block)
            {
                try
                {
                    Console.WriteLine($"Inserting Row number: {i}...");
                    var requestBody = new RentalRateDto(GetCellValue(row, "A"),
                        GetCellValue(row, "B"),
                        new MoneyDto(new Money(decimal.Parse(GetCellValue(row, "C")), GetCellValue(row, "D"))),
                        DateTime.UtcNow.AddDays(2));
                    var materialId = GetCellValue(row, "A");
                    _webClient.Post(requestBody, $"/materials/{materialId}/rental-rates").Wait();
                    Console.WriteLine($"Done with {i}");
                }
                catch (AggregateException ex)
                {
                    var httpResponseException = ex.InnerException as HttpResponseException;
                    var errorMessage = httpResponseException != null
                        ? $"FAILED {httpResponseException.Response.StatusCode.ToString().ToUpper()}: {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                        : $"Error while posting data to component library  {Environment.NewLine}" +
                          $"ErrorMessage: {ex.InnerException?.Message} {Environment.NewLine}" +
                          $"Stack Trace: {ex.StackTrace}";
                    Console.WriteLine(errorMessage);
                    Console.WriteLine("Check error log for Json.");
                    _logger.Error(errorMessage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    i++;
                }
            }
        }

        private static string GetCellValue(Dictionary<string, ICustomCell> row, string columnIndex)
        {
            return row[columnIndex].Value.Trim();
        }
    }
}
