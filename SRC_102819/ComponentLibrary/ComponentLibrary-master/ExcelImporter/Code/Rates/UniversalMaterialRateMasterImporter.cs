using System;
using System.Collections.Generic;
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
    public class UniversalMaterialRateMasterImporter
    {
        private readonly TabularDataParserBuilder _universalMaterialRateMasterParserbuilder;
        private readonly StringBuilder _messageBuilder;

        public UniversalMaterialRateMasterImporter(TabularDataParserBuilder universalMaterialRateMasterParserbuilder)
        {
            _universalMaterialRateMasterParserbuilder = universalMaterialRateMasterParserbuilder;
            _messageBuilder = new StringBuilder();
        }

        public async Task Import(IWebClient<MaterialRateDto> webClient, string universalMaterialRateMasterWorkbookPath, string sheetName, DateTime createdOn)
        {
            var content =
                _universalMaterialRateMasterParserbuilder.BuildParserForUniversalMaterialRateMaster(
                    universalMaterialRateMasterWorkbookPath).Parse(sheetName);

            AddMessage("Started importing material rates");
            foreach (Entry entry in content.Content())
            {
                decimal clearanceCharges = 0;
                decimal freightCharges = 0;
                decimal taxVariance = 0;
                decimal locationVariance = 0;
                decimal marketFluctuation = 0;
                string materialId = null;
                string typeOfPurchase = null;
                string currencyType = null;
                decimal controlBaseRate = 0;
                decimal insuranceCharges = 0;
                decimal basicCustomsDuty = 0;
                string location = null;

                try
                {
                    var cells = entry.ToCellsArray();

                    AddMessage($"Started importing Item/Material ID {materialId}");

                    if (cells[0].Value() != null)
                    {
                        materialId = cells[0].Value();
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
                        insuranceCharges = decimal.Parse(cells[5].Value());
                    }
                    if (cells[6].Value() != null)
                    {
                        basicCustomsDuty = decimal.Parse(cells[6].Value());
                    }
                    if (cells[7].Value() != null)
                    {
                        clearanceCharges = decimal.Parse(cells[7].Value());
                    }
                    if (cells[8].Value() != null)
                    {
                        freightCharges = decimal.Parse(cells[8].Value());
                    }
                    if (cells[10].Value() != null)
                    {
                        taxVariance = decimal.Parse(cells[10].Value());
                    }
                    if (cells[11].Value() != null)
                    {
                        locationVariance = decimal.Parse(cells[11].Value());
                    }
                    if (cells[12].Value() != null)
                    {
                        marketFluctuation = decimal.Parse(cells[12].Value());
                    }
                    var materialRateDto = new MaterialRateDto
                    {
                        ControlBaseRate = new MoneyDto {Currency = currencyType, Value = controlBaseRate},
                        LocationVariance = locationVariance,
                        MarketFluctuation = marketFluctuation,
                        InsuranceCharges = insuranceCharges,
                        FreightCharges = freightCharges,
                        BasicCustomsDuty = basicCustomsDuty,
                        ClearanceCharges = clearanceCharges,
                        TaxVariance = taxVariance,
                        AppliedOn = createdOn,
                        Location = location,
                        Id = materialId,
                        TypeOfPurchase = typeOfPurchase
                    };
                    var postedUniversalMaterialRateMaster = await webClient.Post(materialRateDto, "material-rates");
                    if (postedUniversalMaterialRateMaster == null ||
                        postedUniversalMaterialRateMaster.StatusCode != HttpStatusCode.Created)
                    {
                        AddMessage($"Universal Material Rate for Item/Material {materialId} failed.");
                        continue;
                    }
                    AddMessage($"Successfully posted Item/Material ID {materialId}");
                }
                catch (InvalidDataException ex)
                {
                    AddMessage(ex.Message);
                }
                catch (Exception ex)
                {
                    var message =
                        ((System.Web.Http.HttpResponseException) ex).Response.Content.ReadAsStringAsync().Result;

                    if (message.ToLower().Contains("no exchange rate is found"))
                    {
                        AddMessage(
                            $"Successfully posted Item/Material ID {materialId}. But exchange rate is not found for currency {currencyType}.");
                    }
                    else
                    {
                        AddMessage($"Exception during upload - {message} - {ex.StackTrace}");
                    }
                }
                finally
                {
                    WriteToFile();
                }
            }
            AddMessage("Done importing material rates");
        }

        private void WriteToFile()
        {
            var filePath = "UniversalMaterialRateMasterUpload.txt";
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