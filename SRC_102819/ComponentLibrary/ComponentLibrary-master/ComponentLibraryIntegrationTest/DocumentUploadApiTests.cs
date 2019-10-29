using System.Net;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.RestWebClient;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.IntegrationTests
{
    public class DocumentUploadApiTests : IClassFixture<IntegrationTestFixture<StaticFile>>
    {
        private readonly IWebClient<StaticFile> _client; 
        public DocumentUploadApiTests(IntegrationTestFixture<StaticFile> fixture)
        {
            _client = fixture.Client;
        }
        [Fact]
        public async void Post_ShouldReturnCreatedWithStaticFile_WhenGivenAValidFile()
        {
            var response =
                await _client.PostFile(
                    ".\\RequestImages\\requestImage.jpg",
                    "/upload/static-files");
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
