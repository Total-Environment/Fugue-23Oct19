using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Builder to create classification definition from data.
    /// </summary>
    public interface IClassificationDefinitionBuilder
    {
        /// <summary>
        /// Builds the Classification definition dao.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <param name="classificationData">The classification data.</param>
        /// <returns></returns>
        Task<ClassificationDefinitionDao> BuildDao(string componentType, Dictionary<string, string> classificationData);
    }
}