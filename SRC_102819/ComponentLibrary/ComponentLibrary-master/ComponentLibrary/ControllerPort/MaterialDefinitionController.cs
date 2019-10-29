using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// Represents the material definitions.
	/// </summary>
	/// <seealso cref="BaseController"/>
	[RoutePrefix("material-definitions")]
	public class MaterialDefinitionController : BaseController
	{
		private readonly IDataTypeFactory _dataTypeFactory;
		private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
		private readonly IComponentDefinitionRepository<IMaterialDefinition> _materialDefinitionRepository;
		private readonly ICodePrefixTypeMappingRepository _codePrefixTypeMappingRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialDefinitionController"/> class.
		/// </summary>
		/// <param name="materialDefinitionRepository">The material definition repository.</param>
		/// <param name="dataTypeFactory">The data type factory.</param>
		/// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
		/// <param name="codePrefixTypeMappingRepository">The code prefix type mapping repository.</param>
		public MaterialDefinitionController(
			IComponentDefinitionRepository<IMaterialDefinition> materialDefinitionRepository,
			IDataTypeFactory dataTypeFactory, IDependencyDefinitionRepository dependencyDefinitionRepository,
			ICodePrefixTypeMappingRepository codePrefixTypeMappingRepository)
		{
			_materialDefinitionRepository = materialDefinitionRepository;
			_dependencyDefinitionRepository = dependencyDefinitionRepository;
			_codePrefixTypeMappingRepository = codePrefixTypeMappingRepository;
			_dataTypeFactory = dataTypeFactory;
		}

		/// <summary>
		/// Gets the specified group name.
		/// </summary>
		/// <param name="groupName">Name of the group.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "View Material Definition")]
		[Route("{groupName}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string groupName)
		{
			IMaterialDefinition materialDefinition;
			try
			{
				materialDefinition = await _materialDefinitionRepository.Find(groupName);
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}
			return Ok(new MaterialDefinitionDao(materialDefinition));
		}

		/// <summary>
		/// Patches the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Patch Material Definition")]
		[Route("")]
		[HttpPatch]
		public async Task<IHttpActionResult> Patch([FromBody] MaterialDefinitionDao input)
		{
			try
			{
				var materialDefinition = await _materialDefinitionRepository.Find(input.Name);
				var patch = await Merge(input, materialDefinition);

				var isMatching = await CheckForHeaderColumnMapping(patch);

				if (!isMatching.Item1)
				{
					var exception = new Exception(isMatching.Item2);
					LogToElmah(exception);
					return BadRequest(isMatching.Item2);
				}

				await _materialDefinitionRepository.Update(patch);
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
			return Ok();
		}

		/// <summary>
		/// Posts the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		[ComponentLibraryAuthorize(Permissions = "Create Material Definition")]
		[Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> Post([FromBody] MaterialDefinitionDao input)
		{
			try
			{
				var domain = await input.GetDomain(_dataTypeFactory, _dependencyDefinitionRepository);
				var isMatching = await CheckForHeaderColumnMapping(domain);

				if (!isMatching.Item1)
					return BadRequest(isMatching.Item2);

				await _materialDefinitionRepository.Add(domain);
				await _codePrefixTypeMappingRepository.Add(new CodePrefixTypeMapping(input.Code, ComponentType.Material));
				var material = await _materialDefinitionRepository.Find(input.Name);

				return Created("", new MaterialDefinitionDao(material));
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return BadRequest(e.Message);
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
			if (input.Name == "Generic Material") return new Tuple<bool, string>(true, null);
			var genericMaterial = await _materialDefinitionRepository.Find("Generic Material");
			var invalidColumns = new StringBuilder();
			foreach (var header in input.Headers)
			{
				var headerName = header.Name;
				foreach (var column in header.Columns)
				{
					var columnName = column.Name;
					var columnCheck =
						genericMaterial.Headers.Any(
							h => h.Name == headerName && h.Columns.Any(c => c.Name == columnName));

					if (columnCheck == false)
						invalidColumns.AppendLine($"Header-Column pair {headerName}:{columnName} is invalid.");
				}
			}
			return new Tuple<bool, string>(invalidColumns.Length == 0, invalidColumns.ToString());
		}

		private async Task<IMaterialDefinition> Merge(MaterialDefinitionDao dto, IMaterialDefinition definition)
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
	}
}