using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <inheritdoc/>
    public class ClassificationDefinitionBuilder : IClassificationDefinitionBuilder
    {
        private readonly IMasterDataRepository _masterDataRepository;
        private string _componentKey = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationDefinitionBuilder"/> class.
        /// </summary>
        /// <param name="masterDataRepository">The master data repository.</param>
        public ClassificationDefinitionBuilder(IMasterDataRepository masterDataRepository)
        {
            _masterDataRepository = masterDataRepository;
        }

        /// <inheritdoc/>
        public async Task<ClassificationDefinitionDao> BuildDao(string componentType, Dictionary<string, string> classificationData)
        {
            if (componentType.Equals(Keys.ClassificationKeys.ServiceClassificationKey))
                _componentKey = Keys.ClassificationKeys.ServiceClassificationColumPrefix;
            else if (componentType.Equals(Keys.ClassificationKeys.SfgClassificationKey))
                _componentKey = Keys.ClassificationKeys.SfgClassificationColumPrefix;
            else
                _componentKey = Keys.ClassificationKeys.PackageClassificationColumPrefix;
            
            await Validate(classificationData);
            var serviceClassificationDefinition = ClassificationDefinition.Parse(classificationData);
            var serviceClassificationDefinitionDao = new ClassificationDefinitionDao(serviceClassificationDefinition, componentType);
            return serviceClassificationDefinitionDao;
        }

        /// <summary>
        /// Validates the specified classification definition dto.
        /// </summary>
        /// <param name="classificationDefinitionDto">The classification definition dto.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// classificationDefinitionDto
        /// or
        /// classificationDefinitionDto
        /// or
        /// classificationDefinitionDto
        /// or
        /// </exception>
        private async Task Validate(Dictionary<string, string> classificationDefinitionDto)
        {
            if (classificationDefinitionDto == null)
                throw new ArgumentException(nameof(classificationDefinitionDto) + " is null.");

            if (classificationDefinitionDto.Count == 0)
                throw new ArgumentException(nameof(classificationDefinitionDto) + " is not containing any key/value pairs.");

            if (classificationDefinitionDto.Count > 7)
                throw new ArgumentException(nameof(classificationDefinitionDto) + " is containing more than 7 key/value pairs.");

            var i = 1;
            foreach (var key in classificationDefinitionDto.Keys)
            {
                var exists = await _masterDataRepository.Exists($"{_componentKey}_{i}", key);
                if (!exists)
                    throw new ArgumentException(key + $" is not found in master data list of {_componentKey}_{i}.");
                i++;
            }
        }
    }
}