using System.Collections.Generic;
using System.IO;
using System.Linq;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;
using TE.Diagnostics.Logging;

namespace TE.ComponentLibrary.ExcelImporter.Code.Components
{
    public class ComponentDocumentLogger : IDocumentLogger
    {
        private readonly IConfiguration _configuration;
        private readonly List<Header> _headers;

        public ComponentDocumentLogger(IConfiguration configuration, List<Header> headers)
        {
            _configuration = configuration;
            _headers = headers;
        }

        public string GetLogString(ICustomCell columnNameCell, ICustomCell dataCell)
        {
            var logger = new Logger(typeof(ComponentDocumentLogger));
            var headerName = _headers.FirstOrDefault(h => h.ColumnCells.Select(c => c.Item1.Value).Contains(columnNameCell.Value));
            if (headerName == null)
                logger.Info($"header name is not found for {columnNameCell.Value}");
            string logString;
            var typeOfDocument = "";
            if (typeOfDocument == "checklist")
                logString = $"~{headerName?.Name}: {columnNameCell.Value}% {dataCell.Value}, {dataCell.GetHyperLink()}";
            else
                logString = $"~{headerName?.Name}: {columnNameCell.Value}% {dataCell.Value}";
            return logString;
        }

        public void Write(string materialId, Dictionary<string, List<string>> urlList)
        {
            foreach (var documentType in urlList.Keys)
            {
                var logFileName = documentType + ".txt";
                var path = Path.Combine(_configuration.DocumentLoggingPath, logFileName);
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