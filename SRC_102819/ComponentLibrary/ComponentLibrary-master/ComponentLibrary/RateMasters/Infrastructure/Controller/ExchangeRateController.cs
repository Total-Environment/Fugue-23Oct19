using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller
{
    /// <summary>
    /// Contains all endpoints for Exchange Rates
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("exchange-rates")]
    public class ExchangeRateController : BaseController
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateController"/> class.
        /// </summary>
        /// <param name="exchangeRateRepository">The exchange rate repository.</param>
        public ExchangeRateController(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        /// <summary>
        /// Creates the specified exchange rate dto.
        /// </summary>
        /// <param name="exchangeRateDto">The exchange rate dto.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Setup Exchange Rate version and View Exchange rate history")]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Create([FromBody]ExchangeRateDto exchangeRateDto)
        {
            IExchangeRate exchangeRate;
            try
            {
                ValidateAppliedOnCreate(exchangeRateDto.AppliedFrom);
                AdjustTimeComponenttoMidnightIST(exchangeRateDto.AppliedFrom);
                exchangeRate = await _exchangeRateRepository.CreateExchangeRate(exchangeRateDto.Domain());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (DuplicateResourceException)
            {
                return Conflict();
            }

            ExchangeRateDto rateDto = new ExchangeRateDto(exchangeRate);
            return Created("", rateDto);
        }

        /// <summary>
        /// Gets the specified from currency.
        /// </summary>
        /// <param name="fromCurrency">From currency.</param>
        /// <param name="on">The on.</param>
        /// <param name="toCurrency">To currency.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View current Exchange rate")]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string fromCurrency, DateTime on, string toCurrency = "INR")
        {
            IExchangeRate exchangeRate;
            try
            {
                ValidateAppliedOnForGet(on);
                on = on.InIst().Date;
                exchangeRate = await _exchangeRateRepository.GetExchangeRate(fromCurrency, toCurrency, @on);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            ExchangeRateDto response = new ExchangeRateDto(exchangeRate);
            return Ok(response);
        }

        /// <summary>
        /// Gets the specified to currency.
        /// </summary>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View current Exchange rate")]
        [Route("latest")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(DateTime on)
        {
            try
            {
                ValidateAppliedOnForGet(on);
                on = on.InIst().Date;
                var exchangeRates = await _exchangeRateRepository.GetAll(on);
                var exchangeRateDtos =
                    exchangeRates.OrderByDescending(e => e.AppliedFrom).Select(e => new ExchangeRateDto(e)).ToList();
                return Ok(exchangeRateDtos);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currencyType"></param>
        /// <param name="appliedFrom"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Setup Exchange Rate version and View Exchange rate history")]
        [Route("all")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string currencyType=null, DateTime? appliedFrom = null, 
            int pageNumber = 1, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending)
        {
            try
            {
                var exchangeRates = await _exchangeRateRepository.GetHistory(currencyType, appliedFrom, pageNumber, sortColumn, sortOrder);
                var exchangeRateDtos =
                    new PaginatedAndSortedListDto<ExchangeRateDto>().WithList(exchangeRates, e => new ExchangeRateDto(e));
                return Ok(exchangeRateDtos);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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

        private void AdjustTimeComponenttoMidnightIST(DateTime requestDtoAppliedOn)
        {
            requestDtoAppliedOn = requestDtoAppliedOn.Add(requestDtoAppliedOn.AdditionalTimeSinceMidnightIst());
        }
    }
}