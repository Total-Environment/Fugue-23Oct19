using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
	/// <summary>
	/// </summary>
	[RoutePrefix("external/component")]
	public class ExternalComponentController : BaseController
	{
		private readonly ICodePrefixTypeMappingRepository _codePrefixTypeMappingRepository;
		private readonly ICompositeComponentService _compositeComponentService;
		private readonly IMaterialService _materialService;
		private readonly IComponentRepository _componentRepository;
		private const string RfaFileLinkKey = "rfa_file_link";
		private const string JsonFileLinkKey = "json_file_link";
		private const string RevitFamilyTypeKey = "revit_family_type";
		private const string EDesignHeaderKey = "edesign_specifications";

		/// <summary>
		/// </summary>
		/// <param name="codePrefixTypeMappingRepository"></param>
		/// <param name="compositeComponentService"></param>
		/// <param name="materialService"></param>
		/// <param name="componentRepository"></param>
		public ExternalComponentController(ICodePrefixTypeMappingRepository codePrefixTypeMappingRepository,
			ICompositeComponentService compositeComponentService,
			IMaterialService materialService,
			IComponentRepository componentRepository)
		{
			_codePrefixTypeMappingRepository = codePrefixTypeMappingRepository;
			_compositeComponentService = compositeComponentService;
			_materialService = materialService;
			_componentRepository = componentRepository;
		}

		private async Task<CodePrefixTypeMapping> GetCodeType(string id)
		{
			if (String.IsNullOrEmpty(id) || id.Length < 3)
			{
				throw new ArgumentException("Invalid Assembly Code");
			}
			var codePrefix = id.Substring(0, 3);
			var codeType = await _codePrefixTypeMappingRepository.Get(codePrefix);
			if (codeType == null)
			{
				throw new ArgumentException("Invalid Assembly Code");
			}
			return codeType;
		}

		private static Func<IHeaderData, IHeaderData> UpdateRfaFunc(string rfaFileLink, string jsonFileLink, string revitFamilyType)
		{
			return header =>
			{
				if (header.Key == EDesignHeaderKey)
				{
					header.Columns = header.Columns.Select(col =>
					{
						switch (col.Key)
						{
							case RfaFileLinkKey:
								col.Value = rfaFileLink;
								break;

							case JsonFileLinkKey:
								col.Value = jsonFileLink;
								break;

							case RevitFamilyTypeKey:
								col.Value = revitFamilyType;
								break;

							default:
								break;
						}
						return col;
					}).ToList();
				}
				return header;
			};
		}

        /// <summary>
        /// ` Get replacement for a particular component
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get Component Replacements")]
        [Route("{id}/replacements")]
		[HttpPost]
		public async Task<IHttpActionResult> GetReplacement(string id, JObject searchQuery)
		{
			try
			{
				if (searchQuery == null)
				{
					throw new Exception("Search query cannot be empty");
				}
				var codeType = await GetCodeType(id);
				if (codeType.ComponentType == ComponentType.Material)
				{
					return Ok(((List<IMaterial>)
							await _componentRepository.FindReplacements(codeType.ComponentType, searchQuery))
						.Select(MaterialDtoAdapter.FromMaterial).ToList());
				}
				if (codeType.ComponentType == ComponentType.SFG || codeType.ComponentType == ComponentType.Package)
				{
					return Ok(((List<CompositeComponent>)await _componentRepository.FindReplacements(
						codeType.ComponentType, searchQuery)).Select(c => new CompositeComponentDto(c)));
				}
				return Ok();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
		}

        /// <summary>
        /// Update RFA details for a component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rfaDetails"></param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Update RFA")]
        [Route("rfa/{id}")]
		[HttpPut]
		public async Task<IHttpActionResult> UpdateRfa(string id, JObject rfaDetails)
		{
			try
			{
				if (rfaDetails == null)
				{
					throw new ArgumentException("Missing RFA Details");
				}
				var codeType = await GetCodeType(id);
				var rfaFileLink = rfaDetails.GetValue("rfaFileLink")?.ToString();
				var jsonFileLink = rfaDetails.GetValue("jsonFileLink")?.ToString();
				var revitFamilyType = rfaDetails.GetValue("revitFamilyType")?.ToString();
				if (!(await _componentRepository.UpdateRfaDetails(codeType.ComponentType, id, rfaFileLink, jsonFileLink,
					revitFamilyType)))
				{
					throw new Exception("Failed to update RFA details");
				}
				return Ok();
			}
			catch (Exception ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
		}

        /// <summary>
        /// Fetch a component by assembly code.
        /// </summary>
        /// <param name="id">Assembly Code</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "View Component")]
        [Route("{id}")]
		[HttpGet]
		public async Task<IHttpActionResult> GetComponent(string id)
		{
			try
			{
				var codeType = await GetCodeType(id.Trim());

				if (codeType.ComponentType == ComponentType.Material)
				{
					return Ok(MaterialDtoAdapter.FromMaterial(await _materialService.Find(id.Trim())));
				}
				if (codeType.ComponentType == ComponentType.Package)
				{
					return Ok(new CompositeComponentDto(await _compositeComponentService.Get("package", id.Trim())));
				}
                if (codeType.ComponentType == ComponentType.SFG)
                {
                    return Ok(new CompositeComponentDto(await _compositeComponentService.Get("sfg", id.Trim())));
                }
                throw new Exception($"Invalid Component Code - {id}");
			}
			catch (ResourceNotFoundException resourceNotFoundException)
			{
			    LogToElmah(resourceNotFoundException);
			    return NotFound();
			}
            catch (Exception ex)
			{
				LogToElmah(ex);
				return BadRequest(ex.Message);
			}
		}
	}
}