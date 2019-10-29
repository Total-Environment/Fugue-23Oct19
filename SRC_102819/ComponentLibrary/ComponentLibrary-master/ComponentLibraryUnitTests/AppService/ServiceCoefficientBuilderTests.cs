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
    public class ServiceCoefficientBuilderTests
    {
        [Fact]
        public async Task BuildData_ShouldSetNameAndUnitOfMeasureProperties_WhenValidComponentCoefficientIsPassed()
        {
            var mockServiceRepository = new Mock<IServiceRepository>();
            var mockService = new Mock<IService>();
            mockService.Setup(m => m["General"]["Short Description"]).Returns("test");
            mockService.Setup(m => m["General"]["Unit of Measure"]).Returns("uom");
            mockServiceRepository.Setup(m => m.Find("code")).ReturnsAsync(mockService.Object);

            var serviceCoefficientBuilder = new ServiceCoefficientBuilder(mockServiceRepository.Object);
            var result = await serviceCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
                "", ComponentType.Service));

            result.Name.Should().BeEquivalentTo("test");
            result.UnitOfMeasure.Should().BeEquivalentTo("uom");
        }

        [Fact]
        public void BuildData_ShouldThrowArgumentException_WhenComponentCoefficientWithNonExistingServiceIsPassed()
        {
            var mockServiceRepository = new Mock<IServiceRepository>();
            mockServiceRepository.Setup(m => m.Find("code")).ReturnsAsync(null);

            var serviceCoefficientBuilder = new ServiceCoefficientBuilder(mockServiceRepository.Object);
            Func<Task> func = async () => await serviceCoefficientBuilder.BuildData(new ComponentCoefficient("code", 100, new List<WastagePercentage>(), "",
                "", ComponentType.Service));

            func.ShouldThrow<ArgumentException>().WithMessage("Invalid service code. code");
        }
    }
}