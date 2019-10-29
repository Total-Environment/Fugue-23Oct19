using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Controller.Dto
{
    /// <summary>
    /// Class ServiceDto.
    /// </summary>
    /// <seealso cref="IJsonSerializable"/>
    public class ServiceDto
    {
        private readonly IService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDto"/> class.
        /// </summary>
        /// <param name="service">The service data.</param>
        public ServiceDto(IService service)
        {
            _service = service;
            Headers =
                service.Headers.Select(
                    h =>
                        new HeaderDto
                        {
                            Key = h.Key,
                            Name = h.Name,
                            Columns = h.Columns.Select(c => new ColumnDto { Key = c.Key, Name = c.Name, Value = c.Value })
                        });
            Group = _service.Group;
            Id = _service.Id;
        }

        public ServiceDto() { }

        public IEnumerable<HeaderDto> Headers { get; set; }
        public string Id { get; set; }
        public string Group { get; set; }
    }
}