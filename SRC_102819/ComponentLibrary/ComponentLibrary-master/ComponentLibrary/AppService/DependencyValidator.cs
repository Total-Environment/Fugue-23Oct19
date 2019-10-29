using System;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents a dependency validatpr
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IDependencyValidator" />
    public class DependencyValidator : IDependencyValidator
    {
        /// <inheritdoc/>
        public Tuple<bool,string> Validate(IDependencyDefinition dependencyDefinition, IHeaderData headerData)
        {
            if(dependencyDefinition == null)
                throw new ArgumentException("Dependency definition is null");
            if(headerData == null)
                throw new ArgumentException("Header data is null.");
            return new Tuple<bool, string>(dependencyDefinition.Validate(headerData.Columns.Select(c => (string)c.Value).ToList()),
                $"Invalid column dependency for header {headerData.Name}");
        }
    }
}