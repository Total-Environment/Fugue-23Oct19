using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class BrandDefinitionApiTests : IClassFixture<IntegrationTestFixture<Dictionary<string, object>>>
    {
        private readonly IntegrationTestFixture<Dictionary<string, object>> _fixture;
        private readonly IWebClient<Dictionary<string, object>> _client;

        public BrandDefinitionApiTests(IntegrationTestFixture<Dictionary<string, object>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void Add_WithValidBrand_Returns201CreatedWithBrand()
        {
            _fixture.DropCollection("brandDefinitions");
            var brandDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Brand.json");

            var response = await _client.Post(brandDefinition, "/definitions/brands");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Add_WithBrandDefinitionWithInvalidData_Returns400BadRequest()
        {
            var brandDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("InvalidBrandDefinition.json");

            var response = await _client.Post(brandDefinition, "/definitions/brands");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async void Add_WithBrandDefinitionWhichIsAlreadyExisting_Returns409Conflict()
        {
            var brandDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Brand.json");
            await _client.Post(brandDefinition, "/definitions/brands");

            var duplicateBrandDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Brand.json");
            var response = await _client.Post(duplicateBrandDefinition, "/definitions/brands");

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async void Get_WhenBrandDefinitionExists_Returns200OKWithBrandDefinition()
        {
            _fixture.DropCollection("brandDefinitions");
            var brandDefinition = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Brand.json");
            await _client.Post(brandDefinition, "/definitions/brands");

            var response = await _client.Get("/definitions/brands");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().NotBeNull();
        }

        /// <summary>
        /// Gets the when brand definition does not exist returns404 not found.
        /// </summary>
        [Fact]
        public async void Get_WhenBrandDefinitionDoesNotExist_Returns404NotFound()
        {
            _fixture.DropCollection("brandDefinitions");

            var response = await _client.Get("/definitions/brands");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}