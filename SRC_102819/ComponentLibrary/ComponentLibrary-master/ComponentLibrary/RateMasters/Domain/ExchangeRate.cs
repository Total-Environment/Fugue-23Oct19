using System;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <inheritdoc/>
    public class ExchangeRate : IExchangeRate
    {
        /// <summary>
        /// Gets the base conversion rate.
        /// </summary>
        /// <value>
        /// The base conversion rate.
        /// </value>
        public decimal BaseConversionRate { get; }
        /// <summary>
        /// Gets the fluctuation coefficient.
        /// </summary>
        /// <value>
        /// The fluctuation coefficient.
        /// </value>
        public decimal FluctuationCoefficient { get; }

        /// <inheritdoc/>
        public DateTime AppliedFrom { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRate"/> class.
        /// </summary>
        /// <param name="fromCurrency">From currency.</param>
        /// <param name="toCurrency">To currency.</param>
        /// <param name="baseConversionRate">The base conversion rate.</param>
        /// <param name="fluctuationCoefficient">The fluctuation coefficient.</param>
        /// <param name="appliedFrom">The applied from.</param>
        /// <exception cref="ArgumentException">
        /// From Currency cannot be null.
        /// or
        /// To Currency cannot be null.
        /// or
        /// Base Conversion Rate cannot be zero or negative
        /// or
        /// Fluctuation Coefficient cannot be negative.
        /// </exception>
        public ExchangeRate(string fromCurrency, string toCurrency, decimal baseConversionRate, decimal fluctuationCoefficient, DateTime appliedFrom)
        {
            if(string.IsNullOrWhiteSpace(fromCurrency))
                throw new ArgumentException("From Currency cannot be null.");
            if(string.IsNullOrWhiteSpace(toCurrency))
                throw new ArgumentException("To Currency cannot be null.");
            if(baseConversionRate <=0)
                throw new ArgumentException("Base Conversion Rate cannot be zero or negative");
            if(fluctuationCoefficient < 0)
                throw new ArgumentException("Fluctuation Coefficient cannot be negative.");
            FromCurrency = fromCurrency;
            ToCurrency = toCurrency;
            BaseConversionRate = baseConversionRate;
            FluctuationCoefficient = fluctuationCoefficient;
            AppliedFrom = appliedFrom;
        }

        /// <inheritdoc/>
        public string FromCurrency { get; }
        /// <inheritdoc/>
        public string ToCurrency { get; }
        /// <inheritdoc/>
        public decimal Rate()
        {
            return BaseConversionRate + BaseConversionRate * FluctuationCoefficient/100;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(ExchangeRate other)
        {
            return BaseConversionRate == other.BaseConversionRate && FluctuationCoefficient == other.FluctuationCoefficient && string.Equals(FromCurrency, other.FromCurrency) && string.Equals(ToCurrency, other.ToCurrency);
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
            return Equals((ExchangeRate) obj);
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
                var hashCode = BaseConversionRate.GetHashCode();
                hashCode = (hashCode*397) ^ FluctuationCoefficient.GetHashCode();
                hashCode = (hashCode*397) ^ (FromCurrency != null ? FromCurrency.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ToCurrency != null ? ToCurrency.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}