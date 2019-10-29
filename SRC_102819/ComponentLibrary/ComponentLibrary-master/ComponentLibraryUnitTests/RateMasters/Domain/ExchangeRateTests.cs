using System;
using FluentAssertions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Domain
{
    public class ExchangeRateTest
    {
        [Fact]
        public void Should_ImplementIExchangeRate()
        {
            var systemUnderTest = new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();

            systemUnderTest.Should().BeAssignableTo<IExchangeRate>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionIfFromCurrencyIsPassedAsNull()
        {
            Action action = () => new Fixture()
                .HavingFromCurrencyAsNull()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();
            Action anotherAction = () => new Fixture()
                .HavingFromCurrencyAsEmpty()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();
            Action oneMoreAction = () => new Fixture()
                .HavingFromCurrencyAsWhitespaces()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();

            oneMoreAction.ShouldThrow<ArgumentException>();
            anotherAction.ShouldThrow<ArgumentException>();
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionIfToCurrencyIsPassedAsNullOrWhiteSpace()
        {
            Action action = () => new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsNull()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();
            Action anotherAction = () => new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsEmpty()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();
            Action oneMoreAction = () => new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsOnlyWhitespaces()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();

            oneMoreAction.ShouldThrow<ArgumentException>();
            anotherAction.ShouldThrow<ArgumentException>();
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionIfBaseConversionRateIsPassedAsNegativeOrZero()
        {
            Action action = () => new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAsNegative()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();
            Action secondAction = () => new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAsZero()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();

            action.ShouldThrow<ArgumentException>();
            secondAction.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionIfFluctuationCoefficientIsPassedAsNegative()
        {
            Action action = () => new Fixture()
                .HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAsNegative()
                .SystemUnderTest();
            
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Rate_Should_ReturnSumOfBaseConversionRateAndProductOfBaseConversionRateAndFluctusationCoefficient()
        {
            var systemUnderTest = new Fixture().HavingFromCurrencyAsUsd()
                .HavingToCurrencyAsInr()
                .HavingBaseConversionRateAs10()
                .HavingFluctuationCoefficientAs1Point5()
                .SystemUnderTest();

            var result = systemUnderTest.Rate();

            result.Should().Be(10.15m);
        }

        private class Fixture
        {
            private string _fromCurrency;
            private string _toCurrency;
            private decimal _baseConversionRate;
            private decimal _fluctuationCoefficient;

            public Fixture HavingFromCurrencyAsUsd()
            {
                _fromCurrency = "USD";
                return this;
            }

            public Fixture HavingToCurrencyAsInr()
            {
                _toCurrency = "INR";
                return this;
            }

            public Fixture HavingBaseConversionRateAs10()
            {
                _baseConversionRate = 10m;
                return this;
            }

            public ExchangeRate SystemUnderTest()
            {
                return new ExchangeRate(_fromCurrency,_toCurrency,_baseConversionRate, _fluctuationCoefficient,DateTime.Today);
            }

            public Fixture HavingFluctuationCoefficientAs1Point5()
            {
                _fluctuationCoefficient = 1.5m;
                return this;
            }

            public Fixture HavingFromCurrencyAsNull()
            {
                _fromCurrency = null;
                return this;
            }

            public Fixture HavingToCurrencyAsNull()
            {
                _toCurrency = null;
                return this;
            }

            public Fixture HavingBaseConversionRateAsNegative()
            {
                _baseConversionRate = -1m;
                return this;
            }

            public Fixture HavingBaseConversionRateAsZero()
            {
                _baseConversionRate = 0;
                return this;
            }

            public Fixture HavingFluctuationCoefficientAsNegative()
            {
                _fluctuationCoefficient = -1;
                return this;
            }

            public Fixture HavingFluctuationCoefficientAsZero()
            {
                _fluctuationCoefficient = 0;
                return this;
            }

            public Fixture HavingToCurrencyAsEmpty()
            {
                _toCurrency = "";
                return this;
            }

            public Fixture HavingToCurrencyAsOnlyWhitespaces()
            {
                _toCurrency = "   ";
                return this;
            }

            public Fixture HavingFromCurrencyAsEmpty()
            {
                _fromCurrency = "";
                return this;
            }

            public Fixture HavingFromCurrencyAsWhitespaces()
            {
                _fromCurrency = "   ";
                return this;
            }
        }
    }
}
