using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class MaterialDefinitionApiTests : IClassFixture<IntegrationTestFixture<MaterialDefinitionDao>>
    {
        public MaterialDefinitionApiTests(IntegrationTestFixture<MaterialDefinitionDao> fixture)
        {
            _testFixture = fixture;
            _client = fixture.Client;
        }

        private readonly IWebClient<MaterialDefinitionDao> _client;
        private readonly IntegrationTestFixture<MaterialDefinitionDao> _testFixture;

        private class Fixture
        {
            private readonly IWebClient<MaterialDefinitionDao> _client;

            public Fixture(IWebClient<MaterialDefinitionDao> _client)
            {
                this._client = _client;
            }

            public async Task<Fixture> EnsureThatMaterialExist(string materialdefinitionJson)
            {
                var materialDefinition =
                    RequestBuilder<MaterialDefinitionDao>.BuildPostRequest(materialdefinitionJson);
                var getResponse = await _client.Get($"/material-definitions/{materialDefinition.Name}");
                if (getResponse.StatusCode.Equals(HttpStatusCode.OK))
                    await AddMaterial(materialDefinition);
                return this;
            }

            private async Task<Fixture> AddMaterial(MaterialDefinitionDao columnDefinition)
            {
                var response = await _client.Post(columnDefinition, "/material-definitions");
                return this;
            }
        }

        [Fact]
        public async void Patch_ShouldReturnBadRequest_WhenAlreadyExistingColumnIsPassed()
        {
            _testFixture.ResetDatabase();
            var fixture = new Fixture(_client);
            await fixture.EnsureThatMaterialExist("MaterialDefinition.json");

            var request = RequestBuilder<MaterialDefinitionDao>.BuildPostRequest("MaterialDefinition.json");

            await _client.Patch(request, "/material-definitions");
        }

        [Fact]
        public async void Patch_ShouldReturnNotFound_WhenMaterialISSendThatDontExistSend()
        {
            var request = new MaterialDefinitionDao
                {Code = "SomeCode", Name = "name", Headers = new List<HeaderDefinitionDto>()};

            var response = await _client.Patch(request, "/material-definitions");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Post_WithValidMaterialDefinition_Returns201CreatedWithMaterial()
        {
            var materialDefinition = RequestBuilder<MaterialDefinitionDao>.BuildPostRequest("MaterialDefinition.json");

            var response = await _client.Post(materialDefinition, "/material-definitions");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Name.Should().NotBeNull();
        }

        [Fact]
        public async void Post_WithMaterialDefinitionButDoesNotMatchWithColumnMapping_Returns400BadRequest()
        {
            var materialDefinition = RequestBuilder<MaterialDefinitionDao>.BuildPostRequest("MaterialDefinitionHavingMisMatchColumnMapping.json");

            var response = await _client.Post(materialDefinition, "/material-definitions");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}