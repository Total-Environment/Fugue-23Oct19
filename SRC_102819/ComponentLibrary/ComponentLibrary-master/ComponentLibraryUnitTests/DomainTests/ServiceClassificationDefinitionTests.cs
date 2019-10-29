using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests
{
    public class ServiceClassificationDefinitionTests
    {
        [Fact]
        public void ServiceClassfiationDefinition_ShouldParseForValidDictionary()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>
            {
                {"FLOORING | DADO | PAVIOUR", "FLOORING | DADO | PAVIOUR - description"},
                {"Flooring", "Flooring - description"},
                {"Natural Stone", "Natural Stone - description"},
                {"Kota Blue", "Kota Blue - description"}
            };
            var serviceClassificationDefinition =
                ClassificationDefinition.Parse(serviceClassificationDefinitionDto);
            serviceClassificationDefinition.Should().NotBe(null);
            serviceClassificationDefinition.Value.ShouldBeEquivalentTo("FLOORING | DADO | PAVIOUR");
            serviceClassificationDefinition.Description.ShouldBeEquivalentTo("FLOORING | DADO | PAVIOUR - description");
            serviceClassificationDefinition.ServiceClassificationDefinitions.First()
                .Value.ShouldBeEquivalentTo("Flooring");
            serviceClassificationDefinition.ServiceClassificationDefinitions.First()
                .Description.ShouldBeEquivalentTo("Flooring - description");
        }

        [Fact]
        public void ServiceClassfiationDefinition_ShouldReturnNullForEmptyDictionary()
        {
            var serviceClassificationDefinitionDto = new Dictionary<string, string>();
            var serviceClassificationDefinition =
                ClassificationDefinition.Parse(serviceClassificationDefinitionDto);
            serviceClassificationDefinition.Should().BeNull();
        }

        [Fact]
        public void ServiceClassfiationDefinition_ShouldThrowArgumentExceptionForNullDictionary()
        {
            Dictionary<string, string> serviceClassificationDefinitionDto = null;
            Action act = () => ClassificationDefinition.Parse(serviceClassificationDefinitionDto);
            act.ShouldThrow<ArgumentException>();
        }
    }
}