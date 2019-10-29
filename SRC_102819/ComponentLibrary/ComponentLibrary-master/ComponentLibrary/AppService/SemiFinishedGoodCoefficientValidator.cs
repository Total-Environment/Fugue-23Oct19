using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ComponentCoefficientValidator"/>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientValidator"/>
	public class SemiFinishedGoodCoefficientValidator : ComponentCoefficientValidator, IComponentCoefficientValidator
	{
		private readonly ICompositeComponentRepository _semiFinishedGoodRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodCoefficientValidator"/> class.
		/// </summary>
		/// <param name="semiFinishedGoodRepository">The semi finished good repository.</param>
		public SemiFinishedGoodCoefficientValidator(ICompositeComponentRepository semiFinishedGoodRepository)
		{
			_semiFinishedGoodRepository = semiFinishedGoodRepository;
		}

		/// <summary>
		/// Validates the specified component coefficient.
		/// </summary>
		/// <param name="componentCoefficient">The component coefficient.</param>
		/// <returns></returns>
		public new async Task<Tuple<bool, string>> Validate(ComponentCoefficient componentCoefficient)
		{
			var result = base.Validate(componentCoefficient);
			if (result.Item1)
			{
				return await ValidateSemiFinishedGoodCode(componentCoefficient.Code);
			}
			else
			{
				return result;
			}
		}

		private async Task<Tuple<bool, string>> ValidateSemiFinishedGoodCode(string semiFinishedGoodCode)
		{
			if (string.IsNullOrWhiteSpace(semiFinishedGoodCode))
				return new Tuple<bool, string>(false, "Semi Finished Good code is mandatory.");

			var count = await _semiFinishedGoodRepository.Count("sfg", new Dictionary<string, Tuple<string, object>>
			{
				{"sfg_code", new Tuple<string, object>("Regex", $"((?i){semiFinishedGoodCode}(?-i))")}
			});
			if (count == 1)
				return new Tuple<bool, string>(true, string.Empty);
			else
				return new Tuple<bool, string>(false, $"Invalid semi finished good code {semiFinishedGoodCode}");
		}
	}
}