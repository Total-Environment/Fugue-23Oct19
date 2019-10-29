using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// Material data controller for creating and updating material data.
	/// </summary>
	/// <seealso cref="System.Web.Http.ApiController"/>
	[RoutePrefix("materials-old")]
	public class OldMaterialController : ComponentController
	{
		private const string GenericBrandDefinition = "Generic Brand";
		private readonly IComponentDefinitionRepository<AssetDefinition> _assetDefinitionRepository;
		private readonly int _batchSize;
		private readonly IBrandDefinitionRepository _brandDefinitionRepository;
		private readonly IComponentDefinitionRepository<IMaterialDefinition> _definitionRepository;
		private readonly IFilterCriteriaBuilder _filterCriteriaBuilder;
		private readonly IMaterialRepository _materialRepository;
		private readonly ISapSyncer _sapSyncer;

		/// <summary>
		/// Initializes a new instance of the <see cref="OldMaterialController"/> class.
		/// </summary>
		/// <param name="definitionRepository">The definition repository.</param>
		/// <param name="counterRepository">The counter repository.</param>
		/// <param name="materialRepository">The material repository.</param>
		/// <param name="masterDataRepository">The master data repository.</param>
		/// <param name="sapSyncer">The sap syncer.</param>
		/// <param name="assetDefinitionRepository"></param>
		/// <param name="brandDefinitionRepository">Repository of brand definitions.</param>
		/// <param name="filterCriteriaBuilder">Filter Criteria Builder</param>
		public OldMaterialController(IComponentDefinitionRepository<IMaterialDefinition> definitionRepository,
			ICounterRepository counterRepository, IMaterialRepository materialRepository,
			IMasterDataRepository masterDataRepository,
			ISapSyncer sapSyncer,
			IComponentDefinitionRepository<AssetDefinition> assetDefinitionRepository,
			IBrandDefinitionRepository brandDefinitionRepository, IFilterCriteriaBuilder filterCriteriaBuilder)
			: base(counterRepository, masterDataRepository)
		{
			_definitionRepository = definitionRepository;
			_materialRepository = materialRepository;
			_sapSyncer = sapSyncer;
			_assetDefinitionRepository = assetDefinitionRepository;
			_brandDefinitionRepository = brandDefinitionRepository;
			_filterCriteriaBuilder = filterCriteriaBuilder;
			_batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
		}

		private async Task<IBrandDefinition> GetBrandDefinition()
		{
			return await _brandDefinitionRepository.FindBy(GenericBrandDefinition);
		}

		/// <summary>
		/// Gets the group column.
		/// </summary>
		/// <value>The group column.</value>
		protected override string GroupColumn => "Material Level 2";

		/// <summary>
		/// Gets the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="dataType">Require Data types or not</param>
		/// <returns>Task&lt;IHttpActionResult&gt;.</returns>
		[ComponentLibraryAuthorize(Permissions = "Old - View Material properties")]
		[Route("{id}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string id, [FromUri] bool? dataType = false)
		{
			try
			{
				var material = await _materialRepository.Find(id);
				var materialWithNewHeadersAndColumns =
					await AddNewHeadersAndColumnsIfAny(material, material.ComponentDefinition);
				if (dataType == true)
				{
					var responseWithDataTypes = new MaterialWithDataTypeDto(materialWithNewHeadersAndColumns);
					return Ok(responseWithDataTypes);
				}
				var response = new MaterialDto(materialWithNewHeadersAndColumns);
				return Ok(response);
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
		}

		/// <summary>
		/// Gets the materials by group and column name.
		/// </summary>
		/// <param name="group">The group.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="pageNumber">Page number.</param>
		/// <param name="batchSize">Batch size.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize()]
		[Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> GetByGroupAndColumnName(string group, string columnName, int pageNumber, int batchSize)
		{
			columnName = columnName.ToLowerInvariant();
			IMaterialDefinition materialDefinition;
			try
			{
				materialDefinition = await _definitionRepository.Find(group);
			}
			catch (ResourceNotFoundException resourceNotFoundException)
			{
				LogToElmah(resourceNotFoundException);
				return BadRequest(resourceNotFoundException.Message);
			}

			IHeaderDefinition headerDefinition = null;
			IColumnDefinition columnDefinition = null;
			foreach (var materialDefinitionHeader in materialDefinition.Headers)
				foreach (var materialDefinitionHeaderColumn in materialDefinitionHeader.Columns)
					if (string.Equals(materialDefinitionHeaderColumn.Name, columnName,
						StringComparison.InvariantCultureIgnoreCase))
					{
						headerDefinition = materialDefinitionHeader;
						columnDefinition = materialDefinitionHeaderColumn;
						break;
					}
			if (columnDefinition == null)
				return BadRequest(columnName + " is not valid column in the material definition of " + group + " group.");
			if (!(columnDefinition.DataType is StaticFileDataType) && !(columnDefinition.DataType is CheckListDataType) &&
				(!(columnDefinition.DataType is ArrayDataType) ||
				 !(((ArrayDataType)columnDefinition.DataType).DataType is StaticFileDataType) &&
				 !(((ArrayDataType)columnDefinition.DataType).DataType is CheckListDataType)))
				return BadRequest(columnName + " is neithter static file data type nor check list data type.");

			var totalCount = await _materialRepository.GetTotalCountByGroupAndColumnName(group, columnName);
			var materials = await _materialRepository.GetByGroupAndColumnName(group, columnName, pageNumber, batchSize);
			var materialDocumentDtos =
				materials.Select(m => MaterialDocumentDtoAdaptor.FromMaterial(m, columnName)).ToList();
			return Ok(FormatSearchResponse(totalCount, pageNumber, batchSize, materialDocumentDtos));
		}

		/// <summary>
		/// Gets the materials by group and column name and key word.
		/// </summary>
		/// <param name="group">The group.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="keyWord">The key word.</param>
		/// <param name="pageNumber">Page number.</param>
		/// <param name="batchSize">Batch size.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Old - Get materials by parameters")]
		[Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> GetByGroupAndColumnNameAndKeyWord(string group, string columnName,
			string keyWord, int pageNumber, int batchSize)
		{
			columnName = columnName.ToLowerInvariant();
			List<string> keywords;
			try
			{
				keywords = FetchKeywords(keyWord);
			}
			catch (ArgumentException argumentException)
			{
				LogToElmah(argumentException);
				return BadRequest(argumentException.Message);
			}

			IMaterialDefinition materialDefinition;
			try
			{
				materialDefinition = await _definitionRepository.Find(group);
			}
			catch (ResourceNotFoundException resourceNotFoundException)
			{
				LogToElmah(resourceNotFoundException);
				return BadRequest(resourceNotFoundException.Message);
			}

			IHeaderDefinition headerDefinition = null;
			IColumnDefinition columnDefinition = null;
			foreach (var materialDefinitionHeader in materialDefinition.Headers)
				foreach (var materialDefinitionHeaderColumn in materialDefinitionHeader.Columns)
					if (string.Equals(materialDefinitionHeaderColumn.Name, columnName,
						StringComparison.InvariantCultureIgnoreCase))
					{
						headerDefinition = materialDefinitionHeader;
						columnDefinition = materialDefinitionHeaderColumn;
						break;
					}
			if (columnDefinition == null)
				return BadRequest(columnName + " is not valid column in the material definition of " + group + " group.");
			if (!(columnDefinition.DataType is StaticFileDataType) && !(columnDefinition.DataType is CheckListDataType) &&
				(!(columnDefinition.DataType is ArrayDataType) ||
				 !(((ArrayDataType)columnDefinition.DataType).DataType is StaticFileDataType) &&
				 !(((ArrayDataType)columnDefinition.DataType).DataType is CheckListDataType)))
				return BadRequest(columnName + " is neithter static file data type nor check list data type.");

			var totalCount = await _materialRepository.GetTotalCountByGroupAndColumnNameAndKeyWords(group, columnName,
				keywords);
			var materials = await _materialRepository.GetByGroupAndColumnNameAndKeyWords(group, columnName, keywords,
				pageNumber, batchSize);
			var materialDocumentDtos =
				materials.Select(m => MaterialDocumentDtoAdaptor.FromMaterial(m, columnName)).ToList();
			return Ok(FormatSearchResponse(totalCount, pageNumber, batchSize, materialDocumentDtos));
		}

		/// <summary>
		/// List materials by latest created date
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="details"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Old - Get materials by parameters")]
		[Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> ListMaterials(int batchSize = -1, int pageNumber = 1)
		{
			List<IMaterial> materials;
			var keywords = new List<string>();

			batchSize = batchSize == -1 ? _batchSize : batchSize;
			try
			{
				materials = await _materialRepository.ListComponents(pageNumber, batchSize);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}
			var count = await _materialRepository.Count(keywords, string.Empty);
			var materialDtos = await Task.WhenAll(materials.Select(async m =>
				new MaterialWithDataTypeDto(await AddNewHeadersAndColumnsIfAny(m, m.ComponentDefinition))));
			return Ok(FormatSearchResponse(count, pageNumber, batchSize, materialDtos));
		}

		/// <summary>
		/// Creates the material using specified data.
		/// </summary>
		/// <remarks>
		/// Data is a set of key value pair. e.g. <![CDATA[ { "Classification": { "Material Level 1":
		/// "Secondary", "Material Level 2": "Electrical", "Material Level 3": "Cable & Wire",
		/// "Material Level 4": "Cable", "Material Level 5": "Low Tension", "Material Level 6": null
		/// }, "General": { "Short Description": "PVC Insulated Cable", "Shade Description": "Red",
		/// "Unit Of Measure": "m", "SAP Code": "1002529", "Part of eBuild Library": false,
		/// "Generic": true, "Manufactured": true, "Material Status": "Active" }, "Purchase": {},
		/// "Planning": { "PO Lead Time in Days": "3", "Delivery Lead Time in Days": "5", "Maintain
		/// Lot": false }, "Quality": { "Governing Standard": [ "IS 1554 (Part 1)" ] }, "System
		/// Logs": {}, "Specifications": { "Grade": "Heavy Duty", "Length": null, "Width": null,
		/// "Diameter": null, "Sweep": null, "Power Consumption": null, "Fan Speed": null, "Air
		/// Delivery": null, "Number of Blades": null, "Flame Retardant": null, "Rated Current":
		/// null, "Phase": null, "Number of Modules": null, "Rated Input Voltage": { "Type": "V",
		/// "Value": "1100" }, "Number of Cores": "2", "Cross Sectional Area": { "Type": "mm²",
		/// "Value": "1.5" }, "Core Material": "Copper", "Number of Pins": null, "Insulation Type":
		/// "PVC Insulated", "Sheath Material": null, "Nominal Dimensions of Armour - Wire": null }]]>
		/// </remarks>
		/// <param name="material">The material data in key value format.</param>
		/// <param name="request">The Material Data Transfer Object</param>
		/// <returns>Http Result object</returns>
		/// <response code="200">
		/// Response Body: <![CDATA[ "Classification": { "Material Level 1": "Secondary", "Material
		/// Level 2": "Electrical", "Material Level 3": "Cable & Wire", "Material Level 4": "Cable",
		/// "Material Level 5": "Low Tension", "Material Level 6": null }, "General": { "Short
		/// Description": "PVC Insulated Cable", "Shade Description": "Red", "Unit Of Measure": "m",
		/// "SAP Code": "1002529", "Part of eBuild Library": false, "Generic": true, "Manufactured":
		/// true, "Material Status": "Active" }, "Purchase": {}, "Planning": { "PO Lead Time in
		/// Days": "3", "Delivery Lead Time in Days": "5", "Maintain Lot": false }, "Quality": {
		/// "Governing Standard": [ "IS 1554 (Part 1)" ] }, "System Logs": {}, "Specifications": {
		/// "Grade": "Heavy Duty", "Length": null, "Width": null, "Diameter": null, "Sweep": null,
		/// "Power Consumption": null, "Fan Speed": null, "Air Delivery": null, "Number of Blades":
		/// null, "Flame Retardant": null, "Rated Current": null, "Phase": null, "Number of Modules":
		/// null, "Rated Input Voltage": { "Type": "V", "Value": "1100" }, "Number of Cores": "2",
		/// "Cross Sectional Area": { "Type": "mm²", "Value": "1.5" }, "Core Material": "Copper",
		/// "Number of Pins": null, "Insulation Type": "PVC Insulated", "Sheath Material": null,
		/// "Nominal Dimensions of Armour - Wire": null }, "id": "ELC000026"]]>
		/// </response>
		[ComponentLibraryAuthorize(Permissions = "Old - Create Material properties")]
		[Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> Post(Dictionary<string, object> request)
		{
			try
			{
				var brandDefinition = await _brandDefinitionRepository.FindBy(GenericBrandDefinition);
				var normalizedRequest = NormalizeRequest(request);
				var materialGroup = GetGroup(normalizedRequest);
				var materialDefinition = await MaterialDefinition(materialGroup, normalizedRequest);
				var materialData = await materialDefinition.Parse<Material>(normalizedRequest, brandDefinition);
				await UpdateMaterialCode(normalizedRequest, materialDefinition, materialData);
				materialData.Group = materialGroup;
				await _materialRepository.Add(materialData);

				if (ConfigurationManager.AppSettings["syncToSAP"] == "true")
					_sapSyncer.Sync(materialData, "create");

				var requestUri = RequestUri() + materialData.Id;
				return Created(requestUri, new MaterialDto(await _materialRepository.Find(materialData.Id)));
			}
			catch (BetterKeyNotFoundException e)
			{
				LogToElmah(e);
				Trace.TraceWarning($"{e.Key} was not found");
				return BadRequest($"{e.Key} was not found.");
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				Trace.TraceWarning(ex.Message);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Searches within group by keywords and return results after applying filters, pagination
		/// and sorting.
		/// </summary>
		/// <param name="materialSearchRequest">The search within group request.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Old - Get materials by parameters")]
		[Route("searchwithingroup")]
		[HttpPost]
		public async Task<IHttpActionResult> Post_SearchWithinGroup([FromBody]MaterialSearchRequest materialSearchRequest)
		{
			var batchSize = materialSearchRequest.BatchSize;
			List<IMaterial> materials;
			long count;
			try
			{
				var keywords = FetchKeywords(materialSearchRequest.SearchQuery);
				var materialDefinition = await _definitionRepository.Find(materialSearchRequest.GroupName);
				var brandDefinition = await _brandDefinitionRepository.FindBy(GenericBrandDefinition);
				var filterCriteria = _filterCriteriaBuilder.Build(materialDefinition, brandDefinition, materialSearchRequest.FilterDatas, materialSearchRequest.GroupName, keywords);
				materials =
					await _materialRepository.Search(filterCriteria, materialSearchRequest.PageNumber, batchSize, materialSearchRequest.SortColumn, materialSearchRequest.SortOrder);
				count = await _materialRepository.Count(filterCriteria);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}

			var materialDtos = await Task.WhenAll(materials.Select(async m =>
				new MaterialWithDataTypeDto(await AddNewHeadersAndColumnsIfAny(m, m.ComponentDefinition))));
			return
				Ok(FormatSearchResponse(count, materialSearchRequest.PageNumber, batchSize, materialDtos,
					materialSearchRequest.SortColumn, materialSearchRequest.SortOrder));
		}

		/// <summary>
		/// Puts the specified material identifier.
		/// </summary>
		/// <param name="materialId">The material identifier.</param>
		/// <param name="materialDto">The material.</param>
		/// <returns>Task&lt;IHttpActionResult&gt;.</returns>
		[ComponentLibraryAuthorize(Permissions = "Old - Edit Material properties")]
		[Route("{materialId}")]
		[HttpPut]
		public async Task<IHttpActionResult> Put(string materialId, Dictionary<string, object> materialDto)
		{
			try
			{
				var brandDefinition = await _brandDefinitionRepository.FindBy("Generic Brand");
				var dictionaryMaterial = (IDictionary<string, object>)ConvertJsonToPrimitives(materialDto);
				var group = GetGroup(dictionaryMaterial);
				var definition = await MaterialDefinition(group, dictionaryMaterial);
				var material = await definition.Parse<Material>(dictionaryMaterial, brandDefinition);
				material.Id = materialId;
				material.Group = group;
				await _materialRepository.Update(material);

				if (ConfigurationManager.AppSettings["syncToSAP"] == "true")
					_sapSyncer.Sync(material, "update");

				return Ok(new MaterialDto(await _materialRepository.Find(material.Id)));
			}
			catch (BetterKeyNotFoundException e)
			{
				LogToElmah(e);
				return BadRequest($"{e.Key} was not found.");
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
			catch (FormatException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
			catch (Exception e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
		}

		/// <summary>
		/// Searches by the specified material level2 and the search key word.
		/// </summary>
		/// <param name="materialLevel2">The material level2.</param>
		/// <param name="searchKeyword">The search keyword.</param>
		/// <param name="pageNumber">The page of the recordset.</param>
		/// <param name="sortColumn">Column on which sort is applied</param>
		/// <param name="sortOrder">Ascending or Descending</param>
		/// <returns>Task&lt;IHttpActionResult&gt;.</returns>
		[ComponentLibraryAuthorize(Permissions = "Old - Get materials by parameters")]
		[Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> Search(string searchKeyword, int pageNumber, string materialLevel2 = "",
			string sortColumn = "", SortOrder sortOrder = SortOrder.Ascending)
		{
			if (searchKeyword == null || materialLevel2 == null)
				return BadRequest("Invalid Request");
			var searchKeywords = searchKeyword.Split(' ').ToList();
			if (searchKeywords.Any(k => k.Length < 3))
				return BadRequest("The search keyword should be greater than 3 characters");

			try
			{
				var materialLevel2Exists = await MasterDataRepository.Exists("material_level_2", materialLevel2);
				if (!materialLevel2Exists)
					LogToElmah(new ArgumentException($"Material Level2 does not exist - '{materialLevel2}'"));

				List<IMaterial> materials;
				if (string.IsNullOrEmpty(materialLevel2))
					materials = await _materialRepository.Search(searchKeywords, pageNumber, _batchSize);
				else
					materials =
						await
							_materialRepository.Search(searchKeywords, materialLevel2, pageNumber, _batchSize,
								sortColumn, sortOrder);

				var materialsWithNewHeadersAndColumns = new List<IMaterial>();
				foreach (var material in materials)
				{
					var materialWithNewHeadersAndColumns =
						await AddNewHeadersAndColumnsIfAny(material, material.ComponentDefinition);
					materialsWithNewHeadersAndColumns.Add(materialWithNewHeadersAndColumns);
				}
				var count = await GetRecordCount(materialLevel2, searchKeywords);
				var materialSearchDtos =
					materialsWithNewHeadersAndColumns.Select(x => new MaterialSearchDto(x)).ToList();

				var result = FormatSearchResponse(count, pageNumber, _batchSize, materialSearchDtos, sortColumn, sortOrder);

				return Ok(result);
			}
			catch (Exception exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
		}

		[ComponentLibraryAuthorize(Permissions = "Old - Get materials by parameters")]
		[Route("group-search")]
		[HttpGet]
		public async Task<IHttpActionResult> SearchInGroup(string searchQuery)
		{
			List<string> response;
			try
			{
				var keywords = FetchKeywords(searchQuery);
				response = await _materialRepository.SearchInGroup(keywords);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}
			return Ok(response);
		}

		/// <summary>
		/// Updates the rate.
		/// </summary>
		/// <param name="updateRateMaterialRateRequest">The update rate material rate request.</param>
		/// <returns>Returns the updated rate.</returns>
		[ComponentLibraryAuthorize(Permissions = "Update Last Purchase rates for materials")]
		[Route("~/updatematerialrate")]
		[HttpPut]
		public async Task<IHttpActionResult> UpdateRate(UpdateMaterialRateRequest updateRateMaterialRateRequest)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid update material rate request.");

			try
			{
				var materialCode = updateRateMaterialRateRequest.MaterialCode;

				var moneyDataType = new MoneyDataType();
				var lastPurchaseRate = await moneyDataType.Parse(updateRateMaterialRateRequest.LastPurchaseRate);
				var weightedAveragePurchaseRate =
					await moneyDataType.Parse(updateRateMaterialRateRequest.WeightedAveragePurchaseRate);

				Trace.TraceInformation("Rate of material is being updated with materialCode: " + materialCode +
									   " , lastPurchaseRate: " + lastPurchaseRate + " , weightedAveragePurchaseRate: " +
									   weightedAveragePurchaseRate + ".");

				var material = await _materialRepository.Find(materialCode);
				if (material == null)
				{
					Trace.TraceError("Invalid material code: " + materialCode + ".");
					return BadRequest("Invalid material code: " + materialCode + ".");
				}
				var purchase = material["Purchase"];
				if (purchase == null)
				{
					Trace.TraceError("material of " + material.Id + " is not having Purchase header.");
					return BadRequest("Material of " + material.Id + " is not having Purchase header.");
				}

				var lpc = purchase.Columns.FirstOrDefault(c => c.Name == "Last Purchase Rate");
				var wapr = purchase.Columns.FirstOrDefault(c => c.Name == "Wt. Avg. Purchase Rate");

				if (lpc == null || wapr == null)
				{
					Trace.TraceError("material of " + material.Id +
									 " is not having Last Purchase Rate or Weighted Average Purchase Rate columns in Purchase header.");
					return BadRequest("material of " + material.Id +
									  " is not having Last Purchase Rate or Weighted Average Purchase Rate columns in Purchase header.");
				}

				var materialHeaders = material.Headers as IList<IHeaderData> ??
									  material.Headers.ToList();
				foreach (var header in materialHeaders)
				{
					if (header.Name != "Purchase")
						continue;
					var headerColumns = header.Columns as IList<IColumnData> ?? header.Columns.ToList();
					foreach (var column in headerColumns)
						switch (column.Name)
						{
							case "Last Purchase Rate":
								column.Value = lastPurchaseRate; // lastPurchaseRate.ToString();
								break;

							case "Wt. Avg. Purchase Rate":
								column.Value = weightedAveragePurchaseRate; //weightedAveragePurchaseRate.ToString();
								break;
						}
					header.Columns = headerColumns;
					break;
				}
				material.Headers = materialHeaders;

				await _materialRepository.Update(material);
				Trace.TraceInformation("Updated rate of material with materialCode: " + materialCode +
									   " , lastPurchaseRate: " +
									   lastPurchaseRate + " , weightedAveragePurchaseRate: " +
									   weightedAveragePurchaseRate +
									   ".");
				return Ok("Updated rate of material with materialCode: " + materialCode);
			}
			catch (BetterKeyNotFoundException e)
			{
				Trace.TraceError($"{e.Key} was not found");
				return BadRequest($"{e.Key} was not found.");
			}
			catch (ResourceNotFoundException e)
			{
				Trace.TraceError(e.Message);
				return BadRequest(e.Message);
			}
			catch (Exception e)
			{
				Trace.TraceError(e.Message);
				return BadRequest(e.Message);
			}
		}

		private async Task<int> GetRecordCount(string materialLevel2, List<string> searchKeywords)
		{
			var memoryCache = MemoryCache.Default;
			var keywords = string.Join("_", searchKeywords.ToArray());
			var key = $"{materialLevel2}_{keywords}";
			if (memoryCache.Contains(key))
				return (int)memoryCache.Get(key);
			var count = await _materialRepository.Count(searchKeywords, materialLevel2);
			memoryCache.Add(key, count, DateTimeOffset.UtcNow.AddHours(1));
			return count;
		}

		private async Task<IMaterialDefinition> MaterialDefinition(string materialGroup,
																			IDictionary<string, object> input)
		{
			var materialDefinition = await _definitionRepository.Find(materialGroup);
			var generalHeader = (Dictionary<string, object>)input["General"];
			const string isAsset = "Can be Used as an Asset";
			var canBeUsedAsAsset = generalHeader.ContainsKey(isAsset) ? generalHeader[isAsset] : null;
			if (canBeUsedAsAsset != null && (bool)await new BooleanDataType().Parse(canBeUsedAsAsset))
			{
				var assetDefinition = await _assetDefinitionRepository.Find(materialGroup);
				materialDefinition = assetDefinition.Merge(materialDefinition);
				;
			}
			return materialDefinition;
		}

		private string RequestUri()
		{
			var requestUri = Request != null
				? Request.RequestUri != null ? Request.RequestUri.ToString() : string.Empty
				: string.Empty;
			return requestUri;
		}

		private async Task SyncMaterialCounterWithMaterialId(string materialCode, IMaterialDefinition definition)
		{
			var groupCode = materialCode.Substring(0, 3);
			if (groupCode != definition.Code)
				throw new InvalidDataException($"Group code of material Code: {materialCode} is invalid.");

			var currentCounter = await CounterRepository.CurrentValue("Material");
			var isNumeric = materialCode.Remove(0, 3).All(char.IsDigit);
			if (materialCode.Remove(0, 3).Length < 6 || !isNumeric)
				throw new InvalidDataException($"material Code: {materialCode} is invalid.");
			var code = int.Parse(materialCode.Remove(0, 3).TrimStart('0'));
			if (code > currentCounter)
				await CounterRepository.Update(code, "Material");
		}

		private async Task UpdateMaterialCode(IDictionary<string, object> normalizedRequest,
			IMaterialDefinition materialDefinition,
			Material materialData)
		{
			var generalHeader = normalizedRequest["General"];
			if (generalHeader.GetType() == typeof(Dictionary<string, object>))
			{
				var materialCode = ((Dictionary<string, object>)generalHeader)["Material Code"];
				if (materialCode == null)
				{
					var counter = await CounterRepository.NextValue("Material");
					materialCode = $"{materialDefinition.Code}{counter:D6}";
				}
				else
				{
					await SyncMaterialCounterWithMaterialId(materialCode.ToString(), materialDefinition);
				}

				materialData.Id = materialCode.ToString();
			}
		}
	}
}