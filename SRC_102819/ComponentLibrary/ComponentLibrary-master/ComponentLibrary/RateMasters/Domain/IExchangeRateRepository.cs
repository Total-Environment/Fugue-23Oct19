using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The repository for exchange rates
    /// </summary>
    public interface IExchangeRateRepository
    {
        /// <summary>
        /// Creates the exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns></returns>
        Task<IExchangeRate> CreateExchangeRate(IExchangeRate exchangeRate);

        /// <summary>
        /// Gets the exchange rate.
        /// </summary>
        /// <param name="fromCurrency">From currency.</param>
        /// <param name="toCurrency">To currency.</param>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        Task<IExchangeRate> GetExchangeRate(string fromCurrency, string toCurrency, DateTime @on);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IExchangeRate>> GetAll();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        Task<IEnumerable<IExchangeRate>> GetAll(DateTime @on);

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <returns></returns>
        Task<PaginatedAndSortedList<IExchangeRate>> GetHistory(string currencyType=null, DateTime? appliedFrom = null, int pageNumber = 1, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending);
    }
}