using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <inheritdoc/>
	public class CompositeComponentService : ICompositeComponentService
	{
		private readonly IClassificationDefinitionBuilder _classificationDefinitionBuilder;
		private readonly IRentalRateRepository _rentalRateRepository;
		private readonly IMaterialRateService _materialRateService;
		private readonly IServiceRateService _serviceRateService;
		private readonly IClassificationDefinitionRepository _classificationDefinitionRepository;
		private readonly ICompositeComponentBuilder _compositeComponentBuilder;
		private readonly ICompositeComponentRepository _compositeComponentRepository;
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _compositeComponentDefinitionRepository;
		private readonly IFilterCriteriaBuilder _filterCriteriaBuilder;
	    private readonly ICompositeComponentSapSyncer _compositeComponentSapSyncer;

	    /// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentService"/> class.
		/// </summary>
		/// <param name="compositeComponentBuilder">The semi finished good builder.</param>
		/// <param name="compositeComponentRepository">The semi finished good repository.</param>
		/// <param name="compositeComponentDefinitionRepository">The SFG definition repository.</param>
		/// <param name="classificationDefinitionRepository">Classification Definition Repository.</param>
		/// <param name="classificationDefinitionBuilder">Classification definition builder.</param>
		/// <param name="materialRateService">The material rate service.</param>
		/// <param name="serviceRateService">The service rate service.</param>
		/// <param name="rentalRateRepository">The rental rate repository.</param>
		/// <param name="filterCriteriaBuilder">The filter criteria builder.</param>
		public CompositeComponentService(ICompositeComponentBuilder compositeComponentBuilder,
			ICompositeComponentRepository compositeComponentRepository,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> compositeComponentDefinitionRepository,
			IClassificationDefinitionRepository classificationDefinitionRepository,
			IClassificationDefinitionBuilder classificationDefinitionBuilder,
			IMaterialRateService materialRateService, IServiceRateService serviceRateService,
			IRentalRateRepository rentalRateRepository, IFilterCriteriaBuilder filterCriteriaBuilder, ICompositeComponentSapSyncer compositeComponentSapSyncer)
		{
			_compositeComponentBuilder = compositeComponentBuilder;
			_compositeComponentRepository = compositeComponentRepository;
			_compositeComponentDefinitionRepository = compositeComponentDefinitionRepository;
			_classificationDefinitionRepository = classificationDefinitionRepository;
			_classificationDefinitionBuilder = classificationDefinitionBuilder;
			_materialRateService = materialRateService;
			_serviceRateService = serviceRateService;
			_rentalRateRepository = rentalRateRepository;
			_filterCriteriaBuilder = filterCriteriaBuilder;
		    _compositeComponentSapSyncer = compositeComponentSapSyncer;
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> CloneFromService(IService service, IComponentComposition componentComposition)
		{
			var compositeComponent = Keys.Sfg.SfgDefinitionGroup;
			var compositeComponentDefinition = await _compositeComponentDefinitionRepository.Find("sfg", compositeComponent);
			var sfg = await _compositeComponentBuilder.CloneFromService(service, compositeComponentDefinition, componentComposition);
			sfg = await _compositeComponentRepository.Create("sfg", sfg, compositeComponentDefinition);
			await CloneClassificationDefinition(sfg, service);
			await AmmendClassification("sfg", sfg);
            _compositeComponentSapSyncer.Sync(sfg, false, "sfg");
            return sfg;
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Create(string type, CompositeComponent sfgData)
		{
			var compositeComponentDefinition = await GetCompositeComponentDefinition(type);
			var sfg = await _compositeComponentBuilder.Create(type, compositeComponentDefinition, sfgData);
			sfg = await _compositeComponentRepository.Create(type, sfg, compositeComponentDefinition);
			await AmmendClassification(type, sfg);
		    _compositeComponentSapSyncer.Sync(sfg, false, type);
            return sfg;
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

		/// <inheritdoc/>
		public async Task<CompositeComponent> Update(string type, CompositeComponent compositeComponent)
		{
			var compositeComponentDefinition = await GetCompositeComponentDefinition(type);
			var sfg = await _compositeComponentBuilder.Update(type, compositeComponentDefinition, compositeComponent);
			sfg = await _compositeComponentRepository.Update(type, sfg, compositeComponentDefinition);
			await AmmendClassification(type, sfg);
		    _compositeComponentSapSyncer.Sync(sfg, true, type);
            return sfg;
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Get(string type, string compositeComponent)
		{
			var compositeComponentDefinition = await GetCompositeComponentDefinition(type);
			var sfg = await _compositeComponentRepository.Find(type, compositeComponent, compositeComponentDefinition);
			await AmmendClassification(type, sfg);
			return sfg;
		}
        
		/// <inheritdoc/>
		public async Task<CompositeComponentCost> GetCost(string type, string compositeComponentCode, string location, DateTime appliedOn)
		{
			var compositeComponentCost = new CompositeComponentCost();
			var compositeComponent = await Get(type, compositeComponentCode);
			List<string> componentCodes = new List<string>();
		    string unitOfMeasure = null;
			var coefficients = compositeComponent.ComponentComposition.ComponentCoefficients.ToList();
            var generalHeader = compositeComponent.Headers.FirstOrDefault(h => h.Key.ToLower() == "general");
            var unitOfMeasureColumn = generalHeader?.Columns?.FirstOrDefault(c => c.Key == "unit_of_measure");
            if (unitOfMeasureColumn != null)
                unitOfMeasure = unitOfMeasureColumn?.Value.ToString();
		   
		    decimal cost = 0;
			foreach (var coefficient in coefficients)
			{
				decimal rate = 0;
				switch (coefficient.ComponentType)
				{
					case ComponentType.Material:
						{
							try
							{
								rate =
									(await _materialRateService.GetAverageLandedRate(coefficient.Code, location, appliedOn,
										"INR"))
									.Value;
								break;
							}
							catch (ResourceNotFoundException)
							{
								componentCodes.Add(coefficient.Code);
								continue;
							}
						}
					case ComponentType.Service:
						{
							try
							{
								rate =
									(await _serviceRateService.GetAverageLandedRate(coefficient.Code, location, appliedOn,
										"INR"))
									.Value;
								break;
							}
							catch (ResourceNotFoundException exception)
							{
								componentCodes.Add(coefficient.Code);
								continue;
							}
						}
					case ComponentType.Asset:
						{
							try
							{
								rate =
									(await _rentalRateRepository.Get(coefficient.Code, coefficient.UnitOfMeasure, appliedOn))
									.Value.Value;
								break;
							}
							catch (ResourceNotFoundException exception)
							{
								componentCodes.Add(coefficient.Code);
								continue;
							}
						}
				    case ComponentType.SFG:
				    {
				        try
				        {
                            rate = (await this.GetCost("sfg", coefficient.Code, location, appliedOn)).TotalCost.Value;
				            break;
				        }
				        catch (ResourceNotFoundException exception)
				        {
				            componentCodes.Add(coefficient.Code);
				            continue;
				        }
				    }
                }
				var componentCost = Convert.ToDecimal(decimal.ToDouble(rate) * coefficient.TotalQuantity);
				cost += componentCost;
				compositeComponentCost.ComponentCostBreakup.Add(new ComponentCost(coefficient.Code, new Money(componentCost, "INR")));
			}
			if (componentCodes.Count != 0)
			{
				throw new ResourceNotFoundException(
					$"Rate for {string.Join(",", componentCodes)} as of {appliedOn.ToShortDateString()} for {location} location is");
			}
			compositeComponentCost.TotalCost = new Money(cost, "INR");
		    compositeComponentCost.UnitOfMeasure = unitOfMeasure;
			return compositeComponentCost;
		}

		private async Task AmmendClassification(string type, CompositeComponent compositeComponent)
		{
			if (compositeComponent != null && compositeComponent.Headers != null)
			{
				string level1Value;

				if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
					level1Value = ((ColumnData)((dynamic)compositeComponent).classification.sfg_level_1).Value.ToString();
				else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
					level1Value = ((ColumnData)((dynamic)compositeComponent).classification.pkg_level_1).Value.ToString();
				else
					throw new NotSupportedException(type + " is not supported.");

				var classificationDefinitionDao = await _classificationDefinitionRepository.Find(level1Value, type);
				if (classificationDefinitionDao != null)
				{
					HeaderData classificationDataHeader = new HeaderData("Classification Definition", "classification_definition");
					var mappings = new Dictionary<string, string>();
					mappings = MapDescriptions(new List<ClassificationDefinitionDao>() { classificationDefinitionDao }, mappings);
					var classification = ((HeaderData)((dynamic)compositeComponent).classification);

					foreach (var column in classification.Columns)
					{
						var description = "";
						if (column.Value != null && mappings.ContainsKey(column.Value.ToString()))
						{
							description = mappings[column.Value.ToString()];
						}
						var classificationColumnData = new ColumnData(column.Name + " Definition", column.Key + "_definition", description);
						classificationDataHeader.AddColumns(classificationColumnData);
					}

					compositeComponent.Headers.Add(classificationDataHeader);
				}
			}
		}

		/// <summary>
		/// Builds the SFG classification from Service classification.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		/// <returns></returns>
		private async Task<ClassificationDefinitionDao> BuildSfgClassification(CompositeComponent sfg)
		{
			var sfgLevel1Value = ((ColumnData)((dynamic)sfg).classification.sfg_level_1).Value.ToString();
			var serviceClassification = await _classificationDefinitionRepository.Find(sfgLevel1Value,
				Keys.ClassificationKeys.ServiceClassificationKey);

			if (serviceClassification != null)
			{
				Dictionary<string, string> descriptions = new Dictionary<string, string>();
				descriptions = MapDescriptions(new List<ClassificationDefinitionDao>() { serviceClassification },
					descriptions);
				var headerData = ((HeaderData)((dynamic)sfg).classification);
				var classificationValues = headerData.Columns.Where(c => c.Value != null)
					.Select(c => c.Value.ToString());
				var sfgClassificationdata = new Dictionary<string, string>();

				foreach (var classificationValue in descriptions.Where(d => classificationValues.Contains(d.Key)))
				{
					sfgClassificationdata.Add(classificationValue.Key, classificationValue.Value);
				}

				var classificationdataDao =
					await _classificationDefinitionBuilder.BuildDao(Keys.ClassificationKeys.SfgClassificationKey,
						sfgClassificationdata);
				return classificationdataDao;
			}
			return default(ClassificationDefinitionDao);
		}

		/// <summary>
		/// Clones the classification definition from service classification.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		/// <param name="service">The service.</param>
		/// <returns></returns>
		private async Task CloneClassificationDefinition(CompositeComponent sfg, IService service)
		{
			if (sfg != null && sfg.Headers != null)
			{
				ClassificationDefinitionDao classificationdataDao = await BuildSfgClassification(sfg);

				if (classificationdataDao != null)
				{
					try
					{
						await _classificationDefinitionRepository.CreateClassificationDefinition(classificationdataDao);
					}
					catch (DuplicateResourceException ex)
					{
						//If there is a classification definition already, ignore.
					}
					catch (ArgumentException ex)
					{
						//If there is a classification definition at a particular level already, ignore.
					}
				}
			}
		}

		/// <summary>
		/// Converts the descriptions tree to a list of key value pairs.
		/// </summary>
		/// <param name="serviceClassifications">The service classifications.</param>
		/// <param name="descriptions">The descriptions.</param>
		/// <returns></returns>
		private Dictionary<string, string> MapDescriptions(List<ClassificationDefinitionDao> serviceClassifications, Dictionary<string, string> descriptions)
		{
			foreach (var serviceClassification in serviceClassifications)
			{
				descriptions[serviceClassification.Value] =serviceClassification.Description;
				if (serviceClassification.ClassificationDefinitionDaos != null)
				{
					MapDescriptions(serviceClassification.ClassificationDefinitionDaos, descriptions);
				}
			}
			return descriptions;
		}

		/// <inheritdoc/>
		public async Task UpdateRates(string type, CompositeComponent compositeComponent)
		{
			RemoveClassification(compositeComponent);
			var compositeComponentDefinition = await GetCompositeComponentDefinition(type);
			await _compositeComponentRepository.Update(type, compositeComponent, compositeComponentDefinition);
		}

		private void RemoveClassification(CompositeComponent compositeComponent)
		{
			var classificationDefinitionHeader = compositeComponent.Headers.Find(h => h.Key == "classification_definition");
			if (classificationDefinitionHeader != null)
			{
				compositeComponent.Headers.Remove(classificationDefinitionHeader);
			}
		}

		/// <summary>
		/// Gets the count by the specified filter datas.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <param name="filterDatas">The filter datas.</param>
		/// <returns></returns>
		public async Task<long> Count(string type, List<string> searchKeywords, List<FilterData> filterDatas)
		{
		    var compositeComponentDefinition = await GetCompositeComponentDefinition(type);
            var filterCriteria = _filterCriteriaBuilder.Build(searchKeywords, filterDatas, compositeComponentDefinition);
			return await _compositeComponentRepository.Count(type, filterCriteria);
		}

		/// <summary>
		/// Searches by the specified filter datas.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="searchKeywords">The search keywords.</param>
		/// <param name="filterDatas">The filter datas.</param>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="batchSize">Size of the batch.</param>
		/// <returns></returns>
		public async Task<List<CompositeComponent>> Find(string type, List<string> searchKeywords, List<FilterData> filterDatas,
			string sortColumn, SortOrder sortOrder, int pageNumber, int batchSize)
		{
		    var compositeComponentDefinition = await GetCompositeComponentDefinition(type);
            var filterCriteria = _filterCriteriaBuilder.Build(searchKeywords, filterDatas, compositeComponentDefinition);
			return await _compositeComponentRepository.Find(type, filterCriteria, sortColumn, sortOrder, pageNumber, batchSize);
		}


	    /// <inheritdoc />
	    public async Task<long> GetCountOfCompositeComponentsHavingAttachmentColumnDataInGroup(string type, string group,
	        string columnName, List<string> keywords = null)
	    {
	        return await _compositeComponentRepository
	            .GetTotalCountByGroupAndColumnName(type, group, columnName, keywords);
	    }

	    /// <inheritdoc />
	    public async Task<List<CompositeComponent>> GetCompositeComponentsHavingAttachmentColumnDataInGroup(string type, string @group, string columnName,
	        List<string> keywords = null, int pageNumber = -1, int batchSize = -1)
	    {
	        return await _compositeComponentRepository.GetByGroupAndColumnNameAndKeyWords(type, group, columnName,
	            keywords, pageNumber, batchSize);
	    }
	}
}