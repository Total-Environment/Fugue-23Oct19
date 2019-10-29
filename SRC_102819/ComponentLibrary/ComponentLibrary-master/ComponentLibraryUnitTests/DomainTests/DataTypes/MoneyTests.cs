using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.DomainTests.DataTypes
{
    public class MoneyTest
    {
        [Fact]
        public void It_ShouldReturnMoney_WhenPassedCurrencyAndValueAsZero()
        {
            var money = new MoneyValue(0, "INR");
            money.Amount.Should().Be(0);
            money.Currency.Should().Be("INR");
        }

        [Fact]
        public void It_ShouldReturnMoney_WhenPassedDecimalAndCurrency()
        {
            var money = new MoneyValue(3000, "INR");
            money.Amount.Should().Be(3000);
            money.Currency.Should().Be("INR");
        }

        [Fact]
        public void It_ShouldThrowArgumentNullException_WhenPassedCurrencyAsNull()
        {
            Action act = () => new MoneyValue(1000, null);
            act.ShouldThrow<ArgumentNullException>().WithMessage("Currency is required.");
        }
    }
}