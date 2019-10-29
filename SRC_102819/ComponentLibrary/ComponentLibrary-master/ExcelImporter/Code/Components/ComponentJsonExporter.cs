using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;
using TE.Diagnostics.Logging;

namespace TE.ComponentLibrary.ExcelImporter.Code.Components
{
    public class ComponentJsonExporter : IJsonExporter
    {
        private readonly IConverter _convertor;
        private readonly IDocumentLogger _documentLogger;
        private readonly Logger _logger;
        private readonly Dictionary<string, List<string>> _urlData;
        private readonly IWebClient<object> _webClient;

        public ComponentJsonExporter(IDocumentLogger documentLogger, IConverter converter, IWebClient<object> webClient)
        {
            _documentLogger = documentLogger;
            _webClient = webClient;
            _logger = new Logger(typeof(ComponentJsonExporter));
            _convertor = converter;
            _convertor.UrlUpdater += EncounteredUrl;
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
            var json = JsonConvert.SerializeObject(columnNameCellValuePairs, _convertor.ToJsonConverter());
            var response = PostDataToComponentLibrary(json, _webClient, endPoint);
            WriteToDocumentLogger(response);
            return response;
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

        private string PostDataToComponentLibrary(string json, IWebClient<object> restClient, string endPoint)
        {
            if (restClient == null)
            {
                var errorMessage = $"Rest Client cannot be null. Skipping the post of this json {json}";
                Console.WriteLine(errorMessage);
                throw new ArgumentNullException(errorMessage);
            }
            try
            {
                var objectJson = JsonConvert.DeserializeObject(json);
                if(endPoint.Contains("material"))
                {
                    SetNonExistentMaterialLevelToNull(objectJson);

                } else if(endPoint.Contains("service"))
                {
                    SetNonExistentServiceLevelToNull(objectJson);
                }

                var response = restClient.Post(objectJson, "/" + endPoint).Result;

                if (response == null)
                {
                    var errorMessage = $"Could not post to component library {json}";
                    Console.WriteLine(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
                return response.Body.ToString();
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
                _logger.Error(json);
                _logger.Error(errorMessage);
                throw;
            }
        }

        private void SetNonExistentMaterialLevelToNull(object objectJson)
        {
            for (int i = 1; i < 8; i++)
            {
                if ((((Newtonsoft.Json.Linq.JObject)objectJson))["Classification"][$"Material Level {i}"] == null)
                {
                    (((Newtonsoft.Json.Linq.JObject)objectJson))["Classification"][$"Material Level {i}"] = null;
                }
            }
        }

        private void SetNonExistentServiceLevelToNull(object objectJson)
        {
            for (int i = 1; i < 8; i++)
            {
                if ((((Newtonsoft.Json.Linq.JObject)objectJson))["Classification"][$"Service Level {i}"] == null)
                {
                    (((Newtonsoft.Json.Linq.JObject)objectJson))["Classification"][$"Service Level {i}"] = null;
                }
            }
        }

        private void WriteToDocumentLogger(string response)
        {
            if (string.IsNullOrEmpty(response))
                throw new ArgumentNullException(nameof(response));

            try
            {
                dynamic convertedJson = JsonConvert.DeserializeObject(response);
                var materialId = convertedJson.id;
                _documentLogger.Write(materialId.Value, _urlData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not write to document logger" + ex.StackTrace);
            }
        }
    }
}