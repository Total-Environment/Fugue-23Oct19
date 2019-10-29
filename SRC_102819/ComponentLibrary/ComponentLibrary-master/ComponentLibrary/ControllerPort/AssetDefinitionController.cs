using System;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// Asset Definition Controller.
    /// </summary>
    [RoutePrefix("asset-definitions")]
    public class AssetDefinitionController : BaseController
    {
        private readonly IComponentDefinitionRepository<AssetDefinition> _assetDefinitionRepository;
        private readonly IDataTypeFactory _dataTypeFactory;
        private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
        private readonly IComponentDefinitionRepository<IMaterialDefinition> _materialDefinitionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetDefinitionController" /> class.
        /// </summary>
        /// <param name="assetDefinitionRepository">The asset definition repository.</param>
        /// <param name="materialDefinitionRepository">The material definition repository.</param>
        /// <param name="dataTypeFactory">The data type factory.</param>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        public AssetDefinitionController(IComponentDefinitionRepository<AssetDefinition> assetDefinitionRepository,
            IComponentDefinitionRepository<IMaterialDefinition> materialDefinitionRepository,
            IDataTypeFactory dataTypeFactory, IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            _assetDefinitionRepository = assetDefinitionRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
            _dataTypeFactory = dataTypeFactory;
            _dependencyDefinitionRepository = dependencyDefinitionRepository;
        }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Asset Definition")]
        [Route("{name}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string name)
        {
            ComponentDefinitionDto response;
            try
            {
                ValidateAssetGroupName(name);
                response = new AssetDefinitionDto(await _assetDefinitionRepository.Find(name));
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

        private static void ValidateAssetGroupName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Asset definition name cannot be null or empty.");
        }

        /// <summary>
        /// Posts the specified asset definition dto.
        /// </summary>
        /// <param name="assetDefinitionDto">The asset definition dto.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create Asset Definition")]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(AssetDefinitionDto assetDefinitionDto)
        {
            try
            {
                if (assetDefinitionDto.Name != "Generic Asset")
                    await _materialDefinitionRepository.Find(assetDefinitionDto.Name);
                var assetDefinition = await assetDefinitionDto.GetDomain(_dependencyDefinitionRepository,
                    _dataTypeFactory) as AssetDefinition;
                await _assetDefinitionRepository.Add(assetDefinition);
            }
            catch (ResourceNotFoundException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            catch (DuplicateResourceException ex)
            {
                LogToElmah(ex);
                return Conflict();
            }
            return Ok();
        }
    }
}