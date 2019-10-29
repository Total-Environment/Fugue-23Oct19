using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller
{
    /// <summary>
    /// Represents the service definitions.
    /// </summary>
    /// <seealso cref="BaseController"/>
    [RoutePrefix("service-definitions")]
    public class ServiceDefinitionController : BaseController
    {
        private readonly IDataTypeFactory _dataTypeFactory;
        private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
        private readonly IComponentDefinitionRepository<IServiceDefinition> _serviceDefinitionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDefinitionController"/> class.
        /// </summary>
        /// <param name="serviceDefinitionRepository">The service definition repository.</param>
        /// <param name="dataTypeFactory">The data type factory.</param>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        public ServiceDefinitionController(IComponentDefinitionRepository<IServiceDefinition> serviceDefinitionRepository,
            IDataTypeFactory dataTypeFactory, IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            _serviceDefinitionRepository = serviceDefinitionRepository;
            _dependencyDefinitionRepository = dependencyDefinitionRepository;
            _dataTypeFactory = dataTypeFactory;
        }

        /// <summary>
        /// Gets the specified group name.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Service Definition")]
        [Route("{groupName}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string groupName)
        {
            IServiceDefinition serviceDefinition;
            try
            {
                serviceDefinition = await _serviceDefinitionRepository.Find(groupName);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            return Ok(new ServiceDefinitionDao(serviceDefinition));
        }

        /// <summary>
        /// Patches the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Patch Service Definition")]
        [Route("")]
        [HttpPatch]
        public async Task<IHttpActionResult> Patch([FromBody] ServiceDefinitionDao input)
        {
            try
            {
                var serviceDefinition = await _serviceDefinitionRepository.Find(input.Name);
                var patch = await Merge(input, serviceDefinition);

                var isMatching = await CheckForHeaderColumnMapping(patch);

                if (!isMatching.Item1)
                {
                    return BadRequest(isMatching.Item2);
                }

                await _serviceDefinitionRepository.Update(patch);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        /// <summary>
        /// Posts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create Service Definition")]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ServiceDefinitionDao input)
        {
            try
            {
                var domain = await input.GetDomain(_dataTypeFactory, _dependencyDefinitionRepository);

                var isMatching = await CheckForHeaderColumnMapping(domain);

                if (!isMatching.Item1)
                    return BadRequest(isMatching.Item2);

                await _serviceDefinitionRepository.Add(domain as IServiceDefinition);

                var service = await _serviceDefinitionRepository.Find(input.Name);

                return Created("", new ServiceDefinitionDao(service));
            }
            catch (DuplicateResourceException e)
            {
                LogToElmah(e);
                return Conflict();
            }
            catch (Exception e)
            {
                LogToElmah(e);
                throw;
            }
        }

        private async Task<Tuple<bool, string>> CheckForHeaderColumnMapping(IComponentDefinition input)
        {
            if (input.Name == "Generic Service") return new Tuple<bool, string>(true, null);
            var genericService = await _serviceDefinitionRepository.Find("Generic Service");
            var invalidColumns = new StringBuilder();
            foreach (var header in input.Headers)
            {
                var headerName = header.Name;
                foreach (var column in header.Columns)
                {
                    var columnName = column.Name;
                    var columnCheck =
                        genericService.Headers.Any(
                            h => (h.Name == headerName) && h.Columns.Any(c => c.Name == columnName));

                    if (columnCheck == false)
                        invalidColumns.AppendLine($"Header-Column pair {headerName}:{columnName} is invalid.");
                }
            }
            return new Tuple<bool, string>(invalidColumns.Length == 0, invalidColumns.ToString());
        }

        private async Task<IServiceDefinition> Merge(ServiceDefinitionDao dto, IServiceDefinition definition)
        {
            foreach (var dtoHeader in dto.Headers)
            {
                var header = definition.Headers.FirstOrDefault(h => h.Name.Equals(dtoHeader.Name));
                if (header == null)
                    definition.Headers
                        .Add(await dtoHeader.GetDomain(_dataTypeFactory, _dependencyDefinitionRepository));
                else
                {
                    if ((dtoHeader.Columns == null) || (dtoHeader.Columns.Count <= 0))
                        throw new ArgumentException($"Header already exist {dtoHeader.Name}");
                    var column =
                        dtoHeader.Columns.FirstOrDefault(c => header.Columns.Select(hc => hc.Name).Contains(c.Name));
                    if (column != null)
                        throw new ArgumentException($"Column already exist {column.Name}");
                    var columnTasks = dtoHeader.Columns.Select(c => c.GetDomain(_dataTypeFactory));
                    header.Columns = header.Columns.Concat(await Task.WhenAll(columnTasks)).ToList();
                }
            }
            return definition;
        }
    }
}