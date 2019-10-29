using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents an interface for material definition.
    /// </summary>
    public interface IComponentDefinition
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        string Code { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        List<IHeaderDefinition> Headers { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="IHeaderDefinition"/> with the specified name.
        /// </summary>
        /// <value>The <see cref="IHeaderDefinition"/>.</value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IHeaderDefinition this[string name] { get; }

        /// <summary>
        /// Parses the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="brandDefinition">The brand definition.</param>
        /// <returns></returns>
        Task<T> Parse<T>(IDictionary<string, object> data, IBrandDefinition brandDefinition) where T : IComponent, new();

        /// <summary>
        /// Parses the component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>Component</returns>
        IComponent ParseComponent(IComponent component);
    }
}