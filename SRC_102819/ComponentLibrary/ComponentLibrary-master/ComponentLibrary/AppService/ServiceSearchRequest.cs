using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents the structure for a service search request
    /// </summary>
    public class ServiceSearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSearchRequest"/> class.
        /// </summary>
        public ServiceSearchRequest()
        {
            FilterDatas = new List<FilterData>();
            PageNumber = -1;
            BatchSize = -1;
            SortColumn = "service_code";
            SortOrder = SortOrder.Ascending;
        }

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the filter datas.
        /// </summary>
        /// <value>The filter datas.</value>
        public List<FilterData> FilterDatas { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>The page number.</value>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the search query.
        /// </summary>
        /// <value>The search query.</value>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        /// <value>The sort column.</value>
        public string SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// Ignores search keywords.
        /// </summary>
        public bool IgnoreSearchQuery { get; set; }
    }
}