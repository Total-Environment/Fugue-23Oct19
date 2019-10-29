using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The repository for Component Rates
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComponentRateRepository<T> where T : IComponentRate
    {
        /// <summary>
        /// Gets the rate.
        /// </summary>
        /// <param name="serviceCode">The id.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <param name="typeOfPurchase">The type of purchase.</param>
        /// <returns></returns>
        Task<T> GetRate(string serviceCode, string location, DateTime @on, string typeOfPurchase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <param name="location"></param>
        /// <param name="typeOfPurchase"></param>
        /// <param name="appliedOn"></param>
        /// <returns></returns>
        Task<PaginatedAndSortedList<T>> GetRateHistory(string id, int pageNumber = 1, string sortColumn = "AppliedOn", SortOrder sortOrder = SortOrder.Descending,
            string location = "", string typeOfPurchase = "", DateTime? appliedOn = null);

        /// <summary>
        /// Gets the rates.
        /// </summary>
        /// <param name="materialCode">The identifier.</param>
        /// <param name="on"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetRates(string materialCode, DateTime @on);

        /// <summary>
        /// Adds the rate.
        /// </summary>
        /// <param name="rate">The material rate.</param>
        /// <returns></returns>
        Task<T> AddRate(T materialRate);

        /// <summary>
        /// Gets the rates.
        /// </summary>
        /// <param name="materialCode">The identifier.</param>
        /// <param name="location">The location.</param>
        /// <param name="on">The on.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetRates(string materialCode, string location, DateTime on);
    }
}