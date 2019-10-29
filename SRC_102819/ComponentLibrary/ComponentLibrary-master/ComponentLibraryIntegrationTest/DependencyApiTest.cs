using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class DependencyApiTest : IClassFixture<IntegrationTestFixture<DependencyDefinitionDto>>
    {
        private readonly IntegrationTestFixture<DependencyDefinitionDto> _fixture;
        private readonly IWebClient<DependencyDefinitionDto> _client;

        public DependencyApiTest(IntegrationTestFixture<DependencyDefinitionDto> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task Post_ShouldReturnCreatedDependency_WhenPassedValidDependency()
        {
            var materialDefinition = RequestBuilder<DependencyDefinitionDto>.BuildPostRequest("Dependency.json");

            var response = await _client.Post(materialDefinition, "/dependency");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
            _fixture.ResetDatabase();
        }

        [Fact]
        public async Task Get_ShouldReturnCreatedDependency_WhenCalledWithValidName()
        {
            using (_fixture.SeedDependency())
            {
                var response = await _client.Get("/dependency/classifications");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                response.Body.Should().NotBeNull();
            }
        }
    }
}