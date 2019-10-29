using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The Sum Coefficient
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.ICoefficient" />
    public class SumCoefficient : ICoefficient
    {
        /// <inheritdoc/>
        public Money Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SumCoefficient"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">
        /// Name Cannot be null.
        /// or
        /// Value cannot be null.
        /// </exception>
        public SumCoefficient(string name, Money value)
        {
            if (name == null)
                throw new ArgumentException("Name Cannot be null.");
            if (value == null)
                throw new ArgumentException("Value cannot be null.");
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public string Name { get; }
      
        /// <inheritdoc/>
        public Money Apply(Money money)
        {
            if (money == null)
                throw new ArgumentException("Money Cannot be null.");
            return Value;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(SumCoefficient other)
        {
            return Equals(Value, other.Value) && string.Equals(Name, other.Name);
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
            return Equals((SumCoefficient) obj);
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
                return ((Value != null ? Value.GetHashCode() : 0)*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}