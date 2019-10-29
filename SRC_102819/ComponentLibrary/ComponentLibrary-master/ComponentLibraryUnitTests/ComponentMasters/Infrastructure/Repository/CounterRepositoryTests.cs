using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using Xunit;

namespace TE.ComponentLibrary.ComponentLibrary.UnitTests.ComponentMasters.Infrastructure.Repository
{
    public class CounterRepositoryTests
    {
        class Fixture
        {
            private Mock<IMongoRepository<CounterDao>> _mockRepository;
            private CounterDao _counterDao;
            private readonly IList<Action> _verifications;

            public Fixture()
            {
                _verifications = new List<Action>();
            }

            public Fixture WithMockRepository()
            {
                _mockRepository = new Mock<IMongoRepository<CounterDao>>();
                return this;
            }

            public Fixture WithCounter(CounterDao counterDao)
            {
                _counterDao = counterDao;
                _mockRepository.Setup(m => m.FindBy("CounterId", _counterDao.CounterId)).ReturnsAsync(counterDao);
                return this;
            }

            public Fixture AcceptingIncrementTo(CounterDao incrementedCounter)
            {
                _mockRepository.Setup(m => m.Increment(_counterDao, "Value")).ReturnsAsync(incrementedCounter);
                _verifications.Add(() => _mockRepository.Verify(m => m.Increment(_counterDao, "Value"), Times.Once));
                return this;
            }

            public Fixture VerifyExpectations()
            {
                foreach (var verification in _verifications)
                {
                    verification.Invoke();
                }
                return this;
            }

            public CounterRepository SystemUnderTest()
            {
                return new CounterRepository(_mockRepository.Object);
            }

            public Fixture WithoutCounter(string counterId)
            {
                _mockRepository.Setup(m => m.FindBy("CounterId", counterId)).ReturnsAsync(null);
                return this;
            }

            public Fixture Initializing(string counterId)
            {
                var counter = new CounterDao() { CounterId = counterId, Value = 1 };
                _mockRepository.Setup(r => r.Add(counter)).ReturnsAsync(counter);
                _verifications.Add(() => _mockRepository.Verify(r => r.Add(new CounterDao() {CounterId = counterId, Value = 1}), Times.Once));
                return this;
            }

            public Fixture AcceptingUpdate(CounterDao expected)
            {
                _verifications.Add(() => _mockRepository.Verify(c => c.Update(expected), Times.Once));
                return this;
            }
        }
        [Fact]
        public async void Counter_WhenCalledWithAKeyThatExists_IncrementsTheValueAndReturnsIt()
        {
            const string counterKey = "test-counter";
            var counter = new CounterDao() { CounterId = counterKey, Value = 2 };
            var incrementedCounter = new CounterDao() {CounterId = counterKey, Value = 3};


            var fixture =
                new Fixture().WithMockRepository()
                    .WithCounter(counter)
                    .AcceptingIncrementTo(incrementedCounter);

            var value = await fixture.SystemUnderTest().NextValue(counterKey);
            
            value.Should().Be(3);
            fixture.VerifyExpectations();
        }

        [Fact]
        public async void Counter_WhenCalledWithAKeyThatDoesntExist_InitializesTheKeyAndReturns1()
        {
            const string counterId = "test-counter";


            var fixture =
                new Fixture().WithMockRepository()
                    .WithoutCounter(counterId)
                    .Initializing(counterId);

            var value = await fixture.SystemUnderTest().NextValue(counterId);

            fixture.VerifyExpectations();
            value.Should().Be(1);
        }

        [Fact]
        public async void Update_ShouldUpdatesTheCounter_WhenGivenValidCounterIdAndCounterValue()
        {
            const string counterId = "test-counter";
            var counter = new CounterDao() { CounterId = counterId, Value = 2 };

            var fixture = new Fixture().WithMockRepository()
                .WithCounter(counter)
                .AcceptingUpdate(new CounterDao() { CounterId = counterId, Value = 4 });

            await fixture.SystemUnderTest().Update(4, counterId);

            fixture.VerifyExpectations();
        }

        [Fact]
        public async void CurrentValue_ShouldReturnValueOfCounter_WhenGivenValidCounterId()
        {
            const string counterId = "test-counter";
            var counter = new CounterDao() { CounterId = counterId, Value = 2 };

            var fixture = new Fixture().WithMockRepository()
                .WithCounter(counter);

            var value = await fixture.SystemUnderTest().CurrentValue(counterId);

            value.Should().Be(2);
        }

        [Fact]
        public async void CurrentValue_ShouldReturnZero_WhenGivenCounterIdThatIsNotExist()
        {
            const string counterId = "test-counter";

            var fixture = new Fixture().WithMockRepository();

            var value = await fixture.SystemUnderTest().CurrentValue(counterId);

            value.Should().Be(0);
        }
    }
}
