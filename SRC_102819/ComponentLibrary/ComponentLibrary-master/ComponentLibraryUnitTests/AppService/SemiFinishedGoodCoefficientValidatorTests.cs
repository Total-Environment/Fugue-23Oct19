using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class SemiFinishedGoodCoefficientValidatorTests
	{
		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidComponentCoefficientIsPassed()
		{
			var mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(m => m.Count("sfg", It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(1);

			var semiFinishedGoodCoefficientValidator = new SemiFinishedGoodCoefficientValidator(mockSemiFinishedGoodRepository.Object);
			var result =
				await semiFinishedGoodCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					null, null, ComponentType.SFG));

			result.Item1.Should().BeTrue();
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithNonExistingServiceIsPassed()
		{
			var mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();
			mockSemiFinishedGoodRepository.Setup(m => m.Count("sfg", It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(0);

			var semiFinishedGoodCoefficientValidator = new SemiFinishedGoodCoefficientValidator(mockSemiFinishedGoodRepository.Object);
			var result =
				await semiFinishedGoodCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					null, null, ComponentType.SFG));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo($"Invalid Semi Finished Good code code");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithoutServiceCodeIsPassed()
		{
			var mockSemiFinishedGoodRepository = new Mock<ICompositeComponentRepository>();

			var semiFinishedGoodCoefficientValidator = new SemiFinishedGoodCoefficientValidator(mockSemiFinishedGoodRepository.Object);

			var result =
				await semiFinishedGoodCoefficientValidator.Validate(new ComponentCoefficient(null, 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					null, null, ComponentType.SFG));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo("Semi Finished Good code is mandatory.");
		}
	}
}