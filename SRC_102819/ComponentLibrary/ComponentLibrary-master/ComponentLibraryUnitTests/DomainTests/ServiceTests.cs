using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class ServiceTests
    {
        [Fact]
        public void Service_ShouldTakeAListOfHeaderValuesAndDefinition()
        {
            var serviceDefinition = new ServiceDefinition("Sattar");
            var service = new Service(new List<IHeaderData>(), serviceDefinition);
            service.Headers.Should().Equal(new List<IHeaderData>());
            service.ComponentDefinition.Should().Be(serviceDefinition);
        }

        [Fact]
        public void Service_WhenAccessedByAccessor_ShouldReturnCorrectValue()
        {
            var mockHeaderValue = new Mock<IHeaderData>();
            const string name = "sattar";
            mockHeaderValue.Setup(m => m.Name).Returns(name);
            var service = new Service(new List<IHeaderData> {mockHeaderValue.Object}, new ServiceDefinition("Sattar"));
            service[name].Should().Be(mockHeaderValue.Object);
        }

        [Fact]
        public void Service_WhenAccessedById_ShouldBeAccessible()
        {
            const string id = "CLY0001";
            var service = new Service(new List<IHeaderData>(), new ServiceDefinition("Sattar")) {Id = id};
            service.Id.Should().Be(id);
        }
    }
}