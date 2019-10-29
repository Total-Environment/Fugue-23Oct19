using Newtonsoft.Json;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// ColumnDataTypeDto
    /// </summary>
    public class ColumnDataTypeDto
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnDataTypeDto()
        {
        }

        /// <summary>
        /// Gets and Sets DataType.
        /// </summary>
        public DataTypeDto DataType { get; set; }

        /// <summary>
        /// Gets and Sets Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets and Sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets and Sets Value.
        /// </summary>
        [JsonConverter(typeof(ColumnValueConverter))]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is searchable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is searchable; otherwise, <c>false</c>.
        /// </value>
        public bool IsSearchable { get; set; }
    }
}