using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ICostPriceRatioBuilder"/>
	public class ServiceCostPriceRatioBuilder : ICostPriceRatioBuilder
	{
		private readonly IServiceRepository _serviceRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceCostPriceRatioBuilder"/> class.
		/// </summary>
		/// <param name="serviceRepository">The service repository.</param>
		public ServiceCostPriceRatioBuilder(IServiceRepository serviceRepository)
		{
			_serviceRepository = serviceRepository;
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
				var service = await _serviceRepository.Find(costPriceRatioData.Code);
				var costPriceRatio = new CostPriceRatio(costPriceRatioData.CprCoefficient, costPriceRatioData.AppliedFrom,
					costPriceRatioData.ComponentType, Convert.ToString(service["Classification"]["Service Level 1"]),
					Convert.ToString(service["Classification"]["Service Level 2"]),
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