using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class ConstantDataTypeTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData("ram")]
        [InlineData(null)]
        public void Parse_ShouldThrowFormatException_WhenPassedANonString(object nonString)
        {
            var constDataType = new ConstantDataType("sattar");
            Action act = () => constDataType.Parse(nonString);
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void ConstantDataType_ShouldBeOfTypeIDataType()
        {
            var constDataType = new ConstantDataType("sattar");
            constDataType.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public void New_ShouldReturnConstant_WhenPassedAString()
        {
            var constDataType = new ConstantDataType("sattar");
            constDataType.Should().NotBeNull();
        }

        [Fact]
        public void New_ShouldThrowArgumentNullException_WhenCalledWithNull()
        {
            Action act = () => new ConstantDataType(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public async void Parse_ShouldReturnString_WhenPassedString()
        {
            const string value = "sattar";
            var constDataType = new ConstantDataType(value);
            var result = await constDataType.Parse(value);
            result.Should().Be(value);
        }
    }
}