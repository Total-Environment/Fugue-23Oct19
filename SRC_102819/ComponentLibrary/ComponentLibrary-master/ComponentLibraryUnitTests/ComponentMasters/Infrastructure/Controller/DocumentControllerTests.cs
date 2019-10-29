using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.Shared.CloudServiceFramework.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Controller
{
    public class DocumentControllerTests
    {

        [Fact]
        public void DocumentController_Should_ImplementAPIController()
        {
            var controller = new DocumentController(new Mock<IDocumentRepository>().Object);

            controller.Should().BeAssignableTo<ApiController>();

        }

        [Fact]
        public async void Create_ShouldThrowUnSupportedMediaType_WhenMultipartDataDonotExistInRequest()
        {
            var controller = new Fixture().RequestShouldNotHaveMultipartData();

            var result =  await controller.Sut().Create();

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldCallDocumentRepoWithSomeFileNameAndStream_WhenMultipartContentIsPresent()
        {
            var fixture = new Fixture().RequestShouldHaveMultiPartData().ShouldCallDocumentRepository();

            await fixture.Sut().Create();
            
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Create_ShouldCallDocumentRepositoryWithFileName_WhenFileNameIsRecievedFromRequest()
        {
            var fixture =
                new Fixture().RequestShouldHaveMultiPartDataWithFileName("FileName.pdf")
                    .DocumentShouldBeCalled();

            await fixture.Sut().Create();

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Create_ShouldReturnStatusCodeWith502_WhenUploadThrowsBlobUploads()
        {
            var fixture = new Fixture().RequestShouldHaveMultiPartDataWithFileName("some.pdf").UploadOfDocumentRepositoryThrowsBlobUploadException();

            var result = await fixture.Sut().Create();

            result.Should().BeOfType<StatusCodeResult>();
            var statusCode = (result as StatusCodeResult).StatusCode;
            statusCode.Should().Be(HttpStatusCode.BadGateway);
        }

        [Fact]
        public async void Create_ShouldThrowBadRequest_WhenFileIsNotImageOrPdf()
        {
            var fixture = new Fixture().RequestShouldHaveMultiPartDataWithFileName("Ramukaka.xlsx");

            var result = await fixture.Sut().Create();

            result.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Fact]
        public async void Create_ShouldReturnCreatedWithStaticFile_WhenRepositoryReturnStaticFile()
        {
            var staticFile = new StaticFile("12345","someone.pdf");
            var fixture = new Fixture().RequestShouldHaveMultiPartDataWithFileName("someone.pdf").DocumentRepositoryUploadReturnsStaticFile(staticFile);

            var result = await fixture.Sut().Create();

            result.Should().BeOfType<CreatedNegotiatedContentResult<List<StaticFile>>>();
            var resultFile = (result as CreatedNegotiatedContentResult<List<StaticFile>>).Content;
            resultFile.First().Should().Be(staticFile);
        }

        private class Fixture
        {
            private HttpControllerContext controllerContext = null;
            private List<Action> _expectations = new List<Action>();
            private Mock<IDocumentRepository> _mockDocumentRepo = new Mock<IDocumentRepository>();

            public DocumentController Sut()
            {
                var documentController = new DocumentController(_mockDocumentRepo.Object);
                documentController.ControllerContext = controllerContext;
                return documentController;
            }

            public Fixture RequestShouldNotHaveMultipartData()
            {
                var content = new Mock<HttpContent>();
                CreateControllerContext(content.Object);
                return this;
            }

            private void CreateControllerContext(HttpContent content)
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Content = content
                };
                CreateControllerContext(httpRequestMessage);
            }

            private void CreateControllerContext(HttpRequestMessage httpRequestMessage)
            {
                controllerContext = new HttpControllerContext
                {
                    Request = httpRequestMessage
                };
            }


            public Fixture RequestShouldHaveMultiPartData()
            {
                const string testFile = "test.pdf";
                var request = CreateMultiPartRequest(testFile);

                CreateControllerContext(request);
                return this;
            }

            private static HttpRequestMessage CreateMultiPartRequest(string testFile)
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("http://tw.com"),
                    Method = HttpMethod.Post,
                    Content = MultipartFormDataContent(testFile)
                };
                return request;
            }

            public Fixture ShouldCallDocumentRepository()
            {
                _expectations.Add(()=>_mockDocumentRepo.Verify(m => m.Upload(It.IsAny<string>(), It.IsAny<Stream>())));
                return this;
            }

            public void VerifyExpectations()
            {
                _expectations.ForEach(e=>e.Invoke());
            }

            private static MultipartFormDataContent MultipartFormDataContent(string testFile)
            {
                var multiPartContent =
                        new MultipartFormDataContent("boundary=---011000010111000001101001");
                var streamContent = new StreamContent(new MemoryStream(new byte[0]));
                streamContent.Headers.ContentType =
                                           new MediaTypeHeaderValue("multipart/form-data");

                multiPartContent.Add(streamContent, "TheFormDataKeyForYourFile", testFile);

                return multiPartContent;
            }

            public Fixture RequestShouldHaveMultiPartDataWithFileName(string filename)
            {
                CreateControllerContext(CreateMultiPartRequest(filename));
                return this;
            }

            public Fixture DocumentShouldBeCalled()
            {
                _expectations.Add(()=>_mockDocumentRepo.Verify(m=>m.Upload(It.IsAny<string>(),It.IsAny<Stream>())));
                return this;
            }

            public Fixture UploadOfDocumentRepositoryThrowsBlobUploadException()
            {
                _mockDocumentRepo.Setup(d => d.Upload(It.IsAny<string>(), It.IsAny<Stream>()))
                    .ThrowsAsync(new BlobUploadException("", new Exception()));
                return this;
            }

            public Fixture DocumentRepositoryUploadReturnsStaticFile(StaticFile staticFile)
            {
                _mockDocumentRepo.Setup(d => d.Upload(It.IsAny<string>(), It.IsAny<Stream>())).ReturnsAsync(staticFile);
                return this;
            }
        }
    }
}
