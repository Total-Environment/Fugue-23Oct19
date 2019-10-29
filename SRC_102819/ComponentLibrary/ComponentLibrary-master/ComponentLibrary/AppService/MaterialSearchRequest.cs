using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents the structure for a material search request
    /// </summary>
    public class MaterialSearchRequest : ISearchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialSearchRequest"/> class.
        /// </summary>
        public MaterialSearchRequest()
        {
            FilterDatas = new List<FilterData>();
            PageNumber = -1;
            BatchSize = -1;
            SortColumn = "material_name";
            SortOrder = SortOrder.Ascending;
        }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the search query.
        /// </summary>
        /// <value>The search query.</value>
        public string SearchQuery { get; set; }

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

        /// <summary>
        /// Ignores search keywords
        /// </summary>
        public bool IgnoreSearchQuery { get; set; }
    }
}