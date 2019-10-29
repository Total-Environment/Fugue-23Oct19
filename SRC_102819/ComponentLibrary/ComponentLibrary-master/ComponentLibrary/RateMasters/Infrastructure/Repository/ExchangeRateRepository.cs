using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository
{
    /// <summary>
    /// The exchange rate repository
    /// </summary>
    /// <seealso cref="IExchangeRateRepository"/>
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly IMongoCollection<ExchangeRateDao> _mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo repository.</param>
        public ExchangeRateRepository(IMongoCollection<ExchangeRateDao> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }


        /// <inheritdoc />
        public async Task<IExchangeRate> CreateExchangeRate(IExchangeRate exchangeRate)
        {
            var dao = new ExchangeRateDao(exchangeRate);

            var previousExchangeRate = await FetchExchangeRate(dao.AppliedFrom, dao.FromCurrency, dao.ToCurrency);
            if (previousExchangeRate != null)
                throw new DuplicateResourceException(
                    $"ExchangeRate already exist with fromCurrency : {dao.FromCurrency}, toCurrency : {dao.ToCurrency}, appliedfrom : {dao.AppliedFrom}.");

            await _mongoCollection.InsertOneAsync(dao);
            var insertedExchangeRateDao = (await _mongoCollection.FindAsync(r => dao.ObjectId == r.ObjectId)).FirstOrDefault();
            if (insertedExchangeRateDao == null)
                throw new ResourceNotFoundException("Unable to save exchange rate.");
            return insertedExchangeRateDao.Domain();
        }


        /// <inheritdoc />
        public async Task<IExchangeRate> GetExchangeRate(string fromCurrency, string toCurrency, DateTime @on)
        {
            if (fromCurrency == null)
                throw new ArgumentException("Location cannot be null.");
            if (toCurrency == null)
                throw new ArgumentException("Location cannot be null.");

            var daos = (await _mongoCollection.FindAsync(
                dao => fromCurrency.Equals(dao.FromCurrency)
                       && toCurrency.Equals(dao.ToCurrency)
                       && on > dao.AppliedFrom)).ToList();

            if (daos == null)
                throw new ResourceNotFoundException("Exchange Rate");

            var exchangeRateDao = daos.OrderByDescending(d => d.AppliedFrom).FirstOrDefault();
            if (exchangeRateDao == null)
                throw new ResourceNotFoundException("Exchange Rate");
            return exchangeRateDao.Domain();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IExchangeRate>> GetAll()
        {
            /* TODO This thing is almost there. Find out how to do complete it
             * https://stackoverflow.com/questions/31489254/mongodb-get-id-of-the-maximum-value-in-a-group-by-query
             * return (await _mongoCollection.Aggregate()
                .Sort(new SortDefinitionBuilder<ExchangeRateDao>().Descending(d => d.AppliedFrom))
                .Group(new ProjectionDefinitionBuilder<ExchangeRateDao>().Include(d => d.FromCurrency)
                        .Include(d => d.ToCurrency).Include(d => d))
                        .Project(d => new BsonDocument { { "id", "" } })).ToListAsync();*/
            //group => new BsonDocument{{"$max", "$AppliedFrom"}})
            //.ToListAsync()).Select(s =>new ExchangeRate());

            var completeList = (await _mongoCollection.FindAsync(d => true)).ToList();
            return completeList.GroupBy(d => new { d.FromCurrency, d.ToCurrency })
                .Select(g => g.OrderByDescending(ga => ga.AppliedFrom).FirstOrDefault())
                .Select(s => s.Domain());
        }


        /// <inheritdoc />
        public async Task<IEnumerable<IExchangeRate>> GetAll(DateTime @on)
        {
            var exchangeRateCompleteList = (await _mongoCollection.FindAsync(t => true)).ToList();

            return exchangeRateCompleteList.GroupBy(d => new { d.FromCurrency, d.ToCurrency })
                .Select(g => g.Where(ga => ga.AppliedFrom <= on).OrderByDescending(ga => ga.AppliedFrom).FirstOrDefault())
                .Where(g => g != null)
                .Select(s => s.Domain());
        }


        /// <inheritdoc />
        public async Task<PaginatedAndSortedList<IExchangeRate>> GetHistory(string currencyType=null, DateTime? appliedFrom = null, int pageNumber = 1, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending)
        {
            var builder = Builders<ExchangeRateDao>.Filter;
            var filters = new List<FilterDefinition<ExchangeRateDao>>();
            if (appliedFrom != null)
                filters.Add(builder.Lte(dao => dao.AppliedFrom, appliedFrom.Value.ToUniversalTime()));
            if (currencyType != null)
                filters.Add(builder.Eq(dao => dao.FromCurrency, currencyType));
            var filterDefinition = builder.And(filters);

            var daos = await _mongoCollection.FindWithSortAndPageAsync(filterDefinition, pageNumber, 0, sortColumn, sortOrder);

            int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);

            if (appliedFrom != null)
            {
                var items = daos.Items.GroupBy(c => c.FromCurrency).ToList().Select(c => c.OrderByDescending(m=>m.AppliedFrom).First()).ToList();
                daos = new PaginatedAndSortedList<ExchangeRateDao>(items, pageNumber, items.Count(), batchSize, sortColumn, sortOrder);
            }

            return daos.Select(dao => dao.Domain());
        }

        private async Task<ExchangeRateDao> FetchExchangeRate(DateTime appliedFrom, string fromCurrency, string toCurrency)
        {
            return (await _mongoCollection.FindAsync(dao => fromCurrency == dao.FromCurrency
                                                          && toCurrency == dao.ToCurrency
                                                          && appliedFrom == dao.AppliedFrom)).FirstOrDefault();
        }
    }
}