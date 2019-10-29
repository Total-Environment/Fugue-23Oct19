using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class MasterDataListApiTests : IClassFixture<IntegrationTestFixture<MasterDataList>>
    {
        private readonly IWebClient<MasterDataList> _client;

        public MasterDataListApiTests(IntegrationTestFixture<MasterDataList> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async void Get_WithId()
        {
            var response = await _client.Get("/master-data/5822bf16d354cb293420ff63");
            response.Body.Name.Should().Be("Country");
        }
    }
}
