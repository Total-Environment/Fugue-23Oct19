using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The interface for service rates
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IComponentRate" />
    public interface IServiceRate : IComponentRate
    {
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