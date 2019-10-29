using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents a compoent price
    /// </summary>
    public class ComponentPrice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentPrice"/> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="unitOfMeasure">The unit of measure.</param>
        public ComponentPrice(Money price, string unitOfMeasure)
        {
            Price = price;
            UnitOfMeasure = unitOfMeasure;
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public Money Price { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure.
        /// </summary>
        /// <value>
        /// The unit of measure.
        /// </value>
        public string UnitOfMeasure { get; set; }

    }
}