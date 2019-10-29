using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComponentCoefficientBuilder
    {
        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="componentCoefficient">The component coefficient.</param>
        /// <returns></returns>
        Task<ComponentCoefficient> BuildData(ComponentCoefficient componentCoefficient);
    }
}