using System;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.Components
{
    public class DependencyImporter
    {
        private readonly IWebClient<DependencyDefinitionDto> _webClient;

        public DependencyImporter(IWebClient<DependencyDefinitionDto> webClient)
        {
            _webClient = webClient;
        }

        public void Import(string dependencyJsonPath)
        {
            Log("\n");
            Log("Dependency Import:");
            Log("----------------------------");
            var dependencyJson = File.ReadAllText(dependencyJsonPath);
            var request = JsonConvert.DeserializeObject<DependencyDefinitionDto>(dependencyJson);
            try
            {
                _webClient.Post(request, "dependency").Wait();
                Log($"Created : {request.Name}");
            }
            catch (Exception ex)
            {
                var httpResponseException = ex.InnerException as HttpResponseException;
                Log(httpResponseException != null
                    ? $"{httpResponseException.Response.StatusCode.ToString().ToUpper()} : {request.Name} {httpResponseException.Response.Content.ReadAsStringAsync().Result}"
                    : $"{request.Name} import FAILED {ex.Message}: {ex.StackTrace}");
            }
            Log("\n");
            Log("Dependency Import Complete.");
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}