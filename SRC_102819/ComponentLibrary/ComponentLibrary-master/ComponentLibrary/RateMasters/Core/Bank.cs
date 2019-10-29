using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Core
{
    /// <summary>
    /// The bank
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RateMasters.Core.IBank" />
    public class Bank : IBank
    {
        private readonly IExchangeRateRepository _repository;
        private const decimal UnitRate = 1m;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bank"/> class.
        /// </summary>
        /// <param name="repository"></param>
        public Bank(IExchangeRateRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task<Money> ConvertTo(Money money, string toCurrency)
        {
            if (money == null)
                throw new ArgumentException("Money cannot be null.");
            if (toCurrency == null)
                throw new ArgumentException("ToCurrency cannot be null.");
            var rate = await Rate(money.Currency, toCurrency,DateTime.MinValue);
            return new Money(money.Value * rate, toCurrency, this);
        }

        public async Task<Money> ConvertTo(Money money, string toCurrency, DateTime appliedFrom)
        {
            if (money == null)
                throw new ArgumentException("Money cannot be null.");
            if (toCurrency == null)
                throw new ArgumentException("ToCurrency cannot be null.");
            var rate = await Rate(money.Currency, toCurrency,appliedFrom);
            return new Money(money.Value * rate, toCurrency, this);
        }

        private async Task<decimal> Rate(string fromCurrency, string toCurrency, DateTime appliedFrom)
        {
            List<IExchangeRate> exchangeRates;
            if (fromCurrency.Equals(toCurrency))
                return UnitRate;
            if (!appliedFrom.Equals(DateTime.MinValue))
            {
                exchangeRates = (await _repository.GetAll(appliedFrom)).ToList();
                var matchingExchangeRates = exchangeRates.Where(er =>
                    fromCurrency.Equals(er.FromCurrency, StringComparison.InvariantCultureIgnoreCase) &&
                    toCurrency.Equals(er.ToCurrency, StringComparison.InvariantCultureIgnoreCase));
                    
                var exchangeRateAsOfAppliedFrom =
                    matchingExchangeRates.OrderByDescending(er => er.AppliedFrom).FirstOrDefault();

                if (exchangeRateAsOfAppliedFrom != null)
                    return exchangeRateAsOfAppliedFrom.Rate();

                matchingExchangeRates = exchangeRates.Where(
                        er =>
                            fromCurrency.Equals(er.ToCurrency, StringComparison.InvariantCultureIgnoreCase) &&
                            toCurrency.Equals(er.FromCurrency, StringComparison.InvariantCultureIgnoreCase));

                exchangeRateAsOfAppliedFrom =
                    matchingExchangeRates.OrderByDescending(er => er.AppliedFrom).FirstOrDefault();

                if (exchangeRateAsOfAppliedFrom == null)
                    throw new ArgumentException($"No exchange rate is found for currency type {fromCurrency} to {toCurrency} as of date {appliedFrom.InIst().ToShortDateString()}");

                return 1 / exchangeRateAsOfAppliedFrom.Rate();
            }
            exchangeRates = (await _repository.GetAll()).ToList();
            var exchangeRate =
                exchangeRates.FirstOrDefault(
                    er =>
                        fromCurrency.Equals(er.FromCurrency, StringComparison.InvariantCultureIgnoreCase) &&
                        toCurrency.Equals(er.ToCurrency, StringComparison.InvariantCultureIgnoreCase));

            if (exchangeRate != null)
                return exchangeRate.Rate();

            exchangeRate = exchangeRates.FirstOrDefault(
                er =>
                    fromCurrency.Equals(er.ToCurrency, StringComparison.InvariantCultureIgnoreCase) &&
                    toCurrency.Equals(er.FromCurrency, StringComparison.InvariantCultureIgnoreCase));

            if (exchangeRate == null)
                throw new ArgumentException($"No exchange rate is found for currency type {fromCurrency} to {toCurrency} ");

            return 1 / exchangeRate.Rate();
        }
    }
}