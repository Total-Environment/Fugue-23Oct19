using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.AppService
{
	public class BrandCodeGeneratorTests
	{
		[Fact]
		public async void Generate_ShouldReturnBrandCode_WhenBrandIsPassedWithoutCode()
		{
			var mockCounterRepository = new Mock<ICounterRepository>();
			mockCounterRepository.Setup(m => m.NextValue("Brand")).ReturnsAsync(101);
			var generator = new BrandCodeGenerator(mockCounterRepository.Object);
			var brandCode = await generator.Generate("BAC");
			brandCode.Should().Be("BAC000101");
		}
	}
}