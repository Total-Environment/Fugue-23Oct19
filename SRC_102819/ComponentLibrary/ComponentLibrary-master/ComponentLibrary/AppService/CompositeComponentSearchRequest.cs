using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// Represents the structure for a Composite Component search request
	/// </summary>
	public class CompositeComponentSearchRequest : ISearchRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentSearchRequest"/> class.
		/// </summary>
		public CompositeComponentSearchRequest()
		{
			FilterDatas = new List<FilterData>();
			PageNumber = -1;
			BatchSize = -1;
			SortColumn = "sfg_code";
			SortOrder = SortOrder.Ascending;
		}

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