using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ServiceMasters.Infrastructure
{
    public class ServiceSapSyncerTests
    {
        [Fact]
        public void Sync_ShouldCallServiceAndCompositeComponentSapSyncer_WithSyncWithRightRequestObject()
        {
            var service = new Service(new List<IHeaderData>
            {
                new HeaderData("General", "general")
                {
                    Columns =
                        new List<IColumnData>
                        {
                            new ColumnData("Short Description", "short_description", "Short Description")
                        }
                }
            }, new ServiceDefinition("Sattar"));
            var request = new ServiceAndCompositeComponentRequest
            {
                ComponentType = "service",
                Update = false,
                SACCode = "",
                UnitOfMeasure = "",
                ShortDescription = "Short Description"
            };
            var mockServiceAndCompositeComponentSapSyncer = new Mock<IServiceAndCompositeComponentSapSyncer>();
            mockServiceAndCompositeComponentSapSyncer.Setup(m => m.Sync(request));
            new ServiceSapSyncer(mockServiceAndCompositeComponentSapSyncer.Object).Sync(service, false);
            mockServiceAndCompositeComponentSapSyncer.Verify(m => m.Sync(request), Times.Once);
        }
    }
}