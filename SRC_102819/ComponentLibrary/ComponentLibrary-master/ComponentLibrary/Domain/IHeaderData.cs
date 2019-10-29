using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents header data.
    /// </summary>
    public interface IHeaderData
    {
        /// <summary>
        ///     Gets or sets the columns.
        /// </summary>
        /// <value>
        ///     The columns.
        /// </value>
        IEnumerable<IColumnData> Columns { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        string Key { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        ///     Gets the <see cref="System.Object" /> with the specified name.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Object" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        object this[string name] { get; }

        /// <summary>
        /// Adds the columns.
        /// </summary>
        /// <param name="columnData">The approved vendors columns.</param>
        void AddColumns(ColumnData columnData);

        /// <summary>
        ///     Return the column with specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IColumnData Column(string name);
    }
}