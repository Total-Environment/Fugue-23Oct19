using System.Collections.Generic;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// MaterialDataTypeDto
    /// </summary>
    public class MaterialDataTypeDto
    {
        /// <summary>
        /// Gets and Sets Headers
        /// </summary>
        public List<HeaderDataTypeDto> Headers { get; set; }

        /// <summary>
        /// Gets and Sets Group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets and Sets Id.
        /// </summary>
        public string Id { get; set; }
    }
}