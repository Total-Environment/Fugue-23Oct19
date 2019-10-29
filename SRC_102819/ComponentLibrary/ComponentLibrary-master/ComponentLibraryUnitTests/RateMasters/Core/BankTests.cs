using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Core
{
    public class BankTests
    {
        private class Fixture
        {
            private readonly Mock<IExchangeRate> _exchangeRate = new Mock<IExchangeRate>();

            public Bank SystemUnderTest()
            {
                var mockExchangeRepository = new Mock<IExchangeRateRepository>();
                mockExchangeRepository.Setup(m => m.GetAll())
                    .ReturnsAsync(new List<IExchangeRate> {_exchangeRate.Object});
                mockExchangeRepository.Setup(m => m.GetAll(It.IsAny<DateTime>()))
                    .ReturnsAsync(new List<IExchangeRate> { _exchangeRate.Object });
                var systemUnderTest = new Bank(mockExchangeRepository.Object);
                return systemUnderTest;
            }

            public Fixture HavingExchangeRate10ForDollarToRupee()
            {
                _exchangeRate.Setup(e => e.FromCurrency).Returns("USD");
                _exchangeRate.Setup(e => e.ToCurrency).Returns("INR");
                _exchangeRate.Setup(e => e.Rate()).Returns(10);
                return this;
            }
        }

        [Fact]
        public async Task ConvertTo_Should_ConvertPassedMoneyToPassedCurrency()
        {
            var systemUnderTest = new Fixture()
                .HavingExchangeRate10ForDollarToRupee()
                .SystemUnderTest();

            var result = await systemUnderTest.ConvertTo(new Money(10, "USD", systemUnderTest), "INR");

            result.Should().Be(new Money(100, "INR", systemUnderTest));
        }

        [Fact]
        public void ConvertTo_Should_ThrowArgumentExceptionWhenCurrencyISPassedAsNull()
        {
            var stubBank = new Mock<IBank>().Object;
            var systemUnderTest = new Fixture()
                .SystemUnderTest();

            Func<Task> action = async () => await systemUnderTest.ConvertTo(new Money(10, "INR", stubBank), null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ConvertTo_Should_ThrowArgumentExceptionWhenMoneyIsPassedIsNull()
        {
            var systemUnderTest = new Fixture()
                .SystemUnderTest();

            Func<Task> action = async () => await systemUnderTest.ConvertTo(null, "INR");

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Should_ImplementIBank()
        {
            var systemUnderTest = new Fixture()
                .SystemUnderTest();

            systemUnderTest.Should().BeAssignableTo<IBank>();
        }
    }
}