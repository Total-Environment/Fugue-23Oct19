using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IPriceService"/>
	public class PriceService : IPriceService
	{
		private readonly IMasterDataRepository _masterDataRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IMaterialRateService _materialRateService;
		private readonly IServiceRateService _serviceRateService;
		private readonly ICompositeComponentService _compositeComponentService;
		private readonly ICostPriceRatioService _costPriceRatioService;
		private readonly IMaterialRepository _materialRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly ICompositeComponentRepository _compositeComponentRepository;
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _compositeComponentDefinitionRepository;
		private readonly ICodePrefixTypeMappingRepository _codePrefixTypeMappingRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceService"/> class.
		/// </summary>
		/// <param name="masterDataRepository">The master data repository.</param>
		/// <param name="projectRepository">The project repository.</param>
		/// <param name="materialRateService">The material rate service.</param>
		/// <param name="serviceRateService">The service rate service.</param>
		/// <param name="compositeComponentService">The composite component service.</param>
		/// <param name="costPriceRatioService">The cost price ratio service.</param>
		/// <param name="materialRepository">The material repository.</param>
		/// <param name="serviceRepository">The service repository.</param>
		/// <param name="compositeComponentRepository">The composite component repository.</param>
		/// <param name="compositeComponentDefinitionRepository">
		/// The composite component definition repository.
		/// </param>
		/// <param name="codePrefixTypeMappingRepository">The code prefix type mapping repository.</param>
		public PriceService(IMasterDataRepository masterDataRepository, IProjectRepository projectRepository,
			IMaterialRateService materialRateService, IServiceRateService serviceRateService,
			ICompositeComponentService compositeComponentService, ICostPriceRatioService costPriceRatioService,
			IMaterialRepository materialRepository, IServiceRepository serviceRepository,
			ICompositeComponentRepository compositeComponentRepository,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> compositeComponentDefinitionRepository,
			ICodePrefixTypeMappingRepository codePrefixTypeMappingRepository)
		{
			_masterDataRepository = masterDataRepository;
			_projectRepository = projectRepository;
			_materialRateService = materialRateService;
			_serviceRateService = serviceRateService;
			_compositeComponentService = compositeComponentService;
			_costPriceRatioService = costPriceRatioService;
			_materialRepository = materialRepository;
			_serviceRepository = serviceRepository;
			_compositeComponentRepository = compositeComponentRepository;
			_compositeComponentDefinitionRepository = compositeComponentDefinitionRepository;
			_codePrefixTypeMappingRepository = codePrefixTypeMappingRepository;
		}

	    /// <summary>
	    /// Gets the price.
	    /// </summary>
	    /// <param name="code">The code.</param>
	    /// <param name="location">The location.</param>
	    /// <param name="appliedOn">The applied on.</param>
	    /// <param name="projectCode">The project code.</param>
	    /// <returns></returns>
	    /// <exception cref="ArgumentException">Invalid component type.</exception>
	    public async Task<ComponentPrice> GetPrice(string code, string location, DateTime appliedOn, string projectCode = null)
		{
			string codePrefix;
			if (code.Length > 3)
				codePrefix = code.Substring(0, 3);
			else
				throw new ArgumentException("Invalid code.");
			var codePrefixTypeMapping = await _codePrefixTypeMappingRepository.Get(codePrefix);
			var componentType = codePrefixTypeMapping.ComponentType;

			if (projectCode != null)
			{
				await _projectRepository.Find(projectCode);
			}

			var validLocation = await _masterDataRepository.Exists("location", location);
			if (!validLocation)
				throw new ArgumentException("Invalid location.");

			var costPriceRatio = await GetCostPriceRatio(componentType, code, appliedOn, projectCode);
		    var unitOfMeasure = await GetUnitOfMeasure(componentType, code);
			var cost = await GetCost(componentType, code, location, appliedOn);
			var price = new Money(cost.Value * (decimal)costPriceRatio, cost.Currency);
            var componentPrice = new ComponentPrice(price, unitOfMeasure);
			return componentPrice;
		}

	    private async Task<string> GetUnitOfMeasure(ComponentType componentType, string code)
	    {
            string unitOfMeasure = null;
	        try
            {
                IHeaderData generalHeader;
                switch (componentType)
                {
                    case ComponentType.Material:
                        var material = await _materialRepository.Find(code);
                        generalHeader = material.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
                        unitOfMeasure = GetValue(generalHeader, "unit_of_measure");
                        break;

                    case ComponentType.Service:
                        var service = await _serviceRepository.Find(code);
                        generalHeader = service.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
                        unitOfMeasure = GetValue(generalHeader, "unit_of_measure");
                        break;

                    case ComponentType.Asset:
                        throw new NotSupportedException($"{componentType} is not supported. Try with {ComponentType.Material}.");
                    case ComponentType.SFG:
                        var sfgDefinition = await GetCompositeComponentDefinition("sfg");
                        var sfg = await _compositeComponentRepository.Find("sfg", code, sfgDefinition);
                        generalHeader = sfg.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
                        unitOfMeasure = GetValue(generalHeader, "unit_of_measure");
                        break;

                    case ComponentType.Package:
                        var packageDefinition = await GetCompositeComponentDefinition("package");
                        var package = await _compositeComponentRepository.Find("package", code, packageDefinition);
                        generalHeader = package.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
                        unitOfMeasure = GetValue(generalHeader, "unit_of_measure");
                        break;

                    default:
                        throw new NotImplementedException($"{componentType} is not implemented.");
                }
            }
            catch (ResourceNotFoundException e)
            {
                throw new ArgumentException($"{componentType} : {code} is not found ", e);
            }
            return unitOfMeasure;
        }

	    private string GetValue(IHeaderData generalHeader, string columnKey)
	    {
	        string unitOfMeasure = null;
            var unitOfMeasureColumn = generalHeader?.Columns?.FirstOrDefault(c => c.Key == columnKey);
            if (unitOfMeasureColumn != null)
                unitOfMeasure = unitOfMeasureColumn?.Value.ToString();
	        return unitOfMeasure;
	    }


	    private async Task<ICompositeComponentDefinition> GetCompositeComponentDefinition(string type)
        {
            ICompositeComponentDefinition compositeComponentDefinition;
            if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
                compositeComponentDefinition = await _compositeComponentDefinitionRepository.Find(type, Keys.Sfg.SfgDefinitionGroup);
            else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
                compositeComponentDefinition = await _compositeComponentDefinitionRepository.Find(type,
                    Keys.Package.PackageDefinitionGroup);
            else
                throw new NotSupportedException(type + " is not supported.");
            return compositeComponentDefinition;
        }

        private async Task<Money> GetCost(ComponentType componentType, string code, string location, DateTime appliedOn)
		{
			Money cost;
			try
			{
				switch (componentType)
				{
					case ComponentType.Material:
						cost = await _materialRateService.GetAverageLandedRate(code, location, appliedOn, "INR");
						break;

					case ComponentType.Service:
						cost = await _serviceRateService.GetAverageLandedRate(code, location, appliedOn, "INR");
						break;

					case ComponentType.Asset:
						throw new NotSupportedException($"{componentType} is not supported. Try with {ComponentType.Material}.");
					case ComponentType.SFG:
						cost = (await _compositeComponentService.GetCost("sfg", code, location, appliedOn)).TotalCost;
						break;

					case ComponentType.Package:
						cost = (await _compositeComponentService.GetCost("package", code, location, appliedOn)).TotalCost;
						break;

					default:
						throw new NotImplementedException($"{componentType} is not implemented.");
				}
			}
			catch (ResourceNotFoundException e)
			{
				throw new ArgumentException($"Price cannot be derived as Cost for the {code} is not available.", e);
			}
			return cost;
		}

		private async Task<double> GetCostPriceRatio(ComponentType componentType, string code, DateTime appliedOn,
			string projectCode)
		{
			string level1;
			string level2;
			string level3 = null;

			switch (componentType)
			{
				case ComponentType.Material:
					var material = await _materialRepository.Find(code);
					level1 = Convert.ToString(material["Classification"]["Material Level 1"]);
					level2 = Convert.ToString(material["Classification"]["Material Level 2"]);
					level3 = Convert.ToString(material["Classification"]["Material Level 3"]);
					break;

				case ComponentType.Service:
					var service = await _serviceRepository.Find(code);
					level1 = Convert.ToString(service["Classification"]["Service Level 1"]);
					level2 = Convert.ToString(service["Classification"]["Service Level 2"]);
					break;

				case ComponentType.Asset:
					throw new NotSupportedException($"{componentType} is not supported. Try with {ComponentType.Material}.");
				case ComponentType.SFG:
					var semiFinishedGoodDefinition = await _compositeComponentDefinitionRepository.Find("sfg", Keys.Sfg.SfgDefinitionGroup);
					var semiFinishedGood = await _compositeComponentRepository.Find("sfg", code, semiFinishedGoodDefinition);
					level1 = Convert.ToString(semiFinishedGood["Classification"]["SFG Level 1"]);
					level2 = Convert.ToString(semiFinishedGood["Classification"]["SFG Level 2"]);
					break;

				case ComponentType.Package:
					var packageDefinition = await _compositeComponentDefinitionRepository.Find("package", Keys.Package.PackageDefinitionGroup);
					var package = await _compositeComponentRepository.Find("package", code, packageDefinition);
					level1 = Convert.ToString(package["Classification"]["Package Level 1"]);
					level2 = Convert.ToString(package["Classification"]["Package Level 2"]);
					break;

				default:
					throw new NotImplementedException($"{componentType} is not implemented.");
			}

			CostPriceRatio costPriceRatio;
			try
			{
				costPriceRatio = await _costPriceRatioService.GetCostPriceRatio(appliedOn, componentType, level1, level2, level3,
					code, projectCode);
			}
			catch (ResourceNotFoundException e)
			{
				throw new ArgumentException($"Price cannot be derived as CPR is not setup for the given {code} as of {appliedOn:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'}.", e);
			}
			return costPriceRatio.CprCoefficient.CPR;
		}
	}
}