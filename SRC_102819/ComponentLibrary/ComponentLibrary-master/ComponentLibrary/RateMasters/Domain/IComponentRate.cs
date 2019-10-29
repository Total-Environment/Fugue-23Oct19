using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The interface for Component Rate
    /// </summary>
    public interface IComponentRate
    {
        /// <summary>
        /// Gets the applied on.
        /// </summary>
        /// <value>
        /// The applied on.
        /// </value>
        DateTime AppliedOn { get; }
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        string Location { get; }
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; }
        /// <summary>
        /// Gets the type of purchase.
        /// </summary>
        /// <value>
        /// The type of purchase.
        /// </value>
        string TypeOfPurchase { get; }

        /// <summary>
        /// Gets or sets the control base rate.
        /// </summary>
        /// <value>
        /// The control base rate.
        /// </value>
        Money ControlBaseRate { get; set; }

        /// <summary>
        /// Gets or sets the landed rate.
        /// </summary>
        /// <value>
        /// The landed rate.
        /// </value>
        Task<Money> LandedRate();
    }
}