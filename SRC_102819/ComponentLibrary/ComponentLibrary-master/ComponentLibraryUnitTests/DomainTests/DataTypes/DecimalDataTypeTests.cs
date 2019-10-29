using System;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class DecimalDataTypeTests
    {
        [Fact]
        public void DoubleDataType_ShouldBeOfTypeIDataType()
        {
            var sut = new DecimalDataType();
            sut.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnParsedValue_WhenCalledWithInt()
        {
            var decimalDataType = new DecimalDataType();
            const decimal input = 1.0m;
            (await decimalDataType.Parse(input)).Should().Be(input);
        }

        [Fact]
        public async void Parse_ShouldReturnParsedValue_WhenCalledWithNumericAsObject()
        {
            var decimalDataType = new DecimalDataType();
            object input = 1.0;
            (await decimalDataType.Parse(input)).Should().Be(1.0m);
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenCalledWithNonInt()
        {
            var decimalDataType = new DecimalDataType();
            const string input = "sattar";
            Func<Task> act = async () => await decimalDataType.Parse(input);
            act.ShouldThrow<FormatException>();
        }
    }
}