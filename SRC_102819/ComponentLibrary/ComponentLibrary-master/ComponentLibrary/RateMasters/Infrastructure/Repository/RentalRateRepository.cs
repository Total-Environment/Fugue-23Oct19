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
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository
{
    /// <summary>
    /// The rental rate repository
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IRentalRateRepository" />
    public class RentalRateRepository : IRentalRateRepository
    {
        private readonly IMongoCollection<RentalRateDao> _mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRateRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        public RentalRateRepository(IMongoCollection<RentalRateDao> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        /// <inheritdoc />
        public async Task Add(IRentalRate rentalRate)
        {
            var rentalRateDao = new RentalRateDao(rentalRate);

            if (await EntryAlreadyExisits(rentalRateDao))
            {
                throw new DuplicateResourceException("Rental rate already exists.");
            }
            AdjustAppliedFromToMakeitMidnightIST(rentalRateDao);
            await _mongoCollection.InsertOneAsync(rentalRateDao);
        }

        /// <inheritdoc />
        public async Task<IRentalRate> Get(string materialId, string unitOfMeasure, DateTime appliedFrom)
        {
            var rentalRateDao = (await _mongoCollection.FindAsync(dao => dao.MaterialId == materialId
                                                 && dao.AppliedFrom <= appliedFrom
                                                 && dao.UnitOfMeasure.Equals(unitOfMeasure))).FirstOrDefault();
            if (rentalRateDao == null)
            {
                throw new ResourceNotFoundException($"Rental rate for {materialId} is not available for {unitOfMeasure} as of {appliedFrom.ToShortDateString()}");
            }

            return rentalRateDao.GetDomain();
        }

        /// <inheritdoc />
        public async Task<PaginatedAndSortedList<IRentalRate>> GetAll(string materialId, string rentalUnit = null, DateTime? appliedFrom = null,
            int pageNumber = 1, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending)
        {
            var builder = Builders<RentalRateDao>.Filter;
            var filters = new List<FilterDefinition<RentalRateDao>> { builder.Eq(dao => dao.MaterialId, materialId) };
            if (appliedFrom != null)
                filters.Add(builder.Lte(dao => dao.AppliedFrom, appliedFrom.Value.ToUniversalTime()));
            if (rentalUnit != null)
                filters.Add(builder.Eq(dao => dao.UnitOfMeasure, rentalUnit));
            var filterDefinition = builder.And(filters);

            var rentalRateDaos = await _mongoCollection.FindWithSortAndPageAsync(filterDefinition, pageNumber, 0, sortColumn, sortOrder);

            int batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
            if (appliedFrom != null)
            {
                var items = rentalRateDaos.Items.GroupBy(c => c.UnitOfMeasure).ToList().Select(c => c.OrderByDescending(m => m.AppliedFrom).First()).ToList();
                rentalRateDaos = new PaginatedAndSortedList<RentalRateDao>(items, pageNumber, items.Count(), batchSize, sortColumn, sortOrder);
            }

            if (rentalRateDaos.TotalRecords == 0)
            {
                throw new ResourceNotFoundException("Rental rate not found.");
            }

            return rentalRateDaos.Select(r => r.GetDomain());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IRentalRate>> GetLatest(string materialId, DateTime appliedFrom)
        {
            var rentalRateDaos = (await _mongoCollection.FindAsync(dao => dao.MaterialId == materialId
                                                                         && dao.AppliedFrom <= appliedFrom)).ToList();
            if (!rentalRateDaos.Any())
            {
                throw new ResourceNotFoundException("Rental rate not found.");
            }

            var rentalRates = from rate in rentalRateDaos.ToList()
                              group rate by rate.UnitOfMeasure
                  into groups
                              select groups.OrderByDescending(p => p.AppliedFrom).First().GetDomain();
            return rentalRates;
        }

        private void AdjustAppliedFromToMakeitMidnightIST(RentalRateDao rentalRateDao)
        {
            rentalRateDao.AppliedFrom =
                rentalRateDao.AppliedFrom.Add(rentalRateDao.AppliedFrom.AdditionalTimeSinceMidnightIst());
        }

        private async Task<bool> EntryAlreadyExisits(RentalRateDao rentalRateDao)
        {
            return await _mongoCollection.CountAsync(dao => dao.MaterialId == rentalRateDao.MaterialId
                                                            && dao.UnitOfMeasure == rentalRateDao.UnitOfMeasure
                                                            && dao.AppliedFrom.Equals(rentalRateDao.AppliedFrom)) > 0;
        }
    }
}