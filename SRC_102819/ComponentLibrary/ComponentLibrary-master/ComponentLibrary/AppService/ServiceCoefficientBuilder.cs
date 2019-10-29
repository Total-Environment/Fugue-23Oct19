using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientBuilder"/>
    public class ServiceCoefficientBuilder : IComponentCoefficientBuilder
    {
        private readonly IServiceRepository _serviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCoefficientBuilder"/> class.
        /// </summary>
        /// <param name="serviceRepository">The service repository.</param>
        public ServiceCoefficientBuilder(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="componentCoefficient">The component coefficient.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public async Task<ComponentCoefficient> BuildData(ComponentCoefficient componentCoefficient)
        {
            var service = await _serviceRepository.Find(componentCoefficient.Code);
            if (service == null)
                throw new ArgumentException($"Invalid service code. {componentCoefficient.Code}");
            componentCoefficient.Name = (string)service["General"]["Short Description"];
            componentCoefficient.UnitOfMeasure = (string)service["General"]["Unit of Measure"];
            return componentCoefficient;
        }
    }
}