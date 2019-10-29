using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class RangeDataTypeTests
    {
        [Theory]
        [InlineData("From", "Sattar")]
        [InlineData("To", "Sattar")]
        [InlineData("From", null)]
        [InlineData("Unit", true)]
        public void Parse_ShouldThrowFormatException_WhenPassedDictionaryWithInvalidValues(string key, object value)
        {
            var dict = new Dictionary<string, object> { { "From", 12 } };
            dict[key] = value;
            Func<Task> act = async () => await new RangeDataType("%").Parse(dict);
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void It_ShouldBeOfIDataType()
        {
            var dt = new RangeDataType("%");
            dt.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnRangeValue_WhenPassedADictionaryWithNumericValues()
        {
            var dict = new Dictionary<string, object> { { "From", 12 }, { "To", "13" }, { "Unit", "%" } };
            var dt = new RangeDataType("%");
            (await dt.Parse(dict)).Should().Be(new RangeValue(12, 13, "%"));
        }

        [Fact]
        public async void Parse_ShouldReturnRangeValue_WhenPassedADictionaryWithoutTo()
        {
            var dict = new Dictionary<string, object> { { "From", 12 }, { "Unit", "%" } };
            var dt = new RangeDataType("%");
            (await dt.Parse(dict)).Should().Be(new RangeValue(12, null, "%"));
        }

        [Fact]
        public async void Parse_ShouldReturnRangeValue_WhenPassedADictionaryWithToAsNull()
        {
            var dict = new Dictionary<string, object> { { "From", 12 }, { "To", null }, { "Unit", "%" } };
            var dt = new RangeDataType("%");
            (await dt.Parse(dict)).Should().Be(new RangeValue(12, null, "%"));
        }

        [Fact]
        public async void Parse_ShouldReturnRangeValue_WhenPassedADictionaryWithUnitAsNull()
        {
            var dict = new Dictionary<string, object> { { "From", 12 }, { "Unit", null } };
            var dt = new RangeDataType(null);
            (await dt.Parse(dict)).Should().Be(new RangeValue(12));
        }

        [Fact]
        public async void Parse_ShouldReturnRangeValue_WhenPassedDictionary()
        {
            var dict = new Dictionary<string, object> { { "From", 12m }, { "To", 13m }, { "Unit", "%" } };
            var dt = new RangeDataType("%");
            (await dt.Parse(dict)).Should().Be(new RangeValue(12, 13, "%"));
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenPassedUnitThatDoesNotMatchDefinition()
        {
            var dict = new Dictionary<string, object> { { "From", 12 }, { "To", 13 }, { "Unit", "mm" } };
            var dt = new RangeDataType("%");
            Func<Task> act = async () => await dt.Parse(dict);
            act.ShouldThrow<FormatException>()
                .WithMessage("Range Data type with value. Expected 'Unit' to be '%'. Got 'mm'");
        }
    }
}