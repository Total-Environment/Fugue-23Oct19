using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The Money DTO
    /// </summary>
    public class MoneyDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyDto"/> class.
        /// </summary>
        /// <param name="money">The money.</param>
        /// <exception cref="ArgumentException">Money cannot be null.</exception>
        public MoneyDto(Money money)
        {
            if (money == null)
                throw new ArgumentException("Money cannot be null.");
            Value = money.Value;
            Currency = money.Currency;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }

        /// <summary>
        /// Domains the specified bank.
        /// </summary>
        /// <param name="bank">The bank.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Bank cannot be null.</exception>
        public Money Domain(IBank bank)
        {
            if (bank == null)
                throw new ArgumentException("Bank cannot be null.");
            return new Money(Value, Currency, bank);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyDto"/> class.
        /// </summary>
        public MoneyDto()
        {
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(MoneyDto other)
        {
            return Value == other.Value && Equals(Currency, other.Currency);
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
            return Equals((MoneyDto)obj);
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
                var hashCode = Value.GetHashCode();
                hashCode = (hashCode * 397) ^ (Currency?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}, {nameof(Currency)}: {Currency}";
        }
    }
}