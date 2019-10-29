using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ExcelImporter.Code;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.RestWebClient;
using TE.Shared.CloudServiceFramework.Domain;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests.Import
{
    public class StaticFileImporterTests
    {
        [Fact]
        public void Parse_OnValidParsingOfGivenFile_ReturnsList()
        {
            var mockFileVerifier = new Mock<IFileVerifier>();
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.jpg");
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1002")).Returns("MAC1002.png");
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1003")).Returns("MAC1003.jpeg");
            mockFileVerifier.Setup(c => c.ParseFilePath("MACE1")).Returns("MACE1.pdf");
            var staticFileImporter = new StaticFileImporter(new List<string>
            {
                "1~General:Image%MAC1001, MAC1002, MAC1003~Purchase:Quality Evaluation Method%MACE1"
            }, mockFileVerifier.Object);

            var imageFiles = new StaticFileInformation("general", "image",
                new List<string> { "MAC1001.jpg", "MAC1002.png", "MAC1003.jpeg" });
            var pdfFiles = new StaticFileInformation("purchase", "quality Evaluation Method",
                new List<string> { "MACE1.pdf" });

            var staticFiles = new List<StaticFileInformation> { imageFiles, pdfFiles };
            var actualFileList = staticFileImporter.Parse();

            actualFileList.Should().ContainKey("1");
            actualFileList["1"].Should().Equal(staticFiles);
        }

        [Fact]
        public async void UploadToAzure_ShouldUploadStaticFilesToAzureStorage_ReturnUpdatedStaticFile()
        {
            var mockFileVerifier = new Mock<IFileVerifier>();
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.jpg");
            mockFileVerifier.Setup(c => c.ParseFilePath("MACE1")).Returns("MACE1.pdf");
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.png");
            var staticFileImporter = new StaticFileImporter(new List<string>
            {
                "1~General:Image%MAC1001~Purchase:Quality Evaluation Method%MACE1, MAC1001"
            }, mockFileVerifier.Object);
            var staticFileList = new KeyValuePair<string, List<StaticFileInformation>>("1",
                new List<StaticFileInformation>
                {
                    new StaticFileInformation("general", "image", new List<string> {"MAC1001.jpg"})
                });
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c.ComponentLibraryBaseUrl).Returns("https://testapi.te.com");
            var mockBlobService = new Mock<IBlobStorageService>();
            mockBlobService.Setup(service => service.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(reader => reader.Read(It.IsAny<string>())).Returns(new MemoryStream());
            var result = await staticFileImporter.UploadToAzure(staticFileList, mockConfiguration.Object,
                mockBlobService.Object, mockFileReader.Object, "c:/");

            result.Count.Should().Be(1);
            result[0].StaticFiles.Should().NotBeEmpty();
        }

        [Fact]
        public async void LinkToMaterial_ShouldAddStaticFilesToDBAndUpdateMaterials_ReturnUpdatedMaterial()
        {
            var mockFileVerifier = new Mock<IFileVerifier>();
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.jpg");
            mockFileVerifier.Setup(c => c.ParseFilePath("MACE1")).Returns("MACE1.pdf");
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.png");

            var staticFileImporter = new StaticFileImporter(new List<string>
            {
                "1~General:Image%MAC1001~Purchase:Quality Evaluation Method%MACE1, MAC1001"
            }, mockFileVerifier.Object);
            var staticFileList = new List<StaticFileInformation>
            {
                new StaticFileInformation("general", "image", new List<string> {"MAC1001.jpg"})
            };
            var staticFile = new List<StaticFile>
            {
                new StaticFile("aadfe", "MAC1001.jpg")
            };

            staticFileList[0].UpdateStaticFileList(staticFile);
            var mockStaticFilesWebClient = new Mock<IWebClient<StaticFile>>();
            mockStaticFilesWebClient.Setup(client => client.FindBy(It.IsAny<string>())).ReturnsAsync(new RestClientResponse<StaticFile>(HttpStatusCode.NotFound, null));
            mockStaticFilesWebClient.Setup(client => client.Post(It.IsAny<StaticFile>(), It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<StaticFile>(HttpStatusCode.OK, staticFile[0]));
            var mockMaterialWebClient = new Mock<IWebClient<Dictionary<string, object>>>();
            var expectedMaterial = new Dictionary<string, object>()
            {
                { "general", JObject.FromObject(new Dictionary<string, object>
                {
                    {"image", staticFile}
                }) }
            };
            mockMaterialWebClient.Setup(client => client.Get(It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, expectedMaterial));
            mockMaterialWebClient.Setup(
                    client => client.Put(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, expectedMaterial));

            await staticFileImporter.LinkToComponent("1", staticFileList, mockStaticFilesWebClient.Object,
                    mockMaterialWebClient.Object, "materials");
        }

        [Fact]
        public async void
            LinkToMaterial_ShouldForAStaticFileThatAlreadyExists_ReturnUpdatedMaterialWithExistingStaticFile()
        {
            var mockFileVerifier = new Mock<IFileVerifier>();
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.jpg");
            mockFileVerifier.Setup(c => c.ParseFilePath("MACE1")).Returns("MACE1.pdf");
            mockFileVerifier.Setup(c => c.ParseFilePath("MAC1001")).Returns("MAC1001.png");

            var staticFileImporter = new StaticFileImporter(new List<string>
            {
                "1~General:ImageUrl%MAC1001~Purchase:Quality Evaluation Method%MACE1, MAC1001"
            }, mockFileVerifier.Object);
            var staticFileList = new List<StaticFileInformation>
            {
                new StaticFileInformation("general", "imageUrl", new List<string> {"MAC1001.jpg"})
            };
            var item = new StaticFile("aadfe", "MAC1001.jpg");
            var staticFile = new List<StaticFile>
            {
                item
            };

            staticFileList[0].UpdateStaticFileList(staticFile);
            var mockStaticFilesWebClient = new Mock<IWebClient<StaticFile>>();
            mockStaticFilesWebClient.Setup(client => client.FindBy(It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<StaticFile>(HttpStatusCode.OK, item));
            mockStaticFilesWebClient.Setup(client => client.Post(It.IsAny<StaticFile>(), It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<StaticFile>(HttpStatusCode.OK, staticFile[0]));
            var mockMaterialWebClient = new Mock<IWebClient<Dictionary<string, object>>>();
            var expectedMaterial = new Dictionary<string, object>()
            {
                { "general", JObject.FromObject(new Dictionary<string, object>
                {
                    {"imageUrl", null}
                }) }
            };
            mockMaterialWebClient.Setup(client => client.Get(It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, expectedMaterial));
            mockMaterialWebClient.Setup(
                    client => client.Put(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<string>()))
                .ReturnsAsync(new RestClientResponse<Dictionary<string, object>>(HttpStatusCode.OK, expectedMaterial));

            await staticFileImporter.LinkToComponent("1", staticFileList, mockStaticFilesWebClient.Object,
                mockMaterialWebClient.Object, "materials");

            (expectedMaterial["general"] as JObject)["imageUrl"].Should().NotBeNull();
        }
    }
}