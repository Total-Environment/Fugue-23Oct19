using System;
using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents master data list.
    /// </summary>
    /// <seealso cref="IMasterDataList" />
    public class MasterDataList : IMasterDataList
    {
        private readonly List<MasterDataValue> _values;
        private string _id;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MasterDataList" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="masterDataValues">The master data values.</param>
        /// <exception cref="System.ArgumentException">Name cannot be null or whitespace.</exception>
        public MasterDataList(string name, List<MasterDataValue> masterDataValues = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.");
            Name = name;
            _values = masterDataValues ?? new List<MasterDataValue>();
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<MasterDataValue> Values => _values.AsEnumerable();

        /// <inheritdoc />
        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Id Cannot be null or Whitespace.");
                _id = value;
            }
        }

        /// <inheritdoc />
        public bool HasValueIgnoreCase(string data)
        {
            return _values.Any(v => string.Equals(v.Value, data, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <inheritdoc />
        public MasterDataValue ParseIgnoreCase(object data)
        {
            if (!(data is string))
                throw new FormatException();
            try
            {
                return
                    _values.First(v => string.Equals(v.Value, (string) data, StringComparison.CurrentCultureIgnoreCase));
            }
            catch (InvalidOperationException)
            {
                throw new FormatException();
            }
        }
    }
}