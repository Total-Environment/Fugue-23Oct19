using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Generates the counter.
    /// </summary>
    public interface ICounterGenerator
    {
        /// <summary>
        /// Generates the counter with specified code prefix.
        /// </summary>
        /// <param name="codePrefix">The code prefix.</param>
        /// <param name="code">The code.</param>
        /// <param name="counterCollection">The counter collection.</param>
        /// <returns>Generated Code.</returns>
        Task<string> Generate(string codePrefix, string code, string counterCollection);
    }
}