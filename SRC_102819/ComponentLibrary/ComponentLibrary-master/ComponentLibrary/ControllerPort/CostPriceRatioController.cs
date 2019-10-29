using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController"/>
	[RoutePrefix("costpriceratios")]
	public class CostPriceRatioController : BaseController
	{
		private readonly ICostPriceRatioService _cprService;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioController"/> class.
		/// </summary>
		/// <param name="cprService">The CPR service.</param>
		public CostPriceRatioController(ICostPriceRatioService cprService)
		{
			this._cprService = cprService;
		}

        /// <summary>
        /// Creates the cost price ratio.
        /// </summary>
        /// <param name="costPriceRatioDto">The cost price ratio dto.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Setup CRP Version")]
        [Route("")]
		[HttpPost]
		public async Task<IHttpActionResult> CreateCostPriceRatio(CostPriceRatioDto costPriceRatioDto)
		{
			try
			{
				var costPriceRatioTobeCreated = costPriceRatioDto.ToDomain();
				var costPriceRatio = await _cprService.Create(costPriceRatioTobeCreated);
				return Created("", new CostPriceRatioDto(costPriceRatio));
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (FormatException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (NotImplementedException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (NotSupportedException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
		}

        /// <summary>
        /// ///
        /// </summary>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View CPR values")]
        [Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCostPriceRatioList(DateTime appliedOn, string componentType, string projectCode)
		{
			ClaimsPrincipal cp = ClaimsPrincipal.Current;

			try
			{
				appliedOn = appliedOn.InIst().Date;
				ComponentType componentTypeEnum;
				if (!Enum.TryParse(componentType, true, out componentTypeEnum))
				{
					throw new ArgumentException("Invalid component type.");
				}
				CostPriceRatioList costPriceRatios = await _cprService.GetCostPriceRatioList(appliedOn, componentTypeEnum, projectCode);
				return Ok(new CostPriceListDto(costPriceRatios));
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ResourceNotFoundException e)
			{
				LogToElmah(e);
				return NotFound();
			}
			catch (InvalidOperationException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (ArgumentException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (FormatException ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
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
        [ComponentLibraryAuthorize(Permissions = "View CPR values")]
        [Route("")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCostPriceRatio(DateTime appliedOn, ComponentType componentType, string level1,
			string level2, string level3, string code, string projectCode = null)
		{
			try
			{
				appliedOn = appliedOn.InIst().Date;
				var costPriceRatio = await _cprService.GetCostPriceRatio(appliedOn, componentType, level1, level2, level3, code, projectCode);
				var costPriceRatioDto = new CostPriceRatioDto(costPriceRatio);
				return Ok(costPriceRatioDto);
			}
			catch (NotSupportedException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			catch (NotImplementedException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			catch (ArgumentException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			catch (ResourceNotFoundException exception)
			{
				LogToElmah(exception);
				return BadRequest(exception.Message);
			}
			catch (Exception exception)
			{
				LogToElmah(exception);
				return InternalServerError(exception);
			}
		}
	}
}