using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a boolean data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class BooleanDataType : ISimpleDataType
    {
        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            try
            {
                if (columnData is string)
                {
                    var result = bool.Parse((string)columnData);
                    return Task.FromResult((object)result);
                }
            }
            catch (FormatException e)
            {
                throw new FormatException($"Boolean data type with value '{columnData}'", e);
            }
            if (!(columnData is bool))
                throw new FormatException($"Boolean data type with value {columnData}");
            return Task.FromResult(columnData);
        }
    }
}