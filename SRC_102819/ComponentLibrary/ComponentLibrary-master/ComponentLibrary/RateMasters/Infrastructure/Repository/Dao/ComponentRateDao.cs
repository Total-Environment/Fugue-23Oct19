using System;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for Component Rate
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class ComponentRateDao : Entity
    {
        /// <summary>
        /// Gets or sets the type of purchase.
        /// </summary>
        /// <value>
        /// The type of purchase.
        /// </value>
        public virtual string TypeOfPurchase { get; set; }


        /// <summary>
        /// Gets or sets the control base rate.
        /// </summary>
        /// <value>
        /// The control base rate.
        /// </value>
        public virtual MoneyDao ControlBaseRate { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public virtual string Location { get; set; }

        /// <summary>
        /// Gets or sets the applied on.
        /// </summary>
        /// <value>
        /// The applied on.
        /// </value>
        public virtual DateTime AppliedOn { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(ComponentRateDao other)
        {
            return string.Equals(TypeOfPurchase, other.TypeOfPurchase)
                && string.Equals(Location, other.Location) && AppliedOn.Equals(other.AppliedOn);
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
            return Equals((ComponentRateDao)obj);
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
                var hashCode = (TypeOfPurchase != null ? TypeOfPurchase.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AppliedOn.GetHashCode();
                return hashCode;
            }
        }
    }
}