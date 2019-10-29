using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioDefinitionRepository
	{
		/// <summary>
		/// Adds the specified cost price ratio definition.
		/// </summary>
		/// <param name="costPriceRatioDefinition">The cost price ratio definition.</param>
		/// <returns></returns>
		Task Add(CostPriceRatioDefinition costPriceRatioDefinition);

		/// <summary>
		/// Finds the by.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		Task<CostPriceRatioDefinition> FindBy(string name);
	}
}