using System;
using System.IO;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.ExcelImporter.Code.CPRs
{
	public class CostPriceRatioDefinitionImporter
	{
		private readonly StringBuilder _messageBuilder;
		private readonly IWebClient<CostPriceRatioDefinitionDto> _webClient;

		public CostPriceRatioDefinitionImporter(IWebClient<CostPriceRatioDefinitionDto> webClient)
		{
			_webClient = webClient;
			_messageBuilder = new StringBuilder();
		}

		public void Import(string definitionName, string costPriceRatioDefinitionJson)
		{
			try
			{
				var definition = JsonConvert.DeserializeObject<CostPriceRatioDefinitionDto>(costPriceRatioDefinitionJson);
				_webClient.Post(definition, "/costpriceratio-definitions").Wait();
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
			AddMessage("CostPriceRatioDefinition Import:");
			AddMessage("----------------------------");
			foreach (var file in definitionFolder.EnumerateFiles())
			{
				var jsonContent = File.ReadAllText(file.FullName);
				Import(file.Name, jsonContent);
			}
			AddMessage("\n");
			AddMessage("CostPriceRatioDefinition Import Complete.");
		}

		private void AddMessage(string message)
		{
			Console.WriteLine(message);
			_messageBuilder.Append(message);
			_messageBuilder.Append(Environment.NewLine);
		}

		private void WriteToFile()
		{
			var filePath = "CostPriceRatioDefinitionUpload.txt";
			using (var streamWriter = new StreamWriter(filePath, true))
			{
				streamWriter.WriteLine(_messageBuilder.ToString());
				_messageBuilder.Clear();
			}
		}
	}
}