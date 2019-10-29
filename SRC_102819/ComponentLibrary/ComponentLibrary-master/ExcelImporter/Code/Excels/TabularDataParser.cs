using System.Collections.Generic;
using System.Linq;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class TabularDataParser : ITabularDataParser
    {
        private readonly TabularDataLoadConfiguration _tabularDataLoadConfiguration;
        private readonly IExcelReader _excelReader;


        public TabularDataParser(IExcelReader excelReader,
            TabularDataLoadConfiguration tabularDataLoadConfiguration)
        {
            _excelReader = excelReader;
            _tabularDataLoadConfiguration = tabularDataLoadConfiguration;
        }

        public Table Parse(string sheetName)
        {
            var contentRows = _excelReader.GetContiguousRowBlock(sheetName, _tabularDataLoadConfiguration.DataRowIndex,
               _tabularDataLoadConfiguration.NullColumnReference);
            return BuildTableWithContent(contentRows);
        }

        public Table Parse()
        {
            var contentRows = _excelReader.GetContiguousRowBlock(_tabularDataLoadConfiguration.DataRowIndex,
               _tabularDataLoadConfiguration.NullColumnReference);
            return BuildTableWithContent(contentRows);
        }

        public string ParceCellValue(string cellReference)
        {
            return _excelReader.ParseCell(cellReference);
        }

        private Table BuildTableWithContent(IEnumerable<Dictionary<string, ICustomCell>> contentRows)
        {
            var validColumnReferences =
                _tabularDataLoadConfiguration.HeaderConfiguration.Select(h => h.Key);
            var rowCells = new List<TextCell>();
            var rows = new List<Entry>();
            var headerCells = _tabularDataLoadConfiguration.HeaderConfiguration.Select(h => new TextCell(h.Value));
           

            foreach (var row in contentRows)
            {
                foreach (var columnreference in validColumnReferences)
                    rowCells.Add(new TextCell(row[columnreference].Value));
                rows.Add(new Entry(rowCells));
                rowCells.Clear();
            }

            var table = new Table(headerCells, rows);
            return table;
        }
    }
}