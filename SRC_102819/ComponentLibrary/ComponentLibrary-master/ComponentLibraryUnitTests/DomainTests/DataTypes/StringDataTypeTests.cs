using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class StringDataTypeTests
    {
        [Fact]
        public async void Parse_ShouldReturnString_WhenCalledWithString()
        {
            const string input = "sattar";
            var stringDataType = new StringDataType();
            var result = await stringDataType.Parse(input);
            result.Should().Be(input);
        }

        [Fact]
        public void Parse_ShouldThrowArgumentException_WhenCalledWithNonString()
        {
            const int input = 1;
            var stringDataType = new StringDataType();
            Action act = () => stringDataType.Parse(input);
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void StringDataType_ShouldBeAssignableToIDataType()
        {
            var stringDataType = new StringDataType();
            stringDataType.Should().BeAssignableTo<IDataType>();
        }
    }
}