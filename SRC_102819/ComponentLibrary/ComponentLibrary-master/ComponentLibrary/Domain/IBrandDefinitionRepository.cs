using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents an interface for brand definition
    /// </summary>
    public interface IBrandDefinitionRepository
    {
        /// <summary>
        ///     Adds the specified material definition.
        /// </summary>
        /// <param name="brandDefinition">The brand definition.</param>
        /// <returns></returns>
        Task Add(BrandDefinition brandDefinition);

        /// <summary>
        /// Finds the definition by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<IBrandDefinition> FindBy(string name);
    }
}