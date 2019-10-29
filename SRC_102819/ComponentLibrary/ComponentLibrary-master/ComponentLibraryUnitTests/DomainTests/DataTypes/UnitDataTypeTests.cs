using System;
using System.Collections.Generic;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class UnitDataTypeTests
    {
        [Fact]
        public void Ctor_Should_AcceptUnitOfMeasure()
        {
            var unitDataType = new UnitDataType("mm");

            unitDataType.Should().NotBeNull();
        }

        [Fact]
        public async void Parse_Should_AcceptValueOnlyIfUnitMatchesWithSpecifiedUnit()
        {
            var input = new Dictionary<string, object> {{"Value", 20}, {"Type", "mm"}};
            var unitDataType = new UnitDataType("mm");

            var actualValue = await unitDataType.Parse(input);
            var expectedValue = new UnitValue(20, "mm");

            actualValue.Should().Be(expectedValue);
        }

        [Fact]
        public void Parse_Should_ThrowArgumentExceptionIfUnitMatchesWithSpecifiedUnit()
        {
            var input = new Dictionary<string, object> {{"Value", 20}, {"Type", "mm"}};
            var unitDataType = new UnitDataType("cm");

            Action invoke = () => unitDataType.Parse(input);

            invoke.ShouldThrow<FormatException>().WithMessage("Expected cm, got mm");
        }

        [Fact]
        public void Parse_Should_ThrowFormatExceptionWhenTypeIsPassed()
        {
            var input = new Dictionary<string, object> {{"Value", 20}};
            var unitDataType = new UnitDataType("mm");

            Action act = () => unitDataType.Parse(input);
            act.ShouldThrow<FormatException>().WithMessage("Unit is not present.");
        }

        [Fact]
        public void Parse_Should_ThrowFormatExceptionWhenValueIsPassed()
        {
            var input = new Dictionary<string, object> {{"Type", "mm"}};
            var unitDataType = new UnitDataType("mm");

            Action act = () => unitDataType.Parse(input);
            act.ShouldThrow<FormatException>().WithMessage("Value is not present.");
        }
    }
}