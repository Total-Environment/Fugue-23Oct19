using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerAdaptersTests
{
    public class ServiceDtoAdapterTests
    {

        [Fact]
        public void ToService_ShouldReturnServiceData_WhenServiceDtoIsPassed()
        {
            var materialDataDto = StubServiceDataDto("Clay");
            var material = GetService("Clay");

            var result = ServiceDtoAdapter.ToService(materialDataDto);

            result.Group.Should().Be("Clay");
            result.Id.Should().Be(material.Id);
            result.Headers.Any(h => h.Name == "General").Should().Be(true);
            result.Headers.Any(h => h.Key == "general").Should().Be(true);
            result.Headers.First(h => h.Name == "General").Columns.Any(c => c.Name == "Service Name").Should().Be(true);
            result.Headers.First(h => h.Key == "general").Columns.Any(c => c.Key == "service_name").Should().Be(true);
            result.Headers.First(h => h.Name == "General")
                .Columns.First(c => c.Name == "Service Name")
                .Value.Should()
                .Be("Murrum");
        }

        [Fact]
        public void FromService_ShouldReturnServiceDto_WhenServiceIsPassed()
        {
            var service = GetService("Clay");

            var result = ServiceDtoAdapter.FromService(service);

            result.Group.Should().Be("Clay");
            result.Id.Should().Be(service.Id);
            result.Headers.Any(h => h.Name == "General").Should().Be(true);
            result.Headers.First(h => h.Name == "General").Columns.Any(c => c.Name == "Service Name").Should().Be(true);
            result.Headers.First(h => h.Name == "General")
                .Columns.First(c => c.Name == "Service Name")
                .Value.Should()
                .Be("Murrum");
        }
        private static Service GetService(string group, string id = null)
        {
            return new Service()
            {
                Group = group,
                Id = id,
                Headers = new List<HeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData>()
                        {
                            new ColumnData("Service Name", "service_name", "Murrum")
                        }
                    }
                }
            };
        }


        private static ComponentDataDto<Service> StubServiceDataDto(string materialGroup)
        {
            return new ComponentDataDto<Service>()
            {
                Headers = new List<HeaderDto>
                {
                    new HeaderDto
                    {
                        Name = "General",
                        Key = "general",
                        Columns = new List<ColumnDto>
                        {
                            new ColumnDto
                            {
                                Name = "Service Name",
                                Key = "service_name",
                                Value = "Murrum",
                            }
                        }
                    }
                },
                Group = materialGroup
            };
        }
    }
}
