using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioBuilderFactory
	{
		/// <summary>
		/// Gets the cost price ratio builder.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns></returns>
		ICostPriceRatioBuilder GetCostPriceRatioBuilder(ComponentType componentType);
	}
}