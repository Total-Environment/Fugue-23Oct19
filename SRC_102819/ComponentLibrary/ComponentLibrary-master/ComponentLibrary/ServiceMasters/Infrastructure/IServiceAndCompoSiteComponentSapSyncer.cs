using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    /// <summary>
    /// The SAP Syncer for Services
    /// </summary>
    public interface IServiceAndCompositeComponentSapSyncer
    {
        /// <summary>
        /// Synchronizes the specified material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="updated">if set to <c>true</c> [updated].</param>
        /// <returns></returns>
        Task<bool> Sync(ServiceAndCompositeComponentRequest material);
    }
}
