using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for Money
    /// </summary>
    public class MoneyDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyDao"/> class.
        /// </summary>
        /// <param name="controlBaseRate">The control base rate.</param>
        public MoneyDao(Money controlBaseRate)
        {
            Value = controlBaseRate.Value;
            Currency = controlBaseRate.Currency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyDao"/> class.
        /// </summary>
        public MoneyDao()
        {
            
        }

        /// <summary>
        /// Domains the specified bank.
        /// </summary>
        /// <param name="bank">The bank.</param>
        /// <returns></returns>
        public Money Domain(IBank bank)
        {
            return new Money(Value, Currency, bank);

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
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(MoneyDao other)
        {
            return Value == other.Value && string.Equals(Currency, other.Currency);
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
            return Equals((MoneyDao) obj);
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
                return (Value.GetHashCode() * 397) ^ (Currency != null ? Currency.GetHashCode() : 0);
            }
        }
    }
}