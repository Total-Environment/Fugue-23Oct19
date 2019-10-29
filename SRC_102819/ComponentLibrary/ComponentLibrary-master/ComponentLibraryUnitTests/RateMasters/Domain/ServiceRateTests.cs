using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Domain
{
    public class ServiceRateTests
    {
        private class SystemUnderTest
        {
            private DateTime _appliedOn;
            private string _location;
            private string _serviceId;
            private string _typeOfPurchase;
            private Money _controlBaseRate;
            private decimal _locationVariance = 0;
            private decimal _marketFluctuation = 0;

            public SystemUnderTest HavingSomeAppliedOn()
            {
                _appliedOn = DateTime.Today;
                return this;
            }

            public SystemUnderTest HavingSomeLocation()
            {
                _location = "Somelocation";
                return this;
            }

            public SystemUnderTest HavingSomeServiceId()
            {
                _serviceId = "serviceId";
                return this;
            }

            public SystemUnderTest HavingSomeTypeOfPurchase()
            {
                _typeOfPurchase = "TypeOfPurchase";
                return this;
            }

            public ServiceRate Sut()
            {
                return new ServiceRate(
                    _appliedOn,
                    _location,
                    _serviceId,
                    _controlBaseRate,
                    _locationVariance,
                    _marketFluctuation,
                    _typeOfPurchase);
            }

            public SystemUnderTest HavingSomeRate()
            {
                _controlBaseRate = new Money(10m,"INR", new Mock<IBank>().Object);
                return this;
            }

            public SystemUnderTest HavingSomeCoefficientValueAsNegative()
            {
                _marketFluctuation = -10;
                return this;
            }
        }

        [Fact]
        public void Should_ImplementIServiceRate()
        {
            var sut = new SystemUnderTest()
                .HavingSomeAppliedOn()
                .HavingSomeLocation()
                .HavingSomeServiceId()
                .HavingSomeRate()
                .HavingSomeTypeOfPurchase()
                .Sut();

            sut.Should().BeAssignableTo<IServiceRate>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfServiceIdIsNull()
        {
            Action action =()=> new SystemUnderTest()
                .HavingSomeAppliedOn()
                .HavingSomeLocation()
                .HavingSomeRate()
                .HavingSomeTypeOfPurchase()
                .Sut();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfLocationIsNull()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeRate()
                 .HavingSomeServiceId()
                 .HavingSomeTypeOfPurchase()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfTypeOfPurchaseIsNull()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeServiceId()
                 .HavingSomeLocation()
                 .HavingSomeRate()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void Creation_Should_ThrowExceptionIfAnyOfTheCoefficientsIsNegative()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeServiceId()
                 .HavingSomeLocation()
                 .HavingSomeRate()
                 .HavingSomeTypeOfPurchase()
                 .HavingSomeCoefficientValueAsNegative()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfRateIsNull()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeServiceId()
                 .HavingSomeLocation()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }
    }
}