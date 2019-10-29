using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.RestWebClient;
using TE.ComponentLibrary.TestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class MaterialSearchInGroupApiTests : IClassFixture<IntegrationTestFixture<List<string>>>
    {
        private readonly IWebClient<List<string>> _client;
        private readonly IntegrationTestFixture<List<string>> _fixture;

        public MaterialSearchInGroupApiTests(IntegrationTestFixture<List<string>> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async void SearchInGroup_ShouldReturnMaterialGroup_WhenKeywordsArePassed()
        {
            var materialDataDto = RequestBuilder<MaterialDataDto>.BuildPostRequest("MaterialDto.json");
            try
            {
                await _client.Post(materialDataDto, "/materials");
            }
            catch (Exception)
            {
                // ignored
            }

            var response = await _client.Get("materials/group/SME020001");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Body.Count.Should().Be(1);
            response.Body[0].Should().Be("Some Material");
        }
    }
}
