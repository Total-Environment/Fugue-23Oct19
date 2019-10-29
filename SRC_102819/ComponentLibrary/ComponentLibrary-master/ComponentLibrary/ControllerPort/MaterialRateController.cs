using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using MoneyDto = TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto.MoneyDto;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// Contains all endpoints of Material Rates
	/// </summary>
	/// <seealso cref="BaseController"/>
	public class MaterialRateController : BaseController
	{
		private readonly IMasterDataRepository _masterDataRepository;
		private readonly IMaterialRateService _materialRateService;
		private readonly IBank _bank;
		private readonly IMaterialRepository _materialRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialRateController"/> class.
		/// </summary>
		/// <param name="materialRateService">The material rate service.</param>
		/// <param name="bankRepository">The bank repository.</param>
		/// <param name="masterDataRepository">The master data repository.</param>
		public MaterialRateController(IMaterialRateService materialRateService, IBank bank, IMasterDataRepository masterDataRepository, IMaterialRepository materialRepository)
		{
			_materialRateService = materialRateService;
			_bank = bank;
			_masterDataRepository = masterDataRepository;
			_materialRepository = materialRepository;
		}

		/// <summary>
		/// Endpoint for material rate bulk edit.
		/// </summary>
		/// <param name="requestDtos">The request dtos.</param>
		/// <returns>Records wth response status and data.</returns>
		[ComponentLibraryAuthorize(Permissions = "Material Bulk Rates Edit")]
		[Route("materials/rates/bulk-edit")]
		[HttpPost]
		public async Task<IHttpActionResult> BulkEdit([FromBody] IEnumerable<MaterialRateDto> requestDtos)
		{
			var responseData = new BuildRateResponse<MaterialRateDto>();

			foreach (var requestDto in requestDtos)
			{
				try
				{
					await CreateMaterialRate(requestDto);
					responseData.Records.Add(CreateRateEditResponse(requestDto));
				}
				catch (ArgumentException ex)
				{
					responseData.Records.Add(CreateRateEditResponse(requestDto, ex));
				}
				catch (DuplicateResourceException ex)
				{
					responseData.Records.Add(CreateRateEditResponse(requestDto, ex));
				}
				catch (ResourceNotFoundException ex)
				{
					responseData.Records.Add(CreateRateEditResponse(requestDto, ex));
				}
			}

			var status = "Succeeded";
			status = responseData.Records.All(r => r.Status == RateEditStatus.Error.ToString()) ? "Failed" : responseData.Records.Any(r => r.Status == RateEditStatus.Error.ToString()) ? "PartiallySucceeded" : status;
			responseData.Status = status;
			return Ok(responseData);
		}

		/// <summary>
		/// Creates the specified request dto.
		/// </summary>
		/// <param name="requestDto">The request dto.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Setup Material Rate versions")]
		[Route("material-rates")]
		[HttpPost]
		public async Task<IHttpActionResult> Create([FromBody] MaterialRateDto requestDto)
		{
			IMaterialRate materialRate;
			try
			{
				materialRate = await CreateMaterialRate(requestDto);
				var response = await new MaterialRateDto().SetDomain(materialRate);
				return Created("", response);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (DuplicateResourceException ex)
			{
				return Conflict();
			}
			catch (ResourceNotFoundException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Gets the specified materialid.
		/// </summary>
		/// <param name="materialid">The materialid.</param>
		/// <param name="location">The location.</param>
		/// <param name="typeOfPurchase">The type of purchase.</param>
		/// <param name="on">The on.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Get Material Rate")]
		[Route("material-rates")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string materialid, string location,
			string typeOfPurchase, DateTime on)
		{
			IMaterialRate materialRate;
			try
			{
				ValidateAppliedOnForGet(on);
				on = on.InIst().Date;
				materialRate = await _materialRateService.GetRate(materialid, location, @on, typeOfPurchase);
				var response = await new MaterialRateDto().SetDomain(materialRate);
				return Ok(response);
			}
			catch (ResourceNotFoundException)
			{
				Trace.TraceError(
					$"Material rate not found for materialId : {materialid}, locat:ion : {location}, on : {on}.");
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}
		}

		//TODO: Remove the DateTime on parameter
		/// <summary>
		/// Gets the specified material identifier.
		/// </summary>
		/// <param name="materialId">The material identifier.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "View Material properties")]
		[Route("material-rates/latest")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string materialId, DateTime on)
		{
			IEnumerable<IMaterialRate> materialRates;

			try
			{
				ValidateAppliedOnForGet(on);
				on = on.InIst().Date;
				materialRates = await _materialRateService.GetRates(materialId, on);
			}
			catch (ResourceNotFoundException)
			{
				Trace.TraceError($"Material rates not found for materialId : {materialId}.");
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}

			try
			{
				var convertedMaterialRates = await ConvertMaterialRatesToInr(materialRates, _bank);
				var response = await Task.WhenAll(convertedMaterialRates.Select(x => new MaterialRateDto().SetDomain(x)));
				return Ok(response);
			}
			catch (ArgumentException exception)
			{
				Trace.TraceError("No exchange rate is found");
				return BadRequest(exception.Message);
			}
		}

		/// <summary>
		/// Gets the average landed rate.
		/// </summary>
		/// <param name="materialid">The materialid.</param>
		/// <param name="location">The location.</param>
		/// <param name="onDate">The on date.</param>
		/// <param name="currency">The currency.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "View Material properties,View Material Rate History")]
		[Route("material-average-landed-rates")]
		[HttpGet]
		public async Task<IHttpActionResult> GetAverageLandedRate(string materialid, string location, DateTime onDate,
			string currency = null)
		{
			if (currency == null)
			{
				currency = "INR";
			}
			Money landedRate;
			Money controlBaseRate;
			string unitOfMeasure = null;
			try
			{
				ValidateAppliedOnForGet(onDate);
				onDate = onDate.InIst().Date;

				landedRate = await _materialRateService.GetAverageLandedRate(materialid, location, onDate, currency);
				controlBaseRate = await _materialRateService.GetAverageControlBaseRate(materialid, location, onDate, currency);
				var material = await _materialRepository.Find(materialid);
				var generalHeader = material.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
				var unitOfMeasureColumn = generalHeader?.Columns?.FirstOrDefault(c => c.Key == "unit_of_measure");
				if (unitOfMeasureColumn != null)
					unitOfMeasure = unitOfMeasureColumn?.Value.ToString();
			}
			catch (ResourceNotFoundException)
			{
				Trace.TraceError(
					$"Material Not Found for materialId : {materialid}, location : {location}, onDate : {onDate}, in Currency {currency}");
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}

			Money convertedLandedRate;
			Money convertedControlBaseRate;
			try
			{
				convertedLandedRate = await _bank.ConvertTo(landedRate, currency);
				convertedControlBaseRate = await _bank.ConvertTo(controlBaseRate, currency);
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}

			var response = new LandedRateDto
			{
				LandedRate = new MoneyDto(convertedLandedRate),
				ControlBaseRate = new MoneyDto(convertedControlBaseRate),
				UnitOfMeasure = unitOfMeasure
			};
			return Ok(response);
		}

		/// <summary>
		/// Gets the landed rate.
		/// </summary>
		/// <param name="materialid">The materialid.</param>
		/// <param name="location">The location.</param>
		/// <param name="typeOfPurchase">The type of purchase.</param>
		/// <param name="onDate">The on date.</param>
		/// <param name="currency">The currency.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "View Material properties")]
		[Route("material-landed-rates")]
		[HttpGet]
		public async Task<IHttpActionResult> GetLandedRate(string materialid, string location, string typeOfPurchase,
			DateTime onDate, string currency = null)
		{
			if (currency == null)
			{
				currency = "INR";
			}
			Money landedRate;
			Money controlBaseRate;
			string unitOfMeasure = null;
			try
			{
				ValidateAppliedOnForGet(onDate);
				onDate = onDate.InIst().Date;
				landedRate = await _materialRateService.GetLandedRate(materialid, location, onDate, currency,
					typeOfPurchase);
				controlBaseRate = await _materialRateService.GetControlBaseRate(materialid, location, onDate, currency,
					typeOfPurchase);
				var material = await _materialRepository.Find(materialid);
				var generalHeader = material.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
				var unitOfMeasureColumn = generalHeader?.Columns?.FirstOrDefault(c => c.Key == "unit_of_measure");
				if (unitOfMeasureColumn != null)
					unitOfMeasure = unitOfMeasureColumn?.Value.ToString();
			}
			catch (ResourceNotFoundException)
			{
				Trace.TraceError(
					$"Material Not Found for materialId : {materialid}, location : {location}, onDate : {onDate}, in Currency {currency}");
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				Trace.TraceError($"Internal Server Error Occurred: {ex.Message}");
				throw ex;
			}

			Money convertedLandedRate;
			Money convertedControlBaseRate;
			try
			{
				convertedLandedRate = await _bank.ConvertTo(landedRate, currency);
				convertedControlBaseRate = await _bank.ConvertTo(controlBaseRate, currency);
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}

			object procurementThresholdOfMaterial = null;
			try
			{
				var material = await _materialRepository.Find(materialid);
				var purchaseHeader = material.Headers.FirstOrDefault(h => h.Key == "purchase");
				if (purchaseHeader != null)
					procurementThresholdOfMaterial = purchaseHeader.Columns.FirstOrDefault(c => c.Key == "procurement_rate_threshold");
			}
			catch (ArgumentNullException)
			{
				Trace.TraceError($"Procurement Threshold Rate is not for ServiceId : {materialid}");
				return NotFound();
			}
			catch (ResourceNotFoundException)
			{
				Trace.TraceError($"Service Not Found for serviceId : {materialid}");
				return NotFound();
			}
			catch (BetterKeyNotFoundException bknfe)
			{
				Trace.TraceError($"{bknfe.Key} was not found");
				return NotFound();
			}

			var response = new LandedRateDto
			{
				ProcurementRateThreshold = procurementThresholdOfMaterial,
				LandedRate = new MoneyDto(convertedLandedRate),
				ControlBaseRate = new MoneyDto(convertedControlBaseRate),
				UnitOfMeasure = unitOfMeasure
			};

			return Ok(response);
		}

		[ComponentLibraryAuthorize(Permissions = "View Material Rate History")]
		[Route("material-rates/all")]
		[HttpGet]
		public async Task<IHttpActionResult> GetRateHistroy(string materialId, [FromUri] MaterialRateSearchRequest materialRateSearchRequest)
		{
			try
			{
				var materialRates = await _materialRateService.GetRateHistory(materialId, materialRateSearchRequest);

				Func<IMaterialRate, Task<Money>> keySelector = GetKeySelector(materialRateSearchRequest.SortColumn);
				if (keySelector != default(Func<IMaterialRate, Task<Money>>))
				{
					//We have to do this becase Landed rate is not persisted in DB, so we are doing in memory sorting.
					materialRates = SortMaterialRatesFromLandedRate(materialRates, materialRateSearchRequest.SortOrder, keySelector);
				}
				var materialRateDtos = await new PaginatedAndSortedListDto<MaterialRateDto>().WithListAsync(materialRates, async materialRate => await new MaterialRateDto().SetDomain(materialRate));

				return Ok(materialRateDtos);
			}
			catch (ResourceNotFoundException)
			{
				Trace.TraceError(
					$"Material rate not found for materialId : {materialId}");
				return NotFound();
			}
			catch (ArgumentException ex)
			{
				Trace.TraceError($"Bad Request Recieved : {ex.Message}");
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Converts the material rates to inr.
		/// </summary>
		/// <param name="materialRates">The material rates.</param>
		/// <param name="bank">The bank.</param>
		/// <returns></returns>
		private static async Task<IEnumerable<MaterialRate>> ConvertMaterialRatesToInr(IEnumerable<IMaterialRate> materialRates, IBank bank)
		{
			var convertedMaterialRates = new List<MaterialRate>();
			foreach (var materialRate in materialRates)
			{
				try
				{
					var controlBaseRate = materialRate.ControlBaseRate;
					var convertedControlBaseRate = await bank.ConvertTo(controlBaseRate, "INR");
					var convertedMaterialRate = new MaterialRate(
						materialRate.AppliedOn,
						materialRate.Location,
						materialRate.Id,
						convertedControlBaseRate,
						materialRate.FreightCharges,
						materialRate.InsuranceCharges,
						materialRate.BasicCustomsDuty,
						materialRate.ClearanceCharges,
						materialRate.TaxVariance,
						materialRate.LocationVariance,
						materialRate.MarketFluctuation,
						materialRate.TypeOfPurchase);
					convertedMaterialRates.Add(convertedMaterialRate);
				}
				catch (ArgumentException exception)
				{
					throw new ArgumentException(exception.Message);
				}
				catch (Exception exception)
				{
					Trace.TraceError($"Error occured while converting material rate : {exception.Message}");
				}
			}
			return convertedMaterialRates;
		}

		private static BuildRateResponseRecord<MaterialRateDto> CreateRateEditResponse(MaterialRateDto requestDto, Exception ex = null)
		{
			var responseRecord = new BuildRateResponseRecord<MaterialRateDto>();
			responseRecord.Status = ex == null ? RateEditStatus.Created.ToString() : RateEditStatus.Error.ToString();
			responseRecord.Message = ex?.Message ?? string.Empty;
			responseRecord.RecordData.Add(requestDto);
			return responseRecord;
		}

		private void AdjustTimeComponenttoMidnightIst(MaterialRateDto requestDto)
		{
			requestDto.AppliedOn = requestDto.AppliedOn.Add(requestDto.AppliedOn.AdditionalTimeSinceMidnightIst());
		}

		private async Task<IMaterialRate> CreateMaterialRate(MaterialRateDto requestDto)
		{
			var typeOfPurchaseList = await _masterDataRepository.FindByName(MasterDataListDao.TypeOfPurchase);
			var locationList = await _masterDataRepository.FindByName(MasterDataListDao.Location);
			ValidateAppliedOnCreate(requestDto.AppliedOn);
			AdjustTimeComponenttoMidnightIst(requestDto);
			var domain = requestDto.Domain(_bank, typeOfPurchaseList, locationList);
			return await _materialRateService.CreateRate(domain);
		}

		private Func<IMaterialRate, Task<Money>> GetKeySelector(string sortColumn)
		{
			Func<IMaterialRate, Task<Money>> keySelector = default(Func<IMaterialRate, Task<Money>>);
			switch (sortColumn)
			{
				case "LandedRate":
					keySelector = async m => await m.LandedRate();
					break;

				case "ControlBaseRate":
					keySelector = async m => await Task.FromResult(m.ControlBaseRate);
					break;
			}
			return keySelector;
		}

		private PaginatedAndSortedList<IMaterialRate> SortMaterialRatesFromLandedRate(PaginatedAndSortedList<IMaterialRate> materialRates, SortOrder sortOrder, Func<IMaterialRate, Task<Money>> keySelector)
		{
			var sortedMaterials = materialRates;
			if (sortOrder == SortOrder.Ascending)
			{
				sortedMaterials.Items = materialRates.Items.OrderBy(keySelector);
			}
			else
			{
				sortedMaterials.Items = materialRates.Items.OrderByDescending(async m => await m.LandedRate());
			}
			return sortedMaterials;
		}

		private void ValidateAppliedOnCreate(DateTime requestDtoAppliedOn)
		{
			if (requestDtoAppliedOn.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("Applied on date should be in ISO format.");

			var appliedOnInUtc = TimeZoneInfo.ConvertTimeToUtc(requestDtoAppliedOn);
			var todayIndianTime = DateTime.UtcNow.InIst();
			var appliedOnIndianTime = requestDtoAppliedOn.InIst();

			if (appliedOnInUtc.CompareTillMinutePrecision(DateTime.UtcNow) == -1)
				throw new ArgumentException("Applied on date cannot be set to past.");

			if (appliedOnIndianTime.Date == todayIndianTime.Date)
				throw new ArgumentException("Applied on date cannot be set on same day in IST.");
		}

		private void ValidateAppliedOnForGet(DateTime requestDtoAppliedOn)
		{
			if (requestDtoAppliedOn.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("Applied on date should be in ISO format.");
		}
	}
}