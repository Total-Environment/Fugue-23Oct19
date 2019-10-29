using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.RestWebClient;
using TE.Diagnostics.Logging;
using TE.Shared.CloudServiceFramework.Domain;

namespace TE.ComponentLibrary.ExcelImporter.Code.Documents
{
    public class StaticFileImporter
    {
        private readonly Logger _logger;
        private readonly IEnumerable<string> _sourceFileContents;
        private readonly IFileVerifier _staticFileVerifier;

        public StaticFileImporter(IEnumerable<string> sourceFileContents, IFileVerifier staticFileVerifier)
        {
            _sourceFileContents = sourceFileContents;
            _staticFileVerifier = staticFileVerifier;
            _logger = new Logger(typeof(StaticFileImporter));
        }

        public async Task LinkToComponent(string componentId, List<StaticFileInformation> staticFileInformationList, IWebClient<StaticFile> staticFileWebClient, IWebClient<Dictionary<string, object>> componentWebClient, string endPoint, bool linkToComponent = true)
        {
            foreach (var staticFileInformation in staticFileInformationList)
            {
                for (var index = 0; index < staticFileInformation.StaticFiles.Count; index++)
                {
                    var findStaticFileResponse =
                        await
                            staticFileWebClient.FindBy("static-file?name=" +
                                                       staticFileInformation.StaticFiles[index].Name);
                    if (findStaticFileResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        var staticFileResponse =
                            await staticFileWebClient.Post(staticFileInformation.StaticFiles[index], "static-file");
                        staticFileInformation.StaticFiles[index] = staticFileResponse.Body;
                    }
                    else
                    {
                        staticFileInformation.StaticFiles[index] = findStaticFileResponse.Body;
                    }
                }
            }
            if (linkToComponent)
            {
                var component = (await componentWebClient.Get($"/{endPoint}/" + componentId)).Body;
                if (component == null)
                {
                    Console.WriteLine("Component with " + componentId + " does not exist");
                }

                foreach (var staticFileInformation in staticFileInformationList)
                {
                    try
                    {
                        Dictionary<string, object> componentResult;
                        Dictionary<string, object> definition;
                        var jObject = component[staticFileInformation.PrimaryHeaderName];
                        var dict = (JObject) jObject;
                        object files;
                        if (staticFileInformation.StaticFiles.Count == 0)
                        {
                            files = null;
                        }
                        else if (staticFileInformation.SecondaryHeaderName.ToLower() == "image" &&
                                 staticFileInformation.PrimaryHeaderName.ToLower() == "general")
                        {
                            files = JArray.FromObject(staticFileInformation.StaticFiles);
                        }
                        else
                        {
                            files = staticFileInformation.StaticFiles.FirstOrDefault();
                        }
                        if (files != null)
                        {
                            dict[staticFileInformation.SecondaryHeaderName] = JToken.FromObject(files);
                        }
                        else
                        {
                            dict[staticFileInformation.SecondaryHeaderName] = null;
                        }
                        component[staticFileInformation.PrimaryHeaderName] = dict;

                        if (endPoint.Contains("material"))
                        {
                            GroupColumn = "material Level 2";
                            var group = GetGroup(component);
                            var asset = component.Count(a => a.Key.ToLowerInvariant() == "maintenance");
                            RestClientResponse<Dictionary<string, object>> assetDefinition = null;
                            if (asset != 0)
                            {
                                assetDefinition = await componentWebClient.Get($"asset-definitions/{group}");
                            }
                            var materialDefinition = await componentWebClient.Get($"material-definitions/{@group}");
                            definition = materialDefinition.Body;
                            componentResult = InsertDataIntoDefinition(assetDefinition?.Body,definition, component);
                        }
                        else if (endPoint.Contains("service"))
                        {
                            GroupColumn = "service Level 1";
                            var group = GetGroup(component);
                            var serviceDefinition = await componentWebClient.Get($"service-definitions/{group}");
                            definition = serviceDefinition.Body;
                            componentResult = InsertDataIntoDefinition(null,definition, component);
                        }
                        else
                        {
                            componentResult = component;
                        }
                        Console.WriteLine("For Component " + componentId + ", inserted value in " +
                                          staticFileInformation.PrimaryHeaderName + "," +
                                          staticFileInformation.SecondaryHeaderName);

                        var response = await componentWebClient.Put(componentId, componentResult, $"/{endPoint}");

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            Console.WriteLine("Updated component.");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Update component failed with error message. {response.Body.FirstOrDefault()}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not insert value in " + staticFileInformation.PrimaryHeaderName + ", " +
                                          staticFileInformation.SecondaryHeaderName + " for component " + componentId);
                    }
                }
            }
        }

        private Dictionary<string, object> InsertDataIntoDefinition(Dictionary<string, object> assetDefinitionBody, Dictionary<string, object> definition, Dictionary<string, object> resultBody)
        {
            var headersToBeInserted = new Dictionary<string, object>();

            var headers = definition["headers"] as JArray;
            var assetHeaders = assetDefinitionBody?["headers"] as JArray;

            if (headers == null) return headersToBeInserted;
            foreach (var header in headers)
            {
                var headerInData =
                    resultBody.FirstOrDefault(h => h.Key.ToLowerInvariant().Equals(header["name"].ToString().ToLowerInvariant()));

                var columns = (JArray)header["columns"];
                var columnsToBeInserted = new Dictionary<string, object>();
                foreach (var column in columns)
                {
                    var jObject = JObject.FromObject(headerInData.Value);
                    if (jObject == null) continue;
                    var columnData =
                        (jObject.ToObject<Dictionary<string, object>>()).FirstOrDefault(
                            c => c.Key.ToLowerInvariant().Equals(column["name"].ToString().ToLowerInvariant()));
                    columnsToBeInserted.Add(column["name"].ToString(), columnData.Value);
                }

                headersToBeInserted.Add(header["name"].ToString(), columnsToBeInserted);
            }
            if (assetHeaders == null) return headersToBeInserted;
            foreach (var assetHeader in assetHeaders)
            {
                var headerInData =
                    resultBody.FirstOrDefault(
                        h => h.Key.ToLowerInvariant().Equals(assetHeader["name"].ToString().ToLowerInvariant()));

                var columns = (JArray) assetHeader["columns"];
                var columnsToBeInserted = new Dictionary<string, object>();
                foreach (var column in columns)
                {
                    var jObject = JObject.FromObject(headerInData.Value);
                    if (jObject == null) continue;
                    var columnData =
                        (jObject.ToObject<Dictionary<string, object>>()).FirstOrDefault(
                            c => c.Key.ToLowerInvariant().Equals(column["name"].ToString().ToLowerInvariant()));
                    columnsToBeInserted.Add(column["name"].ToString(), columnData.Value);
                }

                headersToBeInserted.Add(assetHeader["name"].ToString(), columnsToBeInserted);
            }
            return headersToBeInserted;
        }

        public string GroupColumn { get; set; }

        private string GetGroup(Dictionary<string, object> component)
        {
            JObject classification;
            try
            {
                classification = component["classification"] as JObject;
            }
            catch (KeyNotFoundException)
            {
                throw new BetterKeyNotFoundException("classification");
            }

            try
            {
                return (string)classification[GroupColumn];
            }
            catch (KeyNotFoundException)
            {
                throw new BetterKeyNotFoundException(GroupColumn);
            }
        }

        public Dictionary<string, List<StaticFileInformation>> Parse()
        {
            var staticFileDictionary = new Dictionary<string, List<StaticFileInformation>>();

            foreach (var line in _sourceFileContents)
            {
                var splitLine = line.Split('~');
                var componentId = splitLine[0];

                var staticFileList = splitLine.Skip(1).Select(staticFileString => CreateStaticFileFrom(staticFileString, componentId)).ToList();

                if (!staticFileDictionary.ContainsKey(componentId))
                    staticFileDictionary.Add(componentId, staticFileList);
            }

            return staticFileDictionary;
        }

        public async Task<List<StaticFileInformation>> UploadToAzure(
            KeyValuePair<string, List<StaticFileInformation>> staticFileList,
            IConfiguration configuration, IBlobStorageService azureBlobStorageService, IFileReader fileReader,
            string staticFileLocationPath)
        {
            var staticFileInformations = staticFileList.Value;

            foreach (var staticFileInformation in staticFileInformations)
            {
                var staticFiles = new List<StaticFile>();
                foreach (var filePath in staticFileInformation.Names)
                {
                    try
                    {
                        var staticFileStream = fileReader.Read(filePath);
                        
                        var fileName = Path.GetFileName(filePath);
                        if (fileName == null) continue;

                        fileName = Guid.NewGuid().ToString("D") + "." + fileName.Split('.').Last();

                        await azureBlobStorageService.Upload(fileName, staticFileStream, "static-files");
                        var staticFileId = Guid.NewGuid().ToString("N");
                        staticFiles.Add(new StaticFile(staticFileId, fileName));
                        Console.WriteLine(fileName + " was successfully uploaded");
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        _logger.Warn(e.Message);
                    }
                }
                staticFileInformation.UpdateStaticFileList(staticFiles);
            }

            return staticFileInformations;
        }

        private StaticFileInformation CreateStaticFileFrom(string staticFileString, string componentId)
        {
            var names = new List<string>();
            var splitComponents = staticFileString.Split(':', '%', ',');
            var primaryHeaderName = ToCamelCase(splitComponents[0].Trim());
            var secondaryHeaderName = ToCamelCase(splitComponents[1].Trim());

            foreach (var splitComponent in splitComponents.Skip(2))
            {
                var nameWithoutExtension = splitComponent.Trim();
                var fileName = _staticFileVerifier.ParseFilePath(nameWithoutExtension);
                if (!string.IsNullOrEmpty(fileName))
                    names.Add(fileName);

                if (string.IsNullOrEmpty(fileName) && (nameWithoutExtension != "NA" && nameWithoutExtension != ""))
                {
                    var error = $"File {nameWithoutExtension} does not exist or does not exist with the matching file extension.";
                    Console.WriteLine(error);
                    _logger.Error(error);
                }
            }

            return new StaticFileInformation(primaryHeaderName, secondaryHeaderName, names);
        }

        private string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return char.ToLower(value[0]) + value.Substring(1);
        }
    }
}