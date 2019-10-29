using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.Units
{
    public class StaticFileTests
    {
        [Fact]
        public void Equals_ShouldCompareTwoStaticFileObjectsWithDifferentValues_ReturnFalse()
        {
            var staticFile1 = new StaticFile("qweret", "image1.jpg");
            var staticFile2 = new StaticFile("sattar", "image1.jpg");

            staticFile1.Should().NotBe(staticFile2);
        }

        [Fact]
        public void Equals_ShouldCompareTwoStaticFileObjectsWithSameValues_ReturnTrue()
        {
            var staticFile1 = new StaticFile("qweret", "image1.jpg");
            var staticFile2 = new StaticFile("qweret", "image1.jpg");

            staticFile1.Should().Be(staticFile2);
        }

        [Fact]
        public void ToString_ShouldReturnId_Always()
        {
            var staticFile1 = new StaticFile("qweret", "image1.jpg");

            staticFile1.ToString().Should().Be("qweret");
        }
    }
}