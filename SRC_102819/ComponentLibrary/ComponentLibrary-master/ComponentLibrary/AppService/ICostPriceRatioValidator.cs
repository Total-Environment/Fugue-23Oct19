using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioValidator
	{
		/// <summary>
		/// Validates the specified cost price ratio data.
		/// </summary>
		/// <param name="costPriceRatioData">The cost price ratio data.</param>
		/// <returns></returns>
		Task<Tuple<bool, string>> Validate(CostPriceRatio costPriceRatioData);
	}
}