using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class MasterDataTypeTests
    {
        private class Fixture
        {
            private Mock<IMasterDataList> _masterDataMock;

            public Fixture()
            {
                _masterDataMock = new Mock<IMasterDataList>();
            }

            public Fixture WithMasterData(Mock<IMasterDataList> masterDataMock)
            {
                _masterDataMock = masterDataMock;
                return this;
            }

            public MasterDataDataType SystemUnderTest()
            {
                return new MasterDataDataType(_masterDataMock.Object);
            }

            public Fixture Having(string data)
            {
                _masterDataMock.Setup(m => m.HasValueIgnoreCase(data)).Returns(true);
                _masterDataMock.Setup(m => m.ParseIgnoreCase(data)).Returns(new MasterDataValue(data));
                return this;
            }
        }

        [Fact]
        public void MasterDataType_ShouldBeOfTypeIDataType()
        {
            new Fixture().SystemUnderTest().Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public void New_ShouldReturnMasterDataType_WhenPassedMasterValues()
        {
            var masterDataMock = new Mock<IMasterDataList>();
            var sut = new Fixture().WithMasterData(masterDataMock).SystemUnderTest();
            sut.Should().NotBeNull();
        }

        [Fact]
        public async void Parse_ShouldReturnValue_WhenPassedValueFromMasterData()
        {
            var sut = new Fixture().Having("AP").SystemUnderTest();
            var result = await sut.Parse("AP");
            result.Should().Be("AP");
        }
    }
}