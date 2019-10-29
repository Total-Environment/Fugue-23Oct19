using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ControllerAdaptersTests
{
    public class MaterialDataAdapterTests
    {
        private static MaterialDataDto StubMaterialDataDto(string materialGroup)
        {
            return new MaterialDataDto
            {
                Headers = new List<HeaderDto>
                {
                    new HeaderDto
                    {
                        Name = "General",
                        Columns = new List<ColumnDto>
                        {
                            new ColumnDto
                            {
                                Name = "Material Name",
                                Value = "Murrum",
                                Key = "material_name"
                            }
                        }
                    }
                },
                Group = materialGroup
            };
        }

        private static MaterialData GetMaterialData(string group, string id = null)
        {
            var materialData = new MaterialData
            {
                Group = group,
                Id = id
            };
            return materialData;
        }

        [Fact]
        public void ToMaterialData_ShouldMaterialData_WhenMaterialDtoIsPassed()
        {
            var materialDataDto = StubMaterialDataDto("Clay");
            var materialData = GetMaterialData("Clay");
        }
    }
}