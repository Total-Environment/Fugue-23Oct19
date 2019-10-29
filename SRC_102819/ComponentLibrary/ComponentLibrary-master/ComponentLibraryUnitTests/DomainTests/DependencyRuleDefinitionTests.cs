using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class DependencyRuleDefinitionTests
    {
        private class Fixture
        {
            private readonly IList<string> _columns = new List<string>();
            private string _name;

            public DependencyDefinition Sut()
            {
                return new DependencyDefinition(_name, _columns);
            }

            public Fixture HavingSomeName()
            {
                _name = "somename";
                return this;
            }

            public Fixture HavingColumnName(string columnname)
            {
                _columns.Add(columnname);
                return this;
            }
        }

        [Fact]
        public void Creation_ShouldThrowArgumentException_WhenListOfColumnNamePassedIsNullOrEmpty()
        {
            Action nullList = () => new DependencyDefinition("somename", null);
            Action emptyList = () => new DependencyDefinition("somename", new List<string>());

            nullList.ShouldThrow<ArgumentException>();
            emptyList.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_ShouldThrowArgumentException_WhenNameIsPassedAsNullOrWhitespace()
        {
            Action action = () => new DependencyDefinition("", new Mock<IList<string>>().Object);
            action.ShouldThrow<ArgumentException>();
        }
    }
}