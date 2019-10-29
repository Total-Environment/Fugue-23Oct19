using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public partial class ExternalMaterialApiTests : IClassFixture<IntegrationTestFixture<Dictionary<string, object>>>
    {
        public readonly string _endPoint;
        public readonly IntegrationTestFixture<Dictionary<string, object>> _fixture;
        public IWebClient<Dictionary<string, object>> _client;
        private MaterialDataDto _material;

        public ExternalMaterialApiTests(IntegrationTestFixture<Dictionary<string, object>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _endPoint = "materials";
        }

        [Fact]
        public async void Search_ShouldAcceptRequest_WhenMaterialLevelIsNotSent()
        {
            Setup("AnotherMaterial.json");
            await _client.Post(_material, _endPoint);

            var response = await _client.Get("/external/materials?searchKeyword=Liv Roo&pageNumber=1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((long)result["totalPages"]).Should().Be(1);
            ((long)result["batchSize"]).Should().Be(2);
            ((JArray)result["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldBeCaseInsensitive_WhenMaterialLevelIsSent()
        {
            Setup("AnotherMaterial.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get(
                    "/external/materials?materialLevel2=fenestration&searchKeyword=Liv Roo&pageNumber=1&sortColumn=material Name&sortOrder=Ascending");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((JArray)result["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindClayMaterials_WhenIdIsPassed()
        {
            Setup("Material.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get("/external/materials?materialLevel2=Clay Material&searchKeyword=TST000001&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindValidClayMaterials_WhenRelevantSearchKeywordsArePassed()
        {
            Setup("Material.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get("/external/materials?materialLevel2=Clay Material&searchKeyword=topmost flo&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindValidClayMaterials_WhenRelevantSearchPhrasesArePassedAsKeywords()
        {
            Setup("AnotherMaterial.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get("/external/materials?materialLevel2=Fenestration&searchKeyword=Liv Roo&pageNumber=1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((JArray)result["items"]).Count.Should().Be(1);
        }

        public void Setup(string seedDataJson)
        {
            _fixture.DropCollection("materials");
            _material = RequestBuilder<MaterialDataDto>.BuildPostRequest(seedDataJson);
        }
    }
}
