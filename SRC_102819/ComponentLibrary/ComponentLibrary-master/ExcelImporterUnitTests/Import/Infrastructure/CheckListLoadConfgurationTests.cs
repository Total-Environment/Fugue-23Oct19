using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using Xunit;

namespace TE.ComponentLibrary.ExcelImporter.UnitTests.Import.Infrastructure
{
    public class CheckListLoadConfgurationTests
    {
        [Fact]
        public void Create_ParseConfigurationJson()
        {
            var configurationJson =
                "{\"HeaderConfiguration\":[{\"Key\":\"B\",\"Value\":\"S.No\"},{\"Key\":\"C\",\"Value\":\"Work Description\"},{\"Key\":\"D\",\"Value\":\"Selected\"},{\"Key\":\"E\",\"Value\":\"Remarks\"}],\"NullColumnReference\":\"B\",\"DataRowIndex\":10}";
            var configuration =
                JsonConvert.DeserializeObject<TabularDataLoadConfiguration>(configurationJson);

            configuration.DataRowIndex.Should().Be(10);
            configuration.NullColumnReference.Should().Be("B");
            configuration.HeaderConfiguration.Count().Should().Be(4);
            configuration.HeaderConfiguration.FirstOrDefault().Key.Should().Be("B");
            configuration.HeaderConfiguration.FirstOrDefault().Value.Should().Be("S.No");
            configuration.HeaderConfiguration.LastOrDefault().Key.Should().Be("E");
            configuration.HeaderConfiguration.LastOrDefault().Value.Should().Be("Remarks");
        }
    }
}