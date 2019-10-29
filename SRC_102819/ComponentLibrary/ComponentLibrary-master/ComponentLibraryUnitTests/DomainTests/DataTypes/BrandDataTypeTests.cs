using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class BrandDataTypeTests
    {
        [Fact]
        public void It_ShouldBeOfTypeIDataType()
        {
            var dataType = new BrandDataType(new Mock<IBrandDefinition>().Object, "SOME", new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            dataType.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenBrandDefinitionIsNull()
        {
            Action act = () => new BrandDataType(null, "SOME", new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenBrandCodeIsNull()
        {
            Action act =
                () =>
                    new BrandDataType(new Mock<IBrandDefinition>().Object, null, new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldReturnBrandDataType_WhenBrandDefinitionAndBrandCodeArePassed()
        {
            var brandDefinition = new Mock<IBrandDefinition>().Object;
            var dataType = new BrandDataType(brandDefinition, "SOME", new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            dataType.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenBrandCodeGeneratorIsNull()
        {
            Action act =
                () =>
                    new BrandDataType(new Mock<IBrandDefinition>().Object, "SOME", null, new Mock<ICounterRepository>().Object);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenCounterRepositoryIsNull()
        {
            Action act =
                () =>
                    new BrandDataType(new Mock<IBrandDefinition>().Object, "SOME", new Mock<IBrandCodeGenerator>().Object, null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async Task Parse_ShouldReturnBrand_WhenPassedValidDictionary()
        {
            var mockBrandDefinition = new Mock<IBrandDefinition>();
            var columns = new List<Dictionary<string, object>>();
            var dictionary = new Dictionary<string, object> { { "columns", new List<object>() } };
            var expected = new Brand();
            mockBrandDefinition.Setup(m => m.Parse(columns)).ReturnsAsync(expected);
            var dataType = new BrandDataType(mockBrandDefinition.Object, "SOME", new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            var brand = await dataType.Parse(dictionary);
            brand.Should().Be(expected);
        }

        [Fact]
        public async Task Parse_ShouldReturnBrandWithExistingCode_WhenPassedADictionaryWithBrandCode()
        {
            const string brandCode = "BSY000002";
            (var mockBrandDefinition, var dictionary) = MockWithBrandCode(brandCode);
            var mockCounterRepository = new Mock<ICounterRepository>();
            mockCounterRepository.Setup(m => m.CurrentValue("Brand")).ReturnsAsync(1);
            //mockCounterRepository.Setup(m => m.Update(2, "Brand")).Verifiable();
            var dataType = new BrandDataType(mockBrandDefinition.Object, "BSY", new Mock<IBrandCodeGenerator>().Object, mockCounterRepository.Object);

            var result = await dataType.Parse(dictionary);

            result.Should().BeOfType<Brand>();
            var castedResult = (Brand)result;
            castedResult["brand_code"].Should().Be(brandCode);
            mockCounterRepository.Verify(m => m.Update(2, "Brand"), Times.Once);
        }

        private static (Mock<IBrandDefinition>, Dictionary<string, object>) MockWithBrandCode(string brandCode)
        {
            var mockBrandDefinition = new Mock<IBrandDefinition>();
            var columns = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"key", "brand_code"},
                    {"name", "Brand Code"},
                    {"value", brandCode}
                }
            };

            var objectColumns = columns.Select(column => (object)column);
            var dictionary = new Dictionary<string, object>
            {
                {
                    "columns", objectColumns
                }
            };
            var brand = new Brand();

            mockBrandDefinition.Setup(m => m.Parse(columns)).ReturnsAsync(brand);
            return (mockBrandDefinition, dictionary);
        }

        [Fact]
        public async Task Parse_ShouldReturnBrandWithNewCode_WhenPassedADictionaryWithoutBrandCodeColumn()
        {
            var brand = new Brand();
            var mockBrandDefinition = new Mock<IBrandDefinition>();
            var columns = new List<Dictionary<string, object>>();
            var dictionary = new Dictionary<string, object> { { "columns", new List<object>() } };
            mockBrandDefinition.Setup(m => m.Parse(columns)).ReturnsAsync(brand);
            var mockBrandCodeGenerator = new Mock<IBrandCodeGenerator>();
            const string brandCode = "SOME0001";
            mockBrandCodeGenerator.Setup(m => m.Generate("SOME")).ReturnsAsync(brandCode);
            var dataType = new BrandDataType(mockBrandDefinition.Object, "SOME", mockBrandCodeGenerator.Object, new Mock<ICounterRepository>().Object);
            var result = await dataType.Parse(dictionary);
            result.Should().BeOfType<Brand>();
            var castedResult = (Brand)result;
            castedResult["brand_code"].Should().Be(brandCode);
        }

        [Fact]
        public async Task Parse_ShouldReturnBrandWithNewCode_WhenPassedADictionaryWhenBrandCodeIsNull()
        {
            (var mockBrandDefinition, var dictionary) = MockWithBrandCode(null);
            var mockBrandCodeGenerator = new Mock<IBrandCodeGenerator>();
            const string brandCode = "SOME0001";
            mockBrandCodeGenerator.Setup(m => m.Generate("SOME")).ReturnsAsync(brandCode);
            var dataType = new BrandDataType(mockBrandDefinition.Object, "SOME", mockBrandCodeGenerator.Object, new Mock<ICounterRepository>().Object);
            var result = await dataType.Parse(dictionary);
            result.Should().BeOfType<Brand>();
            var castedResult = (Brand)result;
            castedResult["brand_code"].Should().Be(brandCode);
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenDictionaryIsNotPassed()
        {
            var dataType = new BrandDataType(new Mock<IBrandDefinition>().Object, "SOME", new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            Func<Task> act = async () => await dataType.Parse(1);
            act.ShouldThrow<FormatException>();
        }

        [Theory]
        [InlineData("BSY00001")]
        [InlineData("SAT0001")]
        [InlineData("SATSATSAT")]
        public void Parse_ShouldThrowFormatException_WhenBrandCodeIsNotValid(string brandCode)
        {
            var dataType = new BrandDataType(new Mock<IBrandDefinition>().Object, "BSY", new Mock<IBrandCodeGenerator>().Object, new Mock<ICounterRepository>().Object);
            var dictionary = new Dictionary<string, object>
            {
                {"columns", new List<object>
                    {
                        new Dictionary<string, object>
                        {
                            {"key", "brand_code" },
                            {"name", "Brand Code" },
                            {"value", brandCode }
                        }
                    }
                }
            };
            Func<Task> act = async () => await dataType.Parse(dictionary);
            act.ShouldThrow<FormatException>();
        }
    }
}