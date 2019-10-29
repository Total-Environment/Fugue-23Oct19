using System;
using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents the service classification definition
    /// </summary>
    public class ClassificationDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassificationDefinition" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="description">The description.</param>
        /// <param name="serviceClassificationDefinitions">The service classification definitions.</param>
        public ClassificationDefinition(string value, string description,
            List<ClassificationDefinition> serviceClassificationDefinitions)
        {
            Value = value;
            Description = description;
            ServiceClassificationDefinitions = serviceClassificationDefinitions;
        }

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        ///     Gets the service classification definitions.
        /// </summary>
        /// <value>
        ///     The service classification definitions.
        /// </value>
        public List<ClassificationDefinition> ServiceClassificationDefinitions { get; }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value { get; }

        /// <summary>
        ///     Parses the specified service classification definition dto.
        /// </summary>
        /// <param name="serviceClassificationDefinitionDto">The service classification definition dto.</param>
        /// <returns></returns>
        public static ClassificationDefinition Parse(
            Dictionary<string, string> serviceClassificationDefinitionDto)
        {
            if (serviceClassificationDefinitionDto == null)
                throw new ArgumentException(nameof(serviceClassificationDefinitionDto));
            var reversedServiceClassificationDefinitionDto = serviceClassificationDefinitionDto.Reverse();
            ClassificationDefinition serviceClassificationDefinition = null;
            List<ClassificationDefinition> serviceClassificationDefinitions = null;
            foreach (var keyValuePair in reversedServiceClassificationDefinitionDto)
            {
                serviceClassificationDefinition = new ClassificationDefinition(keyValuePair.Key,
                    keyValuePair.Value, serviceClassificationDefinitions);
                serviceClassificationDefinitions = new List<ClassificationDefinition>
                {
                    serviceClassificationDefinition
                };
            }
            return serviceClassificationDefinition;
        }

        /// <summary>
        ///     Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">
        ///     The object to compare with the current object.
        /// </param>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var castObj = obj as ClassificationDefinition;
            return Value == castObj.Value
                   && Description == castObj.Description
                   &&
                   (ServiceClassificationDefinitions == null && castObj.ServiceClassificationDefinitions == null ||
                    ServiceClassificationDefinitions.SequenceEqual(castObj.ServiceClassificationDefinitions));
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>
        ///     A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode()
                   + Description.GetHashCode()
                   + ServiceClassificationDefinitions.GetHashCode();
        }
    }
}