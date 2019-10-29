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
	public class AssetCoefficientValidatorTests
	{
		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidComponentCoefficientIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(1);

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					"uom", null, ComponentType.Asset));

			result.Item1.Should().BeTrue();
		}

		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenValidComponentCoefficientWithoutWastagesIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(1);

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 100, null, "uom", null,
					ComponentType.Asset));

			result.Item1.Should().BeTrue();
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithNonExistingAssetIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(0);

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					"uom", null, ComponentType.Asset));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo($"Invalid asset code code");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithoutAssetCodeIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient(null, 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					"uom", null, ComponentType.Asset));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo("Asset code is mandatory.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithoutUOMIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();
			mockMaterialRepository.Setup(m => m.Count(It.IsAny<Dictionary<string, Tuple<string, object>>>()))
				.ReturnsAsync(1);

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					null, null, ComponentType.Asset));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo("Unit of Measure field is required for asset code");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithInvalidCoefficientIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 0,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", 10)
					},
					"uom", null, ComponentType.Asset));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo("Invalid coefficient value for code");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithInvalidWastageIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage("name", -1)
					},
					"uom", null, ComponentType.Asset));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo("Invalid wastage value for name for code");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenComponentCoefficientWithoutWastageNameIsPassed()
		{
			var mockMaterialRepository = new Mock<IMaterialRepository>();

			var assetCoefficientValidator = new AssetCoefficientValidator(mockMaterialRepository.Object);
			var result =
				await assetCoefficientValidator.Validate(new ComponentCoefficient("code", 100,
					new List<WastagePercentage>
					{
						new WastagePercentage(null, 1)
					},
					"uom", null, ComponentType.Asset));

			result.Item1.Should().BeFalse();
			result.Item2.Should().BeEquivalentTo("Wastage name is mandatory for code");
		}
	}
}