using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class AssetDefinitionTests
    {
        [Fact]
        public void Class_ShouldExtendComponentDefinition()
        {
            var assetDefinition = new AssetDefinition("someName");
            assetDefinition.Should().BeAssignableTo<MaterialDefinition>();
        }

        [Fact]
        public void Merge_ShouldReturnMaterialDefinitionWithAssetDefinitonHeaders_WhenPassedMaterialDefinition()
        {
            var assetDefinition = new AssetDefinition("Machine") {Code = "MCH"};
            var assetHeaderDefinition = new Mock<IHeaderDefinition>().Object;
            var materialHeaderDefinition = new Mock<IHeaderDefinition>().Object;
            assetDefinition.Headers = new List<IHeaderDefinition>
            {
                assetHeaderDefinition
            };
            var materialDefinition = new MaterialDefinition("Machine")
            {
                Code = "MCH",
                Headers = new List<IHeaderDefinition>
                {
                    materialHeaderDefinition
                }
            };

            var result = assetDefinition.Merge(materialDefinition);

            result.Name.Should().Be("Machine");
            result.Code.Should().Be("MCH");
            result.Headers.Should()
                .BeEquivalentTo(new List<IHeaderDefinition> {assetHeaderDefinition, materialHeaderDefinition});
        }
    }
}