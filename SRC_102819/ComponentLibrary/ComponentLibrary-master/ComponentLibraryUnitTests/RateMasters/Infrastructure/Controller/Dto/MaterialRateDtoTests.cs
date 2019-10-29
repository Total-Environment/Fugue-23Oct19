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
    public class MaterialRateDtoTests
    {
        [Fact]
        public void Constructor_WhenNoMaterialRateIsSpecified_ShouldThrowArgumentException()
        {
            Func<Task> action = () => new MaterialRateDto().SetDomain(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task Domain_ShouldThrowArgumentException_WhenNoBankIsSpecified()
        {
            var mockMaterialRate = new Mock<IMaterialRate>();
            mockMaterialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));

            var sut = await new MaterialRateDto().SetDomain(mockMaterialRate.Object);
            Action action = () => sut.Domain(null, null, null);

            action.ShouldThrow<ArgumentException>().WithMessage("Bank cannot be null.");
        }

        [Fact]
        public async Task Domain_ShouldThrowArgumentException_WhenTypeOfPurchaseIsInvalidAsync()
        {
            var mockBank = new Mock<IBank>();
            var mockMaterialRate = new Mock<IMaterialRate>();
            mockMaterialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.TypeOfPurchase).Returns("Import");

            var sut = await new MaterialRateDto().SetDomain(mockMaterialRate.Object);
            Action action = () => sut.Domain(mockBank.Object, new MasterDataList("type_of_purchase", new List<MasterDataValue> { new MasterDataValue("Domestic Interstate") }), null);

            action.ShouldThrow<ArgumentException>().WithMessage("Type of Purchase : Import is invalid.");
        }

        [Fact]
        public async Task Domain_ShouldThrowArgumentException_WhenLocationIsInvalid()
        {
            var mockBank = new Mock<IBank>();
            var mockMaterialRate = new Mock<IMaterialRate>();
            mockMaterialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.TypeOfPurchase).Returns("Import");
            mockMaterialRate.Setup(r => r.Location).Returns("Bengaluru");

            var sut = await new MaterialRateDto().SetDomain(mockMaterialRate.Object);
            Action action = () => sut.Domain(mockBank.Object, new Mock<IMasterDataList>().Object, new MasterDataList("location", new List<MasterDataValue> { new MasterDataValue("Hyderabad") }));

            action.ShouldThrow<ArgumentException>().WithMessage("Type of Purchase : Import is invalid.");
        }

        [Fact]
        public async Task Domain_ShouldConvertDTOToDomainValue_WhenValidValuesArePassed()
        {
            var appliedOn = DateTime.Now;
            var mockBank = new Mock<IBank>();
            var mockMaterialRate = new Mock<IMaterialRate>();
            mockMaterialRate.Setup(r => r.LandedRate()).ReturnsAsync(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.ControlBaseRate).Returns(new Money(10m, "INR", new Mock<IBank>().Object));
            mockMaterialRate.Setup(r => r.TypeOfPurchase).Returns("Domestic Interstate");
            mockMaterialRate.Setup(r => r.Location).Returns("Hyderabad");
            mockMaterialRate.Setup(r => r.Id).Returns("SPF100");
            mockMaterialRate.Setup(r => r.AppliedOn).Returns(appliedOn);

            var sut = await new MaterialRateDto().SetDomain(mockMaterialRate.Object);
            var materialRate = sut.Domain(mockBank.Object,
                new MasterDataList("type_of_purchase", new List<MasterDataValue> { new MasterDataValue("Domestic Interstate") }),
                new MasterDataList("location", new List<MasterDataValue> { new MasterDataValue("Hyderabad") }));

            materialRate.Location.Should().Be("Hyderabad");
            materialRate.Id.Should().Be("SPF100");
            materialRate.AppliedOn.Should().Be(appliedOn);
            materialRate.TypeOfPurchase.Should().Be("Domestic Interstate");
            materialRate.ControlBaseRate.Should().Be(new Money(10m, "INR"));
        }
    }
}