using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
	/// <summary>
	/// Service class for all material rate endpoints
	/// </summary>
	/// <seealso cref="IMaterialRateService"/>
	public class MaterialRateService : IMaterialRateService
	{
		private readonly IMaterialRateRepository _materialRateRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialRateService"/> class.
		/// </summary>
		/// <param name="materialRateRepository">The material rate repository.</param>
		/// <exception cref="System.ArgumentException">MaterialRateRepository cannot be null.</exception>
		public MaterialRateService(IMaterialRateRepository materialRateRepository)
		{
			if (materialRateRepository == null)
				throw new ArgumentException("MaterialRateRepository cannot be null.");
			_materialRateRepository = materialRateRepository;
		}

		/// <inheritdoc/>
		public async Task<IMaterialRate> GetRate(string id, string location, DateTime @on, string typeOfPurchase)
		{
			return await _materialRateRepository.GetRate(id, location, @on, typeOfPurchase);
		}

		/// <inheritdoc/>
		public async Task<PaginatedAndSortedList<IMaterialRate>> GetRateHistory(string materialId, IRateSearchRequest rateSearchRequest)
		{
			var materialRateSearchRequest = (MaterialRateSearchRequest)rateSearchRequest;
			return await _materialRateRepository.GetRateHistory(materialId, materialRateSearchRequest.PageNumber, materialRateSearchRequest.SortColumn, materialRateSearchRequest.SortOrder,
				materialRateSearchRequest.Location, materialRateSearchRequest.TypeOfPurchase, materialRateSearchRequest.AppliedOn);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IMaterialRate>> GetRates(string id, DateTime on)
		{
			return await _materialRateRepository.GetRates(id, @on);
		}

		/// <inheritdoc/>
		public async Task<IMaterialRate> CreateRate(IMaterialRate materialRate)
		{
			if (materialRate == null)
				throw new ArgumentException("MaterialRate canot be null.");
			return await _materialRateRepository.AddRate(materialRate);
		}

		/// <inheritdoc/>
		public async Task<Money> GetAverageLandedRate(string materialid, string location, DateTime @on, string currency)
		{
			var materialRates = (await _materialRateRepository.GetRates(materialid, location, on)).ToList();

			if (materialRates.Count == 0)
				throw new ResourceNotFoundException($"Rate for {materialid} as of {on.ToShortDateString()} for {location} location");
			var rate = (await materialRates.Select(m => m.LandedRate()).Aggregate(Task.FromResult(new Money(0, currency)), async (m, n) => await (await (await n).ConvertToCurrency(currency,DateTime.MinValue)).Add(await m))).Divide(materialRates.Count);
			return rate;
		}

		/// <inheritdoc/>
		public async Task<Money> GetLandedRate(string materialid, string location, DateTime @on, string currency, string typeOfPurchase)
		{
			var materialRate = await _materialRateRepository.GetRate(materialid, location, @on, typeOfPurchase);
			return await materialRate.LandedRate();
		}

		/// <inheritdoc/>
		public async Task<Money> GetControlBaseRate(string materialid, string location, DateTime @on, string currency, string typeOfPurchase)
		{
			var materialRate = await _materialRateRepository.GetRate(materialid, location, @on, typeOfPurchase);
			return materialRate.ControlBaseRate;
		}

		/// <inheritdoc/>
		public async Task<Money> GetAverageControlBaseRate(string materialid, string location, DateTime @on, string currency)
		{
			var materialRates = (await _materialRateRepository.GetRates(materialid, location, on)).ToList();
		    return
		    (await materialRates.Select(m => m.ControlBaseRate)
		        .Aggregate(Task.FromResult(new Money(0, "INR")),
		            async (m, n) => await (await n.ConvertToCurrency(currency, DateTime.MinValue)).Add(await m))).Divide(
		        materialRates.Count);

		}
	}
}