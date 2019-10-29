using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// The DTO for brand data
    /// </summary>
    public class BrandDataDto
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IEnumerable<ColumnDto> Columns { get; set; }
    }
}