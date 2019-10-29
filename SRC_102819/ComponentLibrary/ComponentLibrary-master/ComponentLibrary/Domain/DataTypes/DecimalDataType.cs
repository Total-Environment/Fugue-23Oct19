using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a decimal data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class DecimalDataType : ISimpleDataType
    {
        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            try
            {
                return Task.FromResult((object)decimal.Parse(columnData.ToString()));
            }
            catch (FormatException)
            {
                throw new FormatException($"Expected an Decimal. Got {columnData} which is not an Decimal.");
            }
        }
    }
}