using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Checklists
{
    public class CheckListImporter
    {
        private readonly CheckListConigurationReader _checkListConfigurationReader;
        private readonly Dictionary<string, string> _checkListDictionary = new Dictionary<string, string>();
        private readonly StringBuilder _messageBuilder;
        private readonly TabularDataParserBuilder _parserBuilder;

        public CheckListImporter(TabularDataParserBuilder parserBuilder,
            CheckListConigurationReader checkListConfigurationReader)
        {
            _parserBuilder = parserBuilder;
            _checkListConfigurationReader = checkListConfigurationReader;
            _messageBuilder = new StringBuilder();
        }

        public async Task Import(Dictionary<string, List<ImportRecord>> recordsToImport, IWebClient<CheckList> webClient)
        {
            AddMessage("********** Started uploading of checklists ************");
            foreach (var record in recordsToImport)
                foreach (var importRecord in record.Value)
                    try
                    {
                        AddMessage($"Started uploading for Component {record.Key}");

                        if (_checkListDictionary.ContainsKey(importRecord.CheckListId))
                        {
                            AddMessage(
                                $"Component Id - {record.Key} - Checklist with id {importRecord.CheckListId} already exists, so skipping upload");
                            continue;
                        }

                        var configurationFile = importRecord.Template.Replace(" ", "");
                        configurationFile = configurationFile.Replace("&", "And");
                        var checkListConfiguration =
                            _checkListConfigurationReader.Read($"Data\\Checklists\\Configurations\\{configurationFile}.json");

                        var checkListParser = _parserBuilder.BuildParserForCheckList(
                            importRecord.CheckListPath, checkListConfiguration);

                        if (checkListParser == null)
                        {
                            AddMessage(
                                $"Component Id - {record.Key} - Unable to read record {importRecord.CheckListPath} having checklist id {importRecord.CheckListId}, skipping upload.");
                            continue;
                        }
                        else if (checkListParser.GetType() == typeof(NoTabularDataParser))
                        {
                            AddMessage(
                                $"Component Id - {record.Key} - Skipping upload as checklist is '{importRecord.CheckListId}', it does not have a checklist file to upload.");
                            continue;
                        }

                        var content = checkListParser.Parse();
                        var checkList = new CheckList
                        {
                            Content = content,
                            Template = importRecord.Template,
                            CheckListId = importRecord.CheckListId,
                            Title = checkListParser.ParceCellValue(checkListConfiguration.TitleCellReference)
                        };
                        var result = await webClient.Post(checkList, "/check-lists");
                        if ((result != null) && (result.StatusCode == HttpStatusCode.Created))
                        {
                            _checkListDictionary.Add(checkList.CheckListId, result.Body.Id);
                            AddMessage(
                                $"Component Id - {record.Key} - Successfully imported checklist {importRecord.CheckListId} - Id is {result.Body.Id}");
                        }
                        else
                        {
                            AddMessage(
                                $"Component Id - {record.Key} - Failed to import checklist {importRecord.CheckListId} for component {record.Key}.HttpResponse is {result != null} , HTTPStatusCode is {result.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is HttpResponseException && ((HttpResponseException) ex).Response.StatusCode == HttpStatusCode.Conflict)
                        {
                            var existingCheckListResult = await webClient.Get($"check-lists/{importRecord.CheckListId}");
                            if ((existingCheckListResult == null) ||
                                (existingCheckListResult.StatusCode != HttpStatusCode.OK))
                                AddMessage(
                                    $"Component Id - {record.Key} - Checklist with id {importRecord.CheckListId}, is already imported, but content could not be retrived ");
                            _checkListDictionary.Add(existingCheckListResult.Body.CheckListId,
                                existingCheckListResult.Body.Id);
                            AddMessage(
                                $"Component Id - {record.Key} - Checklist with id {importRecord.CheckListId}, is already imported");
                        }
                        else
                        {
                            AddMessage(
                                $"Component Id - {record.Key} - Failed to import {importRecord.CheckListId} for component {record.Key}. Exception - {ex.Message}. Stacktrace - {ex.StackTrace} - {ex.InnerException}");
                        }
                    }
                    finally
                    {
                        var filePath = "CheklistUpload.txt";
                        using (var streamWriter = new StreamWriter(filePath, true))
                        {
                            streamWriter.WriteLine(_messageBuilder.ToString());
                            _messageBuilder.Clear();
                        }
                    }
        }

        private void AddMessage(string message)
        {
            Console.WriteLine(message);
            _messageBuilder.Append(message);
            _messageBuilder.Append(Environment.NewLine);
        }

        public async Task Update(Dictionary<string, List<ImportRecord>> importRecords,
            IWebClient<Dictionary<string, object>> webClient, string componentName, string filePath)
        {
            _messageBuilder.Clear();
            AddMessage($"Started updating the {componentName}");
            foreach (var record in importRecords)
            {
                
                AddMessage($"started updating component - {record.Key} ");
                try
                {
                    Dictionary<string, object> component;
                    Dictionary<string, object> definition;
                    var result = await webClient.Get($"/{componentName}/{record.Key}");
                    if ((result.Body == null) || (result.StatusCode != HttpStatusCode.OK))
                    {
                        AddMessage(
                            $"component ID {record.Key} -Could not fetch required info, skipping the updation of component {record.Key}");
                        continue;
                    }
                    if (componentName.Contains("material"))
                    {
                        GroupColumn = "material Level 2";
                        var group = GetGroup(result.Body);
                        var asset = result.Body.Count(a => a.Key.ToLowerInvariant() == "maintenance");
                        RestClientResponse<Dictionary<string, object>> assetDefinition = null;
                        if (asset != 0)
                        {
                            assetDefinition = await webClient.Get($"asset-definitions/{group}");
                        }
                        var materialDefinition = await webClient.Get($"material-definitions/{group}");
                        definition = materialDefinition.Body;
                        component = InsertDataIntoDefinition(assetDefinition?.Body,definition, result.Body);
                    }
                    else if (componentName.Contains("service"))
                    {
                        GroupColumn = "service Level 1";
                        var group = GetGroup(result.Body);
                        var serviceDefinition = await webClient.Get($"service-definitions/{group}");
                        definition = serviceDefinition.Body;
                        component = InsertDataIntoDefinition(null,definition, result.Body);
                    }
                    else
                    {
                        component = result.Body;
                    }

                    foreach (var importRecord in record.Value)
                    {
                        SetCheckListId(component, importRecord, importRecord.CheckListId);
                        AddMessage(
                            $"component ID {record.Key} - Setting {importRecord.Header}.{importRecord.Template} = {importRecord.CheckListId}");
                    }

                    var updateResult = await webClient.Put(record.Key, component, componentName);
                    AddMessage(updateResult.StatusCode == HttpStatusCode.OK
                        ? $"component ID {record.Key} - Successfully updated it"
                        : $"component ID {record.Key} - Failed to update it");
                }
                catch (BetterKeyNotFoundException ex)
                {
                    AddMessage(
                        $"Component Id {record.Key} - Error during upload. Error details - Key Not Found: {ex.Message}");
                }
                catch (HttpResponseException ex)
                {
                    var data = await ex.Response.Content.ReadAsStringAsync();
                    AddMessage(
                        $"Component Id {record.Key} - Error during upload. Error details: {data}");
                }
                catch (Exception ex)
                {
                    AddMessage(
                        $"Component Id {record.Key} - Error during upload. Error details: {ex.Message}");
                }
                finally
                {
                    using (var streamWriter = new StreamWriter(filePath, true))
                    {
                        streamWriter.WriteLine(_messageBuilder.ToString());
                        _messageBuilder.Clear();
                    }
                }
            }
        }

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

        public string GroupColumn { get; set; }

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
                    var jObject = headerInData.Value as JObject;
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

                var columns = (JArray)assetHeader["columns"];
                var columnsToBeInserted = new Dictionary<string, object>();
                foreach (var column in columns)
                {
                    var jObject = headerInData.Value as JObject;
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

        private void SetCheckListId(Dictionary<string, object> component, ImportRecord importRecord, string checkListId)
        {
            var header = importRecord.Header.First().ToString().ToUpper() + string.Join("", importRecord.Header.Skip(1));
            var template = importRecord.Template.First().ToString().ToUpper() + string.Join("", importRecord.Template.Skip(1));
            ((Dictionary<string,object>)component[header])[template] = checkListId;
        }

        public async Task UpdateCompositeComponent(Dictionary<string, List<ImportRecord>> importRecords, RestWebClient<CompositeComponentDto> webClient, string componentName, string filePath)
        {
            _messageBuilder.Clear();
            AddMessage($"Started updating the {componentName}");
            foreach (var record in importRecords)
            {
                AddMessage($"started updating component - {record.Key} ");
                try
                {
                    var result = await webClient.Get($"/{componentName}/{record.Key}");
                    var component = result.Body;
                    if ((component == null) || (result.StatusCode != HttpStatusCode.OK))
                    {
                        AddMessage(
                            $"component ID {record.Key} -Could not fetch required info, skipping the updation of component {record.Key}");
                        continue;
                    }

                    foreach (var importRecord in record.Value)
                    {
                        SetCheckListIdForCompositeComponent(component, importRecord, importRecord.CheckListId);
                        AddMessage(
                            $"component ID {record.Key} - Setting {importRecord.Header}.{importRecord.Template} = {importRecord.CheckListId}");
                    }

                    var updateResult = await webClient.Put(record.Key, component, componentName);
                    AddMessage(updateResult.StatusCode == HttpStatusCode.OK
                        ? $"component ID {record.Key} - Successfully updated it"
                        : $"component ID {record.Key} - Failed to update it");
                }
                catch (HttpResponseException ex)
                {
                    var data = await ex.Response.Content.ReadAsStringAsync();
                    AddMessage(
                        $"Component Id {record.Key} - Error during upload. Error details: {data}");
                }
                catch (Exception ex)
                {
                    AddMessage(
                        $"Component Id {record.Key} - Error during upload. Error details: {ex.Message}");
                }
                finally
                {
                    using (var streamWriter = new StreamWriter(filePath, true))
                    {
                        streamWriter.WriteLine(_messageBuilder.ToString());
                        _messageBuilder.Clear();
                    }
                }
            }
        }

        private void SetCheckListIdForCompositeComponent(CompositeComponentDto component, ImportRecord importRecord, string importRecordCheckListId)
        {
            foreach (var header in component.Headers)
            {
                if (header.Name.Equals(importRecord.Header, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var column in header.Columns)
                    {
                        if (column.Name.Equals(importRecord.Template, StringComparison.InvariantCultureIgnoreCase))
                        {
                            column.Value = importRecordCheckListId;
                        }
                    }
                }
            }
        }
    }
}