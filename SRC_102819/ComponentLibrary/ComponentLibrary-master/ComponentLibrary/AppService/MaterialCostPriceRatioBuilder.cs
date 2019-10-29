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
	public class MaterialCostPriceRatioBuilder : ICostPriceRatioBuilder
	{
		private readonly IMaterialRepository _materialRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="MaterialCostPriceRatioBuilder"/> class.
		/// </summary>
		/// <param name="materialRepository">The material repository.</param>
		public MaterialCostPriceRatioBuilder(IMaterialRepository materialRepository)
		{
			_materialRepository = materialRepository;
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
				var material = await _materialRepository.Find(costPriceRatioData.Code);
				var costPriceRatio = new CostPriceRatio(costPriceRatioData.CprCoefficient, costPriceRatioData.AppliedFrom,
					costPriceRatioData.ComponentType, Convert.ToString(material["Classification"]["Material Level 1"]),
					Convert.ToString(material["Classification"]["Material Level 2"]),
					Convert.ToString(material["Classification"]["Material Level 3"]), costPriceRatioData.Code,
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