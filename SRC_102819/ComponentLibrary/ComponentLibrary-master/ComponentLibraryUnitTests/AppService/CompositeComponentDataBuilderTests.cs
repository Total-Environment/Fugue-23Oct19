using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class CompositeComponentDataBuilderTests
	{
		[Fact]
		public async Task BuildData_ShouldReturnSfgGoodComposition_WhenComponentTypeIsMaterial()
		{
			var mockComponentCoefficientBuilderFactory = new Mock<IComponentCoefficientBuilderFactory>();
			var mockcomponentCoefficientBuilder = new Mock<IComponentCoefficientBuilder>();
			mockcomponentCoefficientBuilder.Setup(m => m.BuildData(It.IsAny<ComponentCoefficient>()))
				.ReturnsAsync(new ComponentCoefficient("Code", 10, new List<WastagePercentage>()
					{
						new WastagePercentage("Wastage1",10)
					}, "m2", "abcd",
					ComponentType.Material));
			mockComponentCoefficientBuilderFactory.Setup(m => m.GetComponentCoefficientBuilder(ComponentType.Material))
				.Returns(mockcomponentCoefficientBuilder.Object);
			var SfgGoodCompositionDataBuilder = new CompositeComponentDataBuilder(mockComponentCoefficientBuilderFactory.Object);
			var sfgCompostionData = await SfgGoodCompositionDataBuilder.BuildData(new ComponentComposition(new List<ComponentCoefficient>
				{
					new ComponentCoefficient("Code", 10, new List<WastagePercentage>()
						{
							new WastagePercentage("Wastage1",10)
						},
						null,
						null,
						ComponentType.Material)
				}
			));

			sfgCompostionData.ComponentCoefficients.First().Code.ShouldBeEquivalentTo("Code");
			sfgCompostionData.ComponentCoefficients.First().Coefficient.ShouldBeEquivalentTo(10);
			sfgCompostionData.ComponentCoefficients.First().ComponentType.ShouldBeEquivalentTo(ComponentType.Material);
			sfgCompostionData.ComponentCoefficients.First().Name.ShouldBeEquivalentTo("abcd");
			sfgCompostionData.ComponentCoefficients.First().UnitOfMeasure.ShouldBeEquivalentTo("m2");
			sfgCompostionData.ComponentCoefficients.First().TotalWastagePercentage.ShouldBeEquivalentTo(10);
		}
	}
}