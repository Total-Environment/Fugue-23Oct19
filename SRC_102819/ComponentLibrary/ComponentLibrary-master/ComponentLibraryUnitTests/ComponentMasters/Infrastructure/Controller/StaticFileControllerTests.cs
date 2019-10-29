using System.IO;
using System.Net;
using System.Text;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.Shared.CloudServiceFramework.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Controller
{
    public class StaticFileControllerTests
    {
        [Fact]
        public async void GetStaticFile_ShouldForAValidStaticFileId_ReturnOkStatusCodeWithFileContents()
        {
            var mockStaticFileRepository = new Mock<IStaticFileRepository>();
            mockStaticFileRepository.Setup(r => r.GetById(It.IsAny<string>()))
                .ReturnsAsync(new StaticFile("qweret", "image.jpg"));
            var mockBlobDownloadService = new Mock<IBlobDownloadService>();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Static File Contents"));
            mockBlobDownloadService.Setup(s => s.Download(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(memoryStream);

            var staticFileController = new StaticFileController(mockStaticFileRepository.Object, mockBlobDownloadService.Object);

            var staticFileId = "qweret";
            var result = await staticFileController.GetStaticFile(staticFileId) as ResponseMessageResult;

            result.Response.Content.Should().NotBeNull();
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Get_ShouldForStaticIdThatIsNotPresentInMongoDatabase_ReturnNotFoundStatusCode()
        {
            var mockStaticFileRepository = new Mock<IStaticFileRepository>();
            mockStaticFileRepository.Setup(r => r.GetById(It.IsAny<string>()))
                .ThrowsAsync(new ResourceNotFoundException(""));
            var mockBlobDownloadService = new Mock<IBlobDownloadService>();

            var staticFileController = new StaticFileController(mockStaticFileRepository.Object, mockBlobDownloadService.Object);

            var staticFileId = "2e3d-44rrf-qqwp";
            (await staticFileController.GetStaticFile(staticFileId)).Should()
                .BeOfType<NotFoundResult>();
        }

        [Fact]
        public async void Post_ShouldAddAValidStaticFile_ReturnAddedStaticFile()
        {
            var mockStaticFileRepository = new Mock<IStaticFileRepository>();
            var staticFile = new StaticFile("qweret", "image.jpg");
            mockStaticFileRepository.Setup(r => r.Add(It.IsAny<StaticFile>()))
                .ReturnsAsync(staticFile);
            var mockBlobDownloadService = new Mock<IBlobDownloadService>();
            var staticFileController = new StaticFileController(mockStaticFileRepository.Object, mockBlobDownloadService.Object);

            var result = await staticFileController.Post(staticFile) as CreatedNegotiatedContentResult<StaticFile>;

            result.Content.Should().Be(staticFile);
        }

        [Fact]
        public async void FindBy_ForAnExistingStaticFileWithValidName_ReturnOkResult()
        {
            var staticFileName = "StaticFile.jpg";
            var staticFile = new StaticFile("qweret", staticFileName);

            var mockStaticFileRepository = new Mock<IStaticFileRepository>();
            mockStaticFileRepository.Setup(r => r.FindByName(staticFileName)).ReturnsAsync(staticFile);
            var mockBlobDownloadService = new Mock<IBlobDownloadService>();
            var staticFileController = new StaticFileController(mockStaticFileRepository.Object, mockBlobDownloadService.Object);

            var result = await staticFileController.FindBy(staticFileName) as OkNegotiatedContentResult<StaticFile>;

            result.Content.Should().Be(staticFile);
        }

        [Fact]
        public async void FindBy_ForANonExistentStaticFile_ReturnsNotFoundResult()
        {
            var staticFileName = "StaticFile.jpg";
            var mockStaticFileRepository = new Mock<IStaticFileRepository>();
            mockStaticFileRepository.Setup(r => r.FindByName(staticFileName)).ThrowsAsync(new ResourceNotFoundException(""));
            var mockBlobDownloadService = new Mock<IBlobDownloadService>();
            var staticFileController = new StaticFileController(mockStaticFileRepository.Object, mockBlobDownloadService.Object);

           (await staticFileController.FindBy(staticFileName)).Should().BeOfType<NotFoundResult>();
        } 
    }
}