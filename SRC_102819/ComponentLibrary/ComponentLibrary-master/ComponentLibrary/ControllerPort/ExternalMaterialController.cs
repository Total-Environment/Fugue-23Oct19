using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// External controller that handles all material related endpoints.
    /// </summary>
    [RoutePrefix("external/materials")]
    public class ExternalMaterialController : BaseController
    {
        private readonly int _defaultBatchSize;
        private readonly IMaterialService _materialService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalMaterialController"/> class.
        /// </summary>
        /// <param name="materialService">The material service.</param>
        public ExternalMaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
            _defaultBatchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
        }


        /// <summary>
        ///     Searches by the specified material level2 and the search key word.
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="pageNumber"></param>
        /// <param name="materialLevel2"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Search(string searchKeyword, int pageNumber, string materialLevel2 = "",
            string sortColumn = "", SortOrder sortOrder = SortOrder.Ascending)
        {

            if (searchKeyword == null)
            {
                LogToElmah(new ArgumentException("Search Keywords is not specified."));
                return BadRequest("Search Keywords is not specified.");
            }
            
            List<IMaterial> materials;
            long count;
            try
            {
                var searchKeywords = FetchKeywords(searchKeyword);
                if (string.IsNullOrEmpty(materialLevel2))
                    materials = await _materialService.Search(searchKeywords, pageNumber,
                        _defaultBatchSize);
                else
                    materials = await _materialService.Search(searchKeywords, materialLevel2, pageNumber,
                        _defaultBatchSize, sortColumn, sortOrder);
                count = await _materialService.Count(searchKeywords,
                    materialLevel2);
            }

            catch (ResourceNotFoundException e)
            {
                LogToElmah(e);
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                LogToElmah(ex);
                return BadRequest(ex.Message);
            }
            catch (FormatException e)
            {
                LogToElmah(e);
                return BadRequest(e.Message);
            }

            return Ok(new ListDto<MaterialDataTypeDto>
            {
                BatchSize = _defaultBatchSize,
                TotalPages = count % _defaultBatchSize == 0 ? count / _defaultBatchSize : count / _defaultBatchSize + 1,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                RecordCount = count,
                Items = materials.Select(MaterialDataTypeDtoAdaptor.FromMaterial).ToList()
            });
        }

        /// <summary>
        /// Fetches the keywords.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// Search keyword should be minimum 3 letter long. or Atleast one of the search keyword
        /// should be more than 3 letter long.
        /// </exception>
        private List<string> FetchKeywords(string searchQuery)
        {
            if ((searchQuery == null) || (searchQuery.Length < 3))
                throw new ArgumentException("Search keyword should be atleast 3 letters long.");
            var keywords = searchQuery.Split(' ').Where(k => k.Length > 2).ToList();
            if (!keywords.Any())
                throw new ArgumentException("Atleast one of the search keyword should be more than 3 letters long.");
            return keywords;
        }
    }
}