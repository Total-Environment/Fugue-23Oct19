using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents the column definition.
    /// </summary>
    /// <seealso cref="IColumnDefinition"/>
    public class SimpleColumnDefinition : ISimpleColumnDefinition
    {
        /// <summary>
        /// Not Applicable string
        /// </summary>
        public const string Na = "-- NA --";

        private ISimpleDataType _dataType;
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="isSearchable">if set to <c>true</c> [is searchable].</param>
        /// <param name="isRequired"></param>
        /// <param name="key">Column Key</param>
        public SimpleColumnDefinition(string name, string key, ISimpleDataType dataType, bool isSearchable = false, bool isRequired = false)
        {
            Name = name;
            DataType = dataType;
            IsSearchable = isSearchable;
            IsRequired = isRequired;
            Key = key;
        }

        /// <inheritdoc />
        public ISimpleDataType DataType
        {
            get { return _dataType; }
            set
            {
                if (value == null)
                    throw new ArgumentException("DataType is required.");
                _dataType = value;
            }
        }

        /// <inheritdoc />
        public IColumnDefinition DependentColumn { get; set; }

        /// <inheritdoc />
        public bool IsRequired { get; set; }

        /// <inheritdoc />
        public bool IsSearchable { get; }

        /// <inheritdoc/>
        public string Key { get; set; }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Name is required.");
                _name = value;
            }
        }

        /// <inheritdoc />
        public async Task<IColumnData> Parse(object columnDataValue)
        {
            if (columnDataValue == null)
            {
                if (IsRequired)
                    throw new ArgumentException($"Mandatory field: {Name} cannot be null");
                return new ColumnData(_name, Key, null);
            }
            if (columnDataValue is string && (string)columnDataValue == "" && IsRequired)
                throw new ArgumentException($"Mandatory field: {Name} cannot be blank");
            if (columnDataValue is string && (string)columnDataValue == Na)
            {
                if (IsRequired)
                    throw new ArgumentException($"Mandatory field: {Name} cannot be NA (Not Applicable)");
                return new ColumnData(_name, Key, Na);
            }
            try
            {
                var value = await _dataType.Parse(columnDataValue);
                return new ColumnData(_name, Key, value);
            }
            catch (FormatException ex)
            {
                throw new FormatException($" {Name}: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public bool IsAttachmentType()
        {
            return (DataType is StaticFileDataType) ||
                   (DataType is CheckListDataType) ||
                   ((DataType is ArrayDataType) && ((ArrayDataType)DataType).DataType is StaticFileDataType) ||
                   ((DataType is ArrayDataType) && ((ArrayDataType)DataType).DataType is CheckListDataType);
        }
    }
}