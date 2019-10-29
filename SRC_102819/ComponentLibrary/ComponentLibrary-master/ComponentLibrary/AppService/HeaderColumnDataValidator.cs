using System;
using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents a material validator
    /// </summary>
    public class HeaderColumnDataValidator : IHeaderColumnDataValidator
    {
        private readonly IDependencyValidator _depenedencyValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderColumnDataValidator"/> class.
        /// </summary>
        /// <param name="depenedencyValidator">The depenedency validator.</param>
        public HeaderColumnDataValidator(IDependencyValidator depenedencyValidator)
        {
            _depenedencyValidator = depenedencyValidator;
        }

        /// <inheritdoc/>
        public Tuple<bool, string> Validate(IEnumerable<IHeaderDefinition> headerColumnDefinition, IEnumerable<IHeaderData> headerColumnData)
        {
            var headerErrorMessage = ValidateHeaders(headerColumnDefinition, headerColumnData);

            if (headerErrorMessage.IsNotEmpty())
            {
                return new Tuple<bool, string>(false, headerErrorMessage);
            }
            foreach (var materialHeader in headerColumnData)
            {
                var headerDefinition = HeaderDefinition(headerColumnDefinition, materialHeader);
                var columnErrorMessage = ValidateColumns(headerDefinition, materialHeader);
                if (columnErrorMessage.IsNotEmpty())
                {
                    return new Tuple<bool, string>(false, columnErrorMessage);
                }
                if (headerDefinition.Dependency == null) continue;
                foreach (var dependencyDefinition in headerDefinition.Dependency)
                {
                    var validate = _depenedencyValidator.Validate(dependencyDefinition, materialHeader);
                    if (!validate.Item1)
                    {
                        return validate;
                    }
                }
            }
            return new Tuple<bool, string>(true, null);
        }

        private static IHeaderDefinition HeaderDefinition(IEnumerable<IHeaderDefinition> headerColumnDefinition, IHeaderData headerColumnData)
        {
            return headerColumnDefinition.FirstOrDefault(h => h.Key == headerColumnData.Key);
        }

        private static string ValidateColumns(IHeaderDefinition headerDefinition, IHeaderData header)
        {
            var invalidColumnKeys = header.Columns.Select(column => column.Key).Except(headerDefinition.Columns.Select(c => c.Key))
                .ToList();
            return !invalidColumnKeys.Any() ? null : $"Column(s) not found, keys did not match with definition: { string.Join(", ", invalidColumnKeys)}";
        }

        private static string ValidateHeaders(IEnumerable<IHeaderDefinition> headerColumnDefinition, IEnumerable<IHeaderData> headerColumnData)
        {
            var invalidHeaderKeys = headerColumnData.Select(header => header.Key).Except(headerColumnDefinition.Select(header => header.Key))
                .ToList();
            return !invalidHeaderKeys.Any() ? null : $"Header(s) not found, keys did not match with definition: {string.Join(", ", invalidHeaderKeys)}";
        }
    }
}