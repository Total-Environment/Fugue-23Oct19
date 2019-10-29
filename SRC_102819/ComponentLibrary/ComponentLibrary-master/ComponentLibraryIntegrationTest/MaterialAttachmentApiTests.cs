using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class MaterialAttachmentApiTests : IClassFixture<IntegrationTestFixture<ListDto<MaterialDocumentDto>>>
    {
        private readonly IWebClient<ListDto<MaterialDocumentDto>> _client;
        private readonly IntegrationTestFixture<ListDto<MaterialDocumentDto>> _fixture;

        public MaterialAttachmentApiTests(IntegrationTestFixture<ListDto<MaterialDocumentDto>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void GetMaterialsInGroupHavingAttachmentColumn_ShouldReturnListOfMaterial_WhenGroupAndColumnNameArePassed()
        {
            var materialDataDto = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialWithStaticFile.json");
            await _client.Post(materialDataDto, "/materials");

            var response =
                await _client.Get(
                    "/materials/documents?materialGroup=New Material&columnName=general_po_terms_conditions&batchSize=10&pageNumber=1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().NotBeNull();
            response.Body.Items.Count.Should().Be(1);
        }

        [Fact]
        public async void GetMaterialInGroupHavingAttachmentColumn_ShouldReturnListOfMaterial_WhenGroupColumnNameAndKeywordsArePassed()
        {
            var materialDataDto = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialWithStaticFile.json");
            await _client.Post(materialDataDto, "/materials");

            var response =
                await _client.Get(
                    "/materials/documents?materialGroup=New Material&columnName=general_po_terms_conditions&batchSize=10&pageNumber=1&keywords=Description");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Should().NotBeNull();
            response.Body.Items.Count.Should().Be(1);
        }
    }
}
