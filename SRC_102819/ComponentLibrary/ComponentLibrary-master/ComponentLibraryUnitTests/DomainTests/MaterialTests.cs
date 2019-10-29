using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class MaterialTests
    {
        [Fact]
        public void Material_ShouldTakeAListOfHeaderValuesAndDefinition()
        {
            var materialDefinition = new MaterialDefinition("Sattar");
            var material = new Material(new List<IHeaderData>(), materialDefinition);
            material.Headers.Should().Equal(new List<IHeaderData>());
            material.ComponentDefinition.Should().Be(materialDefinition);
        }

        [Fact]
        public void Material_WhenAccessedByAccessor_ShouldReturnCorrectValue()
        {
            var mockHeaderValue = new Mock<IHeaderData>();
            const string name = "sattar";
            mockHeaderValue.Setup(m => m.Name).Returns(name);
            var material = new Material(new List<IHeaderData> {mockHeaderValue.Object}, new MaterialDefinition("Sattar"));
            material[name].Should().Be(mockHeaderValue.Object);
        }

        [Fact]
        public void Material_WhenAccessedById_ShouldBeAccessible()
        {
            const string id = "CLY0001";
            var material = new Material(new List<IHeaderData>(), new MaterialDefinition("Sattar")) {Id = id};
            material.Id.Should().Be(id);
        }
    }
}