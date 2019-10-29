using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The material rates
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IMaterialRate" />
    public class MaterialRate : IMaterialRate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRate"/> class.
        /// </summary>
        /// <param name="appliedOn">The applied on.</param>
        /// <param name="location">The location.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="controlBaseRate">The control base rate.</param>
        /// <param name="freightCharges">The freight charges.</param>
        /// <param name="insuranceCharges">The insurance charges.</param>
        /// <param name="basicCustomsDuty">The basic customs duty.</param>
        /// <param name="clearanceCharges">The clearance charges.</param>
        /// <param name="taxVariance">The tax variance.</param>
        /// <param name="locationVariance">The location variance.</param>
        /// <param name="marketFluctuation">The market fluctuation.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <exception cref="System.ArgumentException">
        /// MaterialId cannot be null.
        /// or
        /// CreatedOn cannot be null.
        /// or
        /// Location cannot be null or whitespace.
        /// or
        /// TypeOfPurchase cannot be null.
        /// </exception>
        public MaterialRate(DateTime appliedOn, string location, string id, Money controlBaseRate, decimal freightCharges, decimal insuranceCharges, decimal basicCustomsDuty, decimal clearanceCharges, decimal taxVariance, decimal locationVariance, decimal marketFluctuation, string typeOfPurchase)
        {
            if (controlBaseRate == null)
                throw new ArgumentNullException(nameof(controlBaseRate));
            if (controlBaseRate.Value == 0)
                throw new ArgumentException();
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("MaterialId cannot be null.");
            if (appliedOn == null)
                throw new ArgumentException("CreatedOn cannot be null.");
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location cannot be null or whitespace.");
            if (string.IsNullOrWhiteSpace(typeOfPurchase))
                throw new ArgumentException("TypeOfPurchase cannot be null.");
            if (freightCharges < 0
               || insuranceCharges < 0
               || basicCustomsDuty < 0
               || clearanceCharges < 0
               || taxVariance < 0
               || locationVariance < 0
               || marketFluctuation < 0)
            {
                throw new ArgumentException("Coefficient value cannot be negative.");
            }
            AppliedOn = appliedOn;
            Location = location;
            Id = id;
            ControlBaseRate = controlBaseRate;
            FreightCharges = freightCharges;
            InsuranceCharges = insuranceCharges;
            BasicCustomsDuty = basicCustomsDuty;
            ClearanceCharges = clearanceCharges;
            TaxVariance = taxVariance;
            LocationVariance = locationVariance;
            MarketFluctuation = marketFluctuation;
            TypeOfPurchase = typeOfPurchase;
        }

        /// <summary>
        /// Gets or sets the market fluctuation.
        /// </summary>
        /// <value>
        /// The market fluctuation.
        /// </value>
        public decimal MarketFluctuation { get; set; }

        /// <summary>
        /// Gets or sets the location variance.
        /// </summary>
        /// <value>
        /// The location variance.
        /// </value>
        public decimal LocationVariance { get; set; }

        /// <summary>
        /// Gets or sets the tax variance.
        /// </summary>
        /// <value>
        /// The tax variance.
        /// </value>
        public decimal TaxVariance { get; set; }

        /// <summary>
        /// Gets or sets the clearance charges.
        /// </summary>
        /// <value>
        /// The clearance charges.
        /// </value>
        public decimal ClearanceCharges { get; set; }

        /// <summary>
        /// Gets or sets the basic customs duty.
        /// </summary>
        /// <value>
        /// The basic customs duty.
        /// </value>
        public decimal BasicCustomsDuty { get; set; }

        /// <summary>
        /// Gets or sets the insurance charges.
        /// </summary>
        /// <value>
        /// The insurance charges.
        /// </value>
        public decimal InsuranceCharges { get; set; }

        /// <summary>
        /// Gets or sets the freight charges.
        /// </summary>
        /// <value>
        /// The freight charges.
        /// </value>
        public decimal FreightCharges { get; set; }

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
        public async Task<Money> LandedRate() {
            var baseRate = await ControlBaseRate.ConvertToCurrency("INR",AppliedOn);
            var baseRateForIN = await baseRate.Add(baseRate.Percentage(FreightCharges));
            var baseRateForOthers = await baseRateForIN.Add(baseRateForIN.Percentage(InsuranceCharges));
            var baseRateForBCD = baseRateForOthers.Percentage(101);
            var landedRate = baseRateForOthers;
            landedRate = await landedRate.Add(baseRateForBCD.Percentage(BasicCustomsDuty));
            landedRate = await landedRate.Add(baseRateForOthers.Percentage(ClearanceCharges));
            landedRate = await landedRate.Add(baseRateForOthers.Percentage(TaxVariance));
            landedRate = await landedRate.Add(baseRateForOthers.Percentage(LocationVariance));
            landedRate = await landedRate.Add(baseRateForOthers.Percentage(MarketFluctuation));
            //var landedRate = money.Aggregate(baseRateForOthers, async (sum, value) => await sum.Add(value));
            return landedRate;
        }

        /// <inheritdoc/>
        public DateTime AppliedOn { get; }
        
        /// <inheritdoc/>
        public string Location { get; }
        
        /// <inheritdoc/>
        public string TypeOfPurchase { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; }
    }
}