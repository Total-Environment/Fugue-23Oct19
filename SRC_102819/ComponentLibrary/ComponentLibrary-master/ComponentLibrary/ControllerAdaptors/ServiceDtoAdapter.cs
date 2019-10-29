using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    /// ServiceDto Adapter.
    /// </summary>
    public static class ServiceDtoAdapter
    {
        /// <summary>
        /// Transform from service obj -> service dto
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static ComponentDataDto<Service> FromService(IService service)
        {
            return new ComponentDataDto<Service>()
            {
                Id = service.Id,
                Group = service.Group,
                Headers = service.Headers?.Select(h => new HeaderDto()
                {
                    Name = h.Name,
                    Key = h.Key,
                    Columns = h.Columns.Select(c => new ColumnDto()
                    {
                        Name = c.Name,
                        Key = c.Key,
                        Value = c.Value
                    })
                })
            };
        }
        /// <summary>
        /// Converts Service DTO to Service
        /// </summary>
        /// <param name="serviceDto"></param>
        /// <returns></returns>
        public static IService ToService(ComponentDataDto<Service> serviceDto)
        {
            return new Service
            {
                Group = serviceDto.Group,
                Id = serviceDto.Id,
                Headers = serviceDto.Headers.Select(h => h.GetDomain())
            };
        }
    }
}