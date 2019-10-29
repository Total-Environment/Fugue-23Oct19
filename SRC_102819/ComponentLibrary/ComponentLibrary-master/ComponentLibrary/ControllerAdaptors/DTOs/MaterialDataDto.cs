using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for Material Data
    /// </summary>
    public class MaterialDataDto
    {
        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public IEnumerable<HeaderDto> Headers { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string Group { get; set; }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <returns></returns>
        public IMaterial GetDomain()
        {
            return new Material
            {
                Headers = Headers.Select(h => h.GetDomain())
            };
        }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
    }
}