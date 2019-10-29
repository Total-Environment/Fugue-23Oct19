using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository
{
	/// <summary>
	/// The material rate repository
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IMaterialRateRepository"/>
	public class MaterialRateRepository : IMaterialRateRepository
	{
		private readonly IBank _bank;
		private readonly IMongoCollection<MaterialDao> _mongoCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialRateRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		/// <param name="bank">The bank.</param>
		public MaterialRateRepository(IMongoCollection<MaterialDao> mongoCollection, IBank bank)
		{
			_mongoCollection = mongoCollection;
			_bank = bank;
		}

		/// <inheritdoc/>
		public async Task<IMaterialRate> AddRate(IMaterialRate materialRate)
		{
			var materialRateDao = new MaterialRateDao();
			var materialDao = await GetMaterial(materialRate.Id);

			if (materialDao == null)
				throw new ResourceNotFoundException($"Material of {materialRate.Id}");

		    var materialRateToBeAdded = new MaterialRateDao(materialRate);

            materialRateDao = materialDao.AddRate(materialRateToBeAdded);

			var builder = Builders<MaterialDao>.Filter.Eq(MaterialDao.MaterialCode, materialRate.Id);
			await _mongoCollection.ReplaceOneAsync(builder, materialDao);

			return materialRateDao.Domain(_bank);
		}

		private async Task<MaterialDao> GetMaterial(string materialCode)
		{
			var materialDao =
				(await _mongoCollection.FindAsync(Builders<MaterialDao>.Filter.Eq(MaterialDao.MaterialCode, materialCode)))
					.FirstOrDefault();
			return materialDao;
		}

		/// <inheritdoc/>
		public async Task<IMaterialRate> GetRate(string materialCode, string location, DateTime @on, string typeOfPurchase)
		{
			if (string.IsNullOrWhiteSpace(materialCode))
				throw new ArgumentException("Material Code is a mandatory field.");
			if (string.IsNullOrWhiteSpace(location))
				throw new ArgumentException("Location is a mandatory field.");
			if (string.IsNullOrWhiteSpace(typeOfPurchase))
				throw new ArgumentException("Type of Purchase is a mandatory field.");

			var materialDao = await GetMaterial(materialCode);
			if (materialDao == null)
				throw new ResourceNotFoundException($"Material of {materialCode}");

			var materialRate = materialDao.GetRateDaos()
				.FindAll(m => m.Location == location
						&& m.TypeOfPurchase == typeOfPurchase
						&& m.AppliedOn <= on)
				.OrderByDescending(m => m.AppliedOn)
				.FirstOrDefault();

			if (materialRate == null)
				throw new ResourceNotFoundException("Material Rate");

			return materialRate.Domain(_bank);
		}

		/// <inheritdoc/>
		public async Task<PaginatedAndSortedList<IMaterialRate>> GetRateHistory(string materialCode, int pageNumber = 1, string sortColumn = "AppliedOn", SortOrder sortOrder = SortOrder.Descending,
			string location = null, string typeOfPurchase = null, DateTime? appliedOn = null)
		{
			if (string.IsNullOrWhiteSpace(materialCode))
				throw new ArgumentException("Material Code is a mandatory field.");

			var materialDao = await GetMaterial(materialCode);
			if (materialDao == null)
				throw new ResourceNotFoundException($"Material of {materialCode}");

		    var batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
            
			var materialRateDaos = materialDao.GetRateDaos();

			if (materialRateDaos == null || materialRateDaos.Count == 0)
				throw new ResourceNotFoundException($"Material Rate of material {materialCode}");

		    var order = sortColumn + " ascending";
			if (sortOrder.Equals(SortOrder.Descending))
				order = sortColumn + " descending";

            var materialRatesFilteredBytypeOfPurchaseAndLocation = materialRateDaos.OrderBy(order)
                .Where(m => string.IsNullOrEmpty(typeOfPurchase) || m.TypeOfPurchase == typeOfPurchase)
                .Where(m => string.IsNullOrEmpty(location) || m.Location == location);

		    int totalRecords = 0;
           
            List<IMaterialRate> sortedMaterialRates;
            if (appliedOn != null)
            {
                var filteredMaterialRates = materialRatesFilteredBytypeOfPurchaseAndLocation
                                            .Where(m => m.AppliedOn.Date.
                                                        CompareTo(appliedOn.Value.ToUniversalTime().Date) <= 0)
                                            .GroupBy(m => new { m.TypeOfPurchase, m.Location })
                                            .Select(m => m.OrderBy("AppliedOn descending").First());

                totalRecords = filteredMaterialRates.Count();
                sortedMaterialRates  = filteredMaterialRates.Select(m => m.Domain(_bank))
                                                            .Skip((pageNumber - 1) * batchSize)
                                                            .Take(batchSize).ToList();
            }
            else
            {
                totalRecords = materialRatesFilteredBytypeOfPurchaseAndLocation.Count();
                sortedMaterialRates = materialRatesFilteredBytypeOfPurchaseAndLocation
                                        .Select(m => m.Domain(_bank))
                                        .Skip((pageNumber - 1) * batchSize)
                                        .Take(batchSize).ToList();
            }

			if (sortedMaterialRates.Count == 0)
				throw new ResourceNotFoundException($"Material Rate of material {materialCode}");

			return new PaginatedAndSortedList<IMaterialRate>(sortedMaterialRates, pageNumber, totalRecords, batchSize, sortColumn, sortOrder);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IMaterialRate>> GetRates(string materialCode, DateTime @on)
		{
			if (string.IsNullOrWhiteSpace(materialCode))
				throw new ArgumentException("Material Code is a mandatory field.");

			var materialDao = await GetMaterial(materialCode);
			if (materialDao == null)
				throw new ResourceNotFoundException($"Material of {materialCode}");

			var materialRateDaos = materialDao.GetRateDaos();

			if (materialRateDaos == null || materialRateDaos.Count == 0)
				throw new ResourceNotFoundException($"Material Rate of material {materialCode}");
            
			return materialRateDaos.FindAll(m => m.AppliedOn <= on)
					.GroupBy(x => new { x.Location, x.TypeOfPurchase })
					.Select(y => y.OrderByDescending(z => z.AppliedOn).First().Domain(_bank));
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IMaterialRate>> GetRates(string materialCode, string location, DateTime on)
		{
			if (string.IsNullOrWhiteSpace(materialCode))
				throw new ArgumentException("Material Code is a mandatory field.");
			if (string.IsNullOrWhiteSpace(location))
				throw new ArgumentException("Location is a mandatory field.");

			var materialDao = await GetMaterial(materialCode);
			if (materialDao == null)
				throw new ResourceNotFoundException($"Material of {materialCode}");

			var materialRateDaos = materialDao.GetRateDaos();

			if (materialRateDaos == null || materialRateDaos.Count == 0)
				throw new ResourceNotFoundException($"Material Rate of material {materialCode}");
            
			var materialRate = materialRateDaos.FindAll(dao => location.Equals(dao.Location)
														&& dao.AppliedOn <= on).ToList()
													.GroupBy(dao => dao.TypeOfPurchase)
													.Select(group => group.OrderByDescending(g => g.AppliedOn).FirstOrDefault())
													.Select(dao => dao.Domain(_bank));
			return materialRate;
		}
	}
}