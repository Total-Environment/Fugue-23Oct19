using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// The interface for complex data types
    /// </summary>
    public interface IDataType
    {
        /// <summary>
        /// Parses the specified column data.
        /// </summary>
        /// <param name="columnData">The column data.</param>
        /// <returns></returns>
        Task<object> Parse(object columnData);
    }
}