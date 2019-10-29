using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Repository.Dao
{
    public class CoefficientDaoTests
    {
        private class SystemUnderTest
        {
            private MoneyDao _moneyDao;
            private string _name;
            private string _percentage;

            public CoefficientDao Sut()
            {
                return new CoefficientDao {Percentage= _percentage, Name = _name, Value = _moneyDao};
            }

            public SystemUnderTest HavingSomeValue()
            {
                _moneyDao = new MoneyDao {Value = 10, Currency = "INR"};
                return this;
            }

            public SystemUnderTest HavingPercentageSumFactor()
            {
                _percentage = "30";
                return this;
            }

            public SystemUnderTest HavingNameSomeName()
            {
                _name = "SomeName";
                return this;
            }
        }

        private static IBank BankStub()
        {
            return new Mock<IBank>().Object;
        }

        [Fact]
        public void Domain_Should_ReturnSumCoefficientWhenValueIsPassed()
        {
            var sut = new SystemUnderTest()
                .HavingSomeValue()
                .HavingNameSomeName()
                .Sut();

            var result = sut.Domain(BankStub());

            result.Should().BeOfType<SumCoefficient>();
        }

        [Fact]
        public void Domain_Should_ReturnPercentageCoefficientWhenPercentageIsPassed()
        {
            var sut = new SystemUnderTest()
                .HavingPercentageSumFactor()
                .HavingNameSomeName()
                .Sut();

            var result = sut.Domain(BankStub());

            result.Should().BeOfType<PercentageCoefficient>();
        }

        [Fact]
        public void Domain_Should_ThrowArgumentExceptionWhenValueAndFactorAndPercentageIsSpecified()
        {
            var sut = new SystemUnderTest()
                .HavingSomeValue()
                .HavingPercentageSumFactor()
                .HavingNameSomeName()
                .Sut();

            Action action = () => sut.Domain(BankStub());

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Domain_Should_ShouldThrowArgumentExceptionWhenBothValueAndFactorAreNull()
        {
            var sut = new SystemUnderTest()
                .HavingNameSomeName()
                .Sut();

            Action action = ()=>sut.Domain(BankStub());

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowArgumentExceptionCoefficientIsPassedAsNull()
        {
            Action action = ()=> new CoefficientDao(null);

            action.ShouldThrow<ArgumentException>();
        }
    }
}