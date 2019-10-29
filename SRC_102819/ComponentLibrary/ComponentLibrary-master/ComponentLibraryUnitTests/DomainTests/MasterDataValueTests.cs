using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class MasterDataValueTests
    {
        [Fact]
        public void New_ShouldReturnMasterData_WhenPassedAString()
        {
            var masterDataValue = new MasterDataValue("sattar");
            masterDataValue.Value.Should().Be("sattar");
        }
    }
}