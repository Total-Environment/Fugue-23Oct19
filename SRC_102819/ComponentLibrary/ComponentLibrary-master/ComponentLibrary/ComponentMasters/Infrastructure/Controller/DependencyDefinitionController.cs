using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
	/// <summary>
	/// Represents controller for dependency definition.
	/// </summary>
	/// <seealso cref="BaseController"/>
	[RoutePrefix("dependency")]
	public class DependencyDefinitionController : BaseController
	{
		private readonly IDependencyDefinitionRepository _dependencyDefinitionRepo;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyDefinitionController"/> class.
		/// </summary>
		/// <param name="dependencyDefinitionRepo">The dependency definition repo.</param>
		public DependencyDefinitionController(IDependencyDefinitionRepository dependencyDefinitionRepo)
		{
			_dependencyDefinitionRepo = dependencyDefinitionRepo;
		}

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Returns the created dependency definition</returns>
        [ComponentLibraryAuthorize(Permissions = "Create Dependency Definition")]
        [HttpPost]
		[Route("")]
		public async Task<IHttpActionResult> Create([FromBody] DependencyDefinitionDto request)
		{
			try
			{
				var domain = request.Domain();
				await _dependencyDefinitionRepo.CreateDependencyDefinition(domain);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (DuplicateResourceException)
			{
				return Conflict();
			}
			return Created("", request);
		}

        /// <summary>
        /// Updates the specified request.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="request">The request.</param>
        /// <returns>Returns the updated dependency definition</returns>
        [ComponentLibraryAuthorize(Permissions = "Update Dependency Definition")]
        [HttpPut]
		[Route("{name}")]
		public async Task<IHttpActionResult> Update(string name, [FromBody] DependencyDefinitionDto request)
		{
			try
			{
				var domain = request.Domain();
				await _dependencyDefinitionRepo.UpdateDependencyDefinition(domain);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}

			return Ok(request);
		}

        /// <summary>
        /// Patches the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Patch Dependency Definition")]
        [HttpPatch]
	    [Route("{name}")]
	    public async Task<IHttpActionResult> Patch(string name, [FromBody] DependentBlockDto request)
	    {
	        try
	        {
	            var domain = request.Domain();
	            var previousDependencyDefinition = await _dependencyDefinitionRepo.GetDependencyDefinition(name);
	            var columnsList = previousDependencyDefinition.ColumnList;
                var mergedDependencyDefinition = Merge(domain, previousDependencyDefinition);
	            await _dependencyDefinitionRepo.PatchDependencyDefinition(mergedDependencyDefinition,columnsList,name);
	        }
	        catch (ArgumentException ex)
	        {
	            return BadRequest(ex.Message);
	        }

	        return Ok(request);
	    }

	    private IEnumerable<DependentBlock> Merge(DependentBlock domain, DependencyDefinition previousDependencyDefinition)
	    {
	        var noOfColumns = previousDependencyDefinition.ColumnList.Count;
	        if (domain.Data.Length != noOfColumns)
	        {
	            throw new ArgumentException($"Block length should be {noOfColumns} for {previousDependencyDefinition.Name}");
	        }
	        var blocks = previousDependencyDefinition.Blocks;
	        var dependentBlocks = blocks as IList<DependentBlock> ?? blocks.ToList();
	        var isExist = dependentBlocks.Any(domain.Equals);
            if (isExist) throw new ArgumentException("Dependency block already exists");
	        dependentBlocks.Add(domain);
	        return dependentBlocks;
	    }


        /// <summary>
        /// Gets the specified dependency definition by its name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="compressed">if set to <c>true</c> [compressed].</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Dependency Definition")]
        [Route("{name}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string name, [FromUri]bool compressed = false)
		{
			DependencyDefinition dependencyDefinition;
			try
			{
				dependencyDefinition = await _dependencyDefinitionRepo.GetDependencyDefinition(name);
			}
			catch (ResourceNotFoundException)
			{
				return NotFound();
			}

			if (compressed)
			{
				var response = new DependencyDefinitionCompressedDto(dependencyDefinition);
				return Ok(response);
			}
			else
			{
				var response = new DependencyDefinitionDto(dependencyDefinition);
				return Ok(response);
			}
		}
	}
}