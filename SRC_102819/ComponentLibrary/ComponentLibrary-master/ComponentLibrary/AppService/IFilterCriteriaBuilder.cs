using System;
using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// Generates fliter criteria to be applied on the mongo collection based on material definition
	/// using filter data.
	/// </summary>
	public interface IFilterCriteriaBuilder
	{
		/// <summary>
		/// Builds the filter criteria based in specified material definition using filter data provided.
		/// </summary>
		/// <param name="materialDefinition">The material definition.</param>
		/// <param name="brandDefinition">The brand definition.</param>
		/// <param name="filterData">The filter data.</param>
		/// <param name="group">The group.</param>
		/// <param name="searchKeywords">The searchkeywords.</param>
		/// <returns></returns>
		Dictionary<string, Tuple<string, object>> Build(IMaterialDefinition materialDefinition, IBrandDefinition brandDefinition, List<FilterData> filterData, string group, List<string> searchKeywords);

		/// <summary>
		/// Builds the specified service definition.
		/// </summary>
		/// <param name="serviceDefinition">The service definition.</param>
		/// <param name="filterData">The filter data.</param>
		/// <param name="group">The group.</param>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <returns></returns>
		Dictionary<string, Tuple<string, object>> Build(IServiceDefinition serviceDefinition, List<FilterData> filterData, string group, List<string> searchKeywords);

		/// <summary>
		/// Builds the specified semi finished good definition.
		/// </summary>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <param name="filterData">The filter data.</param>
		/// <param name="compositeComponentDefinition">The semi finished good definition.</param>
		/// <returns></returns>
		Dictionary<string, Tuple<string, object>> Build(List<string> searchKeywords,
			List<FilterData> filterData, ICompositeComponentDefinition compositeComponentDefinition);

		/// <summary>
		/// Builds the rate filters.
		/// </summary>
		/// <param name="filterData">The filter data.</param>
		/// <param name="componentType">Material or Service</param>
		/// <returns></returns>
		Dictionary<string, Tuple<string, object>> BuildRateFilters(List<FilterData> filterData, string componentType);
	}
}