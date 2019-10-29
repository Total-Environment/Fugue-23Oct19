using System;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// Contains all the endpoints related to brand definitions
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("definitions/brands")]
    public class BrandDefinitionController : BaseController
    {
        /// <summary>
        /// The brand definition name
        /// </summary>
        private const string BrandDefinitionName = "Generic Brand";

        private readonly IBrandDefinitionRepository _brandDefinitionRepository;
        private readonly ISimpleDataTypeFactory _dataTypeFactory;

        /// <summary>
        /// </summary>
        public BrandDefinitionController(IBrandDefinitionRepository brandDefinitionRepository,
            ISimpleDataTypeFactory dataTypeFactory)
        {
            _brandDefinitionRepository = brandDefinitionRepository;
            _dataTypeFactory = dataTypeFactory;
        }

        /// <summary>
        /// Adds the specified brand definition DTO
        /// </summary>
        /// <param name="brandDefinitionDto">The brand definition DTO.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create Brand Definition")]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Add(BrandDefinitionDto brandDefinitionDto)
        {
            try
            {
                var brandDefinition = await brandDefinitionDto.GetDomain(_dataTypeFactory);
                await _brandDefinitionRepository.Add(brandDefinition);

                var brand = await _brandDefinitionRepository.FindBy(brandDefinitionDto.Name);
                return Created("", new BrandDefinitionDto(brand));
            }
            catch (ResourceNotFoundException ex)
            {
                LogToElmah(ex);
                return NotFound();
            }
            catch (DuplicateResourceException ex)
            {
                LogToElmah(ex);
                return Conflict();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Gets the brand definition
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [ComponentLibraryAuthorize(Permissions = "View Brand Definition")]
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var brand = await _brandDefinitionRepository.FindBy(BrandDefinitionName);
                return Ok(new BrandDefinitionDto(brand));
            }
            catch (ResourceNotFoundException ex)
            {
                LogToElmah(ex);
                return NotFound();
            }
            catch (Exception ex)
            {
                LogToElmah(ex);
                return BadRequest();
            }
        }
    }
}