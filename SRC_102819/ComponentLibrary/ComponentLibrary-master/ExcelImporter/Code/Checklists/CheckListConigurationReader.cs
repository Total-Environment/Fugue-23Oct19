using System.IO;
using Newtonsoft.Json;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;

namespace TE.ComponentLibrary.ExcelImporter.Code.Checklists
{
    public class CheckListConigurationReader
    {
        public virtual TabularDataLoadConfiguration Read(string configurationFile)
        {
            var configData = File.ReadAllText(configurationFile);
            return JsonConvert.DeserializeObject<TabularDataLoadConfiguration>(configData);
        }
    }
}