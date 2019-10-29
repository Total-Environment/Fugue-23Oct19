using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioBuilder
	{
		/// <summary>
		/// Builds the specified cost price ratio data.
		/// </summary>
		/// <param name="costPriceRatioData">The cost price ratio data.</param>
		/// <returns></returns>
		Task<CostPriceRatio> Build(CostPriceRatio costPriceRatioData);
	}
}