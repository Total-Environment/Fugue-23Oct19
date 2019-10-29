using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioService
	{
		/// <summary>
		/// Creates the specified cost price ratio.
		/// </summary>
		/// <param name="costPriceRatioData">The cost price ratio.</param>
		/// <returns></returns>
		Task<CostPriceRatio> Create(CostPriceRatio costPriceRatioData);

		/// <summary>
		/// Gets the cost price ratio.
		/// </summary>
		/// <param name="appliedOn">The applied on.</param>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="level1">The level1.</param>
		/// <param name="level2">The level2.</param>
		/// <param name="level3">The level3.</param>
		/// <param name="code">The code.</param>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		Task<CostPriceRatio> GetCostPriceRatio(DateTime appliedOn, ComponentType componentType, string level1, string level2,
			string level3, string code, string projectCode);

	    /// <summary>
	    /// </summary>
	    /// <param name="appliedOn"></param>
	    /// <param name="componentType"></param>
	    /// <param name="projectCode"></param>
	    /// <returns></returns>
	    Task<CostPriceRatioList> GetCostPriceRatioList(DateTime appliedOn, ComponentType componentType, string projectCode);

	}
}
