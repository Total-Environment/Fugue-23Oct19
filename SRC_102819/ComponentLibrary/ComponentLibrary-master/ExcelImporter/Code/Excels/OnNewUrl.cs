using System.Collections.Generic;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public delegate void OnNewUrl(
        Dictionary<ICustomCell, ICustomCell> keyValuePairs, ICustomCell columnNameCell, ICustomCell dataCell, string documentType);
}