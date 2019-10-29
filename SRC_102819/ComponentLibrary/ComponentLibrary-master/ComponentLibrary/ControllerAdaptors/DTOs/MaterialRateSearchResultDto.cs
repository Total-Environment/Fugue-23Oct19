using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for the Material Rate Search Result
    /// </summary>
    public class MaterialRateSearchResultDto
    {
        public async Task<MaterialRateSearchResultDto> WithDomain(MaterialRateSearchResult materialRateSearchResult)
        {
            MaterialName = materialRateSearchResult.MaterialName;
            MaterialRate = await new MaterialRateDto().SetDomain(materialRateSearchResult.MaterialRate);
            ShortDescription = materialRateSearchResult.ShortDescription;
            return this;
        }

        /// <summary>
        /// Gets the material rate.
        /// </summary>
        /// <value>
        /// The material rate.
        /// </value>
        public MaterialRateDto MaterialRate { get; set; }

        /// <summary>
        /// Gets the name of the material.
        /// </summary>
        /// <value>
        /// The name of the material.
        /// </value>
        public string MaterialName { get; private set; }

        /// <summary>
        /// Gets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public string ShortDescription { get; private set; }
    }
}