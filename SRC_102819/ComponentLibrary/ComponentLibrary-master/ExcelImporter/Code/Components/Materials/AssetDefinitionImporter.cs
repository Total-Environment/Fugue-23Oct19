using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Components.Materials
{
    class AssetDefinitionImporter
    {
        private readonly IWebClient<AssetDefinitionDto> _webClient;
        private readonly List<MasterDataListDto> _masterData;
        private readonly StringBuilder _messageBuilder;

        public AssetDefinitionImporter(IWebClient<AssetDefinitionDto> webClient, List<MasterDataListDto> masterData)
        {
            _webClient = webClient;
            _masterData = masterData;
            _messageBuilder = new StringBuilder();
        }

        public void ImportAll(DirectoryInfo definitionFolder)
        {
            AddMessage("\n");
            AddMessage("AssetDefinition Import:");
            AddMessage("----------------------------");
            foreach (var file in definitionFolder.EnumerateFiles())
            {
                var jsonContent = File.ReadAllText(file.FullName);
                Import(file.Name, jsonContent);
            }
            AddMessage("\n");
            AddMessage("AssetDefinition Import Complete.");
        }

        private void AddMessage(string message)
        {
            Console.WriteLine(message);
            _messageBuilder.Append(message);
            _messageBuilder.Append(Environment.NewLine);
        }

        public void Import(string definitionName, string materialDefinitionJson)
        {
            try
            {
                var definition = JsonConvert.DeserializeObject<AssetDefinitionDto>(materialDefinitionJson);
                SetObjectIdForMasterData(definition);
                _webClient.Post(definition, "/asset-definitions").Wait();
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

        private void SetObjectIdForMasterData(AssetDefinitionDto definition)
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
            var masterDataList = _masterData.Where(m => m.Name == dataTypeSubType).Select(m => m.Id).FirstOrDefault();
            if (masterDataList == null)
            {
                Console.WriteLine($"Definition import failed, master data {dataTypeSubType} not found");
                throw new ArgumentException($"Invalid master data {dataTypeSubType} specified");
            }
            return masterDataList;
        }
    }
}
