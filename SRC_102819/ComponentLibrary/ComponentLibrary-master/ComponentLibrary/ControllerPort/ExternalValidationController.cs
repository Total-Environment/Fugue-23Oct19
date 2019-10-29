using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("external/validate")]
    public class ExternalValidationController : BaseController
    {
        private readonly IValidationService _validationService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationService"></param>
        public ExternalValidationController(IValidationService validationService)
        {
            _validationService = validationService;
        }

        /// <summary>
        /// Gets the specified search object.
        /// </summary>
        /// <param name="searchObject">The search object.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Invalid search request
        /// or
        /// Invalid search request - Missing Assembly Codes
        /// </exception>
        [ComponentLibraryAuthorize(Permissions = "Validate Assembly Code")]
        [HttpPost]
        [Route("assembly-code")]
        public async Task<IHttpActionResult> Get([FromBody] JObject searchObject)
        {
            try
            {
                if (searchObject == null)
                {
                    throw new ArgumentException("Invalid search request");
                }
                var assemblyCodesTokens = searchObject.GetValue("assemblyCodes");

                if (assemblyCodesTokens == null)
                {
                    throw new ArgumentException("Invalid search request - Missing Assembly Codes");
                }

                var assemblyCodes = assemblyCodesTokens.Select(c => c.ToString().Trim()).ToList();

                var materialCodes =
                    await _validationService.ValidateAssemblyCodes(assemblyCodes, "material");

                var notFoundCodes = assemblyCodes.Except(materialCodes).ToList();

                var serviceCodes = await _validationService.ValidateAssemblyCodes(notFoundCodes.Distinct().ToList(), "service");

                notFoundCodes = notFoundCodes.Except(serviceCodes).ToList();

                var sfgCodes = await _validationService.ValidateAssemblyCodes(notFoundCodes.Distinct().ToList(), "sfg");
                
                notFoundCodes = notFoundCodes.Except(sfgCodes).ToList();

                var packageCodes = await _validationService.ValidateAssemblyCodes(notFoundCodes.Distinct().ToList(), "packages");

                notFoundCodes = notFoundCodes.Except(packageCodes).ToList();



                return Ok(new
                {
                    assemblyCodes = new
                    {
                        notFound = notFoundCodes.Distinct()
                    }
                });
            }
            catch (Exception ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}