using System;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.MasterData
{
    public class MasterDataImporter
    {
        private readonly IWebClient<MasterDataListDto> _webClient;

        public MasterDataImporter(IWebClient<MasterDataListDto> webClient)
        {
            _webClient = webClient;
        }

        public void Import(string masterDataJsonPath)
        {
            Log("\n");
            Log("MasterData Import:");
            Log("----------------------------");
            var masterDataJson = File.ReadAllText(masterDataJsonPath);
            var masterData = JsonConvert.DeserializeObject<MasterDataListJson>(masterDataJson);
            var requestList = masterData.RequestList();
            foreach (var request in requestList)
            {
                try
                {
                    _webClient.Post(request, "master-data").Wait();
                    Log($"Created : {request.Name}");
                }
                catch (Exception ex)
                {
                    var httpResponseException = ex.InnerException as HttpResponseException;
                    Log(httpResponseException != null
                        ? $"{httpResponseException.Response.StatusCode.ToString().ToUpper()} : {request.Name} {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                        : $"{request.Name} import FAILED {ex.Message}: {ex.StackTrace}");
                    continue;
                }
                Log($"Created : {request.Name}.");
            }
            Log("\n");
            Log("MasterData Import Complete.");
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
