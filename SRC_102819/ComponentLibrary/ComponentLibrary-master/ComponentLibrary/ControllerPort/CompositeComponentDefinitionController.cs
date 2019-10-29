using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// Semi finished good Definition Controller
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController"/>
	[RoutePrefix("{type:validcompositecomponent}-definitions")]
	public class CompositeComponentDefinitionController : BaseController
	{
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _sfgDefinitionRepository;
		private IDataTypeFactory _dataTypeFactory;
		private IDependencyDefinitionRepository _dependencyDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDefinitionController"/> class.
		/// </summary>
		/// <param name="sfgDefinitionRepository">The SFG definition repository.</param>
		/// <param name="dataTypeFactory">The data type factory.</param>
		/// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
		public CompositeComponentDefinitionController(ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> sfgDefinitionRepository,
			IDataTypeFactory dataTypeFactory, IDependencyDefinitionRepository dependencyDefinitionRepository)
		{
			_sfgDefinitionRepository = sfgDefinitionRepository;
			_dependencyDefinitionRepository = dependencyDefinitionRepository;
			_dataTypeFactory = dataTypeFactory;
		}

        /// <summary>
        /// Posts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create Composite Component Definition")]
        [HttpPost]
		[Route("")]
		public async Task<IHttpActionResult> Post(string type, [FromBody] CompositeComponentDefinitionDao input)
		{
			try
			{
				var sfg = await input.GetDomain(_dataTypeFactory, _dependencyDefinitionRepository);
				await _sfgDefinitionRepository.Add(type, sfg);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest();
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return Conflict();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
			return Ok();
		}

        /// <summary>
        /// Update Definition
        /// </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Patch Composite Component Definition")]
        [HttpPatch]
        [Route("")]
        public async Task<IHttpActionResult> Patch(string type, [FromBody] CompositeComponentDefinitionDao input)
        {
            try
            {
                var name = type == "sfg" ? "Generic SFG" : "Generic Package";
                var existingDefinition = await _sfgDefinitionRepository.Find(type, name);
                var mergedDefinition = await Merge(input, existingDefinition);
                await _sfgDefinitionRepository.Update(type, mergedDefinition);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest();
            }
            catch (InvalidOperationException ex)
            {
                LogToElmah(ex);
                return BadRequest();
            }
            catch (DuplicateResourceException ex)
            {
                LogToElmah(ex);
                return Conflict();
            }
            catch (Exception ex)
            {
                LogToElmah(ex);
                return InternalServerError(ex);
            }
        }

        private async Task<ICompositeComponentDefinition> Merge(CompositeComponentDefinitionDao dto, ICompositeComponentDefinition definition)
        {

            foreach (var dtoHeader in dto.Headers)
            {
                var header = definition.Headers.FirstOrDefault(h => h.Name.Equals(dtoHeader.Name));
                if (header == null)
                {
                    definition.Headers
                        .Add(await dtoHeader.GetDomain(_dataTypeFactory, _dependencyDefinitionRepository));
                }
                else
                {
                    if (dtoHeader.Columns == null || dtoHeader.Columns.Count <= 0)
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

        /// <summary>
        /// Puts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Update Composite Component Definition")]
        [HttpPut]
		[Route("")]
		public async Task<IHttpActionResult> Put(string type, [FromBody] CompositeComponentDefinitionDao input)
		{
			try
			{
				var sfg = await input.GetDomain(_dataTypeFactory, _dependencyDefinitionRepository);
				await _sfgDefinitionRepository.Update(type, sfg);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
			return Ok();
		}

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Composite Component Definition")]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get(string type)
        {
            try
            {
                var name = type == "sfg" ? "Generic SFG" : "Generic Package";
                var sfgDefinition = await _sfgDefinitionRepository.Find(type, name);
                return Ok(new CompositeComponentDefinitionDao(sfgDefinition));
            }
            catch (Exception ex) // We should always have SFG definition in db, so there's no business case for this to happen
            {
                LogToElmah(ex);
                return InternalServerError(ex);
            }
        }
    }
}