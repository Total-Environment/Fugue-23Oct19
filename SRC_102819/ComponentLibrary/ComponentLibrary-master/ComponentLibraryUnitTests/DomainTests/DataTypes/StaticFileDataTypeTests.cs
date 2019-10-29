using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class StaticFileDataTypeTests
    {
        private class Fixture
        {
            private readonly Mock<IStaticFileRepository> _mockRepository = new Mock<IStaticFileRepository>();

            public StaticFileDataType SystemUnderTest()
            {
                return new StaticFileDataType(_mockRepository.Object);
            }

            public Fixture WithStaticFileId(string id)
            {
                _mockRepository.Setup(m => m.GetById(id))
                    .ReturnsAsync(new StaticFile(id, "sattar"));
                return this;
            }

            public Fixture WithoutStaticFileId(string id)
            {
                _mockRepository.Setup(m => m.GetById(id)).Throws(new ResourceNotFoundException("s"));
                return this;
            }
        }

        [Fact]
        public void It_ShouldBeOfTypeIDataType()
        {
            var dt = new Fixture().SystemUnderTest();
            dt.Should().BeAssignableTo<IDataType>();
        }

        [Fact]
        public async void Parse_ShouldReturnAStaticFile_WhenPassedADictionary()
        {
            const string id = "MNS0001";
            var input = new Dictionary<string, object>
            {
                {"Id", id}
            };
            var dt = new Fixture().WithStaticFileId(id).SystemUnderTest();
            ((StaticFile)await dt.Parse(input)).Id.Should().Be(id);
        }

        [Fact]
        public async void Parse_ShouldReturnAStaticFile_WhenPassedId()
        {
            const string id = "MNS0001";
            var dt = new Fixture().WithStaticFileId(id).SystemUnderTest();
            ((StaticFile)await dt.Parse(id)).Id.Should().Be(id);
        }

        [Fact]
        public async void Parse_ShouldReturnPassedString_WhenNonExistingIdIsPassed()
        {
            const string id = "sattar";
            var dt = new Fixture().WithoutStaticFileId(id).SystemUnderTest();
            (await dt.Parse(id)).Should().Be(id);
        }
    }
}