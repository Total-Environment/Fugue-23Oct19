using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Brands
{
    public class BrandJsonExporter : IJsonExporter
    {
        private readonly IConverter _brandConverter;
        private readonly IDocumentLogger _documentLogger;
        private readonly Dictionary<string, List<StaticFileInformation>> _importedFiles;
        private readonly Dictionary<string, List<string>> _urlData;
        private readonly IWebClient<MaterialDataDto> _webClient;

        public BrandJsonExporter(IDocumentLogger documentLogger, IConverter brandConverter, IWebClient<MaterialDataDto> webClient, Dictionary<string, List<StaticFileInformation>> importedFiles)
        {
            _documentLogger = documentLogger;
            _brandConverter = brandConverter;
            _webClient = webClient;
            _importedFiles = importedFiles;
            _brandConverter.UrlUpdater += EncounteredUrl;
            _urlData = new Dictionary<string, List<string>>
            {
                {"checklist", new List<string>()},
                {"imageAndStaticFile", new List<string>()}
            };
        }

        public string ExportJson(Dictionary<ICustomCell, ICustomCell> columnNameCellValuePairs, string endPoint)
        {
            _urlData["checklist"] = new List<string>();
            _urlData["imageAndStaticFile"] = new List<string>();
            var json = JsonConvert.SerializeObject(columnNameCellValuePairs, _brandConverter.ToJsonConverter());
            var brandData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            var response = UpdateMaterialData(brandData, _webClient, endPoint);
            WriteToDocumentLogger(response, endPoint);
            return "";
        }

        public MaterialDataDto UpdateMaterialData(Dictionary<string, object> approvedBrand, IWebClient<MaterialDataDto> materialClient, string materialId)
        {
            if (materialClient == null)
            {
                var errorMessage = $"Rest Client cannot be null. Skipping the post of this json {approvedBrand}";
                Console.WriteLine(errorMessage);
                throw new ArgumentNullException(errorMessage);
            }
            try
            {
                var result = (materialClient.Get($"materials/{materialId}")).Result;
                if (result?.Body == null)
                {
                    Console.WriteLine($"Material id {materialId} does not exist.");
                    throw new ArgumentException($"Material id {materialId} does not exist.");
                }
                var material = result.Body;

                var purchaseHeader = material.Headers.FirstOrDefault(h => h.Key == "purchase");
                if (purchaseHeader != null)
                {
                    var approvedBrandsColumn = purchaseHeader.Columns.FirstOrDefault(c => c.Key == "approved_brands");
                    if (approvedBrandsColumn != null)
                    {
                        var approvedBrandObjects = approvedBrandsColumn.Value as List<object>;
                        if (approvedBrandObjects != null)
                            approvedBrandObjects.Add(approvedBrand);
                        else
                            approvedBrandsColumn.Value = new List<object> { approvedBrand };

                        var response = materialClient.Put(materialId, material, "/materials").Result;

                        if (response == null)
                        {
                            var errorMessage = $"Could not post to component library";
                            Console.WriteLine(errorMessage);
                            throw new InvalidOperationException(errorMessage);
                        }
                        return response.Body;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Approved Brands column is not there in the material with code: " + materialId);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Purchase header is not there in the material with code: " +
                                                        materialId);
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var httpResponseException = ex.InnerException as HttpResponseException;
                var errorMessage = httpResponseException != null
                    ? $"FAILED {httpResponseException.Response.StatusCode.ToString().ToUpper()}: {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                    : $"Error while posting data to component library  {Environment.NewLine}" +
                      $"ErrorMessage: {ex.InnerException?.Message} {Environment.NewLine}" +
                      $"Stack Trace: {ex.StackTrace}";
                Console.WriteLine(errorMessage);
                Console.WriteLine("Check error log for Json");
                throw;
            }
        }

        public void WriteToDocumentLogger(MaterialDataDto response, string materialId)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            try
            {
                _documentLogger.Write(materialId, _urlData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not write to document logger" + ex.StackTrace);
            }
        }

        private void EncounteredUrl(Dictionary<ICustomCell, ICustomCell> keyValuePairs, ICustomCell columnNameCell,
            ICustomCell dataCell, string documentType)
        {
            var logString = _documentLogger.GetLogString(columnNameCell, dataCell);
            if (documentType == "checklist")
            {
                var checklistList = _urlData["checklist"];
                checklistList.Add(logString);
            }
            else
            {
                var imageList = _urlData["imageAndStaticFile"];
                imageList.Add(logString);
            }
        }
    }
}