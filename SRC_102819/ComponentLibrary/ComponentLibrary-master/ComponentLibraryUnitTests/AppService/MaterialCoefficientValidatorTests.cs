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
    public class MaterialCoefficientValidatorTests
    {
        [Fact]
        public async Task Validate_ShouldReturnTrue_WhenValidComponentCoefficientIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();
            mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
                .ReturnsAsync(1);

            var materialCoefficientValidator = new MaterialCoefficientValidator(mockMaterialRepository.Object);
            var result =
                await materialCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
                    new List<WastagePercentage>
                    {
                        new WastagePercentage("name", 10)
                    },
                    null, null, ComponentType.Material));

            result.Item1.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithNonExistingMaterialIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();
            mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
                .ReturnsAsync(0);

            var materialCoefficientValidator = new MaterialCoefficientValidator(mockMaterialRepository.Object);
            var result =
                await materialCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
                    new List<WastagePercentage>
                    {
                        new WastagePercentage("name", 10)
                    },
                    null, null, ComponentType.Material));

            result.Item1.Should().BeFalse();
            result.Item2.Should().BeEquivalentTo($"Invalid Material code code");
        }

        [Fact]
        public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithoutMaterialCodeIsPassed()
        {
            var mockMaterialRepository = new Mock<IMaterialRepository>();

            var materialCoefficientValidator = new MaterialCoefficientValidator(mockMaterialRepository.Object);
            var result =
                await materialCoefficientValidator.Validate(new ComponentCoefficient(null, 100,
                    new List<WastagePercentage>
                    {
                        new WastagePercentage("name", 10)
                    },
                    null, null, ComponentType.Material));

            result.Item1.Should().BeFalse();
            result.Item2.Should().BeEquivalentTo("Material code is mandatory.");
        }
    }
}