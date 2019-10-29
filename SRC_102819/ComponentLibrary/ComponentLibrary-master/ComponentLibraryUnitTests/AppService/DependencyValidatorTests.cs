using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class DependencyValidatorTests
    {
        [Fact]
        public void Validate_ShouldThrowArgumentException_WhenDepdendencyDefinitionReturnFalse()
        {
            var dependencyValidator = new DependencyValidator();
            var mockDependency = new Mock<IDependencyDefinition>();
            mockDependency.Setup(m => m.Validate(It.IsAny<IEnumerable<string>>())).Returns(false);

            var result = dependencyValidator.Validate(mockDependency.Object, new HeaderData("general", "general"));

            result.Item1.Should().BeFalse();
            result.Item2.Should().Be("Invalid column dependency for header general");
        }

        [Fact]
        public void Validate_ShouldThrowArgumentException_WhenDependencyDefinitionIsNull()
        {
            var dependencyValidator = new DependencyValidator();

            Action action = () => dependencyValidator.Validate(null, new HeaderData("general", "general"));

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Validate_ShouldThrowArgumentException_WhenHeaderDataIsNull()
        {
            var dependencyValidator = new DependencyValidator();

            Action action = () => dependencyValidator.Validate(null, new HeaderData("general", "general"));

            action.ShouldThrow<ArgumentException>();
        }
    }
}