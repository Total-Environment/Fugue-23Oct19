using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// Controller that handles all material related endpoints.
	/// </summary>
	[RoutePrefix("materials")]
	public class MaterialController : BaseController
	{
		private static string dummy = "abcd";
		private readonly int _defaultBatchSize;
		private readonly IMaterialService _materialService;
		private readonly ISapSyncer _sapSyncer;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialController"/> class.
		/// </summary>
		/// <param name="materialService">The material service.</param>
		public MaterialController(IMaterialService materialService, ISapSyncer sapSyncer)
		{
			_materialService = materialService;
			_sapSyncer = sapSyncer;
			_defaultBatchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
		}

		/// <summary>
		/// Creates new material in the system. Exposed as HTTP POST.
		/// </summary>
		/// <param name="materialDto"></param>
		/// <returns>Created Material with endpoint</returns>
		[ComponentLibraryAuthorize(Permissions = "Create Material properties")]
		[Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> Post(MaterialDataDto materialDto)
		{
			var materialToCreate = MaterialDtoAdapter.ToMaterial(materialDto);
			IMaterial material;
			try
			{
				material = await _materialService.Create(materialToCreate);
			}
			catch (ArgumentException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			catch (FormatException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return Conflict();
			}

			var createdMaterial = MaterialDtoAdapter.FromMaterial(material);
			if (ConfigurationManager.AppSettings["syncToSAP"] == "true")
			{
				_sapSyncer.Sync(material, "create");
			}
			Trace.TraceInformation($"Material got created with Id : {createdMaterial.Id}");
			return Created($"/materials/{createdMaterial.Id}", createdMaterial);
		}

		/// <summary>
		/// Searches within group by keyword and filters, and returns the results in paginated and
		/// sorted way.
		/// </summary>
		/// <param name="materialSearchRequest">The search within group request.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Material Search Within Group")]
		[Route("searchwithingroup")]
		[HttpPost]
		public async Task<IHttpActionResult> SearchWithinGroup([FromBody]MaterialSearchRequest materialSearchRequest)
		{
			if (string.IsNullOrWhiteSpace(materialSearchRequest.GroupName))
			{
				LogToElmah(new ArgumentException("Material group is not specified."));
				return BadRequest("Material group is not specified.");
			}

			List<IMaterial> materials;
			long count;
			try
			{
				var searchKeywords = !materialSearchRequest.IgnoreSearchQuery
					? FetchKeywords(materialSearchRequest.SearchQuery)
					: new List<string>();
				materials = await _materialService.SearchWithinGroup(materialSearchRequest.FilterDatas,
					materialSearchRequest.GroupName, searchKeywords, materialSearchRequest.PageNumber,
					materialSearchRequest.BatchSize, materialSearchRequest.SortColumn,
					materialSearchRequest.SortOrder);
				count = await _materialService.CountWithinGroup(materialSearchRequest.FilterDatas,
					materialSearchRequest.GroupName, searchKeywords);
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (FormatException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}

			return Ok(new ListDto<MaterialDataTypeDto>
			{
				BatchSize = materialSearchRequest.BatchSize,
				SortColumn = materialSearchRequest.SortColumn,
				SortOrder = materialSearchRequest.SortOrder,
				PageNumber = materialSearchRequest.PageNumber,
				RecordCount = count,
				Items = materials.Select(MaterialDataTypeDtoAdaptor.FromMaterial).ToList()
			});
		}

		/// <summary>
		/// Gets all rates found in materials based on the search request
		/// </summary>
		/// <param name="filters">Filters.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Material Bulk Rates View")]
		[HttpPost]
		[Route("rates")]
		public async Task<IHttpActionResult> GetAllRates([FromBody] List<FilterData> filters)
		{
			try
			{
				var appliedOnFilter = filters.FirstOrDefault(filter => filter.ColumnKey == "AppliedOn");
				if (appliedOnFilter == null)
				{
					return BadRequest("AppliedOn cannot be empty");
				}
				var rates = await _materialService.GetAllRates(filters);

				return Ok((await Task.WhenAll(rates.Select(rate => new MaterialRateSearchResultDto().WithDomain(rate)))).ToList());
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (NullReferenceException e)
			{
				LogToElmah(e);
				return BadRequest();
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Gets Material with pass materialId.
		/// </summary>
		/// <param name="materialId"></param>
		/// <param name="dataType"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{materialId}")]
		[ComponentLibraryAuthorize(Permissions = "View Material properties")]
		public async Task<IHttpActionResult> Get(string materialId, bool dataType = false)
		{
			IMaterial material;
			try
			{
				material = await _materialService.Find(materialId);
			}
			catch (ResourceNotFoundException exception)
			{
				LogToElmah(exception);
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}

			if (dataType)
			{
				return Ok(MaterialDataTypeDtoAdaptor.FromMaterial(material));
			}
			return Ok(MaterialDtoAdapter.FromMaterial(material));
		}

		/// <summary>
		/// Fetch brand documents, for material group and brand column name.
		/// </summary>
		/// <param name="materialGroup"></param>
		/// <param name="pageNumber"></param>
		/// <param name="batchSize"></param>
		/// <param name="brandColumnName"></param>
		/// <param name="keywords"></param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Get Documents for Brands")]
		[Route("brands/documents")]
		[HttpGet]
		public async Task<IHttpActionResult> GetBrandsInGroupHavingAttachmentColumn(string materialGroup,
			int pageNumber,
			int batchSize,
			string brandColumnName,
			string keywords = null)
		{
			List<string> keywordList = null;
			if (keywords != null)
			{
				try
				{
					keywordList = FetchKeywords(keywords);
				}
				catch (ArgumentException e)
				{
					LogToElmah(e);
					return BadRequest(e.Message);
				}
			}
			try
			{
				var totalRecords = await _materialService.GetCountOfBrandsHavingAttachmentColumnDataInGroup(
					materialGroup,
					brandColumnName, keywordList);
				var brands = await _materialService.GetBrandAttachmentsByGroupAndColumnNameKeywods(materialGroup,
					brandColumnName, keywordList, pageNumber, batchSize);

				return Ok(new ListDto<BrandDocumentDto>
				{
					Items = brands.Select(BrandDocumentDtoAdaptor.FromBrand).ToList(),
					PageNumber = pageNumber,
					BatchSize = batchSize,
					RecordCount = totalRecords,
					TotalPages = totalRecords / batchSize
				});
			}
			catch (ArgumentException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
		}

		/// <summary>
		/// Get the material in group which have attachments in the specified column Name.
		/// </summary>
		/// <param name="materialGroup">The material group.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="keywords">The keywords.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Get Documents for Materials")]
		[Route("documents")]
		[HttpGet]
		public async Task<IHttpActionResult> GetMaterialsInGroupHavingAttachmentColumn(string materialGroup, string columnName,
			int pageNumber, int batchSize, string keywords = null)
		{
			List<string> keywordList = null;
			if (keywords != null)
			{
				try
				{
					keywordList = FetchKeywords(keywords);
				}
				catch (ArgumentException e)
				{
					LogToElmah(e);
					return BadRequest(e.Message);
				}
			}

			var totalRecords = await _materialService.GetCountOfMaterialsHavingAttachmentColumnDataInGroup(
				materialGroup, columnName, keywordList);
			IList<IMaterial> materials;
			try
			{
				materials = await _materialService.GetMaterialHavingAttachmentColumnDataInGroup(materialGroup, columnName, pageNumber,
					batchSize, keywordList);
			}
			catch (ArgumentException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			return Ok(new ListDto<MaterialDocumentDto>
			{
				Items = materials.Select(m => MaterialDocumentDtoAdaptor.FromMaterial(m, columnName)).ToList(),
				PageNumber = pageNumber,
				BatchSize = batchSize,
				RecordCount = totalRecords,
				TotalPages = totalRecords / batchSize
			});
		}

		/// <summary>
		/// Fetches the keywords.
		/// </summary>
		/// <param name="searchQuery">The search query.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">
		/// Search keyword should be minimum 3 letter long. or Atleast one of the search keyword
		/// should be more than 3 letter long.
		/// </exception>
		private List<string> FetchKeywords(string searchQuery)
		{
			if ((searchQuery == null) || (searchQuery.Length < 3))
				throw new ArgumentException("Search keyword should be atleast 3 letters long.");
			var keywords = searchQuery.Split(' ').Where(k => k.Length > 2).ToList();
			if (!keywords.Any())
				throw new ArgumentException("Atleast one of the search keyword should be more than 3 letters long.");
			return keywords;
		}

		/// <summary>
		/// Returns all material controlled by the pageNumber and batchSize.
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Recent Materials")]
		[Route("all")]
		[HttpGet]
		public async Task<IHttpActionResult> GetRecentMaterials(int pageNumber = 1, int batchSize = -1)
		{
			List<IMaterial> recentMaterials;
			try
			{
				recentMaterials = await _materialService.GetRecentMaterials(pageNumber, batchSize == -1 ? _defaultBatchSize : batchSize);
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			//var totalCount = await _materialService.GetMaterialCount();
			var totalCount = 10; // Not used actually. This is what was slowing down Material Homepage
			return Ok(new ListDto<MaterialDataTypeDto>
			{
				Items = recentMaterials?.Select(MaterialDataTypeDtoAdaptor.FromMaterial).ToList(),
				RecordCount = totalCount,
				PageNumber = pageNumber,
				BatchSize = batchSize,
				TotalPages = totalCount / batchSize
			});
		}

		/// <summary>
		/// Updates material with passed material Id.
		/// </summary>
		/// <param name="materialCode"></param>
		/// <param name="materialRequest"></param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Edit Material properties")]
		[Route("{materialCode}")]
		[HttpPut]
		public async Task<IHttpActionResult> Put(string materialCode, MaterialDataDto materialRequest)
		{
			if (materialCode == null)
				return BadRequest("Material code cannot be null.");
			var materialData = MaterialDtoAdapter.ToMaterial(materialRequest);
			UpdateMaterialCode(materialCode, materialData);
			IMaterial updatedMaterial;
			try
			{
				updatedMaterial = await _materialService.Update(materialData);
			}
			catch (ArgumentException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
			catch (FormatException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
			catch (DuplicateResourceException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return BadRequest($"Not Found material with material code: {materialCode}.");
			}

			if (ConfigurationManager.AppSettings["syncToSAP"] == "true")
				_sapSyncer.Sync(updatedMaterial, "update");
			Trace.TraceInformation($"Material got updated for Id : {materialCode}");
			return Ok(MaterialDataTypeDtoAdaptor.FromMaterial(updatedMaterial));
		}

		/// <summary>
		/// Synchronizes to sap.
		/// </summary>
		/// <param name="materialCode">The material code.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Material SAP Sync")]
		[Route("{materialCode}/sync")]
		[HttpPost]
		public async Task<IHttpActionResult> SyncToSap(string materialCode)
		{
			IMaterial material;
			var isSynced = false;
			try
			{
				material = await _materialService.Find(materialCode);
			}
			catch (ResourceNotFoundException exception)
			{
				LogToElmah(exception);
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}

			isSynced = await _sapSyncer.Sync(material, "update");

			if (isSynced)
			{
				return Ok($"Material : {materialCode} is synced to SAP");
			}
			return StatusCode(HttpStatusCode.BadGateway);
		}

		private static void UpdateMaterialCode(string materialCode, IMaterial materialData)
		{
			((dynamic)materialData).general.material_code.Value = materialCode;
			materialData.Id = materialCode;
		}

		/// <summary>
		/// Search for keywords in material and return material group which have material having
		/// these keywords.
		/// </summary>
		/// <param name="keywords"></param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Search For Group in Material")]
		[Route("group/{keywords}")]
		[HttpGet]
		public async Task<IHttpActionResult> SearchForGroup(string keywords)
		{
			List<string> groups;
			try
			{
				var keywordList = FetchKeywords(keywords);
				groups = await _materialService.SearchForGroups(keywordList);
			}
			catch (ArgumentException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			catch (ResourceNotFoundException exception)
			{
				LogToElmah(exception);
				return NotFound();
			}
			return Ok(groups);
		}
	}
}