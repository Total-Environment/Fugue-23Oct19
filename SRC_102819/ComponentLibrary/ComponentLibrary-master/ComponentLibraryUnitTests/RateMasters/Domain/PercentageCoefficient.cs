using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Domain
{
    public class PercentageCoefficientTest
    {
        private class Fixture
        {
            private string _name;
            private decimal _value;

            public Fixture HavingSomeName()
            {
                _name = "Name";
                return this;
            }

            public Fixture HavingValueAs50()
            {
                _value = 50m;
                return this;
            }

            public PercentageCoefficient SystemUnderTest()
            {
                return new PercentageCoefficient(_name, _value);
            }

            public Fixture HavingNameAsNull()
            {
                _name = null;
                return this;
            }

            public Fixture HavingValueAsZero()
            {
                _value = 0m;
                return this;
            }

            public Fixture HavingNegativeValue()
            {
                _value = -1m;
                return this;
            }
        }

        [Fact]
        public void Apply_Should_ReturnValueAfterPercentage()
        {
            var bank = new Mock<IBank>().Object;
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs50()
                .SystemUnderTest();

            var result = systemUnderTest.Apply(new Money(10, "INR", bank));

            result.Should().Be(new Money(5, "INR", bank));
        }

        [Fact]
        public void Apply_Should_ThrowArgumentExceptionIfNullIsPassed()
        {
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs50()
                .SystemUnderTest();

            Action action = () => systemUnderTest.Apply(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ShouldArgumentExceptionIfNameIsNull()
        {
            Action action = () => new Fixture()
                .HavingNameAsNull()
                .HavingValueAs50()
                .SystemUnderTest();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ShouldThrowArgumentExceptionIfPercentageIsNegative()
        {
            Action negativePercentage = () => new Fixture()
                .HavingSomeName()
                .HavingNegativeValue()
                .SystemUnderTest();
            
            negativePercentage.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Should_ImplementICoefficient()
        {
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs50()
                .SystemUnderTest();

            systemUnderTest.Should().BeAssignableTo<ICoefficient>();
        }

        [Fact]
        public void Equals_Should_ReturnTrueIfAllAttributesAreEquals()
        {
            var coefficient = new Fixture()
                .HavingSomeName()
                .HavingValueAs50()
                .SystemUnderTest();
            var anotherCoefficient = new Fixture()
                .HavingSomeName()
                .HavingValueAs50()
                .SystemUnderTest();

            var result = coefficient.Equals(anotherCoefficient);

            result.Should().Be(true);
        }
    }
}