using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class ServiceClassificationDefinitionApiTests :
        IClassFixture<IntegrationTestFixture<Dictionary<string, Dictionary<string, string>>>>
    {
        private readonly IWebClient<Dictionary<string, Dictionary<string, string>>> _client;
        private readonly IntegrationTestFixture<Dictionary<string, Dictionary<string, string>>> _fixture;

        public ServiceClassificationDefinitionApiTests(IntegrationTestFixture<Dictionary<string, Dictionary<string, string>>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void Post_WithValidServiceClassificationDefinition_Returns201CreatedWithService()
        {
            var serviceClassificationDefinition =
                RequestBuilder<Dictionary<string, Dictionary<string, string>>>.BuildPostRequest("ServiceClassificationDefinition.json");
            var response = await _client.Post(serviceClassificationDefinition, "/classification-definitions");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Body.Count.Should().Be(1);
        }
    }
}