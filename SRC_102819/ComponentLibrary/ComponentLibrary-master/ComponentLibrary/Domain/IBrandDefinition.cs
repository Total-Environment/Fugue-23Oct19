using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// The interface for brand definition
    /// </summary>
    public interface IBrandDefinition
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        List<ISimpleColumnDefinition> Columns { get; }

        /// <summary>
        /// Parses the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        Task<Brand> Parse(IEnumerable<IDictionary<string, object>> dictionary);
    }
}