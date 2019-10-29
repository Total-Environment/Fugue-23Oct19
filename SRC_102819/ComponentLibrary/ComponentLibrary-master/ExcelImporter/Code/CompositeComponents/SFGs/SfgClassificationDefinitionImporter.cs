using System;
using System.Collections.Generic;
using System.Web.Http;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;
using TE.Diagnostics.Logging;

namespace TE.ComponentLibrary.ExcelImporter.Code.CompositeComponents.SFGs
{
    public class SfgClassificationDefinitionImporter
    {
        private readonly Logger _logger;
        private readonly IWebClient<Dictionary<string, Dictionary<string, string>>> _webClient;

        public SfgClassificationDefinitionImporter(IWebClient<Dictionary<string, Dictionary<string, string>>> webClient)
        {
            _webClient = webClient;
            _logger = new Logger(typeof(ExcelExporter));
        }

        public void Import(string excelPath)
        {
            var reader = new ExcelReader(excelPath);
            var block = reader.GetContiguousRowBlock("Definitions ", 2, "A");

            var columnMapping = new Dictionary<string, string>
            {
                {"A", "B"},
                {"C", "D"},
                {"E", "F"},
                {"G", "H"},
                {"I", "J"},
                {"K", "L"},
                {"M", "N"}
            };
            int i = 2;
            foreach (var row in block)
            {
                try
                {
                    Console.WriteLine($"Inserting Row number: {i}...");
                    var requestBody = new Dictionary<string, string>();

                    foreach (var kv in columnMapping)
                    {
                        if (row.ContainsKey(kv.Key))
                        {
                            if (string.IsNullOrWhiteSpace(row[kv.Key].Value)) continue;
                            requestBody[row[kv.Key].Value.Trim()] = row[kv.Value].Value?.Trim();
                        }
                        else
                        {
                            Console.WriteLine($"Invalid Row : {i}, {kv.Key}");
                        }
                    }
                    _webClient.Post(new Dictionary<string, Dictionary<string, string>>()
                    {
                        {
                            "sfg",
                            requestBody
                        }
                    }, "/classification-definitions").Wait();
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
    }
}