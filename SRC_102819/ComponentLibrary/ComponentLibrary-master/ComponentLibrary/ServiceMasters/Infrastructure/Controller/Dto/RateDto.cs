using System;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Dto for updating the service rates.
    /// </summary>
    public class RateDto
    {
        /// <summary>
        /// Gets or sets the last purchase rate.
        /// </summary>
        /// <value>The last purchase rate.</value>
        public MoneyDto LastPurchaseRate { get; set; }

        /// <summary>
        /// Gets or sets the weighted average purchase rate.
        /// </summary>
        /// <value>The average purchase rate.</value>
        public MoneyDto WeightedAveragePurchaseRate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateDto"/> class.
        /// </summary>
        public RateDto()
        { }

        /// <summary>
        /// Determines whether request is valid or not.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Service code cannot be null. or LastPurchaseRate cannot be null. or LastPurchaseRate
        /// cannot be null.
        /// </exception>
        public void IsValidState()
        {
            if (WeightedAveragePurchaseRate == null)
                throw new ArgumentException("WeightedAveragePurchaseRate cannot be null.");
            if (LastPurchaseRate == null)
                throw new ArgumentException("LastPurchaseRate cannot be null.");
        }
    }
}