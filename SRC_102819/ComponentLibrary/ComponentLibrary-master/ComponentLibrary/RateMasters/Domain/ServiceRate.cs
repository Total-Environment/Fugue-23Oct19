using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The Service Rate
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IServiceRate" />
    public class ServiceRate : IServiceRate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRate"/> class.
        /// </summary>
        /// <param name="appliedOn">The applied on.</param>
        /// <param name="location">The location.</param>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="rate">The rate.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <exception cref="ArgumentException">
        /// Id cannot be null.
        /// or
        /// CreatedOn cannot be null.
        /// or
        /// Location cannot be null or whitespace.
        /// or
        /// Rate cannot be null.
        /// or
        /// TypeOfPurchase cannot be null.
        /// </exception>
        public ServiceRate(DateTime appliedOn, string location, string serviceId, Money controlBaseRate, decimal locationVariance, decimal marketFluctuation, string typeOfPurchase)
        {
            if (string.IsNullOrWhiteSpace(serviceId))
                throw new ArgumentException("Id cannot be null.");
            if (appliedOn == null)
                throw new ArgumentException("CreatedOn cannot be null.");
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be null or whitespace.");
            if(string.IsNullOrWhiteSpace(typeOfPurchase))
                throw new ArgumentException("TypeOfPurchase cannot be null.");
            if(locationVariance < 0 || marketFluctuation < 0) 
                throw new ArgumentException("Coefficient value cannot be negative.");
            AppliedOn = appliedOn;
            Location = location;
            TypeOfPurchase = typeOfPurchase;
            Id = serviceId;
            ControlBaseRate = controlBaseRate;
            LocationVariance = locationVariance;
            MarketFluctuation = marketFluctuation;
        }

        public decimal MarketFluctuation { get; set; }

        public decimal LocationVariance { get; set; }

        /// <inheritdoc/>
        public DateTime AppliedOn { get; }
        /// <inheritdoc/>
        public string Location { get; }
        /// <inheritdoc/>
        public string Id { get; }
        /// <inheritdoc/>
        public string TypeOfPurchase { get; }

        /// <summary>
        /// Gets or sets the control base rate.
        /// </summary>
        /// <value>
        /// The control base rate.
        /// </value>
        public Money ControlBaseRate { get; set; }

        /// <summary>
        /// Gets or sets the landed rate.
        /// </summary>
        /// <value>
        /// The landed rate.
        /// </value>
        public async Task<Money> LandedRate()
        {
            var baseRate = await ControlBaseRate.ConvertToCurrency("INR",AppliedOn);
            var moneyFromCoefficients = await baseRate.Percentage(LocationVariance).Add(baseRate.Percentage(MarketFluctuation));
            var landedRate = await baseRate.Add(moneyFromCoefficients);
            return landedRate;
        }
    }
}