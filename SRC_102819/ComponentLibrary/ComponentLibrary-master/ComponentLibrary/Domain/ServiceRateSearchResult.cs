using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents the data structure for material rates
    /// </summary>
    public class ServiceRateSearchResult
    {
        /// <summary>
        /// The material name
        /// </summary>
        public string ShortDescription { get; private set; }

        /// <summary>
        /// The material rate
        /// </summary>
        public IServiceRate ServiceRate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRateSearchResult"/> class.
        /// </summary>
        /// <param name="shortDescription">Name of the material.</param>
        /// <param name="serviceRate">The material rate.</param>
        public ServiceRateSearchResult(string shortDescription, IServiceRate serviceRate)
        {
            ShortDescription = shortDescription;
            ServiceRate = serviceRate;
        }
    }
}