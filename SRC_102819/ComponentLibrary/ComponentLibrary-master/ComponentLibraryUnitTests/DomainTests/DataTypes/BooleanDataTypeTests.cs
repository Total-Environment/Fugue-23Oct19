using System;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class BooleanDataTypeTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void Parse_ShouldThrowFormatException_WhenCalledWithNonBooleanValue(object value)
        {
            var dt = new BooleanDataType();
            Func<Task> act = async () => await dt.Parse(value);
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void BooleanDataType_IsOfTypeIDataType()
        {
            var booleanDataType = new BooleanDataType();
            booleanDataType.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnBoolean_IfPassedAValidBoolean()
        {
            var dt = new BooleanDataType();
            (await dt.Parse(true)).Should().Be(true);
        }

        [Fact]
        public async void Parse_ShouldReturnBoolean_IfPassedAValidBooleanFalse()
        {
            object subject = false;
            var dt = new BooleanDataType();
            (await dt.Parse(subject)).Should().Be(false);
        }

        [Fact]
        public async void Parse_ShouldReturnBoolean_WhenStringRepresentationOfBoolIsPassed()
        {
            string subject = "False";
            var dt = new BooleanDataType();

            var value = await dt.Parse(subject);

            value.Should().Be(false);
        }
    }
}