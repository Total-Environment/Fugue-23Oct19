using System;
using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public abstract class ComponentCoefficientValidator
	{
		/// <summary>
		/// Validates the specified component coefficient.
		/// </summary>
		/// <param name="componentCoefficient">The component coefficient.</param>
		/// <returns></returns>
		public Tuple<bool, string> Validate(ComponentCoefficient componentCoefficient)
		{
			var result = ValidateCoefficient(componentCoefficient.Coefficient, componentCoefficient.Code);
			if (result.Item1)
				return ValidateWastages(componentCoefficient.WastagePercentages, componentCoefficient.Code);
			else
				return result;
		}

		private Tuple<bool, string> ValidateCoefficient(double coefficient, string code)
		{
			if (coefficient <= 0)
				return new Tuple<bool, string>(false, $"Invalid coefficient value for {code}");
			else
				return new Tuple<bool, string>(true, string.Empty);
		}

		private Tuple<bool, string> ValidateWastages(IEnumerable<WastagePercentage> wastages, string code)
		{
			if (wastages != null)
			{
				foreach (var wastage in wastages)
				{
					if (string.IsNullOrWhiteSpace(wastage.Name))
					{
						return new Tuple<bool, string>(false, $"Wastage name is mandatory for {code}");
					}

					if (wastage.Value < 0)
					{
						return new Tuple<bool, string>(false, $"Invalid wastage value for {wastage.Name} for {code}");
					}
				}
			}

			return new Tuple<bool, string>(true, string.Empty);
		}
	}
}