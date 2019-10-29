using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// App service to manage SFG and Package.
	/// </summary>
	public interface ICompositeComponentService
	{
		/// <summary>
		/// Creates the SFG from service specified.
		/// </summary>
		/// <param name="service">The service with specified service id.</param>
		/// <param name="componentComposition"></param>
		/// <returns>Created SFG.</returns>
		Task<CompositeComponent> CloneFromService(IService service, IComponentComposition componentComposition);

		/// <summary>
		/// Creates the SFG with specified data.
		/// </summary>
		/// <param name="sfgData">The SFG data.</param>
		/// <returns>CReated SFG.</returns>
		Task<CompositeComponent> Create(string type, CompositeComponent sfgData);

		/// <summary>
		/// Gets the SFG with specified identifier.
		/// </summary>
		/// <param name="compositeComponent">The SFG identifier.</param>
		/// <returns>SFG matching the id.</returns>
		Task<CompositeComponent> Get(string type, string compositeComponent);

		/// <summary>
		/// Gets the cost of an SFG.
		/// </summary>
		/// <param name="compositeComponentCode">The SFG code.</param>
		/// <param name="location">The location.</param>
		/// <param name="appliedOn">The applied on.</param>
		/// <returns></returns>
		Task<CompositeComponentCost> GetCost(string type, string compositeComponentCode, string location, DateTime appliedOn);

		/// <summary>
		/// Updates the specified semi finished good.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="compositeComponent">The semi finished good.</param>
		/// <returns></returns>
		Task<CompositeComponent> Update(string type, CompositeComponent compositeComponent);

		/// <summary>
		/// Updates the rates.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="compositeComponent">The semi finished good.</param>
		/// <returns></returns>
		Task UpdateRates(string type, CompositeComponent compositeComponent);

		/// <summary>
		/// Counts the specified filter datas.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <param name="filterDatas">The filter datas.</param>
		/// <returns></returns>
		Task<long> Count(string type, List<string> searchKeywords, List<FilterData> filterDatas);

		/// <summary>
		/// Searches the specified filter datas.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <param name="filterDatas">The filter datas.</param>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="batchSize">Size of the batch.</param>
		/// <returns></returns>
		Task<List<CompositeComponent>> Find(string type, List<string> searchKeywords, List<FilterData> filterDatas, string sortColumn,
			SortOrder sortOrder, int pageNumber, int batchSize);


	    /// <summary>
	    /// Fetch total composite components count with attachments.
	    /// </summary>
	    /// <param name="type"></param>
	    /// <param name="group"></param>
	    /// <param name="columnName"></param>
	    /// <param name="keywords"></param>
	    /// <returns></returns>
	    Task<long> GetCountOfCompositeComponentsHavingAttachmentColumnDataInGroup(string type, string group,
	        string columnName, List<string> keywords = null);

        /// <summary>
        /// Fetch total composite components with attachments.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="group"></param>
        /// <param name="columnName"></param>
        /// <param name="keywords"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        Task<List<CompositeComponent>> GetCompositeComponentsHavingAttachmentColumnDataInGroup(string type, string group,
	        string columnName, List<string> keywords = null, int pageNumber = -1, int batchSize = -1);




	}
}