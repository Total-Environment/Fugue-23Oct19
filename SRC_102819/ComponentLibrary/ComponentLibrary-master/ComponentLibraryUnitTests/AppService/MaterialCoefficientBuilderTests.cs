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
    public class MaterialCoefficientBuilderTests
    {
        [Fact]
        public async Task BuildData_ShouldSetNameAndUnitOfMeasureProperties_WhenValidComponentCoefficientIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();
            var mockMaterial = new Mock<IMaterial>();
            mockMaterial.Setup(m => m["General"]["Material Name"]).Returns("test");
            mockMaterial.Setup(m => m["General"]["Unit of Measure"]).Returns("uom");
            mockMaterialRepository.Setup(m => m.Find("code")).ReturnsAsync(mockMaterial.Object);

            var materialCoefficientBuilder = new MaterialCoefficientBuilder(mockMaterialRepository.Object);
            var result = await materialCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
                "", ComponentType.Material));

            result.Name.Should().BeEquivalentTo("test");
            result.UnitOfMeasure.Should().BeEquivalentTo("uom");
        }

        [Fact]
        public void BuildData_ShouldThrowArgumentException_WhenComponentCoefficientWithNonExistingMaterialIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();
            mockMaterialRepository.Setup(m => m.Find("code")).ReturnsAsync(null);

            var materialCoefficientBuilder = new MaterialCoefficientBuilder(mockMaterialRepository.Object);
            Func<Task> func = async () => await materialCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
                "", ComponentType.Material));

            func.ShouldThrow<ArgumentException>().WithMessage("Invalid material code. code");
        }
    }
}