using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
	public class ProjectControllerTests
	{
		[Fact]
		public async Task Get_ShouldReturnOkResult_WhenExistingProjectCodeIsPassed()
		{
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Find("0053")).ReturnsAsync(new Project("0053", "The Magic Faraway Tree", "TMFT"));
			ProjectController projectController = new ProjectController(mockProjectRepository.Object);
			var result = await projectController.Get("0053");
			result.Should().BeOfType(typeof(OkNegotiatedContentResult<ProjectDto>));
			((OkNegotiatedContentResult<ProjectDto>)result).Content.ProjectCode.ShouldBeEquivalentTo("0053");
		}

		[Fact]
		public async Task Get_ShouldReturnNotFound_WhenExistingProjectCodeIsPassed()
		{
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Find("0053")).Throws(new ResourceNotFoundException("Not found"));
			ProjectController projectController = new ProjectController(mockProjectRepository.Object);
			var result = await projectController.Get("0053");
			result.Should().BeOfType(typeof(NotFoundResult));
		}

		[Fact]
		public async Task Post_ShouldReturnCreated_WhenValidProjectDtoIsPassed()
		{
			Project project = new Project("0053", "The Magic Faraway Tree", "TMFT");
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Add(It.IsAny<Project>())).ReturnsAsync(project);
			ProjectController projectController = new ProjectController(mockProjectRepository.Object);
			ProjectDto projectDto = new ProjectDto(project);
			var result = await projectController.Post(projectDto);
			result.Should().BeOfType(typeof(CreatedNegotiatedContentResult<ProjectDto>));
			((CreatedNegotiatedContentResult<ProjectDto>)result).Content.ProjectCode.ShouldBeEquivalentTo("0053");
		}

		[Fact]
		public async Task Post_ShouldReturnBadRequest_WhenDuplicateProjectDtoIsPassed()
		{
			Mock<IProjectRepository> mockProjectRepository = new Mock<IProjectRepository>();
			mockProjectRepository.Setup(m => m.Add(It.IsAny<Project>())).Throws(new DuplicateResourceException("Duplicate"));
			ProjectController projectController = new ProjectController(mockProjectRepository.Object);
			Project project = new Project("0053", "The Magic Faraway Tree", "TMFT");
			ProjectDto projectDto = new ProjectDto(project);
			var result = await projectController.Post(projectDto);
			result.Should().BeOfType(typeof(BadRequestErrorMessageResult));
			((BadRequestErrorMessageResult)result).Message.ShouldBeEquivalentTo("Duplicate");
		}

	    [Fact]
	    public async Task GetAll_ShouldReturnListOfProjects_WhenCalled()
	    {
            var mockProjectRepository = new Mock<IProjectRepository>();
	        var projects = new List<Project> {new Project("123", "Pursuit of a Radical Rhapsody", "POARR")};
	        mockProjectRepository.Setup(r => r.List()).ReturnsAsync(projects);
            var controller = new ProjectController(mockProjectRepository.Object);
	        var result = await controller.GetAll();
	        result.Should().BeOfType<OkNegotiatedContentResult<IEnumerable<ProjectDto>>>();
            ((OkNegotiatedContentResult<IEnumerable<ProjectDto>>)result).Content.Should().HaveCount(1);
	    }
	}
}