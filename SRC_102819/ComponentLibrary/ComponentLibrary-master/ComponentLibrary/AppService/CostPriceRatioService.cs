using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ICostPriceRatioService"/>
	public class CostPriceRatioService : ICostPriceRatioService
	{
		private const string CostPriceRatioDefinition = "CostPriceRatioDefinition";
		private readonly ICostPriceRatioRepository _costPriceRatioRepository;
		private readonly ICostPriceRatioDefinitionRepository _costPriceRatioDefinitionRepository;
		private readonly ICostPriceRatioValidatorFactory _costPriceRatioValidatorFactory;
		private readonly IColumnDataValidator _columnDataValidator;
		private readonly IColumnDataBuilder _columnDataBuilder;
	    private readonly ICostPriceRatioBuilderFactory _costPriceRatioBuilderFactory;
	    private readonly ICostPriceRatioFilterFactory _costPriceRatioFilterFactory;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="CostPriceRatioService"/> class.
	    /// </summary>
	    /// <param name="costPriceRatioDefinitionRepository">The cost price ratio definition repository.</param>
	    /// <param name="costPriceRatioRepository">The cost price ratio repository.</param>
	    /// <param name="costPriceRatioValidatorFactory">The cost price ratio validator factory.</param>
	    /// <param name="columnDataValidator">The column data validator.</param>
	    /// <param name="columnDataBuilder">The column data builder.</param>
	    /// <param name="costPriceRatioBuilderFactory"></param>
	    /// <param name="costPriceRatioFilterFactory"></param>
	    public CostPriceRatioService(ICostPriceRatioDefinitionRepository costPriceRatioDefinitionRepository,
			ICostPriceRatioRepository costPriceRatioRepository, ICostPriceRatioValidatorFactory costPriceRatioValidatorFactory,
			IColumnDataValidator columnDataValidator, IColumnDataBuilder columnDataBuilder, 
            ICostPriceRatioBuilderFactory costPriceRatioBuilderFactory, 
            ICostPriceRatioFilterFactory costPriceRatioFilterFactory)
		{
			_costPriceRatioRepository = costPriceRatioRepository;
			_costPriceRatioValidatorFactory = costPriceRatioValidatorFactory;
			_columnDataValidator = columnDataValidator;
			_columnDataBuilder = columnDataBuilder;
		    _costPriceRatioBuilderFactory = costPriceRatioBuilderFactory;
		    _costPriceRatioFilterFactory = costPriceRatioFilterFactory;
		    _costPriceRatioDefinitionRepository = costPriceRatioDefinitionRepository;
		}

		/// <summary>
		/// Creates the specified cost price ratio.
		/// </summary>
		/// <param name="costPriceRatioData">The cost price ratio.</param>
		/// <returns></returns>
		public async Task<CostPriceRatio> Create(CostPriceRatio costPriceRatioData)
		{
			var costPriceRatioValidator =
				_costPriceRatioValidatorFactory.GetCostPriceRatioValidator(costPriceRatioData.ComponentType);
			var validationResult = await costPriceRatioValidator.Validate(costPriceRatioData);
			if (!validationResult.Item1)
				throw new ArgumentException($"Invalid cost price ratio data. {validationResult.Item2}");

			var costPriceRatioDefinition = await _costPriceRatioDefinitionRepository.FindBy(CostPriceRatioDefinition);
			var coefficientValidationResult = _columnDataValidator.Validate(costPriceRatioDefinition.Columns, costPriceRatioData.CprCoefficient.Columns);
			if (!coefficientValidationResult.Item1)
				throw new ArgumentException($"Invalid cost price ratio coefficients. {coefficientValidationResult.Item2}");

			var columns = await _columnDataBuilder.BuildData(costPriceRatioDefinition.Columns, costPriceRatioData.CprCoefficient.Columns);
			var appliedFrom = AdjustTimeComponenttoMidnightIst(costPriceRatioData.AppliedFrom);
			var costPriceRatio = new CostPriceRatio(new CPRCoefficient(columns), appliedFrom,
					costPriceRatioData.ComponentType, costPriceRatioData.Level1,
					costPriceRatioData.Level2,
					costPriceRatioData.Level3, costPriceRatioData.Code,
					costPriceRatioData.ProjectCode);

			await _costPriceRatioRepository.Create(costPriceRatio);
			return costPriceRatio;
		}

	    /// <summary>
	    /// </summary>
	    /// <param name="appliedOn"></param>
	    /// <param name="componentType"></param>
	    /// <param name="projectCode"></param>
	    /// <returns></returns>
	    public async Task<CostPriceRatioList> GetCostPriceRatioList(DateTime appliedOn, ComponentType componentType, string projectCode)
	    {
	        var costPriceRatioBuilder = _costPriceRatioBuilderFactory.GetCostPriceRatioBuilder(componentType);
	        var costPriceRatioDefinition = await _costPriceRatioDefinitionRepository.FindBy(CostPriceRatioDefinition);
            var costPriceRatioListFromRepo =  await  _costPriceRatioRepository.GetCostPriceRatioList(appliedOn, componentType, costPriceRatioDefinition);
            var costPriceRatioList = new CostPriceRatioList();
	        foreach (var costPriceRatio in costPriceRatioListFromRepo.costPriceRatios)
	        {
	            costPriceRatioList.Add(await costPriceRatioBuilder.Build(costPriceRatio));
	        }
	        var costPriceRatioFilter = _costPriceRatioFilterFactory.GetCostPriceRatioFilter(componentType);
	        return costPriceRatioFilter.Filter(costPriceRatioList, projectCode);
	    }

        private DateTime AdjustTimeComponenttoMidnightIst(DateTime appliedFrom)
		{
			return appliedFrom.Add(appliedFrom.AdditionalTimeSinceMidnightIst());
		}

		/// <summary>
		/// Gets the cost price ratio.
		/// </summary>
		/// <param name="appliedOn">The applied on.</param>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="level1">The level1.</param>
		/// <param name="level2">The level2.</param>
		/// <param name="level3">The level3.</param>
		/// <param name="code">The code.</param>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		public async Task<CostPriceRatio> GetCostPriceRatio(DateTime appliedOn, ComponentType componentType, string level1,
			string level2, string level3, string code, string projectCode)
		{
			var costPriceRatioDefinition = await _costPriceRatioDefinitionRepository.FindBy(CostPriceRatioDefinition);
			CostPriceRatio costPriceRatio = await _costPriceRatioRepository.GetCostPriceRato(appliedOn, componentType, level1,
				level2, level3, code, projectCode, costPriceRatioDefinition);
			return costPriceRatio;
		}
	}
}
