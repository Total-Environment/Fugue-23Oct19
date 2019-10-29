using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The interface for Component Rate Master
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComponentRateService<T> where T : IComponentRate
    {
        /// <summary>
        /// Gets the rate.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <returns></returns>
        Task<T> GetRate(string id, string location, DateTime @on, string typeOfPurchase);

        /// <summary>
        ///
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="rateSearchRequest"></param>
        /// <returns></returns>
        Task<PaginatedAndSortedList<T>> GetRateHistory(string materialId, IRateSearchRequest rateSearchRequest);

        /// <summary>
        /// Gets the rates.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetRates(string id, DateTime on);

        /// <summary>
        /// Creates the rate.
        /// </summary>
        /// <param name="rate">The rate.</param>
        /// <returns></returns>
        Task<T> CreateRate(T rate);

        /// <summary>
        /// Gets the average landed rate.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        Task<Money> GetAverageLandedRate(string code, string location, DateTime @on, string currency);

        /// <summary>
        /// Gets the landed rate.
        /// </summary>
        /// <param name="materialid">The materialid.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <returns></returns>
        Task<Money> GetLandedRate(string materialid, string location, DateTime @on, string currency, string typeOfPurchase);

        /// <summary>
        /// Gets the control base rate.
        /// </summary>
        /// <param name="materialid">The materialid.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <returns></returns>
        Task<Money> GetControlBaseRate(string materialid, string location, DateTime @on, string currency, string typeOfPurchase);

        /// <summary>
        /// Gets the average control base rate.
        /// </summary>
        /// <param name="materialid">The materialid.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        Task<Money> GetAverageControlBaseRate(string materialid, string location, DateTime @on, string currency);
    }
}