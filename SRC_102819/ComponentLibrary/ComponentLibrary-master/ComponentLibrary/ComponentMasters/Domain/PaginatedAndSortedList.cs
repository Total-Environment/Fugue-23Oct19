using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain
{
    /// <summary>
    /// Model for paginated and sorted list of a given set of items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedAndSortedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedAndSortedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        public PaginatedAndSortedList(IEnumerable<T> items, int pageNumber, long totalRecords, int batchSize, string sortColumn, SortOrder sortOrder)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalRecords = totalRecords;
            BatchSize = batchSize;
            SortColumn = sortColumn;
            SortOrder = sortOrder;
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<T> Items { get; set; }
        
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }
        
        /// <summary>
        /// Gets the total records.
        /// </summary>
        /// <value>
        /// The total records.
        /// </value>
        public long TotalRecords { get; }
        
        /// <summary>
        /// Gets the size of the batch.
        /// </summary>
        /// <value>
        /// The size of the batch.
        /// </value>
        public int BatchSize { get; }
        
        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        /// <value>
        /// The sort column.
        /// </value>
        public string SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// Selects the specified mapper function.
        /// </summary>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        /// <param name="mapperFunc">The mapper function.</param>
        /// <returns></returns>
        public PaginatedAndSortedList<TProjection> Select<TProjection>(Func<T, TProjection> mapperFunc)
        {
            return new PaginatedAndSortedList<TProjection>(Items.Select(mapperFunc), PageNumber, TotalRecords, BatchSize, SortColumn, SortOrder);
        }
    }
}