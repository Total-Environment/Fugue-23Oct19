using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The interface for rental rates
    /// </summary>
    public interface IRentalRate
    {
        /// <summary>
        /// Gets the material identifier.
        /// </summary>
        /// <value>
        /// The material identifier.
        /// </value>
        string MaterialId { get; }
        /// <summary>
        /// Gets the unit of measure.
        /// </summary>
        /// <value>
        /// The unit of measure.
        /// </value>
        string UnitOfMeasure { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        Money Value { get; }

        /// <summary>
        /// Gets the applied from.
        /// </summary>
        /// <value>
        /// The applied from.
        /// </value>
        DateTime AppliedFrom { get; }
    }
}
