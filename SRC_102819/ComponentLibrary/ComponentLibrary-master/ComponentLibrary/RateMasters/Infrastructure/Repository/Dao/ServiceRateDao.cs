using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for Service Rate
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao.ComponentRateDao" />
    public class ServiceRateDao : ComponentRateDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRateDao"/> class.
        /// </summary>
        /// <param name="serviceRate">The service rate.</param>
        /// <exception cref="System.ArgumentException">Service Rate Entity cannot be null.</exception>
        public ServiceRateDao(IServiceRate serviceRate)
        {
            if (serviceRate == null)
                throw new ArgumentException("Service Rate Entity cannot be null.");
            ServiceId = serviceRate.Id;
            Location = serviceRate.Location;
            AppliedOn = serviceRate.AppliedOn;
            TypeOfPurchase = serviceRate.TypeOfPurchase;
            MarketFluctuation = serviceRate.MarketFluctuation;
            LocationVariance = serviceRate.LocationVariance;
            ControlBaseRate = new MoneyDao(serviceRate.ControlBaseRate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRateDao"/> class.
        /// </summary>
        public ServiceRateDao()
        {
        }

        /// <summary>
        /// Gets or sets the location variance.
        /// </summary>
        /// <value>
        /// The location variance.
        /// </value>
        public decimal LocationVariance { get; set; }

        /// <summary>
        /// Gets or sets the market fluctuation.
        /// </summary>
        /// <value>
        /// The market fluctuation.
        /// </value>
        public decimal MarketFluctuation { get; set; }

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// Domains the specified bank.
        /// </summary>
        /// <param name="bank">The bank.</param>
        /// <returns></returns>
        public virtual IServiceRate Domain(IBank bank)
        {
            return new ServiceRate(AppliedOn,Location, ServiceId, ControlBaseRate.Domain(bank), LocationVariance, MarketFluctuation, TypeOfPurchase);
        }
    }
}