using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController"/>
	[RoutePrefix("costpriceratio-definitions")]
	public class CostPriceRatioDefinitionController : BaseController
	{
		private const string CostPriceRatioDefinition = "CostPriceRatioDefinition";
		private readonly ICostPriceRatioDefinitionRepository _costPriceRatioDefinitionRepository;
		private readonly ISimpleDataTypeFactory _dataTypeFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDefinitionController"/> class.
		/// </summary>
		/// <param name="costPriceRatioDefinitionRepository">The cost price ratio definition repository.</param>
		/// <param name="dataTypeFactory">The data type factory.</param>
		public CostPriceRatioDefinitionController(ICostPriceRatioDefinitionRepository costPriceRatioDefinitionRepository,
			ISimpleDataTypeFactory dataTypeFactory)
		{
			_costPriceRatioDefinitionRepository = costPriceRatioDefinitionRepository;
			_dataTypeFactory = dataTypeFactory;
		}

        /// <summary>
        /// Adds the specified costPriceRatio definition DTO
        /// </summary>
        /// <param name="costPriceRatioDefinitionDto">The costPriceRatio definition DTO.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create CPR Definition")]
        [Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> Add(CostPriceRatioDefinitionDto costPriceRatioDefinitionDto)
		{
			try
			{
				var costPriceRatioDefinition = await costPriceRatioDefinitionDto.GetDomain(_dataTypeFactory);
				await _costPriceRatioDefinitionRepository.Add(costPriceRatioDefinition);

				var costPriceRatio = await _costPriceRatioDefinitionRepository.FindBy(costPriceRatioDefinitionDto.Name);
				return Created("", new CostPriceRatioDefinitionDto(costPriceRatio));
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return Conflict();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}

        /// <summary>
        /// Gets the costPriceRatio definition
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [ComponentLibraryAuthorize(Permissions = "View CPR Definition")]
        [Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> Get()
		{
			try
			{
				var costPriceRatio = await _costPriceRatioDefinitionRepository.FindBy(CostPriceRatioDefinition);
				return Ok(new CostPriceRatioDefinitionDto(costPriceRatio));
			}
			catch (ResourceNotFoundException ex)
			{
				LogToElmah(ex);
				return NotFound();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return BadRequest();
			}
		}
	}
}