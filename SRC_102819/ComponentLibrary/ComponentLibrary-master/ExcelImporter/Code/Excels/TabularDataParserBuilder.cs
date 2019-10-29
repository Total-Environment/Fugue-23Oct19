using System.IO;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class TabularDataParserBuilder
    {
        public virtual ITabularDataParser BuildParserForCheckList(string checkListPath, TabularDataLoadConfiguration checkListConfiguration)
        {
            if (string.IsNullOrWhiteSpace(checkListPath))
                return new NoTabularDataParser();

            var excelReader = new ExcelReader(checkListPath);
            return new TabularDataParser(excelReader, checkListConfiguration);
        }

        public virtual ITabularDataParser BuildParserForExchangeRate(string exchangeRateWorkBookPath)
        {
            string configurationFile = $"Data\\Rates\\Configurations\\Exchange_Rate.json";
            var configData = File.ReadAllText(configurationFile);
            var exchangeRateConfiguration = JsonConvert.DeserializeObject<TabularDataLoadConfiguration>(configData);

            var excelReader = new ExcelReader(exchangeRateWorkBookPath);
            return new TabularDataParser(excelReader, exchangeRateConfiguration);
        }

        public virtual ITabularDataParser BuildParserForUniversalMaterialRateMaster(string universalMaterialRateMasterWorkBookPath)
        {
            string configurationFile = $"Data\\Rates\\Configurations\\Universal_Material_Rate_Master.json";
            var configData = File.ReadAllText(configurationFile);
            var universalMaterialRateMasterConfiguration = JsonConvert.DeserializeObject<TabularDataLoadConfiguration>(configData);

            var excelReader = new ExcelReader(universalMaterialRateMasterWorkBookPath);
            return new TabularDataParser(excelReader, universalMaterialRateMasterConfiguration);
        }

        public virtual ITabularDataParser BuildParserForUniversalServiceRateMaster(string universalServiceRateMasterWorkBookPath)
        {
            string configurationFile = $"Data\\Rates\\Configurations\\Universal_Service_Rate_Master.json";
            var configData = File.ReadAllText(configurationFile);
            var universalServiceRateMasterConfiguration = JsonConvert.DeserializeObject<TabularDataLoadConfiguration>(configData);

            var excelReader = new ExcelReader(universalServiceRateMasterWorkBookPath);
            return new TabularDataParser(excelReader, universalServiceRateMasterConfiguration);
        }
    }
}