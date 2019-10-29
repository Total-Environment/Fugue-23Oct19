using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Components.Materials
{
    public class MaterialDefinitionImporter
    {
        private readonly IWebClient<MaterialDefinitionDao> _webClient;
        private readonly List<MasterDataListDto> _masterData;
        private readonly StringBuilder _messageBuilder;

        public MaterialDefinitionImporter(IWebClient<MaterialDefinitionDao> webClient, List<MasterDataListDto> masterData)
        {
            _webClient = webClient;
            _masterData = masterData;
            _messageBuilder = new StringBuilder();
        }

        public void Import(string definitionName, string materialDefinitionJson)
        {
            try
            {
                var definition = JsonConvert.DeserializeObject<MaterialDefinitionDao>(materialDefinitionJson);
                SetObjectIdForMasterData(definition);
                _webClient.Post(definition, "/material-definitions").Wait();
                AddMessage($"Created : {definitionName}");
            }
            catch (Exception ex)
            {
                var httpResponseException = ex.InnerException as HttpResponseException;
                AddMessage(httpResponseException != null
                    ? $"{httpResponseException.Response.StatusCode.ToString().ToUpper()} : {definitionName} {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                    : $"{definitionName} import FAILED {ex.Message}: {ex.StackTrace}");
            }
        }
        public void ImportAll(DirectoryInfo definitionFolder)
        {
            AddMessage("\n");
            AddMessage("MaterialDefinition Import:");
            AddMessage("----------------------------");
            foreach (var file in definitionFolder.EnumerateFiles())
            {
                var jsonContent = File.ReadAllText(file.FullName);
                Import(file.Name, jsonContent);
            }
            AddMessage("\n");
            AddMessage("MaterialDefinition Import Complete.");
        }

        private void SetObjectIdForMasterData(MaterialDefinitionDao definition)
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

        private string GetMasterDataistId(string dataTypeSubType)
        {
            var masterDataList = _masterData.Where(m => m.Name == dataTypeSubType).Select(m=>m.Id).FirstOrDefault();
            if (masterDataList == null)
            {
                Console.WriteLine($"Definition import failed, master data {dataTypeSubType} not found");
                throw new ArgumentException($"Invalid master data {dataTypeSubType} specified");
            }
            return masterDataList;
        }

        private void AddMessage(string message)
        {
            Console.WriteLine(message);
            _messageBuilder.Append(message);
            _messageBuilder.Append(Environment.NewLine);
        }
    }
}