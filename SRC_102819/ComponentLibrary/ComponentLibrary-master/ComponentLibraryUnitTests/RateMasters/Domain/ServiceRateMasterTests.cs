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
    public class ServiceRateMasterTests
    {
        private class Fixture
        {
            private readonly Mock<IServiceRateRepository> _rateRepository = new Mock<IServiceRateRepository>();
            private readonly List<Action> _verifications = new EditableList<Action>();

            public void VerifyExpectations()
            {
                _verifications.ForEach(a => a.Invoke());
            }

            public ServiceRateService Sut()
            {
                return new ServiceRateService(_rateRepository.Object);
            }

            public Fixture RateRepositoryAddAccepts(IServiceRate serviceRate)
            {
                _verifications.Add(() => _rateRepository.Verify(r => r.AddRate(serviceRate), Times.Once));
                return this;
            }

            public Fixture RateRepositoryGetReturns(IServiceRate serviceRate)
            {
                _rateRepository.Setup(
                        r => r.GetRate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                    .ReturnsAsync(serviceRate);
                return this;
            }

            public Fixture RateRepositoryGetsReturns(IEnumerable<IServiceRate> serviceRates)
            {
                _rateRepository.Setup(
                        r => r.GetRates(It.IsAny<string>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(serviceRates);
                return this;
            }
        }

        [Fact]
        public async void CreateRateService_Should_QueryRateRepositoryWithServiceRate()
        {
            var serviceRateStub = new Mock<IServiceRate>();
            serviceRateStub.Setup(s => s.AppliedOn).Returns(DateTime.Now);
            var fixture = new Fixture()
                .RateRepositoryAddAccepts(serviceRateStub.Object);

            await fixture.Sut().CreateRate(serviceRateStub.Object);

            fixture.VerifyExpectations();
        }

        [Fact]
        public void CreateRateService_Should_ThrowArgumentExceptionWhenServiceRateIsPassedAsNull()
        {
            var sut = new Fixture().Sut();

            Func<Task<IServiceRate>> func = () => sut.CreateRate(null);

            func.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Creation_ShouldThrowArgumentException_WhenRateRepositoryIsPassedAsNull()
        {
            Action action = () => new ServiceRateService(null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task GetServiceRate_Should_ReturnValuePassedByServiceRateRepository()
        {
            var serviceRate = new Mock<IServiceRate>();
            var sut = new Fixture()
                .RateRepositoryGetReturns(serviceRate.Object)
                .Sut();

            var result = await sut.GetRate("serviceId", "location", DateTime.Today, "typeOfPurchase");

            result.Should().Be(serviceRate.Object);
        }

        [Fact]
        public async Task GetServiceRates_Should_ReturnValuePassedByServiceRateRepository()
        {
            var serviceRates = new Mock<IEnumerable<IServiceRate>>();
            var sut = new Fixture()
                .RateRepositoryGetsReturns(serviceRates.Object)
                .Sut();

            var result = await sut.GetRates("serviceId", DateTime.Today);

            result.ShouldBeEquivalentTo(serviceRates.Object);
        }

        [Fact]
        public void Should_ImplementIServiceRateMaster()
        {
            var systemUnderTest = new Fixture().Sut();

            systemUnderTest.Should().BeAssignableTo<IServiceRateService>();
        }
    }
}