using System.Collections.Generic;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public interface IDocumentLogger
    {
        string GetLogString(ICustomCell columnNameCell, ICustomCell dataCell);
        void Write(string materialId, Dictionary<string, List<string>> urlList);
    }
}