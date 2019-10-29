using System;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Interface to validate the dependency definition associated with materials
    /// </summary>
    public interface IDependencyValidator
    {
        /// <summary>
        /// Validates the specified dependency definition.
        /// </summary>
        /// <param name="dependencyDefinition">The dependency definition.</param>
        /// <param name="headerData">The header data.</param>
        /// <returns></returns>
        Tuple<bool,string> Validate(IDependencyDefinition dependencyDefinition, IHeaderData headerData);
    }
}