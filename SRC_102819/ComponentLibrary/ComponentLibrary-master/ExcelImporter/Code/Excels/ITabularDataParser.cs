using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public interface ITabularDataParser
    {
        Table Parse(string sheetName);
        Table Parse();
        string ParceCellValue(string cellReference);
    }
}