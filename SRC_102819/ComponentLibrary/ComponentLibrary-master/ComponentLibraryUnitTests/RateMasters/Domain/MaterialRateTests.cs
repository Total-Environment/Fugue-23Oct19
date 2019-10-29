using System;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Domain
{
    public class MaterialRateTests
    {
        private class SystemUnderTest
        {
            private DateTime _appliedOn;
            private string _location;
            private string _materialId;
            private string _typeOfPurchase;
            private decimal _insuranceCharges = 0;
            private decimal _basicCustomsDuty = 0;
            private decimal _clearanceCharges = 0;
            private decimal _taxVariance = 0;
            private decimal _locationVariance = 0;
            private decimal _marketFluctuation = 0;
            private decimal _freightCharges = 0;
            private Money _controlBaseRate;


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

            public SystemUnderTest HavingSomeMaterialId()
            {
                _materialId = "materialId";
                return this;
            }

            public SystemUnderTest HavingSomeTypeOfPurchase()
            {
                _typeOfPurchase = "TypeOfPurchase";
                return this;
            }

            public MaterialRate Sut()
            {
                return new MaterialRate(
                    _appliedOn,
                    _location,
                    _materialId,
                    _controlBaseRate,
                    _freightCharges,
                    _insuranceCharges,
                    _basicCustomsDuty,
                    _clearanceCharges,
                    _taxVariance,
                    _locationVariance,
                    _marketFluctuation,
                    _typeOfPurchase
                 );
            }

            public SystemUnderTest HavingSomeRate(decimal rate = 10m)
            {
                _controlBaseRate = new Money(rate, "INR", new Mock<IBank>().Object);
                return this;
            }

            public SystemUnderTest HavingSomeCoefficientsAsNegative()
            {
                _locationVariance = -1;
                return this;
            }
        }

        [Fact]
        public void Should_ImplementIMaterialRate()
        {
            var sut = new SystemUnderTest()
                .HavingSomeAppliedOn()
                .HavingSomeLocation()
                .HavingSomeMaterialId()
                .HavingSomeRate()
                .HavingSomeTypeOfPurchase()
                .Sut();

            sut.Should().BeAssignableTo<IMaterialRate>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfMaterialIdIsNull()
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
                 .HavingSomeMaterialId()
                 .HavingSomeTypeOfPurchase()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }
        [Fact]
        public void Creation_Should_ThrowExceptionIfControlBaseRateIsZero()
        {
            Action action = () => new SystemUnderTest()
                .HavingSomeAppliedOn()
                .HavingSomeRate(0)
                .HavingSomeMaterialId()
                .HavingSomeTypeOfPurchase()
                .Sut();

            action.ShouldThrow<ArgumentException>();
        }


        [Fact]
        public void Creation_Should_ThrowExceptionIfTypeOfPurchaseIsNull()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeMaterialId()
                 .HavingSomeLocation()
                 .HavingSomeRate()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfOneOfTheCoefficientsIsNegative()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeMaterialId()
                 .HavingSomeLocation()
                 .HavingSomeRate()
                 .HavingSomeTypeOfPurchase()
                 .HavingSomeCoefficientsAsNegative()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_Should_ThrowExceptionIfRateIsNull()
        {
            Action action = () => new SystemUnderTest()
                 .HavingSomeAppliedOn()
                 .HavingSomeMaterialId()
                 .HavingSomeLocation()
                 .Sut();

            action.ShouldThrow<ArgumentException>();
        }
    }
}