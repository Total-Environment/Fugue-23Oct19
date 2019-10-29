using System.Collections.Generic;
using System.IO;
using TE.ComponentLibrary.ExcelImporter.Code.Documents;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;

namespace TE.ComponentLibrary.ExcelImporter.Code.Brands
{
    public class BrandDocumentLogger : IDocumentLogger
    {
        private readonly List<List<StaticFileInformation>> _importedFiles;
        private IConfiguration _configuration;

        public BrandDocumentLogger(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetLogString(ICustomCell columnNameCell, ICustomCell dataCell)
        {
            return $"~: {columnNameCell.Value}% {dataCell.Value}";
        }

        public void Write(string materialId, Dictionary<string, List<string>> urlList)
        {
            foreach (var documentType in urlList.Keys)
            {
                var logFileName = documentType + ".txt";
                var path = Path.Combine(_configuration.DocumentLoggingPath, "brand_" + logFileName);
                var urlDataWithMaterialId = materialId;
                foreach (var logString in urlList[documentType])
                    urlDataWithMaterialId += logString;

                using (var log = new StreamWriter(path, true))
                {
                    log.WriteLine(urlDataWithMaterialId);
                }
            }
        }
    }
}