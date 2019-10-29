using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class CompositeComponentValidatorTests
	{
		[Fact]
		public async Task Validate_ShouldReturnTrue_WhenSfgGoodCompostitionIsPassed()
		{
			var mockComponentCoefficientValidatorfactory = new Mock<IComponentCoefficientValidatorFactory>();
			var mockComponentCoefficientValidator = new Mock<IComponentCoefficientValidator>();
			mockComponentCoefficientValidator.Setup(m => m.Validate(It.IsAny<ComponentCoefficient>()))
				.ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
			mockComponentCoefficientValidatorfactory
				.Setup(m => m.GetComponentCoefficientValidator(ComponentType.Material))
				.Returns(mockComponentCoefficientValidator.Object);
			var sfgGoodCompositionValidator = new CompositeComponentValidator(mockComponentCoefficientValidatorfactory.Object);
			var result = await sfgGoodCompositionValidator.Validate("sfg", new ComponentComposition(new List<ComponentCoefficient>
			{
				new ComponentCoefficient("code",10,new List<WastagePercentage>
				{
					new WastagePercentage("Wastage1",10)
				},"m2","abcd",ComponentType.Material ),
				new ComponentCoefficient("code1",10,new List<WastagePercentage>
				{
					new WastagePercentage("Wastage1",10)
				},"m2","abcd",ComponentType.Material )
			}));

			result.Item1.ShouldBeEquivalentTo(true);
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenSfgGoodCompostitionIsPassed()
		{
			var mockComponentCoefficientValidatorfactory = new Mock<IComponentCoefficientValidatorFactory>();
			var mockComponentCoefficientValidator = new Mock<IComponentCoefficientValidator>();
			mockComponentCoefficientValidator.Setup(m => m.Validate(It.IsAny<ComponentCoefficient>()))
				.ReturnsAsync(new Tuple<bool, string>(false, "Invalid component Coefficient"));
			mockComponentCoefficientValidatorfactory
				.Setup(m => m.GetComponentCoefficientValidator(ComponentType.Material))
				.Returns(mockComponentCoefficientValidator.Object);
			var sfgGoodCompositionValidator = new CompositeComponentValidator(mockComponentCoefficientValidatorfactory.Object);
			var result = await sfgGoodCompositionValidator.Validate("sfg", new ComponentComposition(new List<ComponentCoefficient>
			{
				new ComponentCoefficient("code",10,new List<WastagePercentage>
				{
					new WastagePercentage("Wastage1",10)
				},"m2","abcd",ComponentType.Material ),
				 new ComponentCoefficient("code1",10,new List<WastagePercentage>
				{
					new WastagePercentage("Wastage1",10)
				},"m2","abcd",ComponentType.Material )
			}));

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Invalid component Coefficient");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenSfgGoodCompostitionWithoutComponentCoefficientsIsPassed()
		{
			var mockComponentCoefficientValidatorfactory = new Mock<IComponentCoefficientValidatorFactory>();
			var mockComponentCoefficientValidator = new Mock<IComponentCoefficientValidator>();
			mockComponentCoefficientValidatorfactory
				.Setup(m => m.GetComponentCoefficientValidator(ComponentType.Material))
				.Returns(mockComponentCoefficientValidator.Object);

			var sfgGoodCompositionValidator = new CompositeComponentValidator(mockComponentCoefficientValidatorfactory.Object);
			var result = await sfgGoodCompositionValidator.Validate("sfg", new ComponentComposition(new List<ComponentCoefficient>()));

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Minimum two component coefficients are required");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenSfgGoodCompostitionWithOneComponentCoefficientsIsPassed()
		{
			var mockComponentCoefficientValidatorfactory = new Mock<IComponentCoefficientValidatorFactory>();
			var mockComponentCoefficientValidator = new Mock<IComponentCoefficientValidator>();
			mockComponentCoefficientValidatorfactory
				.Setup(m => m.GetComponentCoefficientValidator(ComponentType.Material))
				.Returns(mockComponentCoefficientValidator.Object);

			var sfgGoodCompositionValidator = new CompositeComponentValidator(mockComponentCoefficientValidatorfactory.Object);
			var result =
				await sfgGoodCompositionValidator.Validate("sfg",
					new ComponentComposition(new List<ComponentCoefficient>
					{
						new ComponentCoefficient("code", 10, new List<WastagePercentage>(), "uom", "name", ComponentType.Material)
					}));

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Minimum two component coefficients are required");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenSfgGoodCompostitionWithDuplicateComponentCoefficientsIsPassed()
		{
			var mockComponentCoefficientValidatorfactory = new Mock<IComponentCoefficientValidatorFactory>();
			var mockComponentCoefficientValidator = new Mock<IComponentCoefficientValidator>();
			mockComponentCoefficientValidator.Setup(m => m.Validate(It.IsAny<ComponentCoefficient>()))
				.ReturnsAsync(new Tuple<bool, string>(true, string.Empty));
			mockComponentCoefficientValidatorfactory
				.Setup(m => m.GetComponentCoefficientValidator(ComponentType.Material))
				.Returns(mockComponentCoefficientValidator.Object);

			var sfgGoodCompositionValidator = new
				CompositeComponentValidator(mockComponentCoefficientValidatorfactory.Object);
			var
				result = await sfgGoodCompositionValidator.Validate("sfg", new ComponentComposition(new
					List<ComponentCoefficient>
					{
						new ComponentCoefficient("code", 100, null, "uom", null,
							ComponentType.Material),
						new ComponentCoefficient("code", 100, null, "uom", null,
							ComponentType.Material),
						new ComponentCoefficient("code1", 100, null, "uom", null,
							ComponentType.Material)
					}));

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Combination of componet type and code should be unique.");
		}

		[Fact]
		public async Task Validate_ShouldReturnFalse_WhenSfgGoodCompostitionWithSfgCoefficientsIsPassed()
		{
			var mockComponentCoefficientValidatorfactory = new Mock<IComponentCoefficientValidatorFactory>();
			var mockComponentCoefficientValidator = new Mock<IComponentCoefficientValidator>();
			mockComponentCoefficientValidatorfactory
				.Setup(m => m.GetComponentCoefficientValidator(ComponentType.Material))
				.Returns(mockComponentCoefficientValidator.Object);

			var sfgGoodCompositionValidator = new
				CompositeComponentValidator(mockComponentCoefficientValidatorfactory.Object);
			var
				result = await sfgGoodCompositionValidator.Validate("sfg", new ComponentComposition(new
					List<ComponentCoefficient>
					{
					new ComponentCoefficient("code1", 100, null, "uom", null,
							ComponentType.SFG),
						new ComponentCoefficient("code", 100, null, "uom", null,
							ComponentType.Material),
						new ComponentCoefficient("code", 100, null, "uom", null,
							ComponentType.Material)
					}));

			result.Item1.ShouldBeEquivalentTo(false);
			result.Item2.ShouldBeEquivalentTo("Semi Finished Good can\'t have Semi Finished Good in its composition.");
		}
	}
}