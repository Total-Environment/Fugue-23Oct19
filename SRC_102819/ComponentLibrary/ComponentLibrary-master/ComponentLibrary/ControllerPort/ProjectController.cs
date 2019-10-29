using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController"/>
	[ValidateModel]
	[RoutePrefix("projects")]
	public class ProjectController : BaseController
	{
		private readonly IProjectRepository _projectRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectController"/> class.
		/// </summary>
		/// <param name="projectRepository">The project repository.</param>
		public ProjectController(IProjectRepository projectRepository)
		{
			_projectRepository = projectRepository;
		}

        /// <summary>
        /// Gets the specified project code.
        /// </summary>
        /// <param name="projectCode">The project code.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get Project Details")]
        [Route("{projectCode}")]
		[HttpGet]
		public async Task<IHttpActionResult> Get(string projectCode)
		{
			try
			{
				var project = await _projectRepository.Find(projectCode);
				var projectDto = new ProjectDto(project);
				return Ok(projectDto);
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
		}

        /// <summary>
        /// Posts the specified project dto.
        /// </summary>
        /// <param name="projectDto">The project dto.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create new Project")]
        [Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> Post(ProjectDto projectDto)
		{
			try
			{
				var project = projectDto.ToProject();
				var result = await _projectRepository.Add(project);
				return Created(project.ProjectCode, new ProjectDto(result));
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
		}

        [ComponentLibraryAuthorize(Permissions = "Get All Projects details")]
        [Route("")]
        [HttpGet]
	    public async Task<IHttpActionResult> GetAll()
	    {
	        return Ok((await _projectRepository.List()).Select(p => new ProjectDto(p)));
	    }
	}
}