using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class RangeValueTests
    {
        [Fact]
        public void It_ShouldReturnRangeValue_WhenPassedProps()
        {
            var rv = new RangeValue(12, 13, "mm");
            rv.From.Should().Be(12);
            rv.To.Should().Be(13);
            rv.Unit.Should().Be("mm");
        }

        [Fact]
        public void It_ShouldReturnRangeValue_WhenPassedToAsNull()
        {
            var rv = new RangeValue(12, null, "mm");
            rv.To.Should().Be(null);
        }

        [Fact]
        public void ToString_ShouldReturnFromAsString_WhenPassedToAsNull()
        {
            var rv = new RangeValue(12, null, "mm");
            rv.ToString().Should().Be("12mm");
        }

        [Fact]
        public void ToString_ShouldReturnHyphenSeparatedValues_WhenPassedUnitAsNull()
        {
            var rv = new RangeValue(12, 13);
            rv.ToString().Should().Be("12-13");
        }

        [Fact]
        public void ToString_ShouldReturnHyphenSeparatedValues_WhenPassedValidValues()
        {
            var rv = new RangeValue(12, 14, "mm");
            rv.ToString().Should().Be("12mm-14mm");
        }
    }
}