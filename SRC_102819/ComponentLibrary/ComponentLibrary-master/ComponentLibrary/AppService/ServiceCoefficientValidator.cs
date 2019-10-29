using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.ComponentCoefficientValidator"/>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientValidator"/>
    public class ServiceCoefficientValidator : ComponentCoefficientValidator, IComponentCoefficientValidator
    {
        private readonly IServiceRepository _serviceRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCoefficientValidator"/> class.
        /// </summary>
        /// <param name="serviceRepository">The service repository.</param>
        public ServiceCoefficientValidator(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        /// <summary>
        /// Validates the specified service coefficient.
        /// </summary>
        /// <param name="serviceCoefficient">The service coefficient.</param>
        /// <returns></returns>
        public new async Task<Tuple<bool, string>> Validate(ComponentCoefficient serviceCoefficient)
        {
            var result = base.Validate(serviceCoefficient);
            if (result.Item1)
            {
                return await ValidateServiceCode(serviceCoefficient.Code);
            }
            else
            {
                return result;
            }
        }

        private async Task<Tuple<bool, string>> ValidateServiceCode(string serviceCode)
        {
            if (string.IsNullOrWhiteSpace(serviceCode))
                return new Tuple<bool, string>(false, "Service code is mandatory.");

            var count = await _serviceRepository.Count(new Dictionary<string, Tuple<string, object>>
            {
                {"service_code", new Tuple<string, object>("Regex", $"((?i){serviceCode}(?-i))")}
            });
            if (count == 1)
                return new Tuple<bool, string>(true, string.Empty);
            else
                return new Tuple<bool, string>(false, $"Invalid service code {serviceCode}");
        }
    }
}