using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// Interface for SAP Syncing
    /// </summary>
    public interface ISapSyncer
    {
        /// <summary>
        /// Syncs the material
        /// </summary>
        /// <param name="material"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        Task<bool> Sync(IMaterial material, string create);
    }
}