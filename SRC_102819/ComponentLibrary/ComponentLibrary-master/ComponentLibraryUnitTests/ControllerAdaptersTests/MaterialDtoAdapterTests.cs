using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerAdaptersTests
{
    public class MaterialDtoAdapterTests
    {
        [Fact]
        public void FromMaterial_ShouldReturnMaterialDto_WhenMaterialIsPassed()
        {
            var materialDataDto = StubMaterialDataDto("Clay");
            var material = GetMaterial("Clay");

            var result = MaterialDtoAdapter.FromMaterial(material);

            result.Group.Should().Be("Clay");
            result.Id.Should().Be(material.Id);
            result.Headers.Any(h => h.Name == "General").Should().Be(true);
            result.Headers.First(h => h.Name == "General").Columns.Any(c => c.Name == "Material Name").Should().Be(true);
            result.Headers.First(h => h.Name == "General")
                .Columns.First(c => c.Name == "Material Name")
                .Value.Should()
                .Be("Murrum");
        }

        [Fact]
        public void ToMaterial_ShouldMaterialData_WhenMaterialDtoIsPassed()
        {
            var materialDataDto = StubMaterialDataDto("Clay");
            var material = GetMaterial("Clay");

            var result = MaterialDtoAdapter.ToMaterial(materialDataDto);

            result.Group.Should().Be("Clay");
            result.Id.Should().Be(material.Id);
            result.Headers.Any(h => h.Name == "General").Should().Be(true);
            result.Headers.Any(h => h.Key == "general").Should().Be(true);
            result.Headers.First(h => h.Name == "General").Columns.Any(c => c.Name == "Material Name").Should().Be(true);
            result.Headers.First(h => h.Key == "general").Columns.Any(c => c.Key == "material_name").Should().Be(true);
            result.Headers.First(h => h.Name == "General")
                .Columns.First(c => c.Name == "Material Name")
                .Value.Should()
                .Be("Murrum");
        }

        private static Material GetMaterial(string group, string id = null)
        {
            var materialData = new Material
            {
                Group = group,
                Id = id,
                Headers = new List<HeaderData>
                {
                    new HeaderData("General", "general")
                    {
                        Columns = new List<IColumnData>(){new ColumnData("Material Name", "Material Name", "Murrum")}
                    }
                }
            };
            materialData.ComponentDefinition = new MaterialDefinition(group)
            {
                Headers = new List<IHeaderDefinition>()
                {
                    new HeaderDefinition("General", "general", new List<IColumnDefinition>
                    {
                        new ColumnDefinition("Material Name", "Material Name", new StringDataType())
                    })
                }
            };
            return materialData;
        }

        private static MaterialDataDto StubMaterialDataDto(string materialGroup)
        {
            return new MaterialDataDto
            {
                Headers = new List<HeaderDto>
                {
                    new HeaderDto
                    {
                        Name = "General",
                        Key = "general",
                        Columns = new List<ColumnDto>
                        {
                            new ColumnDto
                            {
                                Name = "Material Name",
                                Key = "material_name",
                                Value = "Murrum",
                            }
                        }
                    }
                },
                Group = materialGroup
            };
        }
    }
}