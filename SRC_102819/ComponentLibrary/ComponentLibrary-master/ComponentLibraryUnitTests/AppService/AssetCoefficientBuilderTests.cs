using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class AssetCoefficientBuilderTests
    {
        [Fact]
        public async Task BuildData_ShouldSetNameProperty_WhenValidComponentCoefficientIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();
            var mockMaterial = new Mock<IMaterial>();
            mockMaterial.Setup(m => m["General"]["Material Name"]).Returns("test");
            mockMaterialRepository.Setup(m => m.Find("code")).ReturnsAsync(mockMaterial.Object);

            var assetCoefficientBuilder = new AssetCoefficientBuilder(mockMaterialRepository.Object);
            var result = await assetCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
                "", ComponentType.Asset));

            result.Name.Should().BeEquivalentTo("test");
        }

        [Fact]
        public void BuildData_ShouldThrowArgumentException_WhenComponentCoefficientWithNonExistingAssetIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();
            mockMaterialRepository.Setup(m => m.Find("code")).ReturnsAsync(null);

            var assetCoefficientBuilder = new AssetCoefficientBuilder(mockMaterialRepository.Object);
            Func<Task> func = async () => await assetCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
                "", ComponentType.Asset));

            func.ShouldThrow<ArgumentException>().WithMessage("Invalid asset code. code");
        }
    }
}