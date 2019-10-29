using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
    public class ServiceCoefficientValidatorTests
    {
        [Fact]
        public async Task Validate_ShouldReturnTrue_WhenValidComponentCoefficientIsPassed()
        {
            var mockServiceRepository = new Mock<IServiceRepository>();
            mockServiceRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
                .ReturnsAsync(1);

            var serviceCoefficientValidator = new ServiceCoefficientValidator(mockServiceRepository.Object);
            var result =
                await serviceCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
                    new List<WastagePercentage>
                    {
                        new WastagePercentage("name", 10)
                    },
                    null, null, ComponentType.Service));

            result.Item1.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithNonExistingServiceIsPassed()
        {
            var mockServiceRepository = new Mock<IServiceRepository>();
            mockServiceRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
                .ReturnsAsync(0);

            var serviceCoefficientValidator = new ServiceCoefficientValidator(mockServiceRepository.Object);
            var result =
                await serviceCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
                    new List<WastagePercentage>
                    {
                        new WastagePercentage("name", 10)
                    },
                    null, null, ComponentType.Service));

            result.Item1.Should().BeFalse();
            result.Item2.Should().BeEquivalentTo($"Invalid Service code code");
        }

        [Fact]
        public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithoutServiceCodeIsPassed()
        {
            var mockServiceRepository = new Mock<IServiceRepository>();

            var serviceCoefficientValidator = new ServiceCoefficientValidator(mockServiceRepository.Object);
            var result =
                await serviceCoefficientValidator.Validate(new ComponentCoefficient(null, 100,
                    new List<WastagePercentage>
                    {
                        new WastagePercentage("name", 10)
                    },
                    null, null, ComponentType.Service));

            result.Item1.Should().BeFalse();
            result.Item2.Should().BeEquivalentTo("Service code is mandatory.");
        }
    }
}