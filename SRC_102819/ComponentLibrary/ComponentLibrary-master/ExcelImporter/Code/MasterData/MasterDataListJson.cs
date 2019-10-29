using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.ExcelImporter.Code.MasterData
{
    public class MasterDataListJson
    {
        public IList<MasterDataJson> List { get; set; }

        public IEnumerable<MasterDataListDto> RequestList()
        {
            return List.Select(e => e.RequestObject()).ToList();
        }
    }
}