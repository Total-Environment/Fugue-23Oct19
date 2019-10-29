using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using MoneyDto = TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto.MoneyDto;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller
{
    /// <summary>
    /// The controller for Rental Rates
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController" />
    [RoutePrefix("materials")]
    public class RentalRateController : BaseController
    {
        private const string AssetError = "This material is not defined as an Asset and hence can not have Rental rates.";
        private const string FutureDateError = "New version of rental rate can only be created for future dates.";
        private const string General = "General";
        private const string InvalidUoM = "Please specify a valid UoM for Rental";
        private const string IsAssetColumnName = "Can be Used as an Asset";
        private const string MaterialNotFound = "MaterialID not found.";
        private const string NoResultError = "The given asset material doesn’t have any rental rate details in CL.";
        private const string NullInput = "One of input values is null.";
        private readonly IMasterDataRepository _masterDataRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IRentalRateRepository _rentalRateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRateController"/> class.
        /// </summary>
        /// <param name="rentalRateRepository">The rental rate repository.</param>
        /// <param name="materialRepository">The material repository.</param>
        /// <param name="masterDataRepository">The master data repository.</param>
        public RentalRateController(IRentalRateRepository rentalRateRepository,
            IMaterialRepository materialRepository, IMasterDataRepository masterDataRepository)
        {
            _rentalRateRepository = rentalRateRepository;
            _materialRepository = materialRepository;
            _masterDataRepository = masterDataRepository;
        }

        /// <summary>
        /// Creates the rental rate.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="rentalRateDto">The rental rate dto.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Setup Material Rental Rate versions")]
        [Route("{materialId}/rental-rates")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateRentalRate(string materialId, [FromBody]RentalRateDto rentalRateDto)
        {
            try
            {
                rentalRateDto.MaterialId = materialId;
                await ValidateInputData(rentalRateDto);
                await _rentalRateRepository.Add(rentalRateDto.GetDomain());
            }
            catch (DuplicateResourceException ex)
            {
                LogToElmah(ex);
                return Conflict();
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            return Created("", rentalRateDto);
        }

        /// <summary>
        /// Gets the specified material identifier.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="appliedFrom">The applied from.</param>
        /// <param name="unitOfMeasure">The unit of measure.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Material properties")]
        [Route("{materialId}/rental-rates")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string materialId, DateTime appliedFrom, string unitOfMeasure)
        {
            try
            {
                await ValidateInputData(materialId, unitOfMeasure, appliedFrom);
                var material = await _rentalRateRepository.Get(materialId, unitOfMeasure, appliedFrom);
                return Ok(new RentalRateDto(material.MaterialId, material.UnitOfMeasure, new MoneyDto(material.Value), material.AppliedFrom));
            }
            catch (ArgumentException e)
            {
                LogToElmah(e);
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException e)
            {
                LogToElmah(e);
                return NotFound();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="unitOfMeasure"></param>
        /// <param name="appliedFrom"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Rental Rate History")]
        [Route("{materialId}/rental-rates/all")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll(string materialId, string rentalUnit = null, DateTime? appliedFrom = null,
            int pageNumber = 1, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending)
        {
            try
            {
                await ValidateInputData(materialId);
                var rates = await _rentalRateRepository.GetAll(materialId, rentalUnit, appliedFrom, pageNumber, sortColumn, sortOrder);
                return Ok(new PaginatedAndSortedListDto<RentalRateDto>().WithList(rates, rentalRate =>
                    new RentalRateDto(rentalRate.MaterialId, rentalRate.UnitOfMeasure, new MoneyDto(rentalRate.Value), rentalRate.AppliedFrom)));
            }
            catch (ArgumentException e)
            {
                LogToElmah(e);
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException e)
            {
                LogToElmah(e);
                return NotFound();
            }
        }

        /// <summary>
        /// Gets the active rental rates
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="appliedFrom">The applied from.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Material properties")]
        [Route("{materialId}/rental-rates/active")]
        [HttpGet]
        public async Task<IHttpActionResult> GetActive(string materialId, DateTime appliedFrom)
        {
            try
            {
                await ValidateInputData(materialId, appliedFrom);
                var materials = await _rentalRateRepository.GetLatest(materialId, appliedFrom);
                return
                    Ok(
                        materials.Select(
                            material =>
                                new RentalRateDto(material.MaterialId, material.UnitOfMeasure,
                                    new MoneyDto(material.Value), material.AppliedFrom)));
            }
            catch (ArgumentException e)
            {
                LogToElmah(e);
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException e)
            {
                LogToElmah(e);
                return NotFound();
            }
            catch (Exception e)
            {
                LogToElmah(e);
                throw;
            }
        }

        private static void InputValuesShouldNotBeNull(Dictionary<string, object> values)
        {
            foreach (var value in values)
            {
                if (value.Value == null)
                {
                    throw new ArgumentException($"{value.Key} is null.");
                }
                if (value.Value is string)
                {
                    if (string.IsNullOrEmpty(value.Value.ToString()))
                    {
                        throw new ArgumentException($"{value.Key} is null.");
                    }
                }
                if (value.Value is DateTime)
                {
                    if ((DateTime)value.Value == default(DateTime))
                    {
                        throw new ArgumentException($"{value.Key} is null.");
                    }
                }
            }
        }

        private void ApplidFromShouldBeInISOFormat(DateTime appliedFrom)
        {
            if (appliedFrom.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("Date should be in ISO format.");
            }
        }

        private async Task ValidateInputData(string materialId)
        {
            var values = new Dictionary<string, object>();
            values.Add("Material Id", materialId);

            InputValuesShouldNotBeNull(values);
            await VerifyMaterialExistsAndIsAsset(materialId);
        }

        private async Task ValidateInputData(string materialId, DateTime appliedFrom)
        {
            var values = new Dictionary<string, object>();
            values.Add("Material Id", materialId);
            values.Add("Applied From", appliedFrom);

            InputValuesShouldNotBeNull(values);
            ApplidFromShouldBeInISOFormat(appliedFrom);
            await VerifyMaterialExistsAndIsAsset(materialId);
        }

        private async Task ValidateInputData(RentalRateDto rentalRateDto)
        {
            VerifyRentalRateDtoHasNonNullValues(rentalRateDto);

            VerifyAppliedFromIsNotPastOrPresntDayInIst(rentalRateDto);

            await VerifyUnitOfMeasureIsAValidMasterDataValue(rentalRateDto);

            await VerifyMaterialExistsAndIsAsset(rentalRateDto.MaterialId);
        }

        private async Task ValidateInputData(string materialId, string unitOfMeasure, DateTime appliedFrom)
        {
            var values = new Dictionary<string, object>
            {
                {"Material Id", materialId},
                {"Applied From", appliedFrom},
                {"Unit of Measure", unitOfMeasure}
            };
            InputValuesShouldNotBeNull(values);

            ApplidFromShouldBeInISOFormat(appliedFrom);
            await VerifyMaterialExistsAndIsAsset(materialId);
        }

        private void VerifyAppliedFromIsNotPastOrPresntDayInIst(RentalRateDto rentalRateDto)
        {
            try
            {
                rentalRateDto.AppliedFrom.ShouldBeAFutureDayInIst();
            }
            catch (ArgumentException e)
            {
                LogToElmah(e);
                throw new ArgumentException(FutureDateError, e);
            }
        }

        private async Task VerifyMaterialExistsAndIsAsset(string materialId)
        {
            var material = await _materialRepository.Find(materialId);
            if (material == null)
                throw new ArgumentException(MaterialNotFound);
            var isAsset = false;
            if (material.Headers.Any(h => h.Name == General))
            {
                var columns = material.Headers.First(h => h.Name == General).Columns;
                if (columns.Any(c => c.Name == IsAssetColumnName))
                    isAsset = (bool)(columns.FirstOrDefault(c => c.Name == IsAssetColumnName).Value);
            }
            if (!isAsset)
            {
                throw new ArgumentException(AssetError);
            }
        }

        private void VerifyRentalRateDtoHasNonNullValues(RentalRateDto rentalRateDto)
        {
            if (rentalRateDto.MaterialId == null || rentalRateDto.RentalRateValue == null ||
                rentalRateDto.UnitOfMeasure == null ||
                rentalRateDto.AppliedFrom == default(DateTime))
                throw new ArgumentException(NullInput);
        }

        private async Task VerifyUnitOfMeasureIsAValidMasterDataValue(RentalRateDto rentalRateDto)
        {
            var exists = await _masterDataRepository.Exists("rental_unit",
                rentalRateDto.UnitOfMeasure);
            if (!exists)
            {
                throw new ArgumentException(InvalidUoM);
            }
        }
    }
}