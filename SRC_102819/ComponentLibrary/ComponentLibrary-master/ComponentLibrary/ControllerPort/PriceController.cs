using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController"/>
	[Route("prices")]
	public class PriceController : BaseController
	{
		private readonly IPriceService _priceService;

		/// <summary>
		/// Initializes a new instance of the <see cref="PriceController"/> class.
		/// </summary>
		/// <param name="priceService">The price service.</param>
		public PriceController(IPriceService priceService)
		{
			_priceService = priceService;
		}

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="location">The location.</param>
        /// <param name="appliedOn">The applied on.</param>
        /// <param name="projectCode">The project code.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid component type.</exception>
        [ComponentLibraryAuthorize(Permissions = "Get Price")]
        [HttpGet]
		public async Task<IHttpActionResult> Get(string code, string location, DateTime appliedOn,
			string projectCode = null)
		{
			appliedOn = appliedOn.InIst().Date;
			ComponentPrice price;
			try
			{
				price = await _priceService.GetPrice(code, location, appliedOn, projectCode);
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
			return Ok(price);
		}
	}
}