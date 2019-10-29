using System;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.BaseController"/>
	[RoutePrefix("{type:validcompositecomponent}-mapping")]
	public class CompositeComponentMappingController : BaseController
	{
		private readonly ICompositeComponentMappingRepository _compositeComponentMappingRepository;
		private readonly ICodePrefixTypeMappingRepository _codePrefixTypeMappingRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentMappingController"/> class.
		/// </summary>
		/// <param name="compositeComponentMappingRepository">The composite component mapping repository.</param>
		/// <param name="codePrefixTypeMappingRepository">The code prefix type mapping repository.</param>
		public CompositeComponentMappingController(ICompositeComponentMappingRepository compositeComponentMappingRepository,
			ICodePrefixTypeMappingRepository codePrefixTypeMappingRepository)
		{
			_compositeComponentMappingRepository = compositeComponentMappingRepository;
			_codePrefixTypeMappingRepository = codePrefixTypeMappingRepository;
		}

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="mappingData">The mapping data.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Create Composite Component Mapping")]
        [HttpPost]
		[Route("")]
		public async Task<IHttpActionResult> Create(string type, CompositeComponentMapping mappingData)
		{
			try
			{
				await _compositeComponentMappingRepository.Save(type, mappingData);
				foreach (var groupCodeMapping in mappingData.GroupCodeMapping)
				{
					await _codePrefixTypeMappingRepository.Add(new CodePrefixTypeMapping(groupCodeMapping.Value,
						string.Equals(type, "package", StringComparison.InvariantCultureIgnoreCase)
							? ComponentType.Package
							: ComponentType.SFG));
				}
			}
			catch (DuplicateResourceException ex)
			{
				LogToElmah(ex);
				return Conflict();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return InternalServerError(ex);
			}
			return Ok();
		}
	}
}