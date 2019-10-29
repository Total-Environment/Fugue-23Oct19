using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ExcelImporter.Code.MasterData
{
    public class MasterDataJson
    {
        public string Key { get; set; }
        public List<string> Values { get; set; }

        public MasterDataListDto RequestObject()
        {
            return new MasterDataListDto(new MasterDataList(Key, Values.Select(v => new MasterDataValue(v)).ToList()));
        }
    }
}