using System;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// The DAO for Exchange Rate
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.Entity" />
    public class ExchangeRateDao : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateDao"/> class.
        /// </summary>
        /// <param name="rate">The rate.</param>
        /// <exception cref="ArgumentException">
        /// Exchange rate cannot be null.
        /// or
        /// Invalid Implementation of Exchange rate.
        /// </exception>
        public ExchangeRateDao(IExchangeRate rate)
        {
            if (rate == null)
                throw new ArgumentException("Exchange rate cannot be null.");
            if (rate is ExchangeRate)
            {
                var exchangeRate = (ExchangeRate)rate;
                FromCurrency = exchangeRate.FromCurrency;
                ToCurrency = exchangeRate.ToCurrency;
                AppliedFrom = exchangeRate.AppliedFrom;
                BaseConversionRate = exchangeRate.BaseConversionRate;
                FluctuationCoefficient = exchangeRate.FluctuationCoefficient;
                DefinedConversionRate = exchangeRate.Rate();
            }
            else
            {
                throw new ArgumentException("Invalid Implementation of Exchange rate.");
            }
        }

        /// <summary>
        /// Gets or sets the applied from.
        /// </summary>
        /// <value>
        /// The applied from.
        /// </value>
        public DateTime AppliedFrom { get; set; }
        /// <summary>
        /// Gets or sets from currency.
        /// </summary>
        /// <value>
        /// From currency.
        /// </value>
        public virtual string FromCurrency { get; set; }
        /// <summary>
        /// Gets or sets to currency.
        /// </summary>
        /// <value>
        /// To currency.
        /// </value>
        public virtual string ToCurrency { get; set; }
        /// <summary>
        /// Gets or sets the base conversion rate.
        /// </summary>
        /// <value>
        /// The base conversion rate.
        /// </value>
        public virtual decimal BaseConversionRate { get; set; }
        /// <summary>
        /// Gets or sets the fluctuation coefficient.
        /// </summary>
        /// <value>
        /// The fluctuation coefficient.
        /// </value>
        public virtual decimal FluctuationCoefficient { get; set; }

        // This is only saved in the database. Not read from it. Only for sorting.
        /// <summary>
        /// Gets or sets the defined conversion rate.
        /// </summary>
        /// <value>
        /// The defined conversion rate.
        /// </value>
        public decimal DefinedConversionRate { get; set; }

        /// <summary>
        /// Domains this instance.
        /// </summary>
        /// <returns></returns>
        public IExchangeRate Domain()
        {
            return new ExchangeRate(FromCurrency, ToCurrency, BaseConversionRate, FluctuationCoefficient, AppliedFrom);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateDao"/> class.
        /// </summary>
        [BsonConstructor]
        public ExchangeRateDao()
        {
        }
    }
}