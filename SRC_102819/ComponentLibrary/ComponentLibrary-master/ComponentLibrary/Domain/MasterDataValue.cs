using System;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a master data value.
    /// </summary>
    public class MasterDataValue
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MasterDataValue" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException">Value Cannot be null.</exception>
        public MasterDataValue(string value)
        {
            if (value == null)
                throw new ArgumentException("Value Cannot be null for Master data.");
            Value = value;
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value { get; set; }
    }
}