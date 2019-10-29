using System.Collections.Generic;
using System.Configuration;
using System.IO;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ExcelImporter.Code;
using TE.ComponentLibrary.ExcelImporter.Code.Components;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.IntegrationTests.Import.Infrastructure
{
    public class DocumentLoggerTests
    {
        [Fact]
        public void WriteWhenPassedKeyValuePairsAndColumnnAndRowCellsShouldWriteCheckListToFile()
        {
            var checkListPath = Path.Combine(Path.GetTempPath(), $"checklist.txt");
            var imagePath = Path.Combine(Path.GetTempPath(), $"image.txt");
            var xmlPath = ConfigurationManager.AppSettings.Get("ClayMaterialXml");

            if (File.Exists(checkListPath))
                File.Delete(checkListPath);
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            try
            {
                var configuration = new Mock<IConfiguration>();
                configuration.Setup(c => c.DocumentLoggingPath).Returns(Path.GetTempPath());
                var documentLogger = new ComponentDocumentLogger(configuration.Object, null);

                var urlDictionary = new Dictionary<string, List<string>>();

                var expectedChecklistString = ", Quality Check List% sampleData, file://sample.xlsx";
                var expectedImageString = ", Quality Check List, sample image, file://image.jpeg";

                var checklistList = new List<string> { expectedChecklistString };
                var imageList = new List<string> { expectedImageString };
                urlDictionary.Add("checklist", checklistList);
                urlDictionary.Add("image", imageList);

                documentLogger.Write("1", urlDictionary);

                using (var streamReader = new StreamReader(checkListPath))
                {
                    var line = streamReader.ReadLine();
                    line.Should().BeEquivalentTo("1" + expectedChecklistString);
                    streamReader.Close();
                }

                using (var streamReader = new StreamReader(imagePath))
                {
                    var line = streamReader.ReadLine();
                    line.Should().BeEquivalentTo("1" + expectedImageString);
                    streamReader.Close();
                }
            }
            finally
            {
                if (File.Exists(checkListPath))
                    File.Delete(checkListPath);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
        }
    }
}