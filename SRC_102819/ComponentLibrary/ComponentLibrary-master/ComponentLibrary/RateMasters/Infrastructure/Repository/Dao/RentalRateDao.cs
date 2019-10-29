using System;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for Rental Rates
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class RentalRateDao : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRateDao"/> class.
        /// </summary>
        /// <param name="rentalRate">The rental rate.</param>
        public RentalRateDao(IRentalRate rentalRate)
        {
            UnitOfMeasure = rentalRate.UnitOfMeasure;
            Value = new MoneyDao(rentalRate.Value);
            AppliedFrom = rentalRate.AppliedFrom;
            MaterialId = rentalRate.MaterialId;
        }

        /// <summary>
        /// Gets or sets the applied from.
        /// </summary>
        /// <value>
        /// The applied from.
        /// </value>
        public DateTime AppliedFrom { get; set; }
        /// <summary>
        /// Gets the material identifier.
        /// </summary>
        /// <value>
        /// The material identifier.
        /// </value>
        public string MaterialId { get; private set; }
        /// <summary>
        /// Gets the unit of measure.
        /// </summary>
        /// <value>
        /// The unit of measure.
        /// </value>
        public string UnitOfMeasure { get; private set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public MoneyDao Value { get; set; }

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
            return Equals((RentalRateDao)obj);
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <returns></returns>
        public IRentalRate GetDomain()
        {
            return new RentalRate(MaterialId, UnitOfMeasure, new Money(Value.Value,Value.Currency), AppliedFrom);
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
                var hashCode = (MaterialId != null ? MaterialId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AppliedFrom.GetHashCode();
                hashCode = (hashCode * 397) ^ (UnitOfMeasure != null ? UnitOfMeasure.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(RentalRateDao other)
        {
            return string.Equals(MaterialId, other.MaterialId) && AppliedFrom.Equals(other.AppliedFrom) && string.Equals(UnitOfMeasure, other.UnitOfMeasure) && Equals(Value, other.Value);
        }
    }
}