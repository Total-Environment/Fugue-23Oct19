using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioRepository
	{
		/// <summary>
		/// Creates the specified cost price ratio.
		/// </summary>
		/// <param name="costPriceRatio">The cost price ratio.</param>
		/// <returns></returns>
		Task<CostPriceRatio> Create(CostPriceRatio costPriceRatio);

		/// <summary>
		/// Gets the cost price rato.
		/// </summary>
		/// <param name="appliedOn">The applied on.</param>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="level1">The level1.</param>
		/// <param name="level2">The level2.</param>
		/// <param name="level3">The level3.</param>
		/// <param name="code">The code.</param>
		/// <param name="projectCode">The project code.</param>
		/// <param name="costPriceRatioDefinition">The cost price ratio definition.</param>
		/// <returns></returns>
		Task<CostPriceRatio> GetCostPriceRato(DateTime appliedOn, ComponentType componentType, string level1, string level2,
			string level3, string code, string projectCode, CostPriceRatioDefinition costPriceRatioDefinition);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="appliedOn"></param>
	    /// <param name="componentType"></param>
	    /// <param name="costPriceRatioDefinition"></param>
	    /// <returns></returns>
	    Task<CostPriceRatioList> GetCostPriceRatioList(DateTime appliedOn, ComponentType componentType, CostPriceRatioDefinition costPriceRatioDefinition);

	}
}
