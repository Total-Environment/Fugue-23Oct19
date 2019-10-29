using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ExcelImporter.Code.Components;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests
{
    public class MaterialConverterTests
    {
        [Fact]
        public void WriteJsonWhenGivenAUnitTypeShouldWriteAppropriateMethods()
        {
            var columnCustomCell = new Mock<ICustomCell>();
            columnCustomCell.Setup(c => c.Value).Returns("Material Name");
            var dataCustomCell = new Mock<ICustomCell>();
            dataCustomCell.Setup(c => c.Value).Returns("5");

            var materialDefinitionData =
                "{ \"headers\": [{\r\n      \"columns\": [\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Unit\",\r\n            \"subType\": \"mm\"\r\n          },\r\n          \"name\": \"Material Name\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Material Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Array\",\r\n            \"subType\": {\r\n              \"name\": \"String\",\r\n              \"subType\": \"\"\r\n            }\r\n          },\r\n          \"name\": \"Image\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Short Description\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Shade Number\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Shade Description\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Unit Of Measure\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"HSM Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"SAP Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Part of eBuild Library\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Generic\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Manufactured\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"MasterData\",\r\n            \"subType\": \"status\"\r\n          },\r\n          \"name\": \"Material Status\"\r\n        }\r\n      ],\r\n      \"name\": \"General\"\r\n    }]}";
            var materialDefinition = JsonConvert.DeserializeObject<MaterialDefinitionDao>(materialDefinitionData);
            var headers = new List<Header>
            {
                new Header("General")
                {
                      ColumnCells = new List<Tuple<ICustomCell, string>>() {new Tuple<ICustomCell, string>(columnCustomCell.Object, "mm")}
                }
            };

            var columnNameCellDataCellDictonary = new Dictionary<ICustomCell, ICustomCell>
            {
                {columnCustomCell.Object, dataCustomCell.Object}
            };

            var jsonWriter = new Mock<JsonWriter>();
            jsonWriter.Setup(js => js.WritePropertyName(It.IsAny<string>()));
            jsonWriter.Setup(js => js.WriteValue(It.IsAny<string>()));
            var jsonSerializer = new Mock<JsonSerializer>();

            var materialConverter = new ComponentConverter(headers, materialDefinition);
            materialConverter.WriteJson(jsonWriter.Object, columnNameCellDataCellDictonary, jsonSerializer.Object);

            jsonWriter.Verify(js => js.WriteStartObject(), Times.Exactly(3));
            jsonWriter.Verify(js => js.WritePropertyName(It.IsAny<string>()), Times.Exactly(4));
            jsonWriter.Verify(js => js.WriteValue("mm"), Times.Exactly(1));

            jsonWriter.Verify(js => js.WriteValue("5"), Times.Exactly(1));
            jsonWriter.Verify(js => js.WriteEndObject(), Times.Exactly(3));
        }

        [Fact]
        public void WriteJsonShouldThrowInvalidDataExceptionWhenGivenMaterialCodeHasNullValue()
        {
            var columnCustomCell = new Mock<ICustomCell>();
            columnCustomCell.Setup(c => c.Value).Returns("Material Code");
            var dataCustomCell = new Mock<ICustomCell>();

            var materialDefinitionData =
                "{ \"headers\": [{\r\n      \"columns\": [\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Unit\",\r\n            \"subType\": \"mm\"\r\n          },\r\n          \"name\": \"Material Name\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Material Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Array\",\r\n            \"subType\": {\r\n              \"name\": \"String\",\r\n              \"subType\": \"\"\r\n            }\r\n          },\r\n          \"name\": \"Image\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Short Description\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Shade Number\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Shade Description\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Unit Of Measure\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"HSM Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"SAP Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Part of eBuild Library\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Generic\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Manufactured\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"MasterData\",\r\n            \"subType\": \"status\"\r\n          },\r\n          \"name\": \"Material Status\"\r\n        }\r\n      ],\r\n      \"name\": \"General\"\r\n    }]}";
            var materialDefinition = JsonConvert.DeserializeObject<MaterialDefinitionDao>(materialDefinitionData);
            var headers = new List<Header>
            {
                new Header("General")
                {
                      ColumnCells = new List<Tuple<ICustomCell, string>>()
                      {
                          new Tuple<ICustomCell, string>(columnCustomCell.Object, "mm"),
                      }
                }
            };

            var columnNameCellDataCellDictonary = new Dictionary<ICustomCell, ICustomCell>
            {
                {columnCustomCell.Object, dataCustomCell.Object}
            };

            var jsonWriter = new Mock<JsonWriter>();
            jsonWriter.Setup(js => js.WritePropertyName(It.IsAny<string>()));
            jsonWriter.Setup(js => js.WriteValue(It.IsAny<string>()));
            var jsonSerializer = new Mock<JsonSerializer>();

            var materialConverter = new ComponentConverter(headers, materialDefinition);

            Action action = () => materialConverter.WriteJson(jsonWriter.Object, columnNameCellDataCellDictonary, jsonSerializer.Object);

            action.ShouldThrow<InvalidDataException>("Material Code cannot have a null value.");
        }

        [Fact]
        public void WriteJsonShouldThrowInvalidDataExceptionWhenGivenMaterialCodeGroupDoesNotMatchWithDefinition()
        {
            var columnCustomCell = new Mock<ICustomCell>();
            columnCustomCell.Setup(c => c.Value).Returns("Material Code");
            var dataCustomCell = new Mock<ICustomCell>();
            dataCustomCell.Setup(c => c.Value).Returns("SFT000001");

            var materialDefinitionData =
                "{ \"headers\": [{\r\n      \"columns\": [\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Unit\",\r\n            \"subType\": \"mm\"\r\n          },\r\n          \"name\": \"Material Name\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Material Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Array\",\r\n            \"subType\": {\r\n              \"name\": \"String\",\r\n              \"subType\": \"\"\r\n            }\r\n          },\r\n          \"name\": \"Image\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Short Description\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Shade Number\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Shade Description\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Unit Of Measure\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"HSM Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"String\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"SAP Code\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Part of eBuild Library\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Generic\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"Boolean\",\r\n            \"subType\": \"\"\r\n          },\r\n          \"name\": \"Manufactured\"\r\n        },\r\n        {\r\n          \"dataType\": {\r\n            \"name\": \"MasterData\",\r\n            \"subType\": \"status\"\r\n          },\r\n          \"name\": \"Material Status\"\r\n        }\r\n      ],\r\n      \"name\": \"General\"\r\n    }],\r\n \"code\": \"GLS\"}";
            var materialDefinition = JsonConvert.DeserializeObject<MaterialDefinitionDao>(materialDefinitionData);
            var headers = new List<Header>
            {
                new Header("General")
                {
                      ColumnCells = new List<Tuple<ICustomCell, string>>()
                      {
                          new Tuple<ICustomCell, string>(columnCustomCell.Object, "mm"),
                      }
                }
            };

            var columnNameCellDataCellDictonary = new Dictionary<ICustomCell, ICustomCell>
            {
                {columnCustomCell.Object, dataCustomCell.Object}
            };

            var jsonWriter = new Mock<JsonWriter>();
            jsonWriter.Setup(js => js.WritePropertyName(It.IsAny<string>()));
            jsonWriter.Setup(js => js.WriteValue(It.IsAny<string>()));
            var jsonSerializer = new Mock<JsonSerializer>();

            var materialConverter = new ComponentConverter(headers, materialDefinition);

            Action action = () => materialConverter.WriteJson(jsonWriter.Object, columnNameCellDataCellDictonary, jsonSerializer.Object);

            action.ShouldThrow<InvalidDataException>("Invalid Material group code in Material Code.");
        }
    }
}