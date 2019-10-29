using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a int data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class IntDataType : ISimpleDataType
    {
        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            try
            {
                return Task.FromResult((object)int.Parse(columnData.ToString()));
            }
            catch (FormatException)
            {
                throw new FormatException($"Expected an Integer. Got {columnData} which is not an integer.");
            }
        }
    }
}