using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class HeaderDataTests
    {
        [Fact]
        public void HeaderData_Should_BeAssignableToIHeaderData()
        {
            var sut = new Fixture().WithColumns(new List<IColumnData>()).SystemUnderTest();
            sut.Should().BeAssignableTo<IHeaderData>();
        }

        [Fact]
        public void Indexer_ShouldReturnValueOfColumn_WhenColumnNameDoesntExist()
        {
            var sut = new Fixture().SystemUnderTest();
            sut["sattar"].Should().BeNull();
        }

        [Fact]
        public void Indexer_ShouldReturnValueOfColumn_WhenColumnNameIsPassed()
        {
            const string name = "sattar";
            const int value = 1;
            var mockColumn = new Mock<IColumnData>();
            mockColumn.Setup(m => m.Name).Returns(name);
            mockColumn.Setup(m => m.Value).Returns(value);
            var columns = new List<IColumnData> { mockColumn.Object };

            var sut = new Fixture().WithColumns(columns).SystemUnderTest();
            sut[name].Should().Be(value);
        }

        [Fact]
        public void New_ShouldThrowArgumentException_WhenNameIsNull()
        {
            Action act = () => new Fixture().WithoutName().SystemUnderTest();
            act.ShouldThrow<ArgumentException>().WithMessage("name is required for Header.");
        }

        [Fact]
        public void SettingName_ShouldRaiseArgumentException_WhenPassedNull()
        {
            Action act = () => new Fixture().SystemUnderTest().Name = null;
            act.ShouldThrow<ArgumentException>().WithMessage("name is required for Header.");
        }

        public class Fixture
        {
            private List<IColumnData> _columns;
            private string _name;

            public Fixture()
            {
                _name = "sattar";
                _columns = new List<IColumnData>();
            }

            public HeaderData SystemUnderTest()
            {
                return new HeaderData(_name, _name) { Columns = _columns };
            }

            public Fixture WithColumns(List<IColumnData> columns)
            {
                _columns = columns;
                return this;
            }

            public Fixture WithoutName()
            {
                _name = null;
                return this;
            }
        }
    }
}