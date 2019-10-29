using System;
using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Used to validate the material structure and definition
    /// </summary>
    public interface IHeaderColumnDataValidator
    {
        /// <summary>
        /// Validates the specified material definition.
        /// </summary>
        /// <param name="materialDefinition">The material definition.</param>
        /// <param name="headerColumnData">The header data.</param>
        /// <returns></returns>
        Tuple<bool, string> Validate(IEnumerable<IHeaderDefinition> headerColumnDefinition, IEnumerable<IHeaderData> headerColumnData);
    }
}