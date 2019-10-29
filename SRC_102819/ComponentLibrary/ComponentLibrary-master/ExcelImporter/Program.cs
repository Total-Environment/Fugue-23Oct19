using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ExcelImporter.Code;
using TE.ComponentLibrary.ExcelImporter.Code.Brands;
using TE.ComponentLibrary.ExcelImporter.Code.Checklists;
using TE.ComponentLibrary.ExcelImporter.Code.Components;
using TE.ComponentLibrary.ExcelImporter.Code.Components.Materials;
using TE.ComponentLibrary.ExcelImporter.Code.Components.Services;
using TE.ComponentLibrary.ExcelImporter.Code.CompositeComponents.Packages;
using TE.ComponentLibrary.ExcelImporter.Code.CompositeComponents.SFGs;
using TE.ComponentLibrary.ExcelImporter.Code.CPRs;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.ComponentLibrary.ExcelImporter.Code.MasterData;
using TE.ComponentLibrary.ExcelImporter.Code.Rates;
using TE.ComponentLibrary.RestWebClient;
using TE.Shared.CloudServiceFramework.Infrastructure;
using Configuration = TE.ComponentLibrary.ExcelImporter.Code.Configuration;

namespace TE.ComponentLibrary.ExcelImporter
{
	public class Program
	{
		private static void AssetDataImport(string[] args, Configuration configuration)
		{
			var sheetName = args[1];
			var excelPath = args[2];
			var materialDefinitionPath = args[3];
			var assetDefinitionPath = args[4];
			var restWebClient = new RestWebClient<object>(configuration.ComponentLibraryBaseUrl);
			var exporter = new ExcelExporter(configuration, excelPath);
			var materialDefinition = File.ReadAllText(materialDefinitionPath);
			var assetDefinition = File.ReadAllText(assetDefinitionPath);
			var deserialisedMaterialDefinition =
				JsonConvert.DeserializeObject<MaterialDefinitionDao>(materialDefinition);
			var deserialisedAssetDefinition =
				JsonConvert.DeserializeObject<AssetDefinitionDto>(assetDefinition);
			var definition = MergeDefinition(deserialisedAssetDefinition, deserialisedMaterialDefinition);

			Console.WriteLine();
			Console.WriteLine($"{definition.Name} data Import:");
			Console.WriteLine("----------------------------");
			var headers = exporter.GetHeaders(sheetName, "3");
			var assetConverter = new ComponentConverter(headers, definition);
			var documentLogger = new ComponentDocumentLogger(configuration, headers);
			var jsonExporter = new ComponentJsonExporter(documentLogger, assetConverter, restWebClient);
			exporter.ExportExcel(sheetName, jsonExporter, "materials-old");
			Console.WriteLine();
			Console.WriteLine($"{definition.Name} data import Complete.");
		}

		private static void AssetDefinitionImport(string[] args, Configuration configuration)
		{
			var materialDefinitionFolderPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);
			var masterDataResponse = masterDataWebClient.GetAll("/master-data").Result;
			if ((masterDataResponse == null) || (masterDataResponse.StatusCode != HttpStatusCode.OK))
			{
				Console.WriteLine("Master data not available, cannot import. Retry after sometime.");
				return;
			}
			var masterData = masterDataResponse.Body;
			var materialDefinitionDataWebClient =
				new RestWebClient<AssetDefinitionDto>(configuration.ComponentLibraryBaseUrl);
			var materialDefinitionImporter = new AssetDefinitionImporter(materialDefinitionDataWebClient,
				masterData);
			var definitionFolder = new DirectoryInfo(materialDefinitionFolderPath);

			materialDefinitionImporter.ImportAll(definitionFolder);
		}

		private static void CheckListImport(Configuration configuration, string fileName, string resourceName,
			string endPoint)
		{
			var recordsToImport = File.ReadLines(fileName);
			var checklistWebClient = new RestWebClient<CheckList>(configuration.ComponentLibraryBaseUrl);
			var webClient = new RestWebClient<Dictionary<string, object>>(configuration.ComponentLibraryBaseUrl);
			var builder = new TabularDataParserBuilder();
			var checkListConfigurationReader = new CheckListConigurationReader();
			var checkListImporter = new CheckListImporter(builder, checkListConfigurationReader);
			var checkListVerifier = new FileVerifier(configuration.CheckListBasePath,
				new[] { configuration.CheckListFileExtension });
			var importRecords = new ImportRecordParser(checkListVerifier).Parse(recordsToImport);
			checkListImporter.Import(importRecords, checklistWebClient).Wait();
			var message = "Upload of check lists is completed";
			Console.WriteLine(message);
			WriteToFile($"ChecklistUploadFor{resourceName}s.txt", message);
			var filePath = $"{resourceName}Update.txt";
			checkListImporter.Update(importRecords, webClient, endPoint, filePath).Wait();
			message = $"{resourceName} mapping is completed";
			Console.WriteLine(message);
			WriteToFile(filePath, message);
		}

		private static void CheckListImportForSfgs(string[] args, Configuration configuration)
		{
			const string resourceName = "Sfg";
			const string endPoint = "sfgs";
			var fileName = args[1];
			CheckListImportForCompositeComponents(configuration, fileName, resourceName, endPoint);
		}

		private static void CheckListImportForPackages(string[] args, Configuration configuration)
		{
			const string resourceName = "Package";
			const string endPoint = "packages";
			var fileName = args[1];
			CheckListImportForCompositeComponents(configuration, fileName, resourceName, endPoint);
		}

		private static void CheckListImportForCompositeComponents(Configuration configuration, string fileName,
			string resourceName, string endPoint)
		{
			var recordsToImport = File.ReadLines(fileName);
			var checklistWebClient = new RestWebClient<CheckList>(configuration.ComponentLibraryBaseUrl);
			var webClient = new RestWebClient<CompositeComponentDto>(configuration.ComponentLibraryBaseUrl);
			var builder = new TabularDataParserBuilder();
			var checkListConfigurationReader = new CheckListConigurationReader();
			var checkListImporter = new CheckListImporter(builder, checkListConfigurationReader);
			var checkListVerifier = new FileVerifier(configuration.CheckListBasePath,
				new[] { configuration.CheckListFileExtension });
			var importRecords = new ImportRecordParser(checkListVerifier).Parse(recordsToImport);
			checkListImporter.Import(importRecords, checklistWebClient).Wait();
			var message = "Upload of check lists is completed";
			Console.WriteLine(message);
			WriteToFile($"ChecklistUploadFor{resourceName}s.txt", message);
			var filePath = $"{resourceName}Update.txt";
			checkListImporter.UpdateCompositeComponent(importRecords, webClient, endPoint, filePath).Wait();
			message = $"{resourceName} mapping is completed";
			Console.WriteLine(message);
			WriteToFile(filePath, message);
		}

		private static void CheckListImportForMaterials(IReadOnlyList<string> args, Configuration configuration)
		{
			const string resourceName = "Material";
			const string endPoint = "materials-old";
			var fileName = args[1];
			CheckListImport(configuration, fileName, resourceName, endPoint);
		}

		private static void CheckListImportForServices(IReadOnlyList<string> args, Configuration configuration)
		{
			const string resourceName = "Service";
			const string endPoint = "services/old";
			var fileName = args[1];
			CheckListImport(configuration, fileName, resourceName, endPoint);
		}

		private static void DependencyImport(string[] args, Configuration configuration)
		{
			var dependencyJsonPath = args[1];
			var dependencyWebClient =
				new RestWebClient<DependencyDefinitionDto>(configuration.ComponentLibraryBaseUrl);
			var dependencyImporter = new DependencyImporter(dependencyWebClient);
			dependencyImporter.Import(dependencyJsonPath);
		}

		private static void ExchangeRateImport(IReadOnlyList<string> args, Configuration configuration)
		{
			var exchangeRatePath = args[1];
			var exchangeRateParserbuilder = new TabularDataParserBuilder();
			var exchangeRateWebClient =
				new RestWebClient<ExchangeRateDto>(configuration.ComponentLibraryBaseUrl);
			var exchangeRateImporter = new ExchangeRateImporter(exchangeRateParserbuilder);
			exchangeRateImporter.Import(exchangeRateWebClient, exchangeRatePath, "Exchange Rates").Wait();
		}

		private static void GeneralHelpText()
		{
			Console.WriteLine(
				"Use './ExcelImporter.exe -h {option}' to get more details.\n" +
				"The options available are :\n" +
				"\tmasterdataimport\n" +
				"\texchangerate\n" +
				"\tdependency\n" +
				"\tmaterialdefinition\n" +
				"\tassetdefinition\n" +
				"\tmaterial\n" +
				"\tuniversalmaterialratemaster\n" +
				"\tuniversalserviceratemaster\n" +
				"\tmaterialstaticfiles\n" +
				"\tmaterialchecklist\n" +
				"\tservicedefinition\n" +
				"\tsfgdefinition\n" +
				"\tservice\n" +
				"\tservicestaticfiles\n" +
				"\tservicechecklist\n" +
				"\tsfgchecklist\n" +
				"\tpackagechecklist\n" +
				"\tautomate\n" +
				"\tautomatematerialdata\n" +
				"\tautomateservicedata\n" +
				"\tasset\n" +
				"\tautomateassetdata\n" +
				"\tserviceclassificationdefinition\n" +
				"\tsfgclassificationdefinition\n" +
				"\tpackageclassificationdefinition\n" +
				"\trentalrates\n" +
				"\tbranddefinition\n" +
				"\tbranddata\n" +
				"\tsfgmapping\n" +
				"\tpackagemapping\n");
		}

		private static void GenerateHelpText(string[] args)
		{
			var help = new StringBuilder();
			try
			{
				var option = args[1];
				switch (option)
				{
					case "masterdataimport":
						help.Append("0. Usage: ExcelImporter.exe masterdataimport PathToMasterDataJson");
						help.Append(Environment.NewLine);
						break;

					case "exchangerate":
						help.Append("0. Usage: ExcelImporter.exe exchangerate PathToExchangeRateExcel");
						help.Append(Environment.NewLine);
						help.Append("1. Pass and excel sheet which has 'Exchnage Rates' sheet.");
						help.Append(Environment.NewLine);
						break;

					case "dependency":
						break;

					case "materialdefinition":
						help.Append("0. Usage: ExcelImporter.exe materialdefinition PathToFolderWithConfigurationJsons");
						help.Append(Environment.NewLine);
						help.Append("1. Pass and folder which has all the configurations.");
						help.Append(Environment.NewLine);
						break;

					case "material":
						break;

					case "universalmaterialratemaster":
						break;

					case "universalserviceratemaster":
						break;

					case "materialstaticfiles":
						help.Append(
							"0. Usage: ExcelImporter.exe staticfiles staticFilesListingFilePath staticFilesLocationPath");
						help.Append(Environment.NewLine);
						help.Append(
							"1. Pass the file path of the text file that contains all the static files associated with a material");
						help.Append(Environment.NewLine);
						help.Append("2. Pass the path of the folder where all the static files are located");
						help.Append(Environment.NewLine);
						help.Append(
							"3. The import will fail if the static file cannot be found, if the material doesn't exist or if the static file is not of .jpg, .jpeg, .png or .pdf format");
						help.Append(Environment.NewLine);
						break;

					case "materialchecklist":
						help.Append("0. Usage: ExcelImporter.exe checklist import_text_filepath.");
						help.Append(Environment.NewLine);
						help.Append("1. Provide import text file path which has a list of checklists to import.");
						help.Append(Environment.NewLine);
						help.Append("2. Format of record is MaterialID~Header:Subheader%CheckListId,checklistpath.");
						help.Append(Environment.NewLine);
						help.Append("3. All checklist paths are relative to executable.");
						help.Append(Environment.NewLine);
						break;

					case "servicedefinition":
						break;

					case "service":
						break;

					case "serviceclassificationdefinition":
						help.Append(
							"0. Usage: ExcelImporter.exe serviceclassificationdefinition ServiceClassificationDefinitions/");
						help.Append(Environment.NewLine);
						break;

					default:
						Console.WriteLine("Invalid option provided");
						break;
				}

				Console.WriteLine(help.ToString());
			}
			catch (IndexOutOfRangeException)
			{
				GeneralHelpText();
			}
			finally
			{
				Environment.Exit(0);
			}
		}

		private static void ImportAll(Configuration configuration)
		{
			MasterDataImport(new[] { "", ".\\Data\\masterData.json" }, configuration);
			DependencyImport(new[] { "", ".\\Data\\Materials\\materialClassifications.json" }, configuration);
			DependencyImport(new[] { "", ".\\Data\\Services\\serviceClassifications.json" }, configuration);
			DependencyImport(new[] { "", ".\\Data\\SFGs\\sfgClassifications.json" }, configuration);
			DependencyImport(new[] { "", ".\\Data\\Packages\\packageClassifications.json" }, configuration);

			//ImportServiceClassificationDefinitions(new[] { "", ".\\ServiceClassificationDefinitions\\Flooring.xlsx" }, configuration);

			ImportBrandDefinition(new[] { "", ".\\Data\\Brands\\Definitions\\Brand.json" }, configuration);
			MaterialDefinitionImport(new[] { "", ".\\Data\\Materials\\Definitions" }, configuration);
			ServiceDefinitionImport(new[] { "", ".\\Data\\Services\\Definitions" }, configuration);
			AssetDefinitionImport(new[] { "", ".\\Data\\Materials\\Assets\\Definitions" }, configuration);
			SfgDefinitionImport(new[] { "", ".\\Data\\SFGs\\Definitions" }, configuration);
			ImportSfgMapping(new[] { "", ".\\Data\\SFGs\\Mappings\\sfgmapping.json" }, configuration);
			PackageDefinitionImport(new[] { "", ".\\Data\\Packages\\Definitions" }, configuration);
			ImportPackageMapping(new[] { "", ".\\Data\\Packages\\Mappings\\packagemapping.json" }, configuration);
			CostPriceRatioDefinitionImport(new[] { "", ".\\Data\\CPRs\\Definitions" }, configuration);
		}

		private static void ImportAssetData(Configuration configuration)
		{
			AssetDataImport(new[]
			{
				"", "Machines", ".\\Data\\Materials\\Assets\\Excels\\Assets-MachineData.xlsx",
				".\\Data\\Materials\\Definitions\\Machine.json", ".\\Data\\Materials\\Assets\\Definitions\\Machine.json"
			}, configuration);
		}

		private static void ImportBrandData(string[] args, IConfiguration configuration)
		{
			var sheetName = args[1];
			var excelPath = args[2];
			var definitionPath = args[3];

			var brandDefinition = File.ReadAllText(definitionPath);

			var readr = new ExcelReader(excelPath);

			var importedFiles = ImportBrandStaticFilesAsync(configuration, readr, sheetName).Result;

			var restWebClient = new RestWebClient<MaterialDataDto>(configuration.ComponentLibraryBaseUrl);
			var client = new RestWebClient<BrandDefinitionDto>(configuration.ComponentLibraryBaseUrl);
			var brandImporter = new BrandImporter(client, importedFiles);

			var documentLogger = new BrandDocumentLogger(configuration);
			brandImporter.ImportData(excelPath, sheetName, brandDefinition, configuration, documentLogger, restWebClient);
		}

		private static void ImportBrandDefinition(string[] args, IConfiguration configuration)
		{
			var definitionPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);

			var client = new RestWebClient<BrandDefinitionDto>(configuration.ComponentLibraryBaseUrl);
			var brandImporter = new BrandImporter(client, null);
			var brandDefinition = File.ReadAllText(definitionPath);
			brandImporter.ImportDefinition(brandDefinition, masterDataWebClient);
		}

		private static async Task<Dictionary<string, List<StaticFileInformation>>> ImportBrandStaticFilesAsync(
			IConfiguration configuration, ExcelReader reader, string sheetName)
		{
			var staticFiles = reader.GetContiguousRowBlock(sheetName, 2, "A");
			var filesToUpload = new List<string>();
			foreach (var cell in staticFiles)
			{
				var brandCode = cell.ContainsKey("A") ? cell["A"]?.Value : null;
				var manufacturersSpecification = cell.ContainsKey("E") ? cell["E"]?.Value : null;
				var materialSafetyDataSheet = cell.ContainsKey("F") ? cell["F"]?.Value : null;
				var technicalDrawing = cell.ContainsKey("G") ? cell["G"]?.Value : null;
				filesToUpload.Add(
					$"{brandCode}~: Manufacturer's Specification% {manufacturersSpecification}~: Material Safety Data Sheet% {materialSafetyDataSheet}~: Technical Drawing% {technicalDrawing}");
			}

			return StaticFileImport(configuration, filesToUpload, "Brand", "", false).Result;
		}

		private static void ImportMaterialData(IConfiguration configuration)
		{
			MaterialDataImport(
				new[]
				{
					"", "Material", ".\\Data\\Materials\\Excels\\AluminiumAndCopper.xlsx",
					".\\Data\\Materials\\Definitions\\Aluminium and Copper.json"
				}, configuration);
		}

		private static void ImportRentalRates(string[] args, IConfiguration configuration)
		{
			var client = new RestWebClient<RentalRateDto>(configuration.ComponentLibraryBaseUrl);
			var importer = new RentalRatesImporter(client);
			importer.Import(args[1]);
		}

		private static void ImportServiceClassificationDefinitions(string[] args, IConfiguration configuration)
		{
			var client =
				new RestWebClient<Dictionary<string, Dictionary<string, string>>>(configuration.ComponentLibraryBaseUrl);
			var importer = new ServiceClassificationDefinitionImporter(client);
			importer.Import(args[1]);
		}

		private static void ImportSfgClassificationDefinitions(string[] args, IConfiguration configuration)
		{
			var client =
				new RestWebClient<Dictionary<string, Dictionary<string, string>>>(configuration.ComponentLibraryBaseUrl);
			var importer = new SfgClassificationDefinitionImporter(client);
			importer.Import(args[1]);
		}

		private static void ImportPackageClassificationDefinitions(string[] args, IConfiguration configuration)
		{
			var client =
				new RestWebClient<Dictionary<string, Dictionary<string, string>>>(configuration.ComponentLibraryBaseUrl);
			var importer = new PackageClassificationDefinitionImporter(client);
			importer.Import(args[1]);
		}

		private static void ImportServiceData(IConfiguration configuration)
		{
			ServiceDataImport(
				new[] { "", "Service Master", ".\\Data\\Services\\Excels\\Masonry.xlsx", ".\\Data\\Services\\Definitions\\Masonry.json" },
				configuration);
		}

		private static void ImportSfgMapping(string[] args, IConfiguration configuration)
		{
			var mappingFilePath = args[1];
			var mappingDataWebClient =
				new RestWebClient<CompositeComponentMapping>(configuration.ComponentLibraryBaseUrl);
			var definition = File.ReadAllText(mappingFilePath);
			var deserialisedMappingDefinition =
				JsonConvert.DeserializeObject<CompositeComponentMapping>(definition);
			Console.WriteLine();
			Console.WriteLine($"SFG Mapping data Import:");
			Console.WriteLine("----------------------------");
			mappingDataWebClient.Post(deserialisedMappingDefinition, "sfg-mapping").Wait();
			Console.WriteLine($"SFG Mapping data Import Complete.");
		}

		private static void ImportPackageMapping(string[] args, Configuration configuration)
		{
			var mappingFilePath = args[1];
			var mappingDataWebClient =
				new RestWebClient<CompositeComponentMapping>(configuration.ComponentLibraryBaseUrl);
			var definition = File.ReadAllText(mappingFilePath);
			var deserialisedMappingDefinition =
				JsonConvert.DeserializeObject<CompositeComponentMapping>(definition);
			Console.WriteLine();
			Console.WriteLine($"Package Mapping data Import:");
			Console.WriteLine("----------------------------");
			mappingDataWebClient.Post(deserialisedMappingDefinition, "package-mapping").Wait();
			Console.WriteLine($"Package Mapping data Import Complete.");
		}

		private static void Main(string[] args)
		{
			var configuration = new Configuration();
			GeneralHelpText();

			if (args[0].ToLower() == "-h")
				GenerateHelpText(args);

			var selectedOption = args[0].ToLower();

			switch (selectedOption)
			{
				case "masterdataimport":
					MasterDataImport(args, configuration);
					break;

				case "exchangerate":
					ExchangeRateImport(args, configuration);
					break;

				case "dependency":
					DependencyImport(args, configuration);
					break;

				case "materialdefinition":
					MaterialDefinitionImport(args, configuration);
					break;

				case "assetdefinition":
					AssetDefinitionImport(args, configuration);
					break;

				case "material":
					MaterialDataImport(args, configuration);
					break;

				case "universalmaterialratemaster":
					UniversalMaterialRateMasterImport(args, configuration);
					break;

				case "universalserviceratemaster":
					UniversalServiceRateMasterImport(args, configuration);
					break;

				case "materialstaticfiles":
					StaticFileImportForMaterials(args, configuration);
					break;

				case "materialchecklist":
					CheckListImportForMaterials(args, configuration);
					break;

				case "servicedefinition":
					ServiceDefinitionImport(args, configuration);
					break;

				case "sfgdefinition":
					SfgDefinitionImport(args, configuration);
					break;

				case "service":
					ServiceDataImport(args, configuration);
					break;

				case "servicestaticfiles":
					StaticFileImportForServices(args, configuration);
					break;

				case "servicechecklist":
					CheckListImportForServices(args, configuration);
					break;

				case "sfgchecklist":
					CheckListImportForSfgs(args, configuration);
					break;

				case "packagechecklist":
					CheckListImportForPackages(args, configuration);
					break;

				case "automate":
					ImportAll(configuration);
					break;

				case "automatematerialdata":
					ImportMaterialData(configuration);
					break;

				case "automateservicedata":
					ImportServiceData(configuration);
					break;

				case "asset":
					AssetDataImport(args, configuration);
					break;

				case "automateassetdata":
					ImportAssetData(configuration);
					break;

				case "serviceclassificationdefinition":
					ImportServiceClassificationDefinitions(args, configuration);
					break;

				case "sfgclassificationdefinition":
					ImportSfgClassificationDefinitions(args, configuration);
					break;

				case "packageclassificationdefinition":
					ImportPackageClassificationDefinitions(args, configuration);
					break;

				case "rentalrates":
					ImportRentalRates(args, configuration);
					break;

				case "branddefinition":
					ImportBrandDefinition(args, configuration);
					break;

				case "branddata":
					ImportBrandData(args, configuration);
					break;

				case "sfgmapping":
					ImportSfgMapping(args, configuration);
					break;

				case "packagemapping":
					ImportPackageMapping(args, configuration);
					break;

				default:
					GenerateHelpText(new string[0]);
					break;
			}
		}

		private static void MasterDataImport(string[] args, Configuration configuration)
		{
			var masterDataImportPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);
			var masterDataImporter = new MasterDataImporter(masterDataWebClient);
			masterDataImporter.Import(masterDataImportPath);
		}

		private static void MaterialDataImport(string[] args, IConfiguration configuration)
		{
			var sheetName = args[1];
			var excelPath = args[2];
			var definitionPath = args[3];
			var restWebClient = new RestWebClient<object>(configuration.ComponentLibraryBaseUrl);
			var exporter = new ExcelExporter(configuration, excelPath);
			var definition = File.ReadAllText(definitionPath);
			var deserialisedMaterialDefinition =
				JsonConvert.DeserializeObject<MaterialDefinitionDao>(definition);
			Console.WriteLine();
			Console.WriteLine($"{deserialisedMaterialDefinition.Name} data Import:");
			Console.WriteLine("----------------------------");
			var headers = exporter.GetHeaders(sheetName, "3");
			var materialConverter = new ComponentConverter(headers, deserialisedMaterialDefinition);
			var documentLogger = new ComponentDocumentLogger(configuration, headers);
			var jsonExporter = new ComponentJsonExporter(documentLogger, materialConverter, restWebClient);
			exporter.ExportExcel(sheetName, jsonExporter, "materials-old");
			Console.WriteLine();
			Console.WriteLine($"{deserialisedMaterialDefinition.Name} data import Complete.");
		}

		private static void MaterialDefinitionImport(string[] args, Configuration configuration)
		{
			var materialDefinitionFolderPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);
			var masterDataResponse = masterDataWebClient.GetAll("/master-data").Result;
			if ((masterDataResponse == null) || (masterDataResponse.StatusCode != HttpStatusCode.OK))
			{
				Console.WriteLine("Master data not available, cannot import. Retry after sometime.");
				return;
			}
			var masterData = masterDataResponse.Body;
			var materialDefinitionDataWebClient =
				new RestWebClient<MaterialDefinitionDao>(configuration.ComponentLibraryBaseUrl);
			var materialDefinitionImporter = new MaterialDefinitionImporter(materialDefinitionDataWebClient,
				masterData);
			var definitionFolder = new DirectoryInfo(materialDefinitionFolderPath);

			materialDefinitionImporter.ImportAll(definitionFolder);
		}

		private static MaterialDefinitionDao MergeDefinition(AssetDefinitionDto assetDefinitionDto,
			MaterialDefinitionDao materialDefinitionDao)
		{
			assetDefinitionDto.Headers.AddRange(materialDefinitionDao.Headers);
			var headers = assetDefinitionDto.Headers;
			return new MaterialDefinitionDao
			{
				Id = materialDefinitionDao.Id,
				Code = materialDefinitionDao.Code,
				Name = materialDefinitionDao.Name,
				ObjectId = materialDefinitionDao.ObjectId,
				Headers = headers
			};
		}

		private static void ServiceDataImport(string[] args, IConfiguration configuration)
		{
			var sheetName = args[1];
			var excelPath = args[2];
			var definitionPath = args[3];
			var definition = File.ReadAllText(definitionPath);

			var serviceDefinition =
				JsonConvert.DeserializeObject<ServiceDefinitionDao>(definition);
			Console.WriteLine();
			Console.WriteLine($"{serviceDefinition.Name} data Import:");
			Console.WriteLine("----------------------------");
			var restWebClient = new RestWebClient<object>(configuration.ComponentLibraryBaseUrl);
			var exporter = new ExcelExporter(configuration, excelPath);
			var headers = exporter.GetHeaders(sheetName, "3");
			var componentConverter = new ComponentConverter(headers, serviceDefinition);
			var documentLogger = new ComponentDocumentLogger(configuration, headers);
			var jsonExporter = new ComponentJsonExporter(documentLogger, componentConverter, restWebClient);
			exporter.ExportExcel(sheetName, jsonExporter, "services/old");
			Console.WriteLine();
			Console.WriteLine($"{serviceDefinition.Name} data import Complete.");
		}

		private static void ServiceDefinitionImport(string[] args, Configuration configuration)
		{
			var serviceDefinitionFolderPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);
			var masterDataResponse = masterDataWebClient.GetAll("/master-data").Result;
			if ((masterDataResponse == null) || (masterDataResponse.StatusCode != HttpStatusCode.OK))
			{
				Console.WriteLine("Master data not available, cannot import. Retry after sometime.");
				return;
			}
			var masterData = masterDataResponse.Body;
			var serviceDefinitionDataWebClient =
				new RestWebClient<ServiceDefinitionDao>(configuration.ComponentLibraryBaseUrl);
			var serviceDefinitionImporter = new ServiceDefinitionImporter(serviceDefinitionDataWebClient,
				masterData);
			var definitionFolder = new DirectoryInfo(serviceDefinitionFolderPath);

			serviceDefinitionImporter.ImportAll(definitionFolder);
		}

		private static void SfgDefinitionImport(string[] args, Configuration configuration)
		{
			var sfgDefinitionFolderPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);
			var masterDataResponse = masterDataWebClient.GetAll("/master-data").Result;
			if ((masterDataResponse == null) || (masterDataResponse.StatusCode != HttpStatusCode.OK))
			{
				Console.WriteLine("Master data not available, cannot import. Retry after sometime.");
				return;
			}
			var masterData = masterDataResponse.Body;
			var sfgDefinitionDataWebClient =
				new RestWebClient<SemiFinishedGoodDefinitionDao>(configuration.ComponentLibraryBaseUrl);
			var sfgDefinitionImporter = new SfgDefinitionImporter(sfgDefinitionDataWebClient,
				masterData);
			var definitionFolder = new DirectoryInfo(sfgDefinitionFolderPath);

			sfgDefinitionImporter.ImportAll(definitionFolder);
		}

		private static void PackageDefinitionImport(string[] args, Configuration configuration)
		{
			var packageDefinitionFolderPath = args[1];
			var masterDataWebClient = new RestWebClient<MasterDataListDto>(configuration.ComponentLibraryBaseUrl);
			var masterDataResponse = masterDataWebClient.GetAll("/master-data").Result;
			if ((masterDataResponse == null) || (masterDataResponse.StatusCode != HttpStatusCode.OK))
			{
				Console.WriteLine("Master data not available, cannot import. Retry after sometime.");
				return;
			}
			var masterData = masterDataResponse.Body;
			var packageDefinitionDataWebClient =
				new RestWebClient<PackageDefinitionDao>(configuration.ComponentLibraryBaseUrl);
			var packageDefinitionImporter = new PackageDefinitionImporter(packageDefinitionDataWebClient,
				masterData);
			var definitionFolder = new DirectoryInfo(packageDefinitionFolderPath);

			packageDefinitionImporter.ImportAll(definitionFolder);
		}

		private static void CostPriceRatioDefinitionImport(string[] args, Configuration configuration)
		{
			var costPriceRatioDefinitionFolderPath = args[1];
			var costPriceRatioDefinitionDataWebClient =
				new RestWebClient<CostPriceRatioDefinitionDto>(configuration.ComponentLibraryBaseUrl);
			var costPriceRatioDefinitionImporter =
				new CostPriceRatioDefinitionImporter(costPriceRatioDefinitionDataWebClient);
			var definitionFolder = new DirectoryInfo(costPriceRatioDefinitionFolderPath);

			costPriceRatioDefinitionImporter.ImportAll(definitionFolder);
		}

		private static async Task<Dictionary<string, List<StaticFileInformation>>> StaticFileImport(
			IConfiguration configuration,
			IEnumerable<string> fileContents,
			string resourceName, string endPoint, bool linkToComponent = true)
		{
			var importedFiles = new Dictionary<string, List<StaticFileInformation>>();
			var staticFilesLocationPath = configuration.CheckListBasePath;

			var checkListVerifier = new FileVerifier(staticFilesLocationPath,
                new[] { ".jpg", ".JPG", ".jpeg", ".png", ".pdf", ".PNG" , ".PDF", ".JPEG"});
			var staticFilesImporter = new StaticFileImporter(fileContents, checkListVerifier);
			var fileReader = new FileReader();
			Console.WriteLine($"Parsing contents of file and retrieving {resourceName} with static files");
			var staticFilesDictionary = staticFilesImporter.Parse();
			var staticFileWebClient = new RestWebClient<StaticFile>(configuration.ComponentLibraryBaseUrl);
			var materialWebClient = new RestWebClient<Dictionary<string, object>>(
				configuration.ComponentLibraryBaseUrl);
			var azureBlobService = new AzureBlobStorageService(configuration.AzureConnectionString);
			foreach (var staticFileList in staticFilesDictionary)
			{
				Console.WriteLine($"Starting static file import for {resourceName} {staticFileList.Key}");
				var staticFiles =
					await staticFilesImporter.UploadToAzure(staticFileList, configuration, azureBlobService,
						fileReader, staticFilesLocationPath);
				importedFiles.Add(staticFileList.Key, staticFiles);

				await staticFilesImporter.LinkToComponent(staticFileList.Key, staticFiles, staticFileWebClient,
					materialWebClient, endPoint, linkToComponent);
				Console.WriteLine($"Completed operations for material {staticFileList.Key} \n");
			}

			Console.WriteLine($"Static file import for {resourceName}s is completed");
			return importedFiles;
		}

		private static void StaticFileImportForMaterials(string[] args, Configuration configuration)
		{
			const string resourceName = "material";
			const string endPoint = "materials-old";
			var staticFilesUnparsedFileLocationPath = args[1];
			Console.WriteLine($"Reading contents of file : {staticFilesUnparsedFileLocationPath}");
			var fileContents = File.ReadLines(staticFilesUnparsedFileLocationPath);
			StaticFileImport(configuration, fileContents, resourceName, endPoint).Wait();
		}

		private static void StaticFileImportForServices(string[] args, Configuration configuration)
		{
			const string resourceName = "service";
			const string endPoint = "services/old";
			var staticFilesUnparsedFileLocationPath = args[1];
			Console.WriteLine($"Reading contents of file : {staticFilesUnparsedFileLocationPath}");
			var fileContents = File.ReadLines(staticFilesUnparsedFileLocationPath);
			StaticFileImport(configuration, fileContents, resourceName, endPoint).Wait();
		}

		private static void UniversalMaterialRateMasterImport(string[] args, Configuration configuration)
		{
			var universalMaterialRateMasterPath = args[1];
			var universalMaterialRateMasterParserbuilder = new TabularDataParserBuilder();
			var universalMaterialRateMasterWebClient =
				new RestWebClient<MaterialRateDto>(configuration.ComponentLibraryBaseUrl);
			var universalMaterialRateMasterImporter =
				new UniversalMaterialRateMasterImporter(universalMaterialRateMasterParserbuilder);
			universalMaterialRateMasterImporter.Import(universalMaterialRateMasterWebClient,
					universalMaterialRateMasterPath, "Universal Material Rate Master", DateTime.Today.AddDays(1))
				.Wait();
		}

		private static void UniversalServiceRateMasterImport(string[] args, Configuration configuration)
		{
			var universalServiceRateMasterPath = args[1];
			var universalServiceRateMasterParserbuilder = new TabularDataParserBuilder();
			var universalServiceRateMasterWebClient =
				new RestWebClient<ServiceRateDto>(configuration.ComponentLibraryBaseUrl);
			var universalServiceRateMasterImporter =
				new UniversalServiceRateMasterImporter(universalServiceRateMasterParserbuilder);
			universalServiceRateMasterImporter.Import(universalServiceRateMasterWebClient,
					universalServiceRateMasterPath, "Universal Service Rate Master", DateTime.Today.AddDays(1))
				.Wait();
		}

		private static void WriteToFile(string file, string message)
		{
			using (var streamWriter = new StreamWriter(file, true))
			{
				streamWriter.WriteLine(message);
			}
		}
	}
}