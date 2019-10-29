using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Constructs a data type.
    /// </summary>
    public interface IDataTypeFactory
    {
        /// <summary>
        /// Constructs the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="subType">Type of the sub.</param>
        /// <returns></returns>
        Task<IDataType> Construct(string dataType, object subType);
    }
}