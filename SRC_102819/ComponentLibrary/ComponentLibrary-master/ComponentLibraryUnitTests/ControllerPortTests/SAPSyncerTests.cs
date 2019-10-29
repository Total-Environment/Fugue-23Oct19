using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NotificationEngine;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerPort;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.SAPServiceReference;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerPortTests
{
    public class SAPSyncerTests
    {
        [Fact]
        public async Task Sync_Should_SyncToDatabase()
        {
            // Arrange
            var material = new Material(new List<IHeaderData>
            {
                new HeaderData("General","General")
                {
                    Columns =
                        new List<IColumnData>
                        {
                            new ColumnData("HSN Code","HSN Code", "HSN Code"),
                            new ColumnData("Short Description", "Short Description","Short Description")
                        }
                }
            }, new MaterialDefinition("Sattar"));
            var mockMaterialMasterOut = new Mock<MaterialMaster_Out>();
            var mockINotifier = new Mock<INotifier>();
            mockMaterialMasterOut.Setup(x => x.MaterialMaster_Out(It.IsAny<MaterialMaster_OutRequest>()))
                .Returns(new MaterialMaster_OutResponse
                {
                    MaterialMasterRes = new[] { new MaterialMasterResItem { RETCODE = "0" } }
                });
            var sapSyncer = new SapSyncer(mockMaterialMasterOut.Object,mockINotifier.Object);

            // Act
            var result = await sapSyncer.Sync(material,"create");

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task Sync_ShouldNot_SyncToDatabase_WhenColumnDataDoesNotExist()
        {
            // Arrange
            var material = new Material(new List<IHeaderData>
            {
                new HeaderData("General","General")
                {
                    Columns =
                        new List<IColumnData>()
                }
            }, new MaterialDefinition("Sattar"));
            var mockMaterialMasterOut = new Mock<MaterialMaster_Out>();
            var mockINotifier = new Mock<INotifier>();
            mockINotifier.Setup(m => m.SendEmailAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask);
            mockMaterialMasterOut.Setup(x => x.MaterialMaster_Out(It.IsAny<MaterialMaster_OutRequest>()))
                .Returns(It.IsAny<MaterialMaster_OutResponse>());
            var sapSyncer = new SapSyncer(mockMaterialMasterOut.Object,mockINotifier.Object);

            // Act
            var result = await sapSyncer.Sync(material,"create");

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task Sync_ShouldNot_SyncToDatabase_WhenExceptonIsThrown()
        {
            // Arrange
            var material = new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar"));
            var mockMaterialMasterOut = new Mock<MaterialMaster_Out>();
            var mockINotifier = new Mock<INotifier>();
            mockINotifier.Setup(m => m.SendEmailAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask);
            mockMaterialMasterOut.Setup(x => x.MaterialMaster_Out(It.IsAny<MaterialMaster_OutRequest>()))
                .Throws(new Exception("Exception"));
            var sapSyncer = new SapSyncer(mockMaterialMasterOut.Object,mockINotifier.Object);

            // Act
            var result = await sapSyncer.Sync(material,"create");

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task Sync_ShouldNot_SyncToDatabase_WhenHeaderDataDoesNotExist()
        {
            // Arrange
            var material = new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar"));
            var mockMaterialMasterOut = new Mock<MaterialMaster_Out>();
            var mockINotifier = new Mock<INotifier>();
            mockINotifier.Setup(m => m.SendEmailAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask);
            mockMaterialMasterOut.Setup(x => x.MaterialMaster_Out(It.IsAny<MaterialMaster_OutRequest>()))
                .Returns(It.IsAny<MaterialMaster_OutResponse>());
            var sapSyncer = new SapSyncer(mockMaterialMasterOut.Object,mockINotifier.Object);

            // Act
            var result = await sapSyncer.Sync(material,"create");

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task Sync_ShouldNot_SyncToDatabase_WhenResponseDoesNotHaveResponse()
        {
            // Arrange
            var material = new Material(new List<IHeaderData>
            {
                new HeaderData("General","General")
                {
                    Columns =
                        new List<IColumnData>
                        {
                            new ColumnData("HSN Code","HSN Code", "HSN Code"),
                            new ColumnData("Short Description", "Short Description","Short Description")
                        }
                }
            }, new MaterialDefinition("Sattar"));
            var mockMaterialMasterOut = new Mock<MaterialMaster_Out>();
            var mockINotifier = new Mock<INotifier>();
            mockINotifier.Setup(m => m.SendEmailAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask);
            mockMaterialMasterOut.Setup(x => x.MaterialMaster_Out(It.IsAny<MaterialMaster_OutRequest>()))
                .Returns(new MaterialMaster_OutResponse());
            var sapSyncer = new SapSyncer(mockMaterialMasterOut.Object,mockINotifier.Object);

            // Act
            var result = await sapSyncer.Sync(material,"update");

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task Sync_ShouldNot_SyncToDatabase_WhenResponseHasFailure()
        {
            // Arrange
            var material = new Material(new List<IHeaderData>
            {
                new HeaderData("General","General")
                {
                    Columns =
                        new List<IColumnData>
                        {
                            new ColumnData("HSN Code","HSN Code",  "HSN Code"),
                            new ColumnData("Short Description","Short Description", "Short Description")
                        }
                }
            }, new MaterialDefinition("Sattar"));
            var mockMaterialMasterOut = new Mock<MaterialMaster_Out>();
            var mockINotifier = new Mock<INotifier>();
            mockINotifier.Setup(m => m.SendEmailAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>())).Returns(Task.CompletedTask);
            mockMaterialMasterOut.Setup(x => x.MaterialMaster_Out(It.IsAny<MaterialMaster_OutRequest>()))
                .Returns(new MaterialMaster_OutResponse
                {
                    MaterialMasterRes = new[] { new MaterialMasterResItem { RETCODE = "1", MESSAGE = "Failure" } }
                });
            var sapSyncer = new SapSyncer(mockMaterialMasterOut.Object,mockINotifier.Object);

            // Act
            var result = await sapSyncer.Sync(material,"update");

            // Assert
            result.Should().Be(false);
        }
    }
}