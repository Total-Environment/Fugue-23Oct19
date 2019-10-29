using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class NewMaterialApiTest : IClassFixture<IntegrationTestFixture<MaterialDataDto>>
    {
        private readonly IWebClient<MaterialDataDto> _client;
        private readonly IntegrationTestFixture<MaterialDataDto> _fixture;

        public NewMaterialApiTest(IntegrationTestFixture<MaterialDataDto> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void Post_SearchWithinGroup_ShouldFindValidClayMaterials_WhenRelevantSearchKeywordsAndFiltersArePassed()
        {
            _fixture.DropCollection("materials");
            var material = RequestBuilder<Dictionary<string, object>>.BuildPostRequest("Material.json");
            await _client.Post(material, "/materials");

            var filterDatas = new List<FilterData>
            {
                //                                new FilterData("material_name","Tes"),
                //                                new FilterData("generic","true"),
                //                                new FilterData("po_lead_time_in_days","10"),
                //                                new FilterData("last_purchase_rate","123 INR"),
                //                                new FilterData("length","12.0"),
                //                                new FilterData("approved_vendors","dor1"),
                //                                new FilterData("width","5"),
                //                                new FilterData("height","25")
            };
            var searchWithinGroupRequest = new MaterialSearchRequest
            {
                GroupName = "Clay Material",
                SearchQuery = "topmost flo",
                FilterDatas = filterDatas
            };

            var response = await _client.Post(searchWithinGroupRequest, "/materials/searchwithingroup");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            //((JArray)response.Body["items"]).Count.Should().Be(1);
        }

        [Fact]
        public async void Post_WithValidMaterial_Returns201CreatedWithMaterial()
        {
            _fixture.DropCollection("materials");
            var materialDefinition = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialDto.json");

            var response = await _client.Post(materialDefinition, "/materials");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Get_ShouldReturnMaterial_WhenValidMaterialIdIsPassed()
        {
            var materialDataDto = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialDto.json");
            await _client.Post(materialDataDto, "/materials");

            var response = await _client.Get("/materials/SME020001");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().NotBeNull();
        }

        [Fact]
        public async void Put_ShouldUpdateMaterial_WhenMaterialWithMaterialCodeIsPassed()
        {
            var materialDataDto = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialDto.json");
            await _client.Post(materialDataDto, "/materials");

            materialDataDto.Headers.FirstOrDefault(h => h.Key == "general")
                .Columns.FirstOrDefault(c => c.Key == "material_name")
                .Value = "name";
            var response = await _client.Put("SME020001", materialDataDto, "/materials");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Headers.FirstOrDefault(h => h.Key == "general")
                .Columns.FirstOrDefault(c => c.Key == "material_name")
                .Value.Should().Be("name");
        }
    }
}