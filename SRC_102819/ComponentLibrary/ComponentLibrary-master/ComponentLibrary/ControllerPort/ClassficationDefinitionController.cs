using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// The Service Classification Definition Controller
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("classification-definitions")]
    public class ClassficationDefinitionController : BaseController
    {
        private readonly IClassificationDefinitionBuilder _classificationDefinitionBuilder;
        private readonly IClassificationDefinitionRepository _classificationDefinitionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassficationDefinitionController"/> class.
        /// </summary>
        /// <param name="classificationDefinitionRepository">The service classification definition repository.</param>
        /// <param name="masterDataRepository">The master data repository.</param>
        /// <param name="classificationDefinitionBuilder">Classification definition builder.</param>
        public ClassficationDefinitionController(IClassificationDefinitionRepository classificationDefinitionRepository,
            IClassificationDefinitionBuilder classificationDefinitionBuilder)
        {
            _classificationDefinitionRepository = classificationDefinitionRepository;
            _classificationDefinitionBuilder = classificationDefinitionBuilder;
        }

        /// <summary>
        /// Creates the specified service classification definition dto.
        /// </summary>
        /// <param name="classificationDefinitionDto">The service classification definition dto.</param>
        /// <param name="componentType">Indicates which classification in created.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create classification definition")]
        [Route("")]
        public async Task<IHttpActionResult> Create(Dictionary<string, Dictionary<string, string>> classificationDefinitionDto)
        {
            if (classificationDefinitionDto == null)
            {
                return BadRequest("classificationDefinitionDto is null.");
            }
            var componentType = classificationDefinitionDto.Keys.FirstOrDefault();
            if (string.IsNullOrEmpty(componentType))
            {
                return BadRequest("Component Type not specified for classification definition.");
            }

            var classificationData = classificationDefinitionDto[componentType];
            try
            {
                ClassificationDefinitionDao serviceClassificationDefinitionDao = await _classificationDefinitionBuilder.BuildDao(componentType, classificationData);
                await _classificationDefinitionRepository.CreateClassificationDefinition(serviceClassificationDefinitionDao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicateResourceException)
            {
                return Conflict();
            }

            return Created(string.Empty, classificationDefinitionDto);
        }
    }
}