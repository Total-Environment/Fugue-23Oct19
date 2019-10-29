using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    /// Represents a unit data type.
    /// </summary>
    /// <seealso cref="IDataType"/>
    public class UnitDataType : ISimpleDataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDataType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public UnitDataType(string type)
        {
            Value = type;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; }

        /// <inheritdoc/>
        public Task<object> Parse(object columnData)
        {
            if (!(columnData is Dictionary<string, object>)) throw new ArgumentException("Invalid format while parsing Unit Data Type.");
            var columnDictionary = (Dictionary<string, object>)columnData;
            string unit;
            string columnValue;
            if (columnDictionary.ContainsKey("Type"))
                unit = (string)columnDictionary["Type"];
            else if (columnDictionary.ContainsKey("type"))
                unit = (string)columnDictionary["type"];
            else
                throw new FormatException("Unit is not present.");
            if (columnDictionary.ContainsKey("Value"))
                columnValue = columnDictionary["Value"].ToString();
            else if (columnDictionary.ContainsKey("value"))
                columnValue = columnDictionary["value"].ToString();
            else
                throw new FormatException("Value is not present.");
            if (unit != Value) throw new FormatException($"Expected {Value}, got {unit}");
            return Task.FromResult((object)new UnitValue(double.Parse(columnValue), unit));
        }
    }
}