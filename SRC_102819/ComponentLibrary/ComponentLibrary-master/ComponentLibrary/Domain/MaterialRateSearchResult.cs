using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents the data structure for material rates
    /// </summary>
    public class MaterialRateSearchResult
    {
        /// <summary>
        /// The material name
        /// </summary>
        public string MaterialName { get; private set; }

        /// <summary>
        /// The material rate
        /// </summary>
        public IMaterialRate MaterialRate { get; private set; }

        /// <summary>
        /// Gets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public string ShortDescription { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRateSearchResult" /> class.
        /// </summary>
        /// <param name="materialName">Name of the material.</param>
        /// <param name="materialRate">The material rate.</param>
        /// <param name="shortDescription">The short description.</param>
        public MaterialRateSearchResult(string materialName, IMaterialRate materialRate, string shortDescription)
        {
            MaterialName = materialName;
            MaterialRate = materialRate;
            ShortDescription = shortDescription;
        }
    }
}