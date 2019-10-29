using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <inheritdoc/>     
    public class CounterRepository : ICounterRepository
    {
        private readonly IMongoRepository<CounterDao> _mongoRepository;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterRepository"/> class.
        /// </summary>
        /// <param name="mongoRepository">The mongo repository.</param>
        public CounterRepository(IMongoRepository<CounterDao> mongoRepository)
        {
            _mongoRepository = mongoRepository;
            var seconds = Convert.ToInt32(ConfigurationManager.AppSettings["CounterTimeout"]);
            _timeout = TimeSpan.FromSeconds(seconds);
        }

        /// <inheritdoc/>
        public async Task<int> NextValue(string counterId)
        {
            await _semaphoreSlim.WaitAsync(_timeout);
            try
            {
                var counter = await _mongoRepository.FindBy("CounterId", counterId);
                if (counter == null)
                {
                    return (await _mongoRepository.Add(new CounterDao { CounterId = counterId, Value = 1 })).Value;
                }
                return (await _mongoRepository.Increment(counter, "Value")).Value;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <inheritdoc/>
        public async Task Update(int counterValue, string counterId)
        {
            await _semaphoreSlim.WaitAsync(_timeout);
            try
            {
                var counter = await _mongoRepository.FindBy("CounterId", counterId);
                if (counter == null)
                {
                    counter = await _mongoRepository.Add(new CounterDao() { CounterId = counterId, Value = 1 });
                }
                counter.Value = counterValue;
                await _mongoRepository.Update(counter);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <inheritdoc/>
        public async Task<int> CurrentValue(string counterId)
        {
            var counter = await _mongoRepository.FindBy("CounterId", counterId);
            if (counter == null)
            {
                return 0;
            }
            return counter.Value;
        }
    }
}