using System;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="ICompositeComponentValidator"/>
	public class CompositeComponentValidator : ICompositeComponentValidator
	{
		private readonly IComponentCoefficientValidatorFactory _componentCoefficientValidatorFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentValidator"/> class.
		/// </summary>
		/// <param name="componentCoefficientValidatorFactory">
		/// The component coefficient validator factory.
		/// </param>
		public CompositeComponentValidator(IComponentCoefficientValidatorFactory componentCoefficientValidatorFactory)
		{
			_componentCoefficientValidatorFactory = componentCoefficientValidatorFactory;
		}

		/// <summary>
		/// Validates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="componentComposition">The component composition.</param>
		/// <returns></returns>
		public async Task<Tuple<bool, string>> Validate(string type, IComponentComposition componentComposition)
		{
			if (componentComposition.ComponentCoefficients.Count() < 2 && type.ToLower() == "sfg")
			{
				return new Tuple<bool, string>(false, "Minimum two component coefficients are required");
			}
			if (!componentComposition.ComponentCoefficients.Any() && type.ToLower() == "package")
			{
				return new Tuple<bool, string>(false, "Minimum one component coefficients are required");
			}

			foreach (var componentCoeficient in componentComposition.ComponentCoefficients)
			{
				if (componentCoeficient.ComponentType == ComponentType.Package)
					return new Tuple<bool, string>(false, $"{type} can't have Package in its composition.");

				if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase) &&
					componentCoeficient.ComponentType == ComponentType.SFG)
					return new Tuple<bool, string>(false, "Semi Finished Good can't have Semi Finished Good in its composition.");

				var componentCoefficientValidator =
					_componentCoefficientValidatorFactory.GetComponentCoefficientValidator(componentCoeficient
						.ComponentType);

				var result = await componentCoefficientValidator.Validate(componentCoeficient);
				if (result.Item1 == false)
					return result;
			}

			var distinctComponents = componentComposition.ComponentCoefficients.Select(c => new { c.ComponentType, c.Code }).Distinct();
			if (distinctComponents.Count() != componentComposition.ComponentCoefficients.Count())
			{
				return new Tuple<bool, string>(false, "Combination of componet type and code should be unique.");
			}

			return new Tuple<bool, string>(true, String.Empty);
		}
	}
}