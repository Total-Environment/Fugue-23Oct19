using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class MaterialListApiTests : IClassFixture<IntegrationTestFixture<ListDto<MaterialDataDto>>>
    {
        private readonly IWebClient<ListDto<MaterialDataDto>> _client;
        private readonly IntegrationTestFixture<ListDto<MaterialDataDto>> _fixture;

        public MaterialListApiTests(IntegrationTestFixture<ListDto<MaterialDataDto>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void GetRecentMaterial_ShouldReturnRecentMaterial()
        {
            var materialDataDto = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialDto.json");
            await _client.Post(materialDataDto, "/materials");

            var response = await _client.Get("/materials/all");
            response.Body.Items.Count.Should().Be(1);
        }
    }
}
