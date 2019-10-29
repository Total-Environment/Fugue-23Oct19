using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain
{
    /// <summary>
    /// The repository for rental rates
    /// </summary>
    public interface IRentalRateRepository
    {
        /// <summary>
        /// Adds the specified rental rate.
        /// </summary>
        /// <param name="rentalRate">The rental rate.</param>
        Task Add(IRentalRate rentalRate);

        /// <summary>
        /// Gets the specified material identifier.
        /// </summary>
        /// <param name="materialId">The material identifier.</param>
        /// <param name="unitOfMeasure">The unit of measure.</param>
        /// <param name="appliedFrom">The applied from.</param>
        /// <returns></returns>
        Task<IRentalRate> Get(string materialId, string unitOfMeasure, DateTime appliedFrom);

        /// <summary>
        ///
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="unitOfMeasure"></param>
        /// <param name="appliedFrom"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        Task<PaginatedAndSortedList<IRentalRate>> GetAll(string materialId, string rentalUnit = null, DateTime? appliedFrom = null, int pageNumber = 1, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending);

        /// <summary>
        /// Gets the latest rental rates.
        /// </summary>
        /// <param name="isAny">The is any.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        Task<IEnumerable<IRentalRate>> GetLatest(string isAny, DateTime dateTime);
    }
}