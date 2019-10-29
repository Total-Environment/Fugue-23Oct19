using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for the Material Rate Search Result
    /// </summary>
    public class ServiceRateSearchResultDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRateSearchResultDto"/> class.
        /// </summary>
        /// <param name="serviceRateSearchResult">The material rate search result.</param>
        public async Task<ServiceRateSearchResultDto> WithDomain(ServiceRateSearchResult serviceRateSearchResult)
        {
            ShortDescription = serviceRateSearchResult.ShortDescription;
            ServiceRate = await new ServiceRateDto().SetDomain(serviceRateSearchResult.ServiceRate);
            return this;
        }

        /// <summary>
        /// Gets the material rate.
        /// </summary>
        /// <value>
        /// The material rate.
        /// </value>
        public ServiceRateDto ServiceRate { get; private set; }

        /// <summary>
        /// Gets the name of the material.
        /// </summary>
        /// <value>
        /// The name of the material.
        /// </value>
        public string ShortDescription { get; private set; }
    }
}