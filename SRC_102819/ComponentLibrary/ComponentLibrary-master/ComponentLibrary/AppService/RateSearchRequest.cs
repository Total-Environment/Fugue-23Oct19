using System;
using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents a rate search request
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ISearchRequest" />
    public class RateSearchRequest : ISearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RateSearchRequest"/> class.
        /// </summary>
        public RateSearchRequest()
        {
            FilterDatas = new List<FilterData>();
            PageNumber = 1;
            BatchSize = -1;
            SortColumn = "AppliedOn";
            SortOrder = SortOrder.Descending;
        }

        /// <inheritdoc/>
        public int BatchSize { get; set; }

        /// <inheritdoc/>
        public List<FilterData> FilterDatas { get; set; }

        /// <inheritdoc/>
        public int PageNumber { get; set; }

        /// <inheritdoc/>
        public string SortColumn { get; set; }

        /// <inheritdoc/>
        public SortOrder SortOrder { get; set; }

        /// <inheritdoc />
        public bool IgnoreSearchQuery { get; set; }

        /// <summary>
        /// Gets or sets the applied on date
        /// </summary>
        /// <value>
        /// The applied on.
        /// </value>
        public DateTime AppliedOn { get; set; }
    }
}