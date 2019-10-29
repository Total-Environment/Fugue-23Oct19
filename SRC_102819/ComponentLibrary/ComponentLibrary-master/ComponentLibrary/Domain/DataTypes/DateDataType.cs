using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a date data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class DateDataType : ISimpleDataType
    {
        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            if (columnData is string)
                return Task.FromResult((object)DateTime.Parse((string)columnData));
            if (!(columnData is DateTime))
                throw new FormatException($"Date data type with value {columnData}");
            return Task.FromResult(columnData);
        }
    }
}