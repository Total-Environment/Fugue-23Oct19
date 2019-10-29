using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
    /// <summary>
    /// Represents the master data list controller.
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("master-data")]
    public class MasterDataController : BaseController
    {
        private readonly IMasterDataRepository _masterDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataController"/> class.
        /// </summary>
        /// <param name="masterDataRepository">The master data repository.</param>
        public MasterDataController(IMasterDataRepository masterDataRepository)
        {
            _masterDataRepository = masterDataRepository;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns the master data with given id.</returns>
        [ComponentLibraryAuthorize(Permissions = "Get Master Data by Id")]
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            try
            {
                var masterDataList = await _masterDataRepository.Find(id);
                return Ok(new MasterDataListDto(masterDataList));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Gets the master data by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get Master Data by Name")]
        [Route("name/{name}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByName(string name)
        {
            try
            {
                var masterDataList = await _masterDataRepository.FindByName(name);
                return Ok(new MasterDataListDto(masterDataList));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns the master data list that was added.</returns>
        [ComponentLibraryAuthorize(Permissions = "Create Master Data")]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(MasterDataListDto request)
        {
            MasterDataList domain;
            try
            {
                domain = request.Domain();
                await _masterDataRepository.Add(domain);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DuplicateResourceException)
            {
                return Conflict();
            }
            return Created("", new MasterDataListDto(domain));
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Returns the master data list.</returns>
        [ComponentLibraryAuthorize(Permissions = "Get Master Data by Name")]
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(_masterDataRepository.Find().Select(m => new MasterDataListDto(m as MasterDataList)));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Patches the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Patch Master Data")]
        [Route("")]
        [HttpPatch]
        public async Task<IHttpActionResult> Patch([FromBody] MasterDataList input)
        {
            MasterDataList updatedMasterData;
            try
            {
                var previousMasterData = await _masterDataRepository.Find(input.Id);
                var mergedMasterData = Merge(input, previousMasterData);
                updatedMasterData = await _masterDataRepository.Patch(mergedMasterData);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(updatedMasterData);
        }

        private MasterDataList Merge(MasterDataList input, MasterDataList previousMasterData)
        {
            var hasDuplicateData =
                input.Values.Any(
                    v => previousMasterData.Values.Select(pv => pv.Value.ToLower())
                        .Contains(v.Value.ToLower()));
            if (hasDuplicateData) throw new ArgumentException("Master Data already Exists.");
            return new MasterDataList(previousMasterData.Name,
                previousMasterData.Values.Concat(input.Values).ToList()) {Id = previousMasterData.Id};
        }
    }
}