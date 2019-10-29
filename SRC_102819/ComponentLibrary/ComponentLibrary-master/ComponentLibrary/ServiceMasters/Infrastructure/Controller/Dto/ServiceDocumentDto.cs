using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// The DTO for Service Documents
    /// </summary>
    public class ServiceDocumentDto
    {
        private readonly Service _service;
        private readonly string _headerName;
        private readonly string _columnName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDocumentDto"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="columnName">Name of the column.</param>
        public ServiceDocumentDto(Service service, string headerName, string columnName)
        {
            _service = service;
            _headerName = headerName;
            _columnName = columnName;
        }

        /// <summary>
        /// Gets the service code.
        /// </summary>
        /// <value>
        /// The service code.
        /// </value>
        public string ServiceCode => Convert.ToString(_service["General"]["Service Code"]);
        
        /// <summary>
        /// Gets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public string ShortDescription => Convert.ToString(_service["General"]["Short Description"]);

        /// <summary>
        /// Gets the document dtos.
        /// </summary>
        /// <value>
        /// The document dtos.
        /// </value>
        public List<DocumentDto> DocumentDtos
        {
            get
            {
                var documentDtos = new List<DocumentDto>();
                var columnValue = _service[_headerName][_columnName];
                if (columnValue.GetType() == typeof(object[]))
                {
                    documentDtos.AddRange(
                        ((object[])columnValue).Select(GetDocumentDto).Where(documentDto => documentDto != null));
                }
                else
                {
                    var documentDto = GetDocumentDto(columnValue);
                    if (documentDto != null) documentDtos.Add(documentDto);
                }
                return documentDtos;
            }
        }

        private DocumentDto GetDocumentDto(object item)
        {
            if (item.GetType() == typeof(StaticFile))
            {
                var uiRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
                var documentDto = new DocumentDto
                {
                    Id = ((StaticFile)item).Id,
                    Name = ((StaticFile)item).Name,
                    Url = $"{uiRoot}/static-files/{((StaticFile)item).Name}"
                };
                return documentDto;
            }
            if (item.GetType() == typeof(CheckListValue))
            {
                var uiRoot = ConfigurationManager.AppSettings["ComponentLibraryUIRoot"];
                var documentDto = new DocumentDto
                {
                    Id = ((CheckListValue)item).Id,
                    Name = $"/check-lists/{((CheckListValue)item).Id}",
                    Url = $"{uiRoot}/check-lists/{((CheckListValue)item).Id}"
                };
                return documentDto;
            }
            if (item is string)
            {
                var documentDto = new DocumentDto
                {
                    Id = (string)item,
                    Name = (string)item,
                    Url = (string)item
                };
                return documentDto;
            }
            //throw new NotSupportedException(item.GetType() + " is not supported.");
            return null;
        }
    }
}