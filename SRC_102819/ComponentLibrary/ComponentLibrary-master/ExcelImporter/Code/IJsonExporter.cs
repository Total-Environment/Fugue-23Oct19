using System.Collections.Generic;
using TE.ComponentLibrary.ExcelImporter.Code.Excels;

namespace TE.ComponentLibrary.ExcelImporter.Code
{
    public interface IJsonExporter
    {
        string ExportJson(Dictionary<ICustomCell, ICustomCell> columnNameCellValuePairs, string endPoint);
    }
}