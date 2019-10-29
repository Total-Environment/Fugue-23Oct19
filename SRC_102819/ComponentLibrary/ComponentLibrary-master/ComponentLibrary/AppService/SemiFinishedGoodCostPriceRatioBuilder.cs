using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ICostPriceRatioBuilder"/>
	public class SemiFinishedGoodCostPriceRatioBuilder : ICostPriceRatioBuilder
	{
		private readonly ICompositeComponentRepository _compositeComponentRepository;
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _compositeComponentDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodCostPriceRatioBuilder"/> class.
		/// </summary>
		/// <param name="compositeComponentRepository">The composite component repository.</param>
		/// <param name="compositeComponentDefinitionRepository">
		/// The composite component definition repository.
		/// </param>
		public SemiFinishedGoodCostPriceRatioBuilder(ICompositeComponentRepository compositeComponentRepository,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> compositeComponentDefinitionRepository)
		{
			_compositeComponentRepository = compositeComponentRepository;
			_compositeComponentDefinitionRepository = compositeComponentDefinitionRepository;
		}

		/// <summary>
		/// Builds the specified cost price ratio data.
		/// </summary>
		/// <param name="costPriceRatioData">The cost price ratio data.</param>
		/// <returns></returns>
		public async Task<CostPriceRatio> Build(CostPriceRatio costPriceRatioData)
		{
			if (costPriceRatioData.Code != null)
			{
				var semiFinishedGoodDefinition = await _compositeComponentDefinitionRepository.Find("sfg",
					Keys.Sfg.SfgDefinitionGroup);
				var semiFinishedGood = await _compositeComponentRepository.Find("sfg", costPriceRatioData.Code,
					semiFinishedGoodDefinition);
				var costPriceRatio = new CostPriceRatio(costPriceRatioData.CprCoefficient, costPriceRatioData.AppliedFrom,
					costPriceRatioData.ComponentType, Convert.ToString(semiFinishedGood["Classification"]["SFG Level 1"]),
					Convert.ToString(semiFinishedGood["Classification"]["SFG Level 2"]),
					costPriceRatioData.Level3, costPriceRatioData.Code,
					costPriceRatioData.ProjectCode);
				return costPriceRatio;
			}
			else
			{
				return costPriceRatioData;
			}
		}
	}
}