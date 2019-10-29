using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	/// <summary>
	/// </summary>
	public class ComponentComposition : IComponentComposition
	{
		/// <summary>
		/// Gets the component coefficients.
		/// </summary>
		/// <value>The component coefficients.</value>
		public IEnumerable<ComponentCoefficient> ComponentCoefficients { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentComposition"/> class.
		/// </summary>
		public ComponentComposition()
		{
			ComponentCoefficients = new List<ComponentCoefficient>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentComposition"/> class.
		/// </summary>
		/// <param name="componentCoefficients">The component coefficients.</param>
		public ComponentComposition(IEnumerable<ComponentCoefficient> componentCoefficients)
		{
			ComponentCoefficients = componentCoefficients;
		}
	}
}