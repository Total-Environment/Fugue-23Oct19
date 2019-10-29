using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class BrandDefinitionTests
    {
        [Fact]
        public void Constructor_ShouldReturnBrandDefintion_WhenPassedNameAndColumnDefinitions()
        {
            var definition = new BrandDefinition("Generic", new List<ISimpleColumnDefinition>());
            definition.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenPassedNullAsName()
        {
            Action act = () => new BrandDefinition(null, new List<ISimpleColumnDefinition>());
            act.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\\r\\nParameter name: name");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenPassedNullAsColumns()
        {
            Action act = () => new BrandDefinition("Generic", null);
            act.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\\r\\nParameter name: columns");
        }

        [Fact]
        public async Task Parse_ShouldReturnBrand_WhenPassedDictionary()
        {
            var nameColumnDefinition = new Mock<ISimpleColumnDefinition>();
            var nameColumnData = new ColumnData("Name", "name", "Jindal Steel");
            nameColumnDefinition.Setup(n => n.Key).Returns("name");
            nameColumnDefinition.Setup(n => n.Parse("Jindal Steel")).ReturnsAsync(nameColumnData);
            var brandCodeDefinition = new Mock<ISimpleColumnDefinition>();
            var brandCodeData = new ColumnData("Brand Code", "brand_code", "BS001");
            brandCodeDefinition.Setup(n => n.Parse("BS001")).ReturnsAsync(brandCodeData);
            brandCodeDefinition.Setup(n => n.Key).Returns("brand_code");
            var definition = new BrandDefinition("Generic", new List<ISimpleColumnDefinition> { nameColumnDefinition.Object, brandCodeDefinition.Object });
            var dictionary = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"key", "name" },
                    {"name", "Name" },
                    { "value", "Jindal Steel" }
                },
                new Dictionary<string, object>
                {
                    {"key", "brand_code" },
                    {"name", "Brand Code" },
                    { "value", "BS001" }
                }
            };
            var brand = await definition.Parse(dictionary);
            brand.Columns.Should().HaveCount(2);
            brand.Columns.First().Should().Be(nameColumnData);
            brand.Columns.ElementAt(1).Should().Be(brandCodeData);
        }
    }
}