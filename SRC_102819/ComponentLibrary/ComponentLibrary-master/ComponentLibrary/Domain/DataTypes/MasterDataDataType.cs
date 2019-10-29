using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents master data data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class MasterDataDataType : ISimpleDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataDataType"/> class.
        /// </summary>
        /// <param name="masterDataList">The master data list.</param>
        public MasterDataDataType(IMasterDataList masterDataList)
        {
            DataList = masterDataList;
        }

        /// <summary>
        /// Gets the data list.
        /// </summary>
        /// <value>The data list.</value>
        public IMasterDataList DataList { get; }

        /// <inheritdoc/>
        public Task<object> Parse(object data)
        {
            if (!(data is string))
                throw new FormatException();
            var parsedData = (string)data;
            if (!DataList.HasValueIgnoreCase(parsedData))
                throw new FormatException();
            var value = DataList.ParseIgnoreCase(parsedData);
            return Task.FromResult((object)value.Value);
        }
    }
}