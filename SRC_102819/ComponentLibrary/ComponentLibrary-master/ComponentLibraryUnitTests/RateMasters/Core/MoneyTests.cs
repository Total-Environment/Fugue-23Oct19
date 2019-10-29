using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Core
{
    public class MoneyTests
    {
        [Fact]
        public async void Add_ShouldReturnSumOfMoneyOfDifferentCurrencies()
        {
            var fiveRupee = new Fixture()
                .HavingValue(5)
                .HavingCurrencyInr()
                .SystemUnderTest();
            var fiveDollar = new Fixture()
                .ThatConvert5RupeeTo1Dollar()
                .HavingValue(5)
                .HavingCurrencyUsd()
                .SystemUnderTest();

            var sum = await fiveDollar.Add(fiveRupee);

            sum.Should().Be(new Money(6, "USD", BankStub()));
        }

        [Fact]
        public async void Add_ShouldReturnSumOfTwoMoney()
        {
            var five = new Fixture()
                .ThatConvert5RupeeTo5Rupee()
                .HavingValue(5)
                .HavingCurrencyInr()
                .SystemUnderTest();
            var anotherFive = new Fixture()
                .HavingValue(5)
                .HavingCurrencyInr()
                .SystemUnderTest();

            var sum = await five.Add(anotherFive);

            sum.Should().Be(new Money(10, "INR", BankStub()));
        }

        [Fact]
        public async Task AddAndTimes_WhenAppliedTogetherShouldReturnValidResult()
        {
            var fiveRupee = new Fixture()
                .ThatConvert5DollarTo50Rupee()
                .HavingCurrencyInr()
                .HavingValue(5).SystemUnderTest();
            var fiveDollar = new Fixture()
                .HavingValue(5)
                .HavingCurrencyUsd().SystemUnderTest();

            var result = (await fiveRupee.Add(fiveDollar)).Times(2);

            result.Should().Be(new Money(110, "INR", BankStub()));
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionWhenBankIsPAssedAsNull()
        {
            Action action = () => new Money(10, "INR", null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void CurrencyShouldNotBeNullOrEmpty()
        {
            Action emptyException = () => new Fixture().HavingValue(5).HavingEmptyCurrency().SystemUnderTest();
            Action nullException = () => new Fixture().HavingValue(5).HavingNullCurrency().SystemUnderTest();

            emptyException.ShouldThrow<ArgumentException>();
            nullException.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Equal_ShouldReturn_ForTwo5Rupee()
        {
            var five = new Fixture().HavingValue(5).HavingCurrencyInr().SystemUnderTest();
            var secondFive = new Fixture().HavingValue(5).HavingCurrencyInr().SystemUnderTest();

            five.Should().Be(secondFive);
        }

        [Fact]
        public void Equal_ShouldReturnFalse_WhenFiveDollarAndFiveRuppeIsPassed()
        {
            var fiveRupee = new Fixture().HavingValue(5).HavingCurrencyInr().SystemUnderTest();
            var fiveDollar = new Fixture().HavingValue(5).HavingCurrencyUsd().SystemUnderTest();

            fiveDollar.Should().NotBe(fiveRupee);
            fiveRupee.Should().NotBe(fiveDollar);
        }

        [Fact]
        public void FiveRupee_ShouldBeFive()
        {
            var five = new Fixture().HavingValue(5).HavingCurrencyInr().SystemUnderTest();
            five.Value.Should().Be(5);
        }

        [Fact]
        public void FiveRupeeThreeTimes_ShouldBeFifteenRupee()
        {
            var five = new Fixture().HavingValue(5).HavingCurrencyInr().SystemUnderTest();
            var fifteen = five.Times(3);
            fifteen.Value.Should().Be(15);
            var twenty = five.Times(4);
            twenty.Value.Should().Be(20);
        }

        [Fact]
        public async void Money_ShouldBeComparable()
        {
            var money1 = new Money(1, "INR");
            var money2 = new Money(2, "INR");

            var result = await money1.CompareTo(money2);
            result.Should().Be(-1);
        }

        [Fact]
        public async void Money_WithDifferentCurrency_ShouldBeComparable()
        {
            var bank = new Mock<IBank>();
            bank.Setup(b => b.ConvertTo(It.IsAny<Money>(), "USD")).ReturnsAsync(new Money(25, "USD"));
            var twentyfiveRupee = new Money(1, "INR", bank.Object);

            var fiveDollar = new Money(25, "USD");

            var result = await twentyfiveRupee.CompareTo(fiveDollar);

            result.Should().Be(0);
        }

        [Fact]
        public void NegetiveMoneyShouldNotBeCreated()
        {
            Action exception = () => new Fixture().HavingNegativeValue().HavingCurrencyUsd().SystemUnderTest();
            exception.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async void TimeAndAdd_WhenAppliedTogetherShouldReturnValidResult()
        {
            var fiveDollar = new Fixture()
                .ThatConvert10RupeeTo1Dollar()
                .HavingValue(5)
                .HavingCurrencyUsd()
                .SystemUnderTest();
            var fiveRupee = new Fixture()
                .HavingValue(5)
                .HavingCurrencyInr()
                .SystemUnderTest();

            var result = await fiveDollar.Times(3).Add(fiveRupee.Times(2));

            result.Should().Be(new Money(16, "USD", BankStub()));
        }

        private static IBank BankStub()
        {
            return new Mock<IBank>().Object;
        }

        private class Fixture
        {
            private readonly Mock<IBank> _bank = new Mock<IBank>();
            private string _currency;
            private decimal _value;

            public Fixture HavingCurrencyInr()
            {
                _currency = "INR";
                return this;
            }

            public Fixture HavingCurrencyUsd()
            {
                _currency = "USD";
                return this;
            }

            public Fixture HavingEmptyCurrency()
            {
                _currency = "";
                return this;
            }

            public Fixture HavingNegativeValue()
            {
                _value = -10;
                return this;
            }

            public Fixture HavingNullCurrency()
            {
                _currency = null;
                return this;
            }

            public Fixture HavingValue(decimal value)
            {
                _value = value;
                return this;
            }

            public Money SystemUnderTest()
            {
                return new Money(_value, _currency, _bank.Object);
            }

            public Fixture ThatConvert10RupeeTo1Dollar()
            {
                AddConversionToBank(10, "INR", 1, "USD");
                return this;
            }

            public Fixture ThatConvert5DollarTo50Rupee()
            {
                AddConversionToBank(5, "USD", 50, "INR");
                return this;
            }

            public Fixture ThatConvert5RupeeTo1Dollar()
            {
                AddConversionToBank(5, "INR", 1, "USD");
                return this;
            }

            public Fixture ThatConvert5RupeeTo5Rupee()
            {
                AddConversionToBank(5, "INR", 5, "INR");
                return this;
            }

            private void AddConversionToBank(int fromValue, string fromCurrency, int toValue, string toCurrency)
            {
                _bank.Setup(b => b.ConvertTo(new Money(fromValue, fromCurrency, new Mock<IBank>().Object), toCurrency))
                    .ReturnsAsync(new Money(toValue, toCurrency, new Mock<IBank>().Object));
            }
        }
    }
}