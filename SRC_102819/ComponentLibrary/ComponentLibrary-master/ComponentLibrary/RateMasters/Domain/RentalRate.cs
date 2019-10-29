using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The rental rate
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.IRentalRate" />
    public class RentalRate : IRentalRate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRate"/> class.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="unitOfMesure">The unit of mesure.</param>
        /// <param name="money">The money.</param>
        /// <param name="appliedFrom">The applied from.</param>
        public RentalRate(string materialId, string unitOfMesure, Money money, DateTime appliedFrom)
        {
             MaterialId= materialId;
            UnitOfMeasure = unitOfMesure;
            Value = money;
            AppliedFrom = appliedFrom;
        }


        /// <inheritdoc/>
        public string MaterialId { get; }
        /// <inheritdoc/>
        public string UnitOfMeasure { get; }
        /// <inheritdoc/>
        public Money Value { get; }
        /// <inheritdoc/>
        public DateTime AppliedFrom { get; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(RentalRate other)
        {
            return string.Equals(MaterialId, other.MaterialId) && string.Equals(UnitOfMeasure, other.UnitOfMeasure) && Equals(Value, other.Value) && AppliedFrom.Equals(other.AppliedFrom);
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
            return Equals((RentalRate) obj);
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
                hashCode = (hashCode * 397) ^ (UnitOfMeasure != null ? UnitOfMeasure.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AppliedFrom.GetHashCode();
                return hashCode;
            }
        }
    }
}