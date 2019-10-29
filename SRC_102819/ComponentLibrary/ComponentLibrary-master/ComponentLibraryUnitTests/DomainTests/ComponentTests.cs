using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class ComponentTests
    {
        [Fact]
        public void Component_ShouldUpdateSearchKeywordsWithId_WhenIdIsSet()
        {
            IComponent component = new Material();
            var componentId = "Something";
            component.Id = componentId;
            component.SearchKeywords.Should().Contain(componentId);
        }
    }
}