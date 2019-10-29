using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.CompositeComponents.SFGs
{
    public class SfgDefinitionImporter
    {
        private readonly List<MasterDataListDto> _masterData;
        private readonly StringBuilder _messageBuilder;
        private readonly IWebClient<SemiFinishedGoodDefinitionDao> _webClient;

        public SfgDefinitionImporter(IWebClient<SemiFinishedGoodDefinitionDao> webClient, List<MasterDataListDto> masterData)
        {
            _webClient = webClient;
            _masterData = masterData;
            _messageBuilder = new StringBuilder();
        }

        public void Import(string definitionName, string serviceDefinitionJson)
        {
            try
            {
                var definition = JsonConvert.DeserializeObject<SemiFinishedGoodDefinitionDao>(serviceDefinitionJson);
                SetObjectIdForMasterData(definition);
                _webClient.Post(definition, "/sfg-definitions").Wait();
                AddMessage($"Created : {definitionName}");
            }
            catch (Exception ex)
            {
                var httpResponseException = ex.InnerException as HttpResponseException;
                AddMessage(httpResponseException != null
                    ? $"{httpResponseException.Response.StatusCode.ToString().ToUpper()} : {definitionName} {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                    : $"{definitionName} import FAILED {ex.Message}: {ex.StackTrace}");
            }
            finally
            {
                WriteToFile();
            }
        }

        public void ImportAll(DirectoryInfo definitionFolder)
        {
            AddMessage("\n");
            AddMessage("SfgDefinition Import:");
            AddMessage("----------------------------");
            foreach (var file in definitionFolder.EnumerateFiles())
            {
                var jsonContent = File.ReadAllText(file.FullName);
                Import(file.Name, jsonContent);
            }
            AddMessage("\n");
            AddMessage("SfgDefinition Import Complete.");
        }

        private void AddMessage(string message)
        {
            Console.WriteLine(message);
            _messageBuilder.Append(message);
            _messageBuilder.Append(Environment.NewLine);
        }

        private string GetMasterDataistId(string dataTypeSubType)
        {
            var masterDataList = _masterData.Where(m => m.Name == dataTypeSubType).Select(m => m.Id).FirstOrDefault();
            if (masterDataList == null)
            {
                Console.WriteLine($"Definition import failed, master data {dataTypeSubType} not found");
                throw new ArgumentException($"Invalid master data {dataTypeSubType} specified");
            }
            return masterDataList;
        }

        private void SetObjectIdForMasterData(SemiFinishedGoodDefinitionDao definition)
        {
            foreach (var headerDefinitionDto in definition.Headers)
            {
                foreach (var columnDefinitionDto in headerDefinitionDto.Columns)
                {
                    if (columnDefinitionDto.DataType.Name == "MasterData")
                    {
                        columnDefinitionDto.DataType.SubType =
                            GetMasterDataistId(columnDefinitionDto.DataType.SubType.ToString());
                    }
                }
            }
        }

        private void WriteToFile()
        {
            var filePath = "SfgDefinitionUpload.txt";
            using (var streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(_messageBuilder.ToString());
                _messageBuilder.Clear();
            }
        }
    }
}