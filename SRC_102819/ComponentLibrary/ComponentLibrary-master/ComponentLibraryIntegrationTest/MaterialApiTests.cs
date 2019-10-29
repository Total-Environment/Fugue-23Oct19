using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public partial class MaterialApiTests : IClassFixture<IntegrationTestFixture<Dictionary<string, object>>>
    {
        public readonly string _endPoint;
        public readonly IntegrationTestFixture<Dictionary<string, object>> _fixture;
        public IWebClient<Dictionary<string, object>> _client;
        private MaterialDataDto _material;

        public MaterialApiTests(IntegrationTestFixture<Dictionary<string, object>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _endPoint = "materials";
        }

        [Fact]
        public async void GetByGroupAndColumnName_ShouldFindValidMaterials()
        {
            Setup("Material.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get("/materials/documents?materialGroup=Clay Material&columnName=image&pageNumber=1&batchSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void GetByGroupAndColumnNameAndKeyWord_ShouldFindValidMaterials()
        {
            Setup("Material.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get(
                    "/materials/documents?materialGroup=Clay Material&columnName=image&keyWord=Test&pageNumber=1&batchSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void GroupSearch_ShouldReturnGroups_WhenPassedSearchQuery()
        {
            Setup("AnotherMaterial.json");
            await _client.Post(_material, _endPoint);

            var groupSearchClient = new RestClient<List<string>>(_fixture.Server.GetClient());
            var response = await groupSearchClient.Get("/materials/group/primary");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = response.Body;
            result.ShouldAllBeEquivalentTo(new List<string> { "Fenestration" });
        }

        [Fact]
        public async void
                Post_SearchWithinGroup_ShouldFindValidClayMaterials_WhenRelevantSearchKeywordsAndFiltersArePassed()
        {
            Setup("Material.json");
            await _client.Post(_material, _endPoint);

            var filterDatas = new List<FilterData>
                {
                    new FilterData("material_name", "Tes"),
                    new FilterData("generic", "true"),
                    new FilterData("po_lead_time_in_days", "10"),
                    new FilterData("last_purchase_rate", "123 INR"),
                    new FilterData("length", "12.0"),
                    new FilterData("approved_vendors", "dor1"),
                    new FilterData("width", "5"),
                    new FilterData("height", "25")
                };
            var searchWithinGroupRequest = new MaterialSearchRequest
            {
                GroupName = "Clay Material",
                SearchQuery = "topmost flo",
                FilterDatas = filterDatas
            };

            var response = await _client.Post(searchWithinGroupRequest, "/materials/searchwithingroup");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Post_WithValidMaterial_Returns201CreatedWithMaterial()
        {
            Setup("Material.json");

            var response = await _client.Post(_material, _endPoint);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Post_WithValidMaterialWhichDoesNotHavematerialCode_Returns201CreatedWithMaterial()
        {
            Setup("MaterialWithoutCode.json");
            var response = await _client.Post(_material, _endPoint);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Search_ShouldAcceptRequest_WhenMaterialLevelIsNotSent()
        {
            Setup("AnotherMaterial.json");
            await _client.Post(_material, _endPoint);

            var response = await _client.Get("/materials-old?searchKeyword=Liv Roo&pageNumber=1");
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
                    "/materials-old?materialLevel2=fenestration&searchKeyword=Liv Roo&pageNumber=1&sortColumn=material Name&sortOrder=Ascending");
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
                await _client.Get("/materials-old?materialLevel2=Clay Material&searchKeyword=TST000001&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindValidClayMaterials_WhenRelevantSearchKeywordsArePassed()
        {
            Setup("Material.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get("/materials-old?materialLevel2=Clay Material&searchKeyword=topmost flo&pageNumber=1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Search_ShouldFindValidClayMaterials_WhenRelevantSearchPhrasesArePassedAsKeywords()
        {
            Setup("AnotherMaterial.json");
            await _client.Post(_material, _endPoint);

            var response =
                await _client.Get("/materials-old?materialLevel2=Fenestration&searchKeyword=Liv Roo&pageNumber=1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = response.Body;

            ((long)result["recordCount"]).Should().Be(1);
            ((JArray)result["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void GetAllRates_ForNoSearchRequest_ReturnBadRequestStatusCode()
        {
            var response = await _client.Post(null, "/materials/rates");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void GetAllRates_ForASearchRequestWithoutAppliedOnDate_ReturnBadRequestStatusCode()
        {
            var filters = new List<FilterData>
            {
                new FilterData("AppliedOn",null)
            };
            var response = await _client.Post(filters, "/materials/rates");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async void GetAllRates_ForAValidSearchRequest_ReturnStatusOKWithPaginatedListOfRates()
        {
            var filters = new List<FilterData>
            {
                new FilterData("AppliedOn",DateTime.Now.ToString("o"))
            };
            var response = await _client.PostWithoutParse(filters, "/materials/rates");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void Setup(string seedDataJson)
        {
            _fixture.DropCollection("materials");
            _material = RequestBuilder<MaterialDataDto>.BuildPostRequest(seedDataJson);
        }
    }
}