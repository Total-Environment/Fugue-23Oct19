using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientBuilder"/>
    public class MaterialCoefficientBuilder : IComponentCoefficientBuilder
    {
        private readonly IMaterialRepository _materialRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialCoefficientBuilder"/> class.
        /// </summary>
        /// <param name="materialRepository">The material repository.</param>
        public MaterialCoefficientBuilder(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="componentCoefficient">The component coefficient.</param>
        /// <returns></returns>
        public async Task<ComponentCoefficient> BuildData(ComponentCoefficient componentCoefficient)
        {
            var material = await _materialRepository.Find(componentCoefficient.Code);
            if (material == null)
                throw new ArgumentException($"Invalid material code. {componentCoefficient.Code}");
            componentCoefficient.Name = (string)material["General"]["Material Name"];
            componentCoefficient.UnitOfMeasure = (string)material["General"]["Unit of Measure"];
            return componentCoefficient;
        }
    }
}