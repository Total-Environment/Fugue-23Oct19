using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class ServiceDefinitionTests
    {
        [Fact]
        public async void Parse_ShouldGeneraterKeywordsForAMAterial_ReturnUpdatedService()
        {
            const string headerName = "headerDetails";
            var headerDictionary = new Dictionary<string, object>
            {
                {"group", "Clay"},
                {"name", "Clay Service 1"}
            };
            var data = new Dictionary<string, object>
            {
                {headerName, headerDictionary}
            };
            var headerDataMock = new Mock<IHeaderData>();
            headerDataMock.Setup(h => h.Name).Returns(headerName);
            headerDataMock.Setup(h => h.Columns).Returns(new List<IColumnData>
            {
                new ColumnData("Shade No","Shade No", 0),
                new ColumnData("Short Description","Short Description", "Soil Mezzune")
            });
            var headerData = headerDataMock.Object;
            var mockHeaderDefinition = new Mock<IHeaderDefinition>();
            mockHeaderDefinition.Setup(m => m.Name).Returns(headerName);
            mockHeaderDefinition.Setup(h => h.Parse(headerDictionary)).ReturnsAsync(headerData);
            mockHeaderDefinition.Setup(h => h.Columns).Returns(new List<IColumnDefinition>
            {
                new ColumnDefinition("Shade No", "Shade No",new IntDataType(), true),
                new ColumnDefinition("Short Description", "Short Description", new StringDataType(), true),
                new ColumnDefinition("Product Number", "Product Number", new StringDataType(), false)
            });
            var serviceDefinition = new ServiceDefinition("Clay")
            {
                Headers = new List<IHeaderDefinition> { mockHeaderDefinition.Object }
            };

            var service = await serviceDefinition.Parse<Service>(data, null);
            service[headerName].Should().Be(headerData);
            service.SearchKeywords.Count.Should().Be(2);
            service.SearchKeywords[0].Should().Be("0");
            service.SearchKeywords[1].Should().Be("Soil Mezzune");
        }

        [Fact]
        public async void Parse_ShouldReturnService_WhenPassedServiceDetailsAsDictionaryWithGroup()
        {
            const string headerName = "headerDetails";
            var headerDictionary = new Dictionary<string, object>
            {
                {"group", "Clay"},
                {"name", "Clay Service 1"}
            };
            var data = new Dictionary<string, object>
            {
                {headerName, headerDictionary}
            };

            var headerDataMock = new Mock<IHeaderData>();
            headerDataMock.Setup(h => h.Name).Returns(headerName);
            var headerData = headerDataMock.Object;
            var mockHeaderDefinition = new Mock<IHeaderDefinition>();
            mockHeaderDefinition.Setup(m => m.Name).Returns(headerName);
            mockHeaderDefinition.Setup(h => h.Parse(headerDictionary)).ReturnsAsync(headerData);
            var serviceDefinition = new ServiceDefinition("Clay")
            {
                Headers = new List<IHeaderDefinition> { mockHeaderDefinition.Object }
            };

            var service = await serviceDefinition.Parse<Service>(data, null);
            service[headerName].Should().Be(headerData);
        }

        [Fact]
        public void Parse_ShouldThrowArgumentException_WhenPassedServiceDetailsWithoutDefinedHeader()
        {
            const string headerName = "sattar";
            var mockHeaderDefinition = new Mock<IHeaderDefinition>();
            mockHeaderDefinition.Setup(m => m.Name).Returns(headerName);
            var serviceDefinition = new ServiceDefinition("Clay")
            {
                Headers = new List<IHeaderDefinition> { mockHeaderDefinition.Object }
            };
            var data = new Dictionary<string, object>
            {
                {"id", "CLY0001"},
                {"group", "Clay"}
            };
            Func<Task> act = async () => (await serviceDefinition.Parse<Service>(data, null)).Headers.ToList();
            act.ShouldThrow<ArgumentException>().WithMessage($"{headerName} is required.");
        }

        [Fact]
        public void ServiceDefinition_ShouldHaveGroupAndHeaders()
        {
            const string group = "Clay";
            const string code = "CLY";
            var serviceDefinition = new ServiceDefinition(group)
            {
                Code = code
            };

            serviceDefinition.Code.Should().Be(code);
            serviceDefinition.Name.Should().Be(group);
            serviceDefinition.Headers.Should().Equal(new List<IHeaderDefinition>());
        }

        [Fact]
        public void ServiceDefinition_ShouldRaiseArgumentExceptionIfGroupIsNull()
        {
            Action act = () => new ServiceDefinition(null);
            act.ShouldThrow<ArgumentException>();
        }
    }
}