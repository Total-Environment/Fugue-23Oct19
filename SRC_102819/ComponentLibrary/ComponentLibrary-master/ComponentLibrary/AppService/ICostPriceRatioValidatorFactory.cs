using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public interface ICostPriceRatioValidatorFactory
	{
		/// <summary>
		/// Gets the cost price ratio validator.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns></returns>
		ICostPriceRatioValidator GetCostPriceRatioValidator(ComponentType componentType);
	}
}