using System.Collections.Generic;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests.Import
{
    public class StaticFileInformationTests
    {
        [Fact]
        public void Equals_TwoObjectsWithIdenticalValues_ReturnTrue()
        {
            var firstStaticFile = new StaticFileInformation("General", "Image", new List<string> {"image1.jpg"});
            var secondStaticFile = new StaticFileInformation("General", "Image", new List<string> {"image1.jpg"});

            firstStaticFile.Should().Be(secondStaticFile);
        }

        [Fact]
        public void Equals_TwoObjectsWithNonIdenticalValues_ReturnFalse()
        {
            var firstStaticFile = new StaticFileInformation("General", "Image", new List<string> { "image1.jpg" });
            var secondStaticFile = new StaticFileInformation("General", "Image", new List<string> { "image2.jpg" });

            firstStaticFile.Should().NotBe(secondStaticFile);
        }

        [Fact]
        public void UpdateUrlList_ShouldUpdateStaticFileWithListOfUrls_ReturnUpdatedStaticFile()
        {
            var firstStaticFile = new StaticFileInformation("General", "Image", new List<string> { "image1.jpg" });
            var staticFile = new StaticFile("qweret","image1.jpg");
            var staticFileList = new List<StaticFile> { staticFile };

            var result = firstStaticFile.UpdateStaticFileList(staticFileList);

            result.StaticFiles.Count.Should().Be(1);
            result.StaticFiles[0].Should().Be(staticFile);
        } 
    }
}