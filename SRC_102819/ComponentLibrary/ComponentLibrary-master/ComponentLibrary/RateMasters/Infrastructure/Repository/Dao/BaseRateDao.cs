using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    public abstract class BaseRateDao
    {
        /// <summary>
        /// Gets or sets the applied from.
        /// </summary>
        /// <value>
        /// The applied from.
        /// </value>
        public DateTime AppliedFrom { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the type of purchase.
        /// </summary>
        /// <value>
        /// The type of purchase.
        /// </value>
        public string TypeOfPurchase { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the control base rate.
        /// </summary>
        /// <value>
        /// The control base rate.
        /// </value>
        public Money ControlBaseRate { get; set; }


    }
}