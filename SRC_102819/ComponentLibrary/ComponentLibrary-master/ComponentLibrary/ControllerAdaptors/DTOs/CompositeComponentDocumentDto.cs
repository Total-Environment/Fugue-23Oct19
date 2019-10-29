using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class CompositeComponentDocumentDto
    {
        /// <summary>
        ///     Gets the composite component code.
        /// </summary>
        /// <value>
        ///     The composite component code.
        /// </value>
        public string Code { get; set; }

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