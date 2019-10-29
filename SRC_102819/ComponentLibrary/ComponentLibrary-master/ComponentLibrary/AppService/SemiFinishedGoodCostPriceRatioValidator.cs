using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.CostPriceRatioValidator"/>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ICostPriceRatioValidator"/>
	public class SemiFinishedGoodCostPriceRatioValidator : CostPriceRatioValidator, ICostPriceRatioValidator
	{
		private readonly ICompositeComponentRepository _compositeComponentRepository;
		private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodCostPriceRatioValidator"/> class.
		/// </summary>
		/// <param name="compositeComponentRepository">The composite component repository.</param>
		/// <param name="projectRepository">The project repository.</param>
		/// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
		public SemiFinishedGoodCostPriceRatioValidator(ICompositeComponentRepository compositeComponentRepository,
			IProjectRepository projectRepository, IDependencyDefinitionRepository dependencyDefinitionRepository)
			: base(projectRepository)
		{
			_compositeComponentRepository = compositeComponentRepository;
			_dependencyDefinitionRepository = dependencyDefinitionRepository;
		}

		/// <summary>
		/// Validates the specified cost price ratio data.
		/// </summary>
		/// <param name="costPriceRatioData">The cost price ratio data.</param>
		/// <returns></returns>
		public async Task<Tuple<bool, string>> Validate(CostPriceRatio costPriceRatioData)
		{
			var isAppliedFromValid = ValidateAppliedFrom(costPriceRatioData.AppliedFrom);
			if (!isAppliedFromValid.Item1)
				return isAppliedFromValid;

			if (costPriceRatioData.Level1 != null && costPriceRatioData.Level2 == null &&
				costPriceRatioData.Level3 == null && costPriceRatioData.Code == null &&
				costPriceRatioData.ProjectCode == null)
			{
				var result = await ValidateLevels(costPriceRatioData.Level1, costPriceRatioData.Level2);
				return result;
			}
			else if (costPriceRatioData.Level1 != null && costPriceRatioData.Level2 != null &&
					costPriceRatioData.Level3 == null && costPriceRatioData.Code == null &&
					 costPriceRatioData.ProjectCode == null)
			{
				var result = await ValidateLevels(costPriceRatioData.Level1, costPriceRatioData.Level2);
				return result;
			}
			else if (costPriceRatioData.Level1 == null && costPriceRatioData.Level2 == null &&
					 costPriceRatioData.Level3 == null && costPriceRatioData.Code != null &&
					 costPriceRatioData.ProjectCode == null)
			{
				var result = await ValidateCode(costPriceRatioData.Code);
				return result;
			}
			else if (costPriceRatioData.Level1 == null && costPriceRatioData.Level2 == null &&
					 costPriceRatioData.Level3 == null && costPriceRatioData.Code != null &&
					 costPriceRatioData.ProjectCode != null)
			{
				var result = await ValidateCode(costPriceRatioData.Code);
				if (result.Item1)
				{
					var projectCodeResult = await ValidateProjectCode(costPriceRatioData.ProjectCode);
					return projectCodeResult;
				}
				else
				{
					return result;
				}
			}
			else if (costPriceRatioData.Level1 != null && costPriceRatioData.Level2 != null &&
					 costPriceRatioData.Level3 != null && costPriceRatioData.Code == null &&
					 costPriceRatioData.ProjectCode == null)
			{
				return new Tuple<bool, string>(false, "CPR cannot be defined at semi finished good level3.");
			}
			else
			{
				return new Tuple<bool, string>(false, "Invalid combination of semi finished good level1, level2, code and project code.");
			}
		}

		private async Task<Tuple<bool, string>> ValidateLevels(string level1, string level2)
		{
			var dependencyDefinition = await _dependencyDefinitionRepository.GetDependencyDefinition("sfgClassifications");
			if (
				dependencyDefinition.Blocks.Select(b => b.Data[0])
					.Distinct()
					.All(x => !string.Equals(x, level1, StringComparison.InvariantCultureIgnoreCase)))
			{
				return new Tuple<bool, string>(false, $"Invalid semi finished good level1 value.");
			}
			if (level2 != null &&
				dependencyDefinition.Blocks.Where(b => string.Equals(b.Data[0], level1, StringComparison.InvariantCultureIgnoreCase))
					.Select(b => b.Data[1])
					.Distinct()
					.All(x => !string.Equals(x, level2, StringComparison.InvariantCultureIgnoreCase)))
			{
				return new Tuple<bool, string>(false, $"Invalid combination of semi finished good level1 and level2 values.");
			}

			return new Tuple<bool, string>(true, string.Empty);
		}

		private async Task<Tuple<bool, string>> ValidateCode(string code)
		{
			var count = await _compositeComponentRepository.Count("sfg",
				new Dictionary<string, Tuple<string, object>>
				{
					{"sfg_code", new Tuple<string, object>("Regex", $"((?i){code}(?-i))")}
				});
			return count == 0
				? new Tuple<bool, string>(false, $"{code} is not valid. Please enter a valid semi finished good code.")
				: new Tuple<bool, string>(true, string.Empty);
		}
	}
}