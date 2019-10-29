using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller.Dto
{
    public class MoneyDtoTests
    {
        [Fact]
        public void Should_ImplementIMoneyDto()
        {
            var sut = new MoneyDto(MoneyStub(10m,"INR"));

            sut.Should().BeAssignableTo<MoneyDto>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionWhenMoneyPassedAsNull()
        {
            Action action = () => new MoneyDto(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ValueAndCurrency_Should_ReturnValueOfPassedMoney()
        {
            const string currency = "INR";
            const decimal value = 10m;
            var money = MoneyStub(value, currency);
            var sut = new MoneyDto(money);

            var resultValue = sut.Value;
            var resultCurrency = sut.Currency;

            resultCurrency.Should().Be(currency);
            resultValue.Should().Be(value);
        }

        [Fact]
        public void Domain_Should_ThrowArgumentExceptionWhenNull()
        {
            var sut = new MoneyDto(MoneyStub(10,"INR"));

            Action action = ()=>sut.Domain(null);

            action.ShouldThrow<ArgumentException>();
        }

        private Money MoneyStub(decimal value, string currency)
        {
            return new Money(value,currency,new Mock<IBank>().Object);
        }
    }
}
