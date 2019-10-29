using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class SemiFinishedGoodApiTests : IClassFixture<IntegrationTestFixture<Dictionary<string, object>>>
    {
        private readonly IWebClient<Dictionary<string, object>> _client;

        public SemiFinishedGoodApiTests(IntegrationTestFixture<Dictionary<string, object>> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async void GetCost_ShouldComputeCostOfSfg_Return200OKReponse()
        {
            var response = await _client.Get($"sfgs/FLY400001/cost?location=Hyderabad&appliedOn=2025-05-27T18:30:00Z");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}