using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// The interface for a search request with filters
    /// </summary>
    public interface ISearchRequest
    {
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>
        /// The size of the batch.
        /// </value>
        int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the filter datas.
        /// </summary>
        /// <value>
        /// The filter datas.
        /// </value>
        List<FilterData> FilterDatas { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        /// <value>
        /// The sort column.
        /// </value>
        string SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        SortOrder SortOrder { get; set; }


        /// <summary>
        /// Ignores search query.
        /// </summary>
        bool IgnoreSearchQuery { get; set; }
    }
}