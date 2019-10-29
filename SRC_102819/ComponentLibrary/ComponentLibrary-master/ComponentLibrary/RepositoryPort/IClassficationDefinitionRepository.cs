using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// </summary>
    public interface IClassificationDefinitionRepository
    {
        /// <summary>
        ///     Creates the service classification definition.
        /// </summary>
        /// <param name="classificationDefinitionDao">The service classification definition DAO.</param>
        /// <returns></returns>
        Task CreateClassificationDefinition(ClassificationDefinitionDao classificationDefinitionDao);

        /// <summary>
        ///     Finds the specified service level1.
        /// </summary>
        /// <param name="groupLevel">The service level1.</param>
        /// <param name="componentType">Specifies which component type this classification maps to.</param>
        /// <returns></returns>
        Task<ClassificationDefinitionDao> Find(string groupLevel, string componentType);
    }
}