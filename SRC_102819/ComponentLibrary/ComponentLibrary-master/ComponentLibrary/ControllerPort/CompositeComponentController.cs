using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// Web api controller for semi finished good and package.
	/// </summary>
	/// <seealso cref="BaseController"/>
	[RoutePrefix("{type:validcompositecomponent}s")]
	public class CompositeComponentController : BaseController
	{
		private readonly int _defaultBatchSize;
		private readonly IServiceRepository _serviceRepository;
		private readonly ICompositeComponentService _compositeComponentService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentController"/> class.
		/// </summary>
		/// <param name="compositeComponentService">The composite component service.</param>
		/// <param name="serviceRepository">The service repository.</param>
		public CompositeComponentController(ICompositeComponentService compositeComponentService,
			IServiceRepository serviceRepository)
		{
			_compositeComponentService = compositeComponentService;
			_serviceRepository = serviceRepository;
			_defaultBatchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
		}

        /// <summary>
        /// Endpoint that clones a SFG from a service.
        /// </summary>
        /// <param name="fromService">From service.</param>
        /// <param name="componentCompositionDto">The component composition dto.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions= "Create SFG")]
        [Route("~/sfgs")]
		[HttpPost]
		public async Task<IHttpActionResult> CloneFromService(string fromService,
			ComponentCompositionDto componentCompositionDto)
		{
			if (componentCompositionDto == null)
			{
				LogToElmah(new NullReferenceException(nameof(componentCompositionDto)));
				return BadRequest("ComponentComposition is required.");
			}

			try
			{
				var service = await _serviceRepository.Find(fromService);
				var componentComposition = componentCompositionDto.ToDomain();
				var compositeComponent = await _compositeComponentService.CloneFromService(service, componentComposition);
				return Ok(new CompositeComponentDto(compositeComponent));
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
		}

        /// <summary>
        /// Creates the composite component from data provided.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="compositeComponentData">The composite component data.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create SFG,Create Package")]
        [Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> CreateCompositeComponent(string type, CompositeComponentDto compositeComponentData)
		{
			if (compositeComponentData.ComponentComposition == null)
			{
				LogToElmah(new NullReferenceException(nameof(compositeComponentData.ComponentComposition)));
				return BadRequest("ComponentComposition is required.");
			}

			try
			{
				var compositeComponentTobeCreated = compositeComponentData.ToDomain();
				var compositeComponent = await _compositeComponentService.Create(type, compositeComponentTobeCreated);
				return Created($"/{type}/{compositeComponent.Code}", new CompositeComponentDto(compositeComponent));
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (FormatException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
		}

        /// <summary>
        /// Updates the compoiste component from data provided.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code">The code.</param>
        /// <param name="compositeComponentData">The compoiste component data.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Edit SFG properties,Edit Package properties")]
        [Route("{code}")]
		[HttpPut]
		public async Task<IHttpActionResult> UpdateCompositeComponent(string type, string code,
			CompositeComponentDto compositeComponentData)
		{
			if (compositeComponentData.ComponentComposition == null)
			{
				LogToElmah(new NullReferenceException(nameof(compositeComponentData.ComponentComposition)));
				return BadRequest("ComponentComposition is required.");
			}

			try
			{
				var compositeComponentTobeUpdated = compositeComponentData.ToDomain();
				compositeComponentTobeUpdated.Code = code;
				if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
					((dynamic)compositeComponentTobeUpdated).general.sfg_code.Value = code;
				else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
					((dynamic)compositeComponentTobeUpdated).general.package_code.Value = code;
				else
					throw new NotSupportedException(type + " is not supported.");

				var compositeComponent = await _compositeComponentService.Update(type, compositeComponentTobeUpdated);
				return Ok(new CompositeComponentDto(compositeComponent));
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
		}

        /// <summary>
        /// Gets the cost for a given composite component.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code">The code.</param>
        /// <param name="location">The location.</param>
        /// <param name="appliedOn">The applied on.</param>
        [ComponentLibraryAuthorize(Permissions = "View SFG properties,View Package properties")]
        [Route("{code}/cost")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCost(string type, string code, string location, DateTime appliedOn)
		{
			try
			{
				appliedOn = appliedOn.InIst().Date;
				var cost = await _compositeComponentService.GetCost(type, code, location, appliedOn);
				return Ok(cost);
			}
			catch (ResourceNotFoundException e)
			{
				return Content(HttpStatusCode.NotFound, e.Message);
			}
			catch (InvalidOperationException e)
			{
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				return BadRequest(e.Message);
			}
		}

        /// <summary>
        /// Gets the composite component.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View SFG properties,View Package properties")]
        [Route("{code}")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCompositeComponent(string type, string code)
		{
			try
			{
				var compositeComponent = await _compositeComponentService.Get(type, code);
				return Ok(new CompositeComponentDto(compositeComponent));
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (Exception e)
			{
				LogToElmah(e);
				return InternalServerError();
			}
		}

        /// <summary>
        /// Searches the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Search Composite Component")]
        [Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> Search(string type, string searchKeyword = "", int pageNumber = -1, int batchSize = -1,
			string sortColumn = "", SortOrder sortOrder = SortOrder.Ascending)
		{
			if (batchSize < 0)
			{
				batchSize = _defaultBatchSize;
			}
			if (string.IsNullOrEmpty(searchKeyword))
			{
				LogToElmah(new ArgumentException("Search keyword not specified"));
				return BadRequest("Search keyword is not specified");
			}

			try
			{
				var keywords = FetchKeywords(searchKeyword);
				var count = await _compositeComponentService.Count(type, keywords, new List<FilterData>());
				var result = await _compositeComponentService.Find(type, keywords, new List<FilterData>(), sortColumn, sortOrder,
					pageNumber, batchSize);
				return Ok(new ListDto<CompositeComponentDto>
				{
					BatchSize = batchSize,
					SortColumn = sortColumn,
					SortOrder = sortOrder,
					PageNumber = pageNumber,
					RecordCount = count,
					Items = result.Select(sfg => new CompositeComponentDto(sfg)).ToList()
				});
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
		}

        /// <summary>
        /// Updates the rates.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="code">The code.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Update Last Purchase rates for composite component")]
        [Route("{code}/rates")]
		[HttpPut]
		public async Task<IHttpActionResult> UpdateRates(string type, string code, RateDto request)
		{
			try
			{
				request.IsValidState();
				var compositeComponent = await _compositeComponentService.Get(type, code);
				compositeComponent.UpdateColumn(type, "last_purchase_rate", ConvertToDictionary(request.LastPurchaseRate));
				compositeComponent.UpdateColumn(type, "weighted_average_purchase_rate",
					ConvertToDictionary(request.WeightedAveragePurchaseRate));
				await _compositeComponentService.UpdateRates(type, compositeComponent);
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError(ex.Message);
				return BadRequest(ex.Message);
			}
			catch (ResourceNotFoundException)
			{
			    var errMessage = $"{type} not found with code {code}";
                Trace.TraceError(errMessage);
			    return BadRequest(errMessage);
			}
			return Ok($"Updated rates of {type} with code: {code}.");
		}

        /// <summary>
        /// Searches by keyword and filters, and returns the results in paginated and sorted way.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="compositeComponentSearchRequest">The search request.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Search Composite Component")]
        [Route("search")]
		[HttpPost]
		public async Task<IHttpActionResult> Search(string type, [FromBody]CompositeComponentSearchRequest compositeComponentSearchRequest)
		{
			List<CompositeComponent> compositeComponents;
			long count;
			try
			{
				var searchKeywords = !compositeComponentSearchRequest.IgnoreSearchQuery
					? FetchKeywords(compositeComponentSearchRequest.SearchQuery)
					: new List<string>();
				compositeComponents = await _compositeComponentService.Find(type, searchKeywords,
					compositeComponentSearchRequest.FilterDatas, compositeComponentSearchRequest.SortColumn,
					compositeComponentSearchRequest.SortOrder, compositeComponentSearchRequest.PageNumber,
					compositeComponentSearchRequest.BatchSize);
				count = await _compositeComponentService.Count(type, searchKeywords, compositeComponentSearchRequest.FilterDatas);
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

			return Ok(new ListDto<CompositeComponentDto>
			{
				BatchSize = compositeComponentSearchRequest.BatchSize,
				SortColumn = compositeComponentSearchRequest.SortColumn,
				SortOrder = compositeComponentSearchRequest.SortOrder,
				PageNumber = compositeComponentSearchRequest.PageNumber,
				RecordCount = count,
				Items = compositeComponents.Select(s => new CompositeComponentDto(s)).ToList()
			});
		}

        /// <summary>
        /// Returns all composite components controlled by the pageNumber and batchSize.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Recent Composite Components")]
        [Route("all")]
		[HttpGet]
		public async Task<IHttpActionResult> GetRecentCompositeComponents(string type, int pageNumber = 1, int batchSize = -1)
		{
			List<CompositeComponent> recentCompositeComponents;
			long totalCount;
			try
			{
				recentCompositeComponents = await _compositeComponentService.Find(type, new List<string>(), new List<FilterData>(), ComponentDao.DateCreated,
					SortOrder.Descending, pageNumber, batchSize == -1 ? _defaultBatchSize : batchSize);
				totalCount = await _compositeComponentService.Count(type, new List<string>(), new List<FilterData>());
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}

			return Ok(new ListDto<CompositeComponentDto>
			{
				Items = recentCompositeComponents?.Select(s => new CompositeComponentDto(s)).ToList(),
				RecordCount = totalCount,
				PageNumber = pageNumber,
				BatchSize = batchSize,
				TotalPages = totalCount / batchSize
			});
		}

        /// <summary>
        /// Fetch Composite components with attachments
        /// </summary>
        /// <param name="type"></param>
        /// <param name="group"></param>
        /// <param name="columnName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Edit SFG properties,Edit Package properties,Create SFG,Create Package")]
        [Route("documents")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCompositeComponentsInGroupHavingAttachmentColumn(string type, string group,
			string columnName,
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

			try
			{
				var totalRecords =
					await _compositeComponentService.GetCountOfCompositeComponentsHavingAttachmentColumnDataInGroup(type,
						group,
						columnName, keywordList);

				var result =
					await _compositeComponentService.GetCompositeComponentsHavingAttachmentColumnDataInGroup(type, group,
						columnName, keywordList, pageNumber, batchSize);

				return Ok(new ListDto<CompositeComponentDocumentDto>
				{
					Items = result.Select(m => CompositeComponentDocumentDtoAdaptor.FromCompositeComponent(m, columnName)).ToList(),
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

		private Dictionary<string, object> ConvertToDictionary(MoneyDto moneyDto)
		{
			return new Dictionary<string, object>
			{
				{"Amount", moneyDto.Amount.ToString(CultureInfo.InvariantCulture)},
				{"Currency", moneyDto.Currency}
			};
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
	}
}