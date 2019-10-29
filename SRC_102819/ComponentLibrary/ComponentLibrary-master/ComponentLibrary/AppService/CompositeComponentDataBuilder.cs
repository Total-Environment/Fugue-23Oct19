using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public class CompositeComponentDataBuilder : ICompositeComponentDataBuilder
	{
		private readonly IComponentCoefficientBuilderFactory _componentCoefficientBuilderFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDataBuilder"/> class.
		/// </summary>
		/// <param name="componentCoefficientBuilderFactory">
		/// The component coefficient builder factory.
		/// </param>
		public CompositeComponentDataBuilder(IComponentCoefficientBuilderFactory componentCoefficientBuilderFactory)
		{
			_componentCoefficientBuilderFactory = componentCoefficientBuilderFactory;
		}

		/// <summary>
		/// Builds the data.
		/// </summary>
		/// <param name="componentComposition">The semi finished good composition.</param>
		/// <returns></returns>
		public async Task<IComponentComposition> BuildData(IComponentComposition componentComposition)
		{
			var componentCoefficients = new List<ComponentCoefficient>();
			foreach (var componentCoeficient in componentComposition.ComponentCoefficients)
			{
				var componentCoefficientBuilder =
					_componentCoefficientBuilderFactory.GetComponentCoefficientBuilder(componentCoeficient
						.ComponentType);

				var componentCoeficientResult = await componentCoefficientBuilder.BuildData(componentCoeficient);
				componentCoefficients.Add(componentCoeficientResult);
			}
			var componentCompositionResult = new ComponentComposition(componentCoefficients);
			return componentCompositionResult;
		}
	}
}