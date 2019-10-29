using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// Interface for Material Rates
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IComponentRate" />
    public interface IMaterialRate : IComponentRate

    {
        /// <summary>
        /// Gets or sets the insurance charges.
        /// </summary>
        /// <value>
        /// The insurance charges.
        /// </value>
        decimal InsuranceCharges { get; set; }

        /// <summary>
        /// Gets or sets the freight charges.
        /// </summary>
        /// <value>
        /// The freight charges.
        /// </value>
        decimal FreightCharges { get; set; }

        /// <summary>
        /// Gets or sets the basic customs duty.
        /// </summary>
        /// <value>
        /// The basic customs duty.
        /// </value>
        decimal BasicCustomsDuty { get; set; }

        /// <summary>
        /// Gets or sets the clearance charges.
        /// </summary>
        /// <value>
        /// The clearance charges.
        /// </value>
        decimal ClearanceCharges { get; set; }

        /// <summary>
        /// Gets or sets the tax variance.
        /// </summary>
        /// <value>
        /// The tax variance.
        /// </value>
        decimal TaxVariance { get; set; }

        /// <summary>
        /// Gets or sets the location variance.
        /// </summary>
        /// <value>
        /// The location variance.
        /// </value>
        decimal LocationVariance { get; set; }

        /// <summary>
        /// Gets or sets the market fluctuation.
        /// </summary>
        /// <value>
        /// The market fluctuation.
        /// </value>
        decimal MarketFluctuation { get; set; }
    }
}