using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Material Code Generator interface
    /// </summary>
    public interface IMaterialCodeGenerator
    {
        /// <summary>
        /// Generate material code. If code is passed in correct column it is validate and return. 
        /// Else new code is generated and returned.
        /// </summary>
        /// <param name="materialCodePrefix"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        Task<string> Generate(string materialCodePrefix, IMaterial material);
    }
}