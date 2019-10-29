using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// Repository of semi finished good.
	/// </summary>
	public interface ICompositeComponentRepository
	{
		/// <summary>
		/// Finds the SFG with specified identifier.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="sfgId">The SFG identifier.</param>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <returns>Matching SFG.</returns>
		Task<CompositeComponent> Find(string type, string sfgId, ICompositeComponentDefinition sfgDefintion);

		/// <summary>
		/// Saves the specified SFG.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="sfg">The SFG.</param>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <returns>Saved SFG</returns>
		Task<CompositeComponent> Create(string type, CompositeComponent sfg, ICompositeComponentDefinition sfgDefintion);

		/// <summary>
		/// Updates the specified SFG.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="sfg">The SFG.</param>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <returns></returns>
		Task<CompositeComponent> Update(string type, CompositeComponent sfg, ICompositeComponentDefinition sfgDefintion);

		/// <summary>
		/// Gets the count by the specified filter criteria.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filterCriteria">The filter criteria.</param>
		/// <returns></returns>
		Task<long> Count(string type, Dictionary<string, Tuple<string, object>> filterCriteria);

		/// <summary>
		/// Searches by the specified filter criteria.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filterCriteria">The filter criteria.</param>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="batchSize">Size of the batch.</param>
		/// <returns></returns>
		Task<List<CompositeComponent>> Find(string type, Dictionary<string, Tuple<string, object>> filterCriteria, string sortColumn, SortOrder sortOrder, int pageNumber, int batchSize);

	    /// <summary>
	    /// Fetch composite components count by group and column
	    /// </summary>
	    /// <param name="type"></param>
	    /// <param name="group"></param>
	    /// <param name="columnName"></param>
	    /// <param name="keywords"></param>
	    /// <returns></returns>
	    Task<long> GetTotalCountByGroupAndColumnName(string type, string group, string columnName, List<string> keywords);

	    /// <summary>
	    /// Fetch composite components list by group and column
	    /// </summary>
	    /// <param name="group"></param>
	    /// <param name="columnName"></param>
	    /// <param name="keywords"></param>
	    /// <param name="pageNumber"></param>
	    /// <param name="batchSize"></param>
	    /// <returns></returns>
	    Task<List<CompositeComponent>> GetByGroupAndColumnNameAndKeyWords(string type, string group, string columnName,
	        List<string> keywords, int pageNumber = -1, int batchSize = -1);


	}
}