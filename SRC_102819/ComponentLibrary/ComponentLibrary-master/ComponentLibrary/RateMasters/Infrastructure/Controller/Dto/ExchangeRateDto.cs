using System;
using System.Globalization;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The DTO for Exchange Rates
    /// </summary>
    public class ExchangeRateDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateDto"/> class.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <exception cref="System.ArgumentException">
        /// ExchangeRate cannot be null.
        /// or
        /// Invalid Exchange Rate Implementation.
        /// </exception>
        public ExchangeRateDto(IExchangeRate exchangeRate)
        {
            if (exchangeRate == null)
                throw new ArgumentException("ExchangeRate cannot be null.");
            var rate = exchangeRate as ExchangeRate;
            if (rate != null)
            {
                var domain = rate;
                FromCurrency = domain.FromCurrency;
                ToCurrency = domain.ToCurrency;
                BaseConversionRate = domain.BaseConversionRate.ToString(CultureInfo.InvariantCulture);
                CurrencyFluctuationCoefficient = domain.FluctuationCoefficient.ToString(CultureInfo.InvariantCulture);
                AppliedFrom = domain.AppliedFrom;
                DefinedConversionRate = domain.Rate().ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException("Invalid Exchange Rate Implementation.");
            }
        }

        /// <summary>
        /// Gets or sets from currency.
        /// </summary>
        /// <value>
        /// From currency.
        /// </value>
        public string FromCurrency { get; set; }

        /// <summary>
        /// Gets or sets to currency.
        /// </summary>
        /// <value>
        /// To currency.
        /// </value>
        public string ToCurrency { get; set; }

        /// <summary>
        /// Gets or sets the base conversion rate.
        /// </summary>
        /// <value>
        /// The base conversion rate.
        /// </value>
        public string BaseConversionRate { get; set; }

        /// <summary>
        /// Gets or sets the defined conversion rate.
        /// </summary>
        /// <value>
        /// The defined conversion rate.
        /// </value>
        public string DefinedConversionRate { get; set; }

        /// <summary>
        /// Gets or sets the currency fluctuation coefficient.
        /// </summary>
        /// <value>
        /// The currency fluctuation coefficient.
        /// </value>
        public string CurrencyFluctuationCoefficient { get; set; }

        /// <summary>
        /// Gets or sets the applied from.
        /// </summary>
        /// <value>
        /// The applied from.
        /// </value>
        public virtual DateTime AppliedFrom { get; set; }

        /// <summary>
        /// Domains this instance.
        /// </summary>
        /// <returns></returns>
        public virtual IExchangeRate Domain()
        {
            decimal result;
            if (!decimal.TryParse(CurrencyFluctuationCoefficient, out result))
                result = 0;
            return new ExchangeRate(FromCurrency, ToCurrency, decimal.Parse(BaseConversionRate), result, AppliedFrom);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateDto"/> class.
        /// </summary>
        public ExchangeRateDto()
        {
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(ExchangeRateDto other)
        {
            return BaseConversionRate == other.BaseConversionRate &&
                   CurrencyFluctuationCoefficient == other.CurrencyFluctuationCoefficient &&
                   FromCurrency == other.FromCurrency;
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
            return Equals((ExchangeRateDto) obj);
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
                hashCode = (hashCode*397) ^ (CurrencyFluctuationCoefficient?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (FromCurrency?.GetHashCode() ?? 0);
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
            return $"{nameof(FromCurrency)}: {FromCurrency}, {nameof(ToCurrency)}: {ToCurrency}, {nameof(BaseConversionRate)}: {BaseConversionRate}, {nameof(CurrencyFluctuationCoefficient)}: {CurrencyFluctuationCoefficient}, {nameof(AppliedFrom)}: {AppliedFrom}";
        }
    }
}