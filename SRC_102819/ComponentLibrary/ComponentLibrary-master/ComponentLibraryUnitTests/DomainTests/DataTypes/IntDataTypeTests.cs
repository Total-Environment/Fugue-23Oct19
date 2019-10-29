using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class IntDataTypeTests
    {
        [Theory]
        [InlineData(3.14f)]
        [InlineData("sattar")]
        public void Parse_ShouldThrowFormatException_WhenCalledWithNonInt(object input)
        {
            var intDataType = new IntDataType();
            Action act = () => intDataType.Parse(input);
            act.ShouldThrow<FormatException>().WithMessage($"Expected an Integer. Got {input} which is not an integer.");
        }

        [Theory]
        [InlineData(64L)]
        public void Parse_ShouldReturnParsedValue_WhenCalledWithNumericIntValues(object input)
        {
            var intDataType = new IntDataType();
            intDataType.Parse(input).Should().NotBeNull();
        }

        [Fact]
        public void IntDataType_ShouldBeAssignableToIDataType()
        {
            var intDataType = new IntDataType();
            intDataType.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnParsedValue_WhenCalledWithInt()
        {
            var intDataType = new IntDataType();
            var input = 1;
            var result = await intDataType.Parse(input);
            result.Should().Be(input);
        }
    }
}