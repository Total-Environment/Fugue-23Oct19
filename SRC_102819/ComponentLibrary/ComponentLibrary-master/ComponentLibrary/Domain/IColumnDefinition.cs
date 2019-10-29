using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents a column definition.
    /// </summary>
    public interface IColumnDefinition
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is searchable.
        /// </summary>
        /// <value><c>true</c> if this instance is searchable; otherwise, <c>false</c>.</value>
        bool IsSearchable { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
        bool IsRequired { get; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        IDataType DataType { get; set; }

        /// <summary>
        /// Gets and Sets column key.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Parses the specified column data.
        /// </summary>
        /// <param name="columnDataValue">The column data.</param>
        /// <returns></returns>
        Task<IColumnData> Parse(object columnDataValue);

        /// <summary>
        /// Returns if the column represents an attachment column.
        /// </summary>
        /// <returns></returns>
        bool IsAttachmentType();
    }
}