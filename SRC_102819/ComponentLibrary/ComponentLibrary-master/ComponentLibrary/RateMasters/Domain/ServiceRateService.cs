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
	/// The Service Rate Service
	/// </summary>
	/// <seealso cref="IServiceRateService"/>
	public class ServiceRateService : IServiceRateService
	{
		private readonly IServiceRateRepository _serviceRateRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRateService"/> class.
		/// </summary>
		/// <param name="serviceRateRepository">The service rate repository.</param>
		/// <exception cref="System.ArgumentException">ServiceRateRepository cannot be null.</exception>
		public ServiceRateService(IServiceRateRepository serviceRateRepository)
		{
			if (serviceRateRepository == null)
				throw new ArgumentException("ServiceRateRepository cannot be null.");
			_serviceRateRepository = serviceRateRepository;
		}

		/// <inheritdoc/>
		public async Task<IServiceRate> GetRate(string id, string location, DateTime @on, string typeOfPurchase)
		{
			return await _serviceRateRepository.GetRate(id, location, @on, typeOfPurchase);
		}

		/// <inheritdoc/>
		public Task<PaginatedAndSortedList<IServiceRate>> GetRateHistory(string id, IRateSearchRequest rateSearchRequest)
		{
			var serviceRateSearchRequest = (ServiceRateSearchRequest)rateSearchRequest;
			return _serviceRateRepository.GetRateHistory(id, serviceRateSearchRequest.PageNumber, serviceRateSearchRequest.SortColumn, serviceRateSearchRequest.SortOrder,
				serviceRateSearchRequest.Location, serviceRateSearchRequest.TypeOfPurchase, serviceRateSearchRequest.AppliedOn);
		}

		/// <inheritdoc/>
		public Task<IEnumerable<IServiceRate>> GetRates(string id, DateTime on)
		{
			return _serviceRateRepository.GetRates(id, on);
		}

		/// <inheritdoc/>
		public async Task<IServiceRate> CreateRate(IServiceRate serviceRate)
		{
			if (serviceRate == null)
				throw new ArgumentException("ServiceRate canot be null.");
			return await _serviceRateRepository.AddRate(serviceRate);
		}

		/// <summary>
		/// Gets the average landed rate.
		/// </summary>
		/// <param name="serviceid">The serviceid.</param>
		/// <param name="location">The location.</param>
		/// <param name="on">The on.</param>
		/// <param name="currency">The currency.</param>
		/// <returns></returns>
		/// <exception cref="ResourceNotFoundException"></exception>
		/// <inheritdoc/>
		public async Task<Money> GetAverageLandedRate(string serviceid, string location, DateTime @on, string currency)
		{
			var serviceRates = (await _serviceRateRepository.GetRates(serviceid, location, on)).ToList();

			if (serviceRates.Count == 0)
				throw new ResourceNotFoundException($"Rate for {serviceid} as of {on.ToShortDateString()} for {location} location");
			var rate = (await serviceRates.Select(m => m.LandedRate()).Aggregate(Task.FromResult(new Money(0, currency)), async (m, n) => await (await (await n).ConvertToCurrency(currency, DateTime.MinValue)).Add(await m))).Divide(serviceRates.Count);
			return rate;
		}

		/// <inheritdoc/>
		public async Task<Money> GetLandedRate(string serviceid, string location, DateTime @on, string currency, string typeOfPurchase)
		{
			var serviceRate = await _serviceRateRepository.GetRate(serviceid, location, @on, typeOfPurchase);
			return await serviceRate.LandedRate();
		}

		/// <inheritdoc/>
		public async Task<Money> GetControlBaseRate(string serviceid, string location, DateTime @on, string currency, string typeOfPurchase)
		{
			var serviceRate = await _serviceRateRepository.GetRate(serviceid, location, @on, typeOfPurchase);
			return serviceRate.ControlBaseRate;
		}

		/// <inheritdoc/>
		public async Task<Money> GetAverageControlBaseRate(string serviceid, string location, DateTime @on, string currency)
		{
			var serviceRates = (await _serviceRateRepository.GetRates(serviceid, location, on)).ToList();
			return (await serviceRates.Select(m => m.ControlBaseRate).Aggregate(Task.FromResult(new Money(0, "INR")), async (m, n) => await (await n.ConvertToCurrency(currency, DateTime.MinValue)).Add(await m))).Divide(serviceRates.Count);
        }
	}
}