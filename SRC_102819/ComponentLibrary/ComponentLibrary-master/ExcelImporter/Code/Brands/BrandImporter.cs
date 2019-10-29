using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.RestWebClient;
using TE.Diagnostics.Logging;

namespace TE.ComponentLibrary.ExcelImporter.Code.Brands
{
    public class BrandImporter
    {
        private readonly IWebClient<BrandDefinitionDto> _brandDefinitionClient;
        private readonly Dictionary<string, List<StaticFileInformation>> _importedFiles;
        private readonly ILogger _logger;
        private readonly StringBuilder _messageBuilder;

        public BrandImporter(IWebClient<BrandDefinitionDto> brandDefinitionClient, Dictionary<string, List<StaticFileInformation>> importedFiles)
        {
            _brandDefinitionClient = brandDefinitionClient;
            _importedFiles = importedFiles;
            _messageBuilder = new StringBuilder();
            _logger = new Logger(typeof(BrandImporter));
        }

        public void ImportData(string excelPath, string sheetName, string serialisedBrandDefinition, IConfiguration configuration,
            IDocumentLogger documentLogger, IWebClient<MaterialDataDto> webClient)
        {
            AddMessage("\n");
            AddMessage("Brand Data Import:");
            AddMessage("----------------------------");
            var reader = new ExcelReader(excelPath);

            var brandDefinition = JsonConvert.DeserializeObject<BrandDefinitionDto>(serialisedBrandDefinition);
            var brandConverter = new BrandConverter(brandDefinition, _importedFiles);
            var jsonExporter = new BrandJsonExporter(documentLogger, brandConverter, webClient, _importedFiles);

            try
            {
                var rows = reader.GetRows(sheetName, configuration, 2).ToList();
                var startLoggingInfo =
                    $"Started writing {rows.Count} rows for sheet named {sheetName} of excel document {configuration.ExcelPath}";
                _logger.Info(startLoggingInfo);
                Console.WriteLine(startLoggingInfo);
                foreach (var row in rows)
                {
                    try
                    {
                        var materialId = reader.ReadRow(row, sheetName)["O"].Value.Trim();
                        var startInfo = $"Started exporting row with index {row.RowIndex}";
                        Console.WriteLine(startInfo);

                        var columnNameCellValuePairs = reader.ReadRowWithColumnNames(row, sheetName, "1");
                        jsonExporter.ExportJson(columnNameCellValuePairs, materialId);

                        var endInfo = $"Created : Rox Index {row.RowIndex}";
                        _logger.Info(endInfo);
                        Console.WriteLine(endInfo);
                    }
                    catch (NullReferenceException nre)
                    {
                        var message = $"ERROR : RoxIndex {row.RowIndex} : {nre.StackTrace}. Continuing with next row..";
                        _logger.Error(
                            message);
                        Console.WriteLine($"ERROR : RoxIndex {row.RowIndex} : Material Code not present. Continuing with next row.");
                    }
                    catch (KeyNotFoundException knfe)
                    {
                        var message = $"ERROR : RoxIndex {row.RowIndex} : {knfe.StackTrace}. Continuing with next row..";
                        _logger.Error(
                            message);
                        Console.WriteLine($"ERROR : RoxIndex {row.RowIndex} : Material Code not present. Continuing with next row.");
                    }
                    catch (InvalidOperationException ioe)
                    {
                        var message = $"ERROR : RoxIndex {row.RowIndex} : {ioe.StackTrace}. Continuing with next row..";
                        _logger.Error(
                            message);
                        Console.WriteLine($"ERROR : RoxIndex {row.RowIndex} : {ioe.Message} Continuing with next row.");
                    }
                    catch (Exception ex)
                    {
                        var message = $"ERROR : RoxIndex {row.RowIndex} : {ex.Message}. Continuing with next row.";
                        _logger.Error(
                            message);
                        Console.WriteLine($"ERROR : RoxIndex {row.RowIndex}. Continuing with next row.");
                    }
                }
                Console.WriteLine("Importing data complete");
                AddMessage("\n");
                AddMessage("Brand Data Import Complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        public void ImportDefinition(string serialisedBrandDefinition,
                    RestWebClient<MasterDataListDto> masterDataWebClient)
        {
            var masterDataResponse = masterDataWebClient.GetAll("/master-data").Result;
            if ((masterDataResponse == null) || (masterDataResponse.StatusCode != HttpStatusCode.OK))
            {
                Console.WriteLine("Master data not available, cannot import. Retry after sometime.");
                return;
            }
            var masterData = masterDataResponse.Body;

            AddMessage("\n");
            AddMessage("Brand Definition Import:");
            AddMessage("----------------------------");
            var brandDefinition = JsonConvert.DeserializeObject<BrandDefinitionDto>(serialisedBrandDefinition);
            SetObjectIdForMasterData(brandDefinition, masterData);
            try
            {
                _brandDefinitionClient.Post(brandDefinition, "definitions/brands").Wait();
                AddMessage($"Created : {brandDefinition.Name}");
            }
            catch (Exception ex)
            {
                var httpResponseException = ex.InnerException as HttpResponseException;
                AddMessage(httpResponseException != null
                    ? $"{httpResponseException.Response.StatusCode.ToString().ToUpper()} : Brand Definition {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                    : $"Brand Definition import FAILED {ex.Message}: {ex.StackTrace}");
            }
            AddMessage("\n");
            AddMessage("Brand Definition Import Complete.");
        }

        private void AddMessage(string message)
        {
            Console.WriteLine(message);
            _messageBuilder.Append(message);
            _messageBuilder.Append(Environment.NewLine);
        }

        private string GetMasterDataistId(string dataTypeSubType, List<MasterDataListDto> masterData)
        {
            var masterDataList = masterData.Where(m => m.Name == dataTypeSubType).Select(m => m.Id).FirstOrDefault();
            if (masterDataList == null)
            {
                Console.WriteLine($"Definition import failed, master data {dataTypeSubType} not found");
                throw new ArgumentException($"Invalid master data {dataTypeSubType} specified");
            }
            return masterDataList;
        }

        private void SetObjectIdForMasterData(BrandDefinitionDto definition, List<MasterDataListDto> masterData)
        {
            foreach (var columnDefinitionDto in definition.Columns)
                if (columnDefinitionDto.DataType.Name == "MasterData")
                    columnDefinitionDto.DataType.SubType =
                        GetMasterDataistId(columnDefinitionDto.DataType.SubType.ToString(), masterData);
        }
    }
}