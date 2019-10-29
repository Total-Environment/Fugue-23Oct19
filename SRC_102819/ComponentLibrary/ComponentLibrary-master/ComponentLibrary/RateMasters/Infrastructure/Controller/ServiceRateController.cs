using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Elmah;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using MoneyDto = TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto.MoneyDto;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller
{
    /// <summary>
    /// The controller for service rates
    /// </summary>
    /// <seealso cref="BaseController"/>
    public class ServiceRateController : BaseController
    {
        private readonly IServiceRateService _serviceRateService;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IBank _bank;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRateController" /> class.
        /// </summary>
        /// <param name="serviceRateService">The service rate master.</param>
        /// <param name="bankRepository">The bank repository.</param>
        /// <param name="serviceRepository">The Service Repository</param>
        /// <param name="masterDataRepository">The master data repository.</param>
        public ServiceRateController(IServiceRateService serviceRateService, IBank bank,
            IServiceRepository serviceRepository, IMasterDataRepository masterDataRepository)
        {
            _serviceRateService = serviceRateService;
            _bank = bank;
            _serviceRepository = serviceRepository;
            _masterDataRepository = masterDataRepository;
        }

        [ComponentLibraryAuthorize(Permissions = "Service Bulk Rates Edit")]
        [Route("services/rates/bulk-edit")]
        [HttpPost]
        public async Task<IHttpActionResult> BulkEdit([FromBody] IEnumerable<ServiceRateDto> requestDtos)
        {
            var responseData = new BuildRateResponse<ServiceRateDto>();

            foreach (var requestDto in requestDtos)
            {
                try
                {
                    await CreateServiceRate(requestDto);
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
        [ComponentLibraryAuthorize(Permissions = "Setup Service Rate versions")]
        [Route("service-rates")]
        [HttpPost]
        public async Task<IHttpActionResult> Create([FromBody] ServiceRateDto requestDto)
        {
            IServiceRate serviceRate;
            try
            {
                serviceRate = await CreateServiceRate(requestDto);
                var response = await new ServiceRateDto().SetDomain(serviceRate);
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
        /// Gets the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="location">The location.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get Service Rate")]
        [Route("service-rates")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string serviceId, string location,
            string typeOfPurchase, DateTime on)
        {
            IServiceRate serviceRate;
            try
            {
                ValidateAppliedOnForGet(on);
                on = on.InIst().Date;
                serviceRate = await _serviceRateService.GetRate(serviceId, location, @on, typeOfPurchase);
            }
            catch (ResourceNotFoundException e)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(e));
                Trace.TraceError(
                    $"Service rate not found for serviceId : {serviceId}, location : {location}, on : {on}.");
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ErrorLog.GetDefault(HttpContext.Current).Log(new Error(ex));
                Trace.TraceError($"Bad Request Recieved : {ex.Message}");
                return BadRequest(ex.Message);
            }
            var response = await new ServiceRateDto().SetDomain(serviceRate);
            return Ok(response);
        }

        /// <summary>
        /// Gets the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Service properties")]
        [Route("service-rates/latest")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string serviceId, DateTime on)
        {
            IEnumerable<IServiceRate> serviceRates;
            try
            {
                ValidateAppliedOnForGet(on);
                on = on.InIst().Date;
                serviceRates = await _serviceRateService.GetRates(serviceId, on);
            }
            catch (ResourceNotFoundException)
            {
                Trace.TraceError($"Service rates not found for serviceId : {serviceId}.");
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError($"Bad Request Recieved : {ex.Message}");
                return BadRequest(ex.Message);
            }
            
            try
            {
                var convertedServiceRates = await ConvertServiceRatesToInr(serviceRates, _bank);
                var response = await Task.WhenAll(convertedServiceRates.Select(x => new ServiceRateDto().SetDomain(x)));
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
        /// <param name="serviceId">The serviceId.</param>
        /// <param name="location">The location.</param>
        /// <param name="onDate">The on date.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Service properties,View Service Rate History")]
        [Route("service-average-landed-rates")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAverageLandedRate(string serviceId, string location, DateTime onDate,
            string currency = null)
        {
            if (currency == null)
                currency = "INR";
            Money landedRate;
            Money controlBaseRate;
            string unitOfMeasure = null;
            try
            {
                ValidateAppliedOnForGet(onDate);
                onDate = onDate.InIst().Date;
                landedRate = await _serviceRateService.GetAverageLandedRate(serviceId, location, onDate, currency);
                controlBaseRate = await _serviceRateService.GetAverageControlBaseRate(serviceId, location, onDate, currency);
                var service = await _serviceRepository.Find(serviceId);
                var generalHeader = service.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
                var unitOfMeasureColumn = generalHeader?.Columns?.FirstOrDefault(c => c.Key == "unit_of_measure");
                if (unitOfMeasureColumn != null)
                    unitOfMeasure = unitOfMeasureColumn?.Value.ToString();
            }
            catch (ResourceNotFoundException)
            {
                Trace.TraceError(
                    $"Service Not Found for serviceId : {serviceId}, location : {location}, onDate : {onDate}, in Currency {currency}");
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
        /// <param name="serviceId">The serviceId.</param>
        /// <param name="location">The location.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <param name="onDate">The on date.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Service properties")]
        [Route("service-landed-rates")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLandedRate(string serviceId, string location, string typeOfPurchase,
            DateTime onDate, string currency = null)
        {
            if (currency == null)
                currency = "INR";
            Money landedRate;
            Money controlBaseRate;
            string unitOfMeasure = null;

            try
            {
                ValidateAppliedOnForGet(onDate);
                onDate = onDate.InIst().Date;
                landedRate = await _serviceRateService.GetLandedRate(serviceId, location, onDate, currency, typeOfPurchase);
                controlBaseRate = await _serviceRateService.GetControlBaseRate(serviceId, location, onDate, currency,
                    typeOfPurchase);
                var service = await _serviceRepository.Find(serviceId);
                var generalHeader = service.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
                var unitOfMeasureColumn = generalHeader?.Columns?.FirstOrDefault(c => c.Key == "unit_of_measure");
                if (unitOfMeasureColumn != null)
                    unitOfMeasure = unitOfMeasureColumn?.Value.ToString();
            }
            catch (ResourceNotFoundException)
            {
                Trace.TraceError(
                    $"Service Rate Not Found for serviceId : {serviceId}, location : {location}, on : {onDate}, in Currency {currency}");
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

            object procurementThresholdOfService = null;
            try
            {
                var service = await _serviceRepository.Find(serviceId);
                var purchaseHeader = service.Headers.FirstOrDefault(h => h.Key == "purchase");
                if (purchaseHeader != null)
                    procurementThresholdOfService = purchaseHeader.Columns.FirstOrDefault(c => c.Key == "procurement_rate_threshold");
            }
            catch (ArgumentNullException)
            {
                Trace.TraceError($"Procurement Threshold Rate is not for ServiceId : {serviceId}");
                return NotFound();
            }
            catch (ResourceNotFoundException)
            {
                Trace.TraceError($"Service Not Found for serviceId : {serviceId}");
                return NotFound();
            }
            catch (BetterKeyNotFoundException bknfe)
            {
                Trace.TraceError($"{bknfe.Key} was not found");
                return NotFound();
            }

            var response = new LandedRateDto
            {
                ProcurementRateThreshold = procurementThresholdOfService,
                LandedRate = new MoneyDto(convertedLandedRate),
                ControlBaseRate = new MoneyDto(convertedControlBaseRate),
                UnitOfMeasure = unitOfMeasure
            };
            return Ok(response);
        }

        /// <summary>
        /// ///
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="serviceRateSearchRequest"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Service Rate History")]
        [Route("service-rates/all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetRateHistory(string serviceId, [FromUri]ServiceRateSearchRequest serviceRateSearchRequest)
        {
            try
            {
                var serviceRates = await _serviceRateService.GetRateHistory(serviceId, serviceRateSearchRequest);

                var serviceRateDtos = await new PaginatedAndSortedListDto<ServiceRateDto>().WithListAsync(serviceRates, serviceRate => new ServiceRateDto().SetDomain(serviceRate));

                return Ok(serviceRateDtos);
            }
            catch (ResourceNotFoundException)
            {
                Trace.TraceError(
                    $"Service rate not found for servicelId : {serviceId}");
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                Trace.TraceError($"Bad Request Recieved : {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Converts the service rates to inr.
        /// </summary>
        /// <param name="serviceRates">The service rates.</param>
        /// <param name="bank">The bank.</param>
        /// <returns></returns>
        private static async Task<IEnumerable<ServiceRate>> ConvertServiceRatesToInr(IEnumerable<IServiceRate> serviceRates,
            IBank bank)
        {
            var convertedServiceRates = new List<ServiceRate>();
            foreach (var serviceRate in serviceRates)
                try
                {
                    var convertedControlBaseRate = await bank.ConvertTo(serviceRate.ControlBaseRate, "INR");
                    var convertedServiceRate = new ServiceRate(
                        serviceRate.AppliedOn,
                        serviceRate.Location,
                        serviceRate.Id,
                        convertedControlBaseRate,
                        serviceRate.LocationVariance,
                        serviceRate.MarketFluctuation,
                        serviceRate.TypeOfPurchase
                        );
                    convertedServiceRates.Add(convertedServiceRate);
                }
                catch (ArgumentException exception)
                {
                    throw new ArgumentException(exception.Message);
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"Error occured while converting service rate : {exception.Message}");
                }
            return convertedServiceRates;
        }

        private static BuildRateResponseRecord<ServiceRateDto> CreateRateEditResponse(ServiceRateDto requestDto, Exception ex = null)
        {
            var responseRecord = new BuildRateResponseRecord<ServiceRateDto>();
            responseRecord.Status = ex == null ? RateEditStatus.Created.ToString() : RateEditStatus.Error.ToString();
            responseRecord.Message = ex?.Message ?? string.Empty;
            responseRecord.RecordData.Add(requestDto);
            return responseRecord;
        }

        private void AdjustTimeComponenttoMidnightIST(ServiceRateDto requestDto)
        {
            requestDto.AppliedOn = requestDto.AppliedOn.Add(requestDto.AppliedOn.AdditionalTimeSinceMidnightIst());
        }

        private async Task<IServiceRate> CreateServiceRate(ServiceRateDto requestDto)
        {
            var typeOfPurchaseList = await _masterDataRepository.FindByName(MasterDataListDao.TypeOfPurchase);
            var locationList = await _masterDataRepository.FindByName(MasterDataListDao.Location);
            ValidateAppliedOnForCreate(requestDto.AppliedOn);
            AdjustTimeComponenttoMidnightIST(requestDto);
            
            var domain = requestDto.Domain(_bank, typeOfPurchaseList, locationList);
            return await _serviceRateService.CreateRate(domain);
        }

        private void ValidateAppliedOnForCreate(DateTime requestDtoAppliedOn)
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