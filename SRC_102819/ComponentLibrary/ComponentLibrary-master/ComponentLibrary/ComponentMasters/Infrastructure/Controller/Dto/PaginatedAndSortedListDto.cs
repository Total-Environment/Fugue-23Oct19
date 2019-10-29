using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedAndSortedListDto<T>
    {
        /// <summary>
        /// Maps the list to an entity and returns back the DTO
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns></returns>
        public PaginatedAndSortedListDto<T> WithList<TEntity>(PaginatedAndSortedList<TEntity> list,
            Func<TEntity, T> mapper)
        {
            PageNumber = list.PageNumber;
            TotalRecords = list.TotalRecords;
            BatchSize = list.BatchSize;
            SortColumn = list.SortColumn;
            SortOrder = list.SortOrder;
            Items = list.Items.Select(mapper).ToList();
            return this;
        }

        public async Task<PaginatedAndSortedListDto<T>> WithListAsync<TEntity>(PaginatedAndSortedList<TEntity> list,
            Func<TEntity, Task<T>> mapper)
        {
            PageNumber = list.PageNumber;
            TotalRecords = list.TotalRecords;
            BatchSize = list.BatchSize;
            SortColumn = list.SortColumn;
            SortOrder = list.SortOrder;
            Items = (await Task.WhenAll(list.Items.Select(mapper))).ToList();
            return this;
        }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }
        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>
        /// The total records.
        /// </value>
        public long TotalRecords { get; set; }
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>
        /// The size of the batch.
        /// </value>
        public int BatchSize { get; set; }
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
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<T> Items { get; set; }
    }
}