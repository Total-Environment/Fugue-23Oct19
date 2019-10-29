using System;
using System.Threading.Tasks;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class DateDataTypeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void Prase_ShouldThrowFormatException_WhenCalledWithNonDateTime(object data)
        {
            Action act = () => new DateDataType().Parse(data);
            act.ShouldThrow<FormatException>();
        }

        [Fact]
        public void DateDataType_ShouldBeOfTypeIDataType()
        {
            var date = new DateDataType();
            date.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnDateTime_WhenPassedDateTimeObject()
        {
            var dateDataType = new DateDataType();
            var date = DateTime.Now;
            var result = await dateDataType.Parse(date);
            result.Should().Be(date);
        }

        [Fact]
        public void Parse_ShouldReturnDateTime_WhenValidDateFormatStringIsPassed()
        {
            new DateDataType().Parse("2017-03-11T21:00:00.0000000+05:30").Result.Should().BeOfType<DateTime>();
        }

        [Fact]
        public void Parse_ShouldThrowArgumentException_WhenStringPassedIsNotValidDateFormat()
        {
            Func<Task> action = async () => await new DateDataType().Parse("2017-13-13T21:00:00.0000000+05:30");

            action.ShouldThrow<FormatException>();
        }
    }
}