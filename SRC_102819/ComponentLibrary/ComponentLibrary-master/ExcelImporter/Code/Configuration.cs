using System.Configuration;
using System.IO;

namespace TE.ComponentLibrary.ExcelImporter.Code
{
    public class Configuration : IConfiguration
    {
        private readonly string _xmlsPath;

        public Configuration()
        {
            _xmlsPath = ConfigurationManager.AppSettings.Get("XmlsPath");
        }

        public string SubHeaderRowNumber => ConfigurationManager.AppSettings.Get("SubHeaderRowNumber") ?? "3";

        public string ExcelPath => ConfigurationManager.AppSettings.Get("ExcelPath");
        public string AzureConnectionString => ConfigurationManager.AppSettings.Get("StaticFileAzureConnectionString");
        public string CheckListBasePath => ConfigurationManager.AppSettings.Get("CheckListBasePath");
        public string CheckListFileExtension => ConfigurationManager.AppSettings.Get("CheckListFileExtension");
        public string RowNumberToStartFrom => ConfigurationManager.AppSettings.Get("RowNumberToStartFrom") ?? "6";
        public string ComponentLibraryBaseUrl => ConfigurationManager.AppSettings.Get("ComponentLibraryBaseUrl");

        public string DocumentLoggingPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings.Get("DocumentLoggingPath") ?? Path.GetTempPath();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public string XmlPathFor(string group)
        {
            return Path.Combine(_xmlsPath, $"{group}.xml");
        }
    }
}