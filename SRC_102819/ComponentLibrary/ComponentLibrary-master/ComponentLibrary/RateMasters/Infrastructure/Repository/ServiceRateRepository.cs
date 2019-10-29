using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository
{
	public class ServiceRateRepository : IServiceRateRepository
	{
		private readonly IBank _bank;
		private readonly IMongoCollection<ServiceDao> _mongoCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRateRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		/// <param name="bank">The bank repo.</param>
		public ServiceRateRepository(IMongoCollection<ServiceDao> mongoCollection, IBank bank)
		{
			_mongoCollection = mongoCollection;
			_bank = bank;
		}

		/// <inheritdoc/>
		public async Task<IServiceRate> AddRate(IServiceRate materialRate)
		{
			var serviceDao = await GetService(materialRate.Id);

			if (serviceDao == null)
				throw new ResourceNotFoundException($"Service of {materialRate.Id}");

			var serviceRate = serviceDao.AddRate(new ServiceRateDao(materialRate));

			var builder = Builders<ServiceDao>.Filter.Eq(ServiceDao.ServiceCode, materialRate.Id);
			await _mongoCollection.ReplaceOneAsync(builder, serviceDao);

			return serviceRate.Domain(_bank);
		}

		private async Task<ServiceDao> GetService(string serviceCode)
		{
			var serviceDao =
				(await _mongoCollection.FindAsync(Builders<ServiceDao>.Filter.Eq(ServiceDao.ServiceCode, serviceCode)))
				.FirstOrDefault();
			return serviceDao;
		}

		/// <inheritdoc/>
		public async Task<IServiceRate> GetRate(string serviceCode, string location, DateTime @on, string typeOfPurchase)
		{
			if (string.IsNullOrWhiteSpace(serviceCode))
				throw new ArgumentException("Service Code is a mandatory field.");
			if (string.IsNullOrWhiteSpace(location))
				throw new ArgumentException("Location is a mandatory field.");
			if (string.IsNullOrWhiteSpace(typeOfPurchase))
				throw new ArgumentException("Type of Purchase is a mandatory field.");

			var serviceDao = await GetService(serviceCode);
			if (serviceDao == null)
				throw new ResourceNotFoundException($"Service of {serviceCode}");

			var serviceRate = serviceDao.GetRateDaos()
				.FindAll(m => m.Location == location
							  && m.TypeOfPurchase == typeOfPurchase
							  && m.AppliedOn <= on)
				.OrderByDescending(m => m.AppliedOn)
				.FirstOrDefault();

			if (serviceRate == null)
				throw new ResourceNotFoundException("Service Rate");

			return serviceRate.Domain(_bank);
		}

		/// <inheritdoc/>
		public async Task<PaginatedAndSortedList<IServiceRate>> GetRateHistory(string serviceCode, int pageNumber = 1, string sortColumn = "AppliedOn", SortOrder sortOrder = SortOrder.Descending,
			string location = "", string typeOfPurchase = "", DateTime? appliedOn = null)
		{
			if (string.IsNullOrWhiteSpace(serviceCode))
				throw new ArgumentException("Service Code is a mandatory field.");

			var serviceDao = await GetService(serviceCode);
			if (serviceDao == null)
				throw new ResourceNotFoundException($"Service of {serviceCode}");

			var batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);

			var serviceRateDaos = serviceDao.GetRateDaos();

			if (serviceRateDaos == null || serviceRateDaos.Count == 0)
				throw new ResourceNotFoundException($"Service Rate of service {serviceCode}");

			var order = sortColumn + " ascending";
			if (sortOrder.Equals(SortOrder.Descending))
				order = sortColumn + " descending";

			var serviceRatesFilteredBytypeOfPurchaseAndLocation = serviceRateDaos.OrderBy(order)
				.Where(m => string.IsNullOrEmpty(typeOfPurchase) || m.TypeOfPurchase == typeOfPurchase)
				.Where(m => string.IsNullOrEmpty(location) || m.Location == location);

			int totalRecords = 0;

			List<IServiceRate> sortedServiceRates;
			if (appliedOn != null)
			{
				var filteredServiceRates = serviceRatesFilteredBytypeOfPurchaseAndLocation
										   .Where(m => m.AppliedOn.Date.
													   CompareTo(appliedOn.Value.ToUniversalTime().Date) <= 0)
										   .GroupBy(m => new { m.TypeOfPurchase, m.Location })
										   .Select(m => m.OrderBy("AppliedOn descending").First());

				totalRecords = filteredServiceRates.Count();
				sortedServiceRates = filteredServiceRates.Select(m => m.Domain(_bank))
														 .Skip((pageNumber - 1) * batchSize)
														 .Take(batchSize).ToList();
			}
			else
			{
				totalRecords = serviceRatesFilteredBytypeOfPurchaseAndLocation.Count();
				sortedServiceRates = serviceRatesFilteredBytypeOfPurchaseAndLocation.Select(m => m.Domain(_bank))
					.Skip((pageNumber - 1) * batchSize)
					.Take(batchSize).ToList();
			}

			if (sortedServiceRates.Count == 0)
				throw new ResourceNotFoundException($"Service Rate of service {serviceCode}");

			return new PaginatedAndSortedList<IServiceRate>(sortedServiceRates, pageNumber, totalRecords, batchSize, sortColumn, sortOrder);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IServiceRate>> GetRates(string serviceCode, DateTime @on)
		{
			if (string.IsNullOrWhiteSpace(serviceCode))
				throw new ArgumentException("Service Code is a mandatory field.");

			var serviceDao = await GetService(serviceCode);
			if (serviceDao == null)
				throw new ResourceNotFoundException($"Service of {serviceCode}");

			var serviceRateDaos = serviceDao.GetRateDaos();

			if (serviceRateDaos == null || serviceRateDaos.Count == 0)
				throw new ResourceNotFoundException($"Service Rate of service {serviceCode}");

			return serviceRateDaos.FindAll(m => m.AppliedOn <= on)
				.GroupBy(x => new { x.Location, x.TypeOfPurchase })
				.Select(y => y.OrderByDescending(z => z.AppliedOn).First().Domain(_bank));
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IServiceRate>> GetRates(string serviceCode, string location, DateTime on)
		{
			if (string.IsNullOrWhiteSpace(serviceCode))
				throw new ArgumentException("Service Code is a mandatory field.");
			if (string.IsNullOrWhiteSpace(location))
				throw new ArgumentException("Location is a mandatory field.");

			var serviceDao = await GetService(serviceCode);
			if (serviceDao == null)
				throw new ResourceNotFoundException($"Service of {serviceCode}");

			var serviceRateDaos = serviceDao.GetRateDaos();

			if (serviceRateDaos == null || serviceRateDaos.Count == 0)
				throw new ResourceNotFoundException($"Service Rate of service {serviceCode}");

			var serviceRate = serviceRateDaos.FindAll(dao => location.Equals(dao.Location)
															   && dao.AppliedOn <= on).ToList()
				.GroupBy(dao => dao.TypeOfPurchase)
				.Select(group => group.OrderByDescending(g => g.AppliedOn).FirstOrDefault())
				.Select(dao => dao.Domain(_bank));
			return serviceRate;
		}
	}
}