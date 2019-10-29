using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a data type for string.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class StringDataType : ISimpleDataType
    {
        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            if (!(columnData is string))
                throw new FormatException($"Parsing String Data type with value {columnData}.");
            return Task.FromResult(columnData);
        }
    }
}