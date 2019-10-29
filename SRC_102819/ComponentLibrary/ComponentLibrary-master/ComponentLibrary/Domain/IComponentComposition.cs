using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	public interface IComponentComposition
	{
		/// <summary>
		/// Gets the component coefficients.
		/// </summary>
		/// <value>The component coefficients.</value>
		IEnumerable<ComponentCoefficient> ComponentCoefficients { get; }
	}
}