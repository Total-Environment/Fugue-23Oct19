using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class LandedRateDto
    {
        public LandedRateDto()
        {
        }

        /// <summary>
        /// Gets or sets the control base rate.
        /// </summary>
        /// <value>
        /// The control base rate.
        /// </value>
        public MoneyDto ControlBaseRate { get; set; }

        /// <summary>
        /// Gets or sets the landed rate.
        /// </summary>
        /// <value>
        /// The landed rate.
        /// </value>
        public MoneyDto LandedRate { get; set; }

        /// <summary>
        /// Gets or sets the procurement rate threshold.
        /// </summary>
        /// <value>
        /// The procurement rate threshold.
        /// </value>
        public object ProcurementRateThreshold { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure.
        /// </summary>
        /// <value>
        /// The unit of measure.
        /// </value>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(LandedRateDto other)
        {
            return Equals(ControlBaseRate, other.ControlBaseRate);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LandedRateDto)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ControlBaseRate.GetHashCode();
                hashCode = (hashCode * 397);
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(ControlBaseRate)}: {ControlBaseRate}";
        }
    }
}