using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.RateMasters.Domain
{
    public class MaterialRateMasterTests
    {
        private class Fixture
        {
            private readonly Mock<IMaterialRateRepository> _rateRepository = new Mock<IMaterialRateRepository>();
            private readonly List<Action> _verifications = new EditableList<Action>();

            public void VerifyExpectations()
            {
                _verifications.ForEach(a => a.Invoke());
            }

            public MaterialRateService Sut()
            {
                return new MaterialRateService(_rateRepository.Object);
            }

            public Fixture RateRepositoryAddAccepts(IMaterialRate materialRate)
            {
                _verifications.Add(() => _rateRepository.Verify(r => r.AddRate(materialRate), Times.Once));
                return this;
            }

            public Fixture RateRepositoryGetReturns(IMaterialRate materialRate)
            {
                _rateRepository.Setup(
                        r => r.GetRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync(materialRate);
                return this;
            }

            public Fixture RateRepositoryGetsReturns(IEnumerable<IMaterialRate> materialRates)
            {
                _rateRepository.Setup(
                        r => r.GetRates(It.IsAny<string>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(materialRates);
                return this;
            }
        }

        [Fact]
        public async void CreateRateMaterial_Should_QueryRateRepositoryWithMaterialRate()
        {
            var materialRateStub = new Mock<IMaterialRate>();
            var fixture = new Fixture()
                .RateRepositoryAddAccepts(materialRateStub.Object);

            await fixture.Sut().CreateRate(materialRateStub.Object);

            fixture.VerifyExpectations();
        }

        [Fact]
        public void CreateRateMaterial_Should_ThrowArgumentExceptionWhenMaterialRateIsPassedAsNull()
        {
            var sut = new Fixture().Sut();

            Func<Task<IMaterialRate>> func = () => sut.CreateRate(null);

            func.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_ShouldThrowArgumentException_WhenRateRepositoryIsPassedAsNull()
        {
            Action action = () => new MaterialRateService(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task GetMaterialRate_Should_ReturnValuePassedByMaterialRateRepository()
        {
            var materialRate = new Mock<IMaterialRate>();
            var sut = new Fixture()
                .RateRepositoryGetReturns(materialRate.Object)
                .Sut();

            var result = await sut.GetRate("materialId", "location", DateTime.Today, "typeOfPurchase");

            result.Should().Be(materialRate.Object);
        }

        [Fact]
        public async Task GetMaterialRates_Should_ReturnValuePassedByMaterialRateRepository()
        {
            var materialRates = new Mock<IEnumerable<IMaterialRate>>();
            var sut = new Fixture()
                .RateRepositoryGetsReturns(materialRates.Object)
                .Sut();

            var result = await sut.GetRates("materialId", DateTime.Today);

            result.ShouldBeEquivalentTo(materialRates.Object);
        }

        [Fact]
        public void Should_ImplementIMaterialRateMaster()
        {
            var systemUnderTest = new Fixture().Sut();

            systemUnderTest.Should().BeAssignableTo<IMaterialRateService>();
        }
    }
}