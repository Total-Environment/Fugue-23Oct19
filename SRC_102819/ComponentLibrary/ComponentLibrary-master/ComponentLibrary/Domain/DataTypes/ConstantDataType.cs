using System;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a data type for constant.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class ConstantDataType : ISimpleDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantDataType"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConstantDataType(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; }

        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            if (columnData as string != Value)
                throw new FormatException();
            return Task.FromResult(columnData);
        }
    }
}