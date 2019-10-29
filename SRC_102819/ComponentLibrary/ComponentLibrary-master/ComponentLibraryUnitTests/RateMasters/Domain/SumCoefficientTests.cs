using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Domain
{
    public class SumCoefficientTests
    {

        private IBank BankStubThatConvert25RupeeTo25Rupee()
        {
            return GetBankThatConvert(25, "INR", 25, "INR").Object;
        }

        private IBank BankStubThatConvert50DollarTo5Rupee()
        {
            return GetBankThatConvert(50, "USD", 5, "INR").Object;
        }

        private Mock<IBank> GetBankThatConvert(decimal fromValue, string fromCurrency, decimal toValue,
            string toCurrency)
        {
            var bankStub = new Mock<IBank>();
            bankStub.Setup(b => b.ConvertTo(new Money(fromValue, fromCurrency, BankStub()), toCurrency))
                .ReturnsAsync(new Money(toValue, toCurrency, BankStub()));
            return bankStub;
        }

        private static IBank BankStub()
        {
            return new Mock<IBank>().Object;
        }

        private class Fixture
        {
            private string _name;
            private Money _value;

            public Fixture HavingSomeName()
            {
                _name = "Name";
                return this;
            }

            public Fixture HavingValueAs25Rupee()
            {
                _value = new Money(25, "INR", BankStub());
                return this;
            }

            public SumCoefficient SystemUnderTest()
            {
                return new SumCoefficient(_name, _value);
            }

            public Fixture HavingNullName()
            {
                _name = null;
                return this;
            }

            public Fixture HavingValueAsNull()
            {
                _value = null;
                return this;
            }

            public Fixture HavingValueAs50Dollar()
            {
                _value = new Money(50, "USD", BankStub());
                return this;
            }
        }

        [Fact]
        public void Apply_Should_ReturnSameValueToMoneyPassed()
        {
            var bankStub = BankStubThatConvert25RupeeTo25Rupee();
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs25Rupee()
                .SystemUnderTest();

            var result = systemUnderTest.Apply(new Money(25, "INR", bankStub));

            result.Should().Be(new Money(25, "INR", bankStub));
        }

//        [Fact]
//        public void Apply_Should_ReturnMoneyInCurrencyOfPassedMoney()
//        {
//            var bankStub = BankStubThatConvert50DollarTo5Rupee();
//            var systemUnderTest = new Fixture()
//                .HavingValueAs50Dollar()
//                .HavingSomeName()
//                .SystemUnderTest();
//
//            var result = systemUnderTest.Apply(new Money(10, "INR", bankStub));
//
//            result.Currency.Should().Be("INR");
//        }

        [Fact]
        public void Apply_Should_ThrowArgumentExceptionIfMoneyPassedIsNull()
        {
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs25Rupee()
                .SystemUnderTest();

            Action action = () => systemUnderTest.Apply(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptioIfNameIsPassedAsNull()
        {
            Action action = () => new Fixture()
            .HavingNullName()
            .HavingValueAs25Rupee()
            .SystemUnderTest();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionIfValueIsPassedAsNull()
        {
            Action action = () => new Fixture()
            .HavingSomeName()
            .HavingValueAsNull()
            .SystemUnderTest();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void SumCoefficient_Should_ImplementICoefficient()
        {
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs25Rupee()
                .SystemUnderTest();

            systemUnderTest.Should().BeAssignableTo<ICoefficient>();
        }

        [Fact]
        public void Equal_Should_ReturnTrueIfAllPropertyAreEqual()
        {
            var systemUnderTest = new Fixture()
                .HavingSomeName()
                .HavingValueAs25Rupee()
                .SystemUnderTest();
            var anotherSystem = new Fixture()
                .HavingSomeName()
                .HavingValueAs25Rupee()
                .SystemUnderTest();

            var result = systemUnderTest.Equals(anotherSystem);

            result.Should().Be(true);
        }
    }
}