using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class NoTabularDataParser : ITabularDataParser
    {
        public Table Parse(string sheetName)
        {
            return new Table();
        }

        public Table Parse()
        {
            return new Table();
        }

        public string ParceCellValue(string cellReference)
        {
            return string.Empty;
        }
    }
}