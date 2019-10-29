using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComponentCoefficientBuilderFactory
    {
        /// <summary>
        /// Gets the component coefficient builder.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns></returns>
        IComponentCoefficientBuilder GetComponentCoefficientBuilder(ComponentType componentType);
    }
}