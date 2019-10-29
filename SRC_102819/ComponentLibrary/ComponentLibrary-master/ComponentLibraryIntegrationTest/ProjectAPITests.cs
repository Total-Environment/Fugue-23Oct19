using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
	public class ProjectAPITests : IClassFixture<IntegrationTestFixture<ProjectDto>>
	{
		private readonly IWebClient<ProjectDto> _client;
		private readonly IntegrationTestFixture<ProjectDto> _fixture;

		public ProjectAPITests(IntegrationTestFixture<ProjectDto> fixture)
		{
			_fixture = fixture;
			_client = fixture.Client;
		}

		[Fact]
		public async Task CreateProject_ShouldCreateProject()
		{
			_fixture.DropCollection("projects");
			ProjectDto projectDto = new ProjectDto { ProjectCode = "0053", ProjectName = "The Magic Faraway Tree", ShortName = "TMFT" };
			var response = await _client.Post(projectDto, "/projects");
			response.StatusCode.Should().Be(HttpStatusCode.Created);
			response.Body.Should().NotBeNull();
		}
	}
}