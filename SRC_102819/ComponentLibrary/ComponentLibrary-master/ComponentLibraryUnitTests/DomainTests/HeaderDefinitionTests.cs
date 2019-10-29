using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class HeaderDefinitionTest
    {
        [Fact]
        public void HeaderDefinition_IsAssignableTo_IHeaderDefinition()
        {
            var sut = new Fixture().SystemUnderTest();
            sut.Should().BeAssignableTo<IHeaderDefinition>();
        }

        [Fact]
        public void New_ShouldHaveNameAndHeaders()
        {
            const string name = "abdul";
            var columnDefinitions = MockColumns();
            var sut = new Fixture().WithName(name).WithColumns(columnDefinitions).SystemUnderTest();
            sut.Name.Should().Be(name);
            sut.Columns.Should().Equal(columnDefinitions);
        }

        [Fact]
        public async void Parse_ShouldReturnHeaderData_WhenDependencyDefinitionReturnTrue()
        {
            var columnName = "name";
            var columnValue = "1";
            var columnStub = new ColumnDefinitionStub()
                .HavingSomeName(columnName)
                .WhichParses(columnValue, 1).Stub();
            var columnDataDictionary = new Dictionary<string, string> { { columnName, columnValue } };
            var sut = new Fixture()
                .WithName("somename")
                .WithColumns(new List<IColumnDefinition> { columnStub })
                .WithDependencyDefinitionValidate(columnDataDictionary, true);

            var result = await sut.SystemUnderTest().Parse(new Dictionary<string, object> { { columnName, columnValue } });

            result[columnName].Should().Be(1);
            sut.VerifyExpectations();
        }

        [Fact]
        public async void Parse_ShouldReturnHeaderData_WhenDependencyIsNullAndIsPassedDictionary()
        {
            const string columnName = "Sattar";
            const string headerName = "sattar";
            var sut =
                new Fixture().WithName(headerName)
                    .WithNullDependency()
                    .WithColumns(MockColumnsWhichParse(columnName, 1, 1))
                    .SystemUnderTest();
            var result = await sut.Parse(new Dictionary<string, object> { { columnName, 1 } });
            result.Name.Should().Be(headerName);
            result.Columns.First(c => c.Name == columnName).Value.Should().Be(1);
        }

        [Fact]
        public async void Parse_ShouldReturnHeaderData_WhenPassedADictionaryWithoutColumnValue()
        {
            const string columnName = "Sattar";
            const string headerName = "sattar";
            var sut =
                new Fixture().WithName(headerName)
                    .WithColumns(MockColumnsWhichParse(columnName, null, null))
                    .SystemUnderTest();
            var result = await sut.Parse(new Dictionary<string, object>());
            result.Name.Should().Be(headerName);
            result.Columns.First(c => c.Name == columnName).Value.Should().BeNull();
        }

        [Fact]
        public async void Parse_ShouldReturnHeaderData_WhenPassedDictionary()
        {
            const string columnName = "Sattar";
            const string headerName = "sattar";
            var sut =
                new Fixture().WithName(headerName)
                    .WithColumns(MockColumnsWhichParse(columnName, 1, 1))
                    .SystemUnderTest();
            var result = await sut.Parse(new Dictionary<string, object> { { columnName, 1 } });
            result.Name.Should().Be(headerName);
            result.Columns.First(c => c.Name == columnName).Value.Should().Be(1);
        }

        [Fact]
        public void Parse_ShouldThrowFormatException_WhenDependencyDefinitionReturnFalse()
        {
            var columnName = "name";
            var columnValue = "1";
            var columnStub = new ColumnDefinitionStub()
                .HavingSomeName(columnName)
                .WhichParses(columnValue, 1).Stub();
            var columnDataDictionary = new Dictionary<string, string> { { columnName, columnValue } };
            var sut = new Fixture()
                .WithName("somename")
                .WithColumns(new List<IColumnDefinition> { columnStub })
                .WithDependencyDefinitionValidate(columnDataDictionary, false);

            Func<Task> act =
                async () =>
                    await sut.SystemUnderTest().Parse(new Dictionary<string, object> { { columnName, columnValue } });
            act.ShouldThrow<FormatException>()
                .WithMessage("Failed parsing somename:One of the dependent values is not valid.");
        }

        private static List<IColumnDefinition> MockColumns()
        {
            var columns = new List<IColumnDefinition> { new Mock<IColumnDefinition>().Object };
            return columns;
        }

        private static List<IColumnDefinition> MockColumnsWhichParse(string name, object data, object parsedValue)
        {
            var mockColumnData = new Mock<IColumnData>();
            mockColumnData.Setup(c => c.Name).Returns(name);
            mockColumnData.Setup(m => m.Value).Returns(parsedValue);

            var column = new Mock<IColumnDefinition>();
            column.Setup(c => c.Name).Returns(name);
            // column.Setup(c => c.Parse(data)).Returns(Task.FromResult(mockColumnData.Object));
            column.Setup(c => c.Parse(data)).ReturnsAsync(mockColumnData.Object);

            var columns = new List<IColumnDefinition> { column.Object };
            return columns;
        }

        private class ColumnDefinitionStub
        {
            private readonly Mock<IColumnDefinition> _stub = new Mock<IColumnDefinition>();
            private string _name;

            public ColumnDefinitionStub HavingSomeName(string name)
            {
                _name = name;
                return this;
            }

            public IColumnDefinition Stub()
            {
                _stub.Setup(s => s.Name).Returns(_name);
                return _stub.Object;
            }

            public ColumnDefinitionStub WhichParses(string input, int output)
            {
                _stub.Setup(s => s.Parse(input))
                    .ReturnsAsync(new ColumnData(_name, _name, output));
                return this;
            }
        }

        private class Fixture
        {
            private readonly Mock<IDependencyDefinition> _dependencyDef = new Mock<IDependencyDefinition>();
            private readonly List<Action> _verifications = new List<Action>();
            private List<IColumnDefinition> _columnDefinitions = new List<IColumnDefinition>();
            private List<IDependencyDefinition> _dependencyDefinitions = new List<IDependencyDefinition>();
            private string _name;

            public HeaderDefinition SystemUnderTest()
            {
                return new HeaderDefinition(_name, _name, _columnDefinitions) { Dependency = _dependencyDefinitions };
            }

            public void VerifyExpectations()
            {
                _verifications.ForEach(v => v.Invoke());
            }

            public Fixture WithColumns(List<IColumnDefinition> columnDefinitions)
            {
                _columnDefinitions = columnDefinitions;
                return this;
            }

            public Fixture WithDependencyDefinitionValidate(IDictionary<string, string> dic, bool output)
            {
                _dependencyDef.Setup(d => d.Validate(dic)).Returns(output);
                _verifications.Add(() => _dependencyDef.Verify(d => d.Validate(dic), Times.Once));
                _dependencyDefinitions.Add(_dependencyDef.Object);
                return this;
            }

            public Fixture WithName(string name)
            {
                _name = name;
                return this;
            }

            public Fixture WithNullDependency()
            {
                _dependencyDefinitions = null;
                return this;
            }
        }
    }
}