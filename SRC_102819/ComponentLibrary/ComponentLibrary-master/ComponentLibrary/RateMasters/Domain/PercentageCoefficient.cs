using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The Percentage Coefficient
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain.ICoefficient" />
    public class PercentageCoefficient : ICoefficient
    {
        /// <inheritdoc/>
        public decimal Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PercentageCoefficient"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">
        /// Name cannot be null
        /// or
        /// Value cannot be zero or negative
        /// </exception>
        public PercentageCoefficient(string name, decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be zero or negative");
            if (name == null)
            {
                throw new ArgumentException("Name cannot be null");
            }
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Money Apply(Money money)
        {
            if (money == null)
                throw new ArgumentException("Money cannot be negative");
            var finalmoney =  money.Percentage(Value);
            return finalmoney;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(PercentageCoefficient other)
        {
            return Value == other.Value && string.Equals(Name, other.Name);
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
            return obj.GetType() == GetType() && Equals((PercentageCoefficient) obj);
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
                return (Value.GetHashCode()*397) ^ (Name?.GetHashCode() ?? 0);
            }
        }
    }
}