using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComponentCoefficientValidatorFactory
    {
        /// <summary>
        /// Gets the component coefficient validator.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        IComponentCoefficientValidator GetComponentCoefficientValidator(ComponentType componentType);
    }
}