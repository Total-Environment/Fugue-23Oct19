using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents header defintion.
    /// </summary>
    public interface IHeaderDefinition
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        IEnumerable<IColumnDefinition> Columns { get; set; }

        /// <summary>
        /// Gets or sets the dependency.
        /// </summary>
        /// <value>The dependency.</value>
        IEnumerable<IDependencyDefinition> Dependency { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Parses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        Task<IHeaderData> Parse(IDictionary<string, object> data);

        /// <summary>
        /// Gets the <see cref="IColumnDefinition"/> with the specified name.
        /// </summary>
        /// <value>The <see cref="IColumnDefinition"/>.</value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IColumnDefinition this[string name] { get; }
    }
}