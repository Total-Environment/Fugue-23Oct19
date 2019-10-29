using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure
{
    public class CompositeComponentSapSyncerTests
    {
        [Theory]
        [InlineData("package")]
        [InlineData("sfg")]
        public void Sync_ShouldCallServiceAndCompositeComponentSapSyncer_WithSyncWithRightRequestObject(string componentType)
        {
            var service = new CompositeComponent
            {
                Code = "STN0001",
                Headers = new List<IHeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns =
                            new List<IColumnData>
                            {
                                new ColumnData("Short Description", "short_description", "Short Description")
                            }
                    },
                    new HeaderData("System Logs", "system_logs")
                    {
                        Columns =
                            new List<IColumnData>
                            {
                                new ColumnData("Date Created", "date_created", new DateTime())
                            }
                    }
                }
            };
            var request = new ServiceAndCompositeComponentRequest
            {
                Code = "STN0001",
                ComponentType = componentType,
                Update = false,
                SACCode = "",
                UnitOfMeasure = "",
                ShortDescription = "Short Description"
            };
            var mockServiceAndCompositeComponentSapSyncer = new Mock<IServiceAndCompositeComponentSapSyncer>();
            mockServiceAndCompositeComponentSapSyncer.Setup(m => m.Sync(request));
            new CompositeComponentSapSyncer(mockServiceAndCompositeComponentSapSyncer.Object).Sync(service, false,
                componentType);
            mockServiceAndCompositeComponentSapSyncer.Verify(m => m.Sync(request), Times.Once);
        }
    }
}