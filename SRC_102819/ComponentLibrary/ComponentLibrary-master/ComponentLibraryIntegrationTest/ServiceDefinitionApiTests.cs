using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class ServiceDefinitionApiTests : IClassFixture<IntegrationTestFixture<ServiceDefinitionDao>>
    {
        public ServiceDefinitionApiTests(IntegrationTestFixture<ServiceDefinitionDao> fixture)
        {
            _testFixture = fixture;
            _client = fixture.Client;
        }

        private readonly IWebClient<ServiceDefinitionDao> _client;
        private readonly IntegrationTestFixture<ServiceDefinitionDao> _testFixture;

        private class Fixture
        {
            private readonly IWebClient<ServiceDefinitionDao> _client;

            public Fixture(IWebClient<ServiceDefinitionDao> _client)
            {
                this._client = _client;
            }

            public async Task<Fixture> EnsureThatServiceExist(string servicedefinitionJson)
            {
                var serviceDefinition =
                    RequestBuilder<ServiceDefinitionDao>.BuildPostRequest("ServiceDefinition.json");
                var getResponse = await _client.Get($"/service-definitions/{serviceDefinition.Name}");
                if (getResponse.StatusCode.Equals(HttpStatusCode.OK))
                    await AddService(serviceDefinition);
                return this;
            }

            private async Task<Fixture> AddService(ServiceDefinitionDao serviceDefinition)
            {
                var response = await _client.Post(serviceDefinition, "/service-definitions");
                return this;
            }
        }

        [Fact]
        public async void Patch_ShouldReturnBadRequest_WhenAlreadyExistingColumnIsPassed()
        {
            _testFixture.ResetDatabase();
            var fixture = new Fixture(_client);
            await fixture.EnsureThatServiceExist("ServiceDefinition.json");

            var request = RequestBuilder<ServiceDefinitionDao>.BuildPostRequest("ServiceDefinition.json");

            await _client.Patch(request, "/service-definitions");
        }

        [Fact]
        public async void Patch_ShouldReturnNotFound_WhenServiceISSendThatDontExistSend()
        {
            var request = new ServiceDefinitionDao
                {Code = "SomeCode", Name = "name", Headers = new List<HeaderDefinitionDto>()};

            var response = await _client.Patch(request, "/service-definitions");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Post_WithValidServiceDefinition_Returns201CreatedWithService()
        {
            var serviceDefinition = RequestBuilder<ServiceDefinitionDao>.BuildPostRequest("ServiceDefinition.json");

            var response = await _client.Post(serviceDefinition, "/service-definitions");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Name.Should().NotBeNull();
        }

        [Fact]
        public async void Post_WithServiceDefinitionButDoesNotMatchWithColumnMapping_Returns400BadRequest()
        {
            var serviceDefinition = RequestBuilder<ServiceDefinitionDao>.BuildPostRequest("ServiceDefinitionHavingMisMatchColumnMapping.json");

            var response = await _client.Post(serviceDefinition, "/service-definitions");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}