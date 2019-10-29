using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents an array data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class ArrayDataType : ISimpleDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDataType"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <exception cref="ArgumentNullException">dataType</exception>
        public ArrayDataType(IDataType dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));
            DataType = dataType;
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public IDataType DataType { get; }

        /// <inheritdoc/>
        public async Task<object> Parse(object columnData)
        {
            if (!(columnData is IEnumerable<object>))
                throw new FormatException($"Array data type with value {columnData}");
            var objects =
                (await Task.WhenAll(((IEnumerable<object>)columnData).ToArray().Select(o => DataType.Parse(o)))).ToArray();
            return objects;
        }
    }
}