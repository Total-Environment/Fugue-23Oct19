using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Material Builder interface
    /// </summary>
    public interface IMaterialBuilder
    {
        /// <summary>
        /// Builds the material using the passed definition.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="materialDefinition"></param>
        /// <returns></returns>
        Task<IMaterial> BuildAsync(IMaterial material, IMaterialDefinition materialDefinition);
    }
}