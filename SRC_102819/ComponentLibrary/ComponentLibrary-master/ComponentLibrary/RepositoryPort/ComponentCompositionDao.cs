using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	public class ComponentCompositionDao
	{
		/// <summary>
		/// Gets or sets the component coefficients.
		/// </summary>
		/// <value>The component coefficients.</value>
		public IEnumerable<ComponentCoefficientDao> ComponentCoefficients { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentCompositionDao"/> class.
		/// </summary>
		public ComponentCompositionDao()
		{
			ComponentCoefficients = new List<ComponentCoefficientDao>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentCompositionDao"/> class.
		/// </summary>
		/// <param name="componentComposition">The semi finished good composition.</param>
		public ComponentCompositionDao(IComponentComposition componentComposition)
		{
			ComponentCoefficients =
				componentComposition.ComponentCoefficients.Select(m => new ComponentCoefficientDao(m)).ToList();
		}

		/// <summary>
		/// To the semi finished good composition.
		/// </summary>
		/// <returns></returns>
		public ComponentComposition ToComponentComposition()
		{
			return new ComponentComposition(ComponentCoefficients.Select(m => m.ToComponentCoeficient()));
		}
	}
}