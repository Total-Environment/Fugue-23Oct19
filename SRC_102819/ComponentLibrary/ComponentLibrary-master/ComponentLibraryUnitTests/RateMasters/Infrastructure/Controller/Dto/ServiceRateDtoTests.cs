using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Infrastructure.Controller.Dto
{
    public class ServiceRateDtoTests
    {
       [Fact]
        public void Constructor_WhenNoServiceRateIsSpecified_ShouldThrowArgumentException()
        {
            Func<Task> action = () => new ServiceRateDto().SetDomain(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task Domain_ShouldThrowArgumentException_WhenNoBankIsSpecified()
        {
            var mockServiceRate = new Mock<IServiceRate>();
            mockServiceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR"));

            var sut = await new ServiceRateDto().SetDomain(mockServiceRate.Object);
            Action action = () => sut.Domain(null, null, null);

            action.ShouldThrow<ArgumentException>().WithMessage("Bank cannot be null.");
        }

        [Fact]
        public async Task Domain_ShouldThrowArgumentException_WhenTypeOfPurchaseIsInvalid()
        {
            var mockBank = new Mock<IBank>();
            var mockServiceRate = new Mock<IServiceRate>();
            mockServiceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.TypeOfPurchase).Returns("Import");

            var sut = await new ServiceRateDto().SetDomain(mockServiceRate.Object);
            Action action = () => sut.Domain(mockBank.Object, new MasterDataList("type_of_purchase", new List<MasterDataValue> { new MasterDataValue("Domestic Interstate") }), null);

            action.ShouldThrow<ArgumentException>().WithMessage("Type of Purchase : Import is invalid.");
        }

        [Fact]
        public async Task Domain_ShouldThrowArgumentException_WhenLocationIsInvalid()
        {
            var mockBank = new Mock<IBank>();
            var mockServiceRate = new Mock<IServiceRate>();
            mockServiceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.TypeOfPurchase).Returns("Import");
            mockServiceRate.Setup(r => r.Location).Returns("Bengaluru");

            var sut = await new ServiceRateDto().SetDomain(mockServiceRate.Object);
            Action action = () => sut.Domain(mockBank.Object, new Mock<IMasterDataList>().Object, new MasterDataList("location", new List<MasterDataValue> { new MasterDataValue("Hyderabad") }));

            action.ShouldThrow<ArgumentException>().WithMessage("Type of Purchase : Import is invalid.");
        }

        [Fact]
        public async Task Domain_ShouldConvertDTOToDomainValue_WhenValidValuesArePassed()
        {
            var appliedOn = DateTime.Now;
            var mockBank = new Mock<IBank>();
            var mockServiceRate = new Mock<IServiceRate>();
            mockServiceRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR"));
            mockServiceRate.Setup(r => r.TypeOfPurchase).Returns("Domestic Interstate");
            mockServiceRate.Setup(r => r.Location).Returns("Hyderabad");
            mockServiceRate.Setup(r => r.Id).Returns("SPF100");
            mockServiceRate.Setup(r => r.AppliedOn).Returns(appliedOn);

            var sut = await new ServiceRateDto().SetDomain(mockServiceRate.Object);
            var serviceRate = sut.Domain(mockBank.Object,
                new MasterDataList("type_of_purchase", new List<MasterDataValue> { new MasterDataValue("Domestic Interstate") }),
                new MasterDataList("location", new List<MasterDataValue> { new MasterDataValue("Hyderabad") }));

            serviceRate.Location.Should().Be("Hyderabad");
            serviceRate.Id.Should().Be("SPF100");
            serviceRate.AppliedOn.Should().Be(appliedOn);
            serviceRate.TypeOfPurchase.Should().Be("Domestic Interstate");
            serviceRate.ControlBaseRate.Should().Be(new Money(10m, "INR"));
        }
    }
}