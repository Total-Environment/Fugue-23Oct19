using System.Collections.Generic;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public interface IExcelReader
    {
        Dictionary<string, ICustomCell> ReadRow(string sheetName, int rowIndex);

        IEnumerable<Dictionary<string, ICustomCell>> GetContiguousRowBlock(int initialRowIndex,
            string nullValueColumnReference);

        IEnumerable<Dictionary<string, ICustomCell>> GetContiguousRowBlock(string sheetName, int initialRowIndex,
            string nullValueColumnReference);

        string ParseCell(string cellReference);
    }
}