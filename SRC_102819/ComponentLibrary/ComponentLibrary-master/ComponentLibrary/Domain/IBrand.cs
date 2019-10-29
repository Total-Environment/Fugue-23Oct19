using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// </summary>
    public interface IBrand
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        IEnumerable<IColumnData> Columns { get; set; }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>The <see cref="System.Object"/>.</value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        object this[string key] { get; }

        /// <summary>
        /// Columns the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        IColumnData Column(string key);

        /// <summary>
        /// Gets or sets the brand definition.
        /// </summary>
        /// <value>The brand definition.</value>
        IBrandDefinition BrandDefinition { get; set; }
    }
}