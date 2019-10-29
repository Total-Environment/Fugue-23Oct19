using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Rates
{
    public class UniversalServiceRateMasterImporter
    {
        private readonly TabularDataParserBuilder _universalServiceRateMasterParserbuilder;
        private readonly StringBuilder _messageBuilder;

        public UniversalServiceRateMasterImporter(TabularDataParserBuilder universalServiceRateMasterParserbuilder)
        {
            _universalServiceRateMasterParserbuilder = universalServiceRateMasterParserbuilder;
            _messageBuilder = new StringBuilder();
        }

        public async Task Import(IWebClient<ServiceRateDto> webClient, string universalServiceRateMasterWorkbookPath, string sheetName, DateTime createdOn)
        {
            var content =
                _universalServiceRateMasterParserbuilder.BuildParserForUniversalServiceRateMaster(
                    universalServiceRateMasterWorkbookPath).Parse(sheetName);

            AddMessage("Started importing service rates");
            foreach (Entry entry in content.Content())
            {
                string serviceId = null;

                try
                {
                    decimal locationVariance = 0;
                    decimal marketFluctuation = 0;
                    string typeOfPurchase = null;
                    string currencyType = null;
                    decimal controlBaseRate = 0;
                    string location = null;

                    var cells = entry.ToCellsArray();

                    AddMessage($"Started importing Item/Service ID {serviceId}");

                    if (cells[0].Value() != null)
                    {
                        serviceId = cells[0].Value();
                    }
                    if (cells[1].Value() != null)
                    {
                        typeOfPurchase = cells[1].Value();
                    }
                    if (cells[2].Value() != null)
                    {
                        currencyType = cells[2].Value();
                    }
                    if (cells[3].Value() != null)
                    {
                        controlBaseRate = decimal.Parse(cells[3].Value());
                        if (controlBaseRate == 0)
                        {
                            throw new InvalidDataException("Control Base Rate cannot be zero");
                        }
                    }
                    if (cells[4].Value() != null)
                    {
                        location = cells[4].Value();
                        if (location == null)
                        {
                            throw new InvalidDataException("Location cannot be null");
                        }
                    }
                    if (cells[5].Value() != null)
                    {
                        locationVariance = decimal.Parse(cells[5].Value());
                    }
                    if (cells[6].Value() != null)
                    {
                        marketFluctuation = decimal.Parse(cells[6].Value());
                    }
                    var serviceRateDto = new ServiceRateDto
                    {
                        ControlBaseRate = new MoneyDto { Currency = currencyType, Value = controlBaseRate },
                        LocationVariance = locationVariance,
                        MarketFluctuation = marketFluctuation,
                        AppliedOn = createdOn,
                        Location = location,
                        Id = serviceId,
                        TypeOfPurchase = typeOfPurchase
                    };
                    var postedUniversalServiceRateMaster = await webClient.Post(serviceRateDto, "service-rates");
                    if (postedUniversalServiceRateMaster == null ||
                        postedUniversalServiceRateMaster.StatusCode != HttpStatusCode.Created)
                    {
                        AddMessage($"Universal Service Rate for Item/Service {serviceId} failed.");
                        continue;
                    }
                    AddMessage($"Successfully posted Item/Service ID {serviceId}");
                }
                catch (InvalidDataException ex)
                {
                    AddMessage(ex.Message);
                }
                catch (HttpResponseException ex)
                {
                    if (ex.Response.StatusCode == HttpStatusCode.Conflict)
                        AddMessage($"Item/Service {serviceId} is already imported.");
                    else
                        AddMessage($"Exception during upload - {ex.Message} - {ex.StackTrace}");
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
            AddMessage("Done importing service rates");
        }

        private void WriteToFile()
        {
            var filePath = "UniversalServiceRateMasterUpload.txt";
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