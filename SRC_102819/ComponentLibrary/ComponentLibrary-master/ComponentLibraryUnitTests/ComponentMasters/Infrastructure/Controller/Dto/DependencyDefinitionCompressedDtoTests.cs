using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Controller.Dto
{
    public class DependencyDefinitionCompressedDtoTests
    {
        [Fact]
        public void Sort_Should_SortBlockByName_AndAlsoChildren()
        {
            var sut = new BlockDto("Parent");

            sut.AddChild("A,P,G".Split(','));
            sut.AddChild("A,P,F".Split(','));
            sut.AddChild("A,C".Split(','));
            sut.AddChild(new string[] { "B" });

            var first = sut.Children.First();
            first.Name.Should().Be("A");
            first.Children.First().Name.Should().Be("C");
            first.Children.Last().Name.Should().Be("P");
            first.Children.Last().Children.First().Name.Should().Be("F");
            first.Children.Last().Children.Last().Name.Should().Be("G");
            sut.Children.Last().Name.Should().Be("B");
        }
    }
}