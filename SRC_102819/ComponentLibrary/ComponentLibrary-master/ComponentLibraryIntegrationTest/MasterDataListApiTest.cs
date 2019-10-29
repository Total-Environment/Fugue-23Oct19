using ComponentLibrary.MaterialMasters.Domain;
using FluentAssertions;
using RestWebClient;
using Xunit;

namespace ComponentLibraryIntegrationTest
{
    public class MasterDataListApiTest : IClassFixture<IntegrationTestFixture<MasterDataList>>
    {
        private readonly IWebClient<MasterDataList> _client;

        public MasterDataListApiTest(IntegrationTestFixture<MasterDataList> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async void Get_WithId()
        {
            var response = await _client.Get("/master-data-lists/5822bf16d354cb293420ff63");
            response.Body.Name.Should().Be("Country");
        }
    }
}
