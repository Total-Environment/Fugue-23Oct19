using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors
{
    /// <summary>
    /// The Adapter for Brand Documents
    /// </summary>
    public class BrandDocumentDtoAdaptor
    {
        public static BrandDocumentDto FromBrand(Dictionary<string, object> brand)
        {
            return new BrandDocumentDto
            {
                MaterialCode = (string)brand["material_code"],
                BrandCode = (string)brand["brand_code"],
                ManufacturersName = (string)brand["manufacturers_name"],
                DocumentDtos = GetDocumentDtos((object[])brand["files"])
            };
        }

        private static List<DocumentDto> GetDocumentDtos(object[] fileObjects)
        {
            var urlRoot = ConfigurationManager.AppSettings["CdnBaseUrl"];
            return (from Dictionary<string, object> file in fileObjects
                    where file.Count > 0
                    select new DocumentDto
                    {
                        Id = (string) file["id"],
                        Name = (string) file["name"],
                        Url = $"{urlRoot}/static-files/{(string) file["name"]}"
                    }).ToList();
        }
    }
}