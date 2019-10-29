using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class ArrayDataTypeTests
    {
        [Fact]
        public void ArrayDataType_ShouldBeOfType()
        {
            var sut = new ArrayDataType(new Mock<IDataType>().Object);
            sut.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public void New_ShouldThrowArgumentException_WhenPassedNull()
        {
            Action act = () => new ArrayDataType(null);
            act.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: dataType");
        }

        [Fact]
        public void New_WhenPassedIDataType_ShouldConstruct()
        {
            var sut = new ArrayDataType(new Mock<IDataType>().Object);
            sut.Should().NotBeNull();
        }

        [Fact]
        public async void Parse_ShouldReturnArray_WhenPassedValidDataTypeAndData()
        {
            var subType = new Mock<IDataType>();
            subType.Setup(s => s.Parse(It.IsAny<object>())).Returns((object x) => Task.FromResult(x));

            var type = new ArrayDataType(subType.Object);
            var result = await type.Parse(new List<object> { 1, 2, 3 });
            result.As<object[]>().Should().Equal(1, 2, 3);
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenPassedDataThatIsNotArray()
        {
            var type = new ArrayDataType(new Mock<IDataType>().Object);
            Func<Task> act = async () => await type.Parse("sattar");
            act.ShouldThrow<FormatException>();
        }
    }
}