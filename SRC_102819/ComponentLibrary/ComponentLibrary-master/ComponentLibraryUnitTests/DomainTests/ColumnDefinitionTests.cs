using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class ColumnDefinitionTests
    {
        [Fact]
        public void ColumnDefinition_ShouldBeAssignableToIColumnDefinition()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Should().BeAssignableTo<IColumnDefinition>();
        }

        [Fact]
        public void New_ShouldSetNameAndDataType_WhenCalledWithNameAndDataType()
        {
            const string name = "sattar";
            var dataType = new Mock<IDataType>().Object;
            var sut = new Fixture().WithName(name).WithDataType(dataType).SystemUnderTest();
            sut.Name.Should().Be(name);
            sut.DataType.Should().Be(dataType);
        }

        [Fact]
        public async void Parse_ShouldReturnColumnData_WhenPassedObject()
        {
            const int data = 1;
            const int expected = 2;
            var mockDataType = new Mock<IDataType>();
            mockDataType.Setup(d => d.Parse(data)).ReturnsAsync(expected);
            const string name = "sattar";
            var sut = new Fixture().WithName(name).WithDataType(mockDataType.Object).SystemUnderTest();
            var result = await sut.Parse(data);
            result.Name.Should().Be(name);
            result.Value.Should().Be(expected);
        }

        [Fact]
        public async void Parse_ShouldReturnNa_WhenPassedNa()
        {
            var result = await new Fixture().WithName("Sattar").SystemUnderTest().Parse("-- NA --");
            result.Name.Should().Be("Sattar");
            result.Value.Should().Be("-- NA --");
        }

        [Fact]
        public async void Parse_ShouldReturnNull_WhenPassedNull()
        {
            var dataTypeMock = new Mock<IDataType>();
            dataTypeMock.Setup(m => m.Parse(It.IsAny<object>()))
                .Throws(new FormatException());
            var result =
                await new Fixture().WithName("Sattar").WithDataType(dataTypeMock.Object).SystemUnderTest().Parse(null);
            result.Name.Should().Be("Sattar");
            result.Value.Should().Be(null);
        }

        [Fact]
        public void SetDataType_ShouldSetDataType()
        {
            var mockDataType = new Mock<IDataType>().Object;
            var sut = new Fixture().WithDataType(mockDataType).SystemUnderTest();
            sut.DataType.Should().Be(mockDataType);
        }

        [Fact]
        public void SetDataType_ShouldThrowArgumentException_WhenCalledWithNull()
        {
            var sut = new Fixture().SystemUnderTest();
            Action act = () => sut.DataType = null;
            act.ShouldThrow<ArgumentException>().WithMessage("DataType is required.");
        }

        [Fact]
        public void SetName_ShouldSetName_WhenCalledWithName()
        {
            const string name = "sattar";
            var sut = new Fixture().SystemUnderTest();
            sut.Name = name;
            sut.Name.Should().Be(name);
        }

        [Fact]
        public void SetName_ShouldThrowArgumentException_WhenCalledWithNull()
        {
            var sut = new Fixture().SystemUnderTest();
            Action act = () => sut.Name = null;
            act.ShouldThrow<ArgumentException>().WithMessage("Name is required.");
        }

        private class Fixture
        {
            private IDataType _dataType;
            private string _name;

            public Fixture()
            {
                _name = "sattar";
                _dataType = new Mock<IDataType>().Object;
            }

            public ColumnDefinition SystemUnderTest()
            {
                return new ColumnDefinition(_name, _name, _dataType, false);
            }

            public Fixture WithDataType(IDataType dataType)
            {
                _dataType = dataType;
                return this;
            }

            public Fixture WithName(string name)
            {
                _name = name;
                return this;
            }
        }

        [Fact]
        public void IsAttachmentType_ShouldReturnTrue_WhenColumnTypeIsEitherAStaticFileOrAChecklist()
        {
            var sut = new Fixture().WithName("QuantityEvaluationMethod").WithDataType(new StaticFileDataType()).SystemUnderTest();
            sut.IsAttachmentType().Should().BeTrue();
        }
    }
}