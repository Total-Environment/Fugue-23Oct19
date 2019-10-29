using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
	/// <summary>
	/// </summary>
	public class ComponentCompositionDto
	{
		/// <summary>
		/// Gets or sets the component coefficients.
		/// </summary>
		/// <value>The component coefficients.</value>
		public IEnumerable<ComponentCoefficientDto> ComponentCoefficients { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentCompositionDto"/> class.
		/// </summary>
		public ComponentCompositionDto()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentCompositionDto"/> class.
		/// </summary>
		/// <param name="componentComposition">The semi finished good composition.</param>
		public ComponentCompositionDto(IComponentComposition componentComposition)
		{
			ComponentCoefficients =
				componentComposition.ComponentCoefficients.Select(m => new ComponentCoefficientDto(m));
		}

		/// <summary>
		/// To the domain.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IComponentComposition ToDomain()
		{
			return new ComponentComposition(ComponentCoefficients.Select(c => c.ToDomain()));
		}
	}
}