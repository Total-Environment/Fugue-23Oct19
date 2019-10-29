using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComponentCoefficientValidator
    {
        /// <summary>
        /// Validates the specified component coefficient.
        /// </summary>
        /// <param name="componentCoefficient">The component coefficient.</param>
        /// <returns></returns>
        Task<Tuple<bool, string>> Validate(ComponentCoefficient componentCoefficient);
    }
}