using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// </summary>
    public class MaterialDocumentDto
    {
        /// <summary>
        ///     Gets the material code.
        /// </summary>
        /// <value>
        ///     The material code.
        /// </value>
        public string MaterialCode { get; set; }

        /// <summary>
        ///     Gets the short description.
        /// </summary>
        /// <value>
        ///     The short description.
        /// </value>
        public string ShortDescription { get; set; }

        /// <summary>
        ///     Gets the document dtos.
        /// </summary>
        /// <value>
        ///     The document dtos.
        /// </value>
        public List<DocumentDto> DocumentDtos { get; set; }
    }
}