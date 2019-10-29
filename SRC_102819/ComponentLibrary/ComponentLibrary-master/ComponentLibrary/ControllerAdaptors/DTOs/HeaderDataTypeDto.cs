using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    ///     HeaderDataTypeDto
    /// </summary>
    public class HeaderDataTypeDto
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HeaderDataTypeDto()
        {
        }

        /// <summary>
        ///     Gets and Sets Columns.
        /// </summary>
        public List<ColumnDataTypeDto> Columns { get; set; }

        /// <summary>
        ///     Gets and Sets Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets and Sets Name.
        /// </summary>
        public string Name { get; set; }
    }
}