using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Used to generate the brand code
    /// </summary>
    public interface IBrandCodeGenerator
    {
        /// <summary>
        /// Generates the specified brand code prefix.
        /// </summary>
        /// <param name="brandCodePrefix">The brand code prefix.</param>
        /// <returns></returns>
        Task<string> Generate(string brandCodePrefix);
    }
}