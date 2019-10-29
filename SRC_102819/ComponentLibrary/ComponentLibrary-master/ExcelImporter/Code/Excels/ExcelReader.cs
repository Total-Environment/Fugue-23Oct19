using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class ExcelReader : IDisposable, IExcelReader
    {
        public SharedStringTable SharedStringTable { get; private set; }
        public ImageExtractor ImageExtractor { get; }
        private readonly string _path;
        private SpreadsheetDocument _document;

        public ExcelReader(string path)
        {
            _path = path;
            Initialize();
            ImageExtractor = new ImageExtractor();
        }

        public void Dispose()
        {
            _document.Close();
        }

        public Dictionary<string, ICustomCell> ReadRow(string sheetName, int rowIndex)
        {
            var sheet = GetSheet(sheetName);
            var worksheet = GetWorksheetForSheet(sheet);
            var row = worksheet.Descendants<Row>().SingleOrDefault(r => r.RowIndex == rowIndex);
            return row == default(Row) ? null : ReadRow(row, sheetName);
        }

        public IEnumerable<Dictionary<string, ICustomCell>> GetContiguousRowBlock(string sheetName,
            int initialRowIndex, string nullValueColumnReference)
        {
            var sheet = GetSheet(sheetName);
            return ReadContigousRowBlock(sheet, initialRowIndex, nullValueColumnReference);
        }

        public IEnumerable<Dictionary<string, ICustomCell>> GetContiguousRowBlock(
            int initialRowIndex, string nullValueColumnReference)
        {
            var sheet = GetSheets().FirstOrDefault();
            return ReadContigousRowBlock(sheet, initialRowIndex, nullValueColumnReference);
        }

        public string GetColumnReference(string columnName, string sheetName, int rowIndex)
        {
            var sheet = GetSheet(sheetName);
            if (sheet == null)
                throw new ArgumentException($"Sheets are absent in {_path}");
            var worksheet = GetWorksheetForSheet(sheet);
            var rows =
                worksheet.Descendants<Row>()
                    .Where(r => r.RowIndex == rowIndex)
                    .Select(row => ReadRow(row, sheet.Name));

            foreach (var row in rows)
            {
                foreach (var key in row.Keys)
                {
                    if (row[key].Value == columnName)
                    {
                        return key;
                    }
                }
            }
            return "";
        }

        public string ParseCell(string cellReference)
        {
            var sheet = GetSheets().FirstOrDefault();
            return ParseCell(cellReference, sheet.Name);
        }

        public string ParseCell(string cellReference, string sheetName)
        {
            var cell = Cell(sheetName, cellReference);
            var customCell = new CustomCell(cell, cellReference, SharedStringTable, GetHyperlinks(sheetName),
                GetHyperlinkRelationships(sheetName));
            return (customCell != null && customCell.Value != null) ? customCell.Value : string.Empty;
        }

        public IEnumerable<Row> GetRows(string sheetName, IConfiguration configuration, int rowNumberToStartFrom)
        {
            var sheet = GetSheet(sheetName);
            var worksheet = GetWorksheetForSheet(sheet);
            var rows = (worksheet.Descendants<Row>()
                .Where(row => row.Descendants<Cell>().Any())).ToList();

            return
                rows.Skip(rowNumberToStartFrom - 1)
                    .Where(row => row.Descendants<Cell>().Any(cell => !string.IsNullOrEmpty(cell.InnerText.Trim())));
        }

        private IEnumerable<Dictionary<string, ICustomCell>> ReadContigousRowBlock(Sheet sheet, int initialRowIndex,
            string nullValueColumnReference)
        {
            if (sheet == null)
                throw new ArgumentException($"Sheets are absent in {_path}");
            var worksheet = GetWorksheetForSheet(sheet);
            var rows =
                worksheet.Descendants<Row>()
                    .Where(r => r.RowIndex >= initialRowIndex)
                    .Select(row => ReadRow(row, sheet.Name));

            return
                rows.TakeWhile(
                    d =>
                        d.ContainsKey(nullValueColumnReference) &&
                        !string.IsNullOrWhiteSpace(d[nullValueColumnReference].Value));
        }

        protected virtual void Initialize()
        {
            _document = SpreadsheetDocument.Open(_path, false);
            _document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            _document.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;
            SharedStringTable = _document.WorkbookPart.SharedStringTablePart.SharedStringTable;
        }

        public Cell Cell(string sheetName, string cellLocation)
        {
            var sheet = GetSheet(sheetName);
            var worksheet = GetWorksheetForSheet(sheet);
            return worksheet.Descendants<Cell>().FirstOrDefault(cell => cell.CellReference.Value.Equals(cellLocation));
        }

        public Sheet GetSheet(string sheetName)
        {
            var sheet = GetSheets().FirstOrDefault(s => s.Name == sheetName);
            if (sheet == null)
                throw new FileNotFoundException($"Sheet with \"{sheetName}\" is not found in excel at path \"{_path}\".");
            return sheet;
        }

        private IEnumerable<Sheet> GetSheets()
        {
            var descendants = _document.WorkbookPart.Workbook.Descendants<Sheet>();
            return descendants;
        }

        public string GetCellLink(string sheetName, string cellLocation)
        {
            var hyperLink = GetHyperLink(sheetName, cellLocation);
            return hyperLink.Location;
        }

        private Hyperlink GetHyperLink(string sheetName, string cellLocation)
        {
            var hyperLinks = GetHyperlinks(sheetName);
            var hyperLink = hyperLinks.FirstOrDefault(hl => hl.Reference.Value.Equals(cellLocation));
            return hyperLink;
        }

        public IEnumerable<Hyperlink> GetHyperlinks(string sheetName)
        {
            var sheet = GetSheet(sheetName);
            var worksheetPart = GetWorksheetForSheet(sheet);
            var hyperLinks = worksheetPart.Descendants<Hyperlink>();
            return hyperLinks;
        }

        public WorksheetPart GetWorksheetPart(StringValue id)
        {
            var worksheetPart = _document.WorkbookPart.GetPartById(id) as WorksheetPart;
            return worksheetPart;
        }

        private Worksheet GetWorksheetForSheet(Sheet sheet)
        {
            var relationshipId = sheet.Id.Value;
            var worksheetPart = (WorksheetPart)_document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart.Worksheet;
        }

        public Dictionary<string, ICustomCell> ReadRow(Row row, string sheetName)
        {
            var cellsKeyValuePairs = row.Descendants<Cell>().Select(
                    cell => new
                    {
                        columnName = GetColumnReference(cell.CellReference.Value),
                        cellValue =
                        new CustomCell(cell, SharedStringTable, GetHyperlinks(sheetName),
                            GetHyperlinkRelationships(sheetName))
                    }).Where(kv => kv.columnName != null)
                .ToDictionary(kv => kv.columnName, kv => kv.cellValue as ICustomCell);
            return cellsKeyValuePairs;
        }

        public string GetColumnReference(string cellReference)
        {
            return new string(cellReference.TakeWhile(char.IsLetter).ToArray());
        }

        public int GetRowReference(string cellReference)
        {
            var rowAsString = new string(cellReference.SkipWhile(char.IsLetter).ToArray());
            return int.Parse(rowAsString);
        }

        private int GetBase26FromColumn(string column)
        {
            var len = column.Length;
            var colNum = 0;
            var exponent = 1;
            for (var i = len - 1; i >= 0; i--)
            {
                colNum += exponent * (column[i] - 'A' + 1);
                exponent *= 26;
            }
            return colNum;
        }

        private string GetColumnFromBase26(int index)
        {
            var solution = "";
            while (index > 0)
            {
                var dig = (index - 1) % 26;
                solution = (char)(dig + 'A') + solution;
                if (index % 26 == 0)
                    index -= 26;
                index = index / 26;
            }
            return solution;
        }

        private IEnumerable<string> GetCellRefsFromRange(string sourceRange, int destinationRow)
        {
            var startRegex = new Regex(@"^([A-Z]+)");
            var endRegex = new Regex(@":([A-Z]+)");
            var startIndex = GetBase26FromColumn(startRegex.Match(sourceRange).Groups[1].Value);
            var endIndex = GetBase26FromColumn(endRegex.Match(sourceRange).Groups[1].Value);
            var cellRefs =
                Enumerable.Range(startIndex, endIndex - startIndex + 1)
                    .Select(i => GetColumnFromBase26(i) + destinationRow.ToString());
            return cellRefs;
        }

        public MergeCells GetMergeCells(Sheet sheet)
        {
            var worksheet = GetWorksheetForSheet(sheet);
            var mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault();
            return mergeCells;
        }

        public IEnumerable<string> FetchMergeCellReferences(MergeCell mergeCell)
        {
            var mergeCellRef = mergeCell.Reference.Value;
            var mergeCellStartRef = mergeCellRef.Split(':')[0];
            var row = Regex.Match(mergeCellStartRef, @"(\d+)$").Groups[1].Value;
            return GetCellRefsFromRange(mergeCellRef, int.Parse(row) + 1);
        }

        public IEnumerable<HyperlinkRelationship> GetHyperlinkRelationships(string sheetName)
        {
            var sheet = GetSheet(sheetName);
            var workSheetPart = GetWorksheetPart(sheet.Id);
            return workSheetPart.HyperlinkRelationships;
        }

        public Dictionary<ICustomCell, ICustomCell> ReadRowWithColumnNames(Row row, string sheetName, string subHeaderRowNumber)
        {
            var columnNameDataCells = row.Descendants<Cell>()
                .Select(cell => new
                {
                    columnNameCell = CreateCustomCellForColumnName(cell.CellReference, sheetName, subHeaderRowNumber),
                    dataCell = CreateCustomDataCell(cell, sheetName)
                }).ToList();

            var cellsKeyValuePairs = columnNameDataCells.Where(kv =>
            {
                if (kv.columnNameCell == null) return false;
                return !string.IsNullOrEmpty(kv.columnNameCell.Value);
            });

            var x = cellsKeyValuePairs.ToDictionary(kv => kv.columnNameCell, kv => kv.dataCell);
            return x;
        }

        private ICustomCell CreateCustomCellForColumnName(StringValue cellReference, string sheetName, string subHeaderRowNumber)
        {
            try
            {
                var columnNameCell = GetColumnNameCell(cellReference.Value, sheetName, subHeaderRowNumber);
                if (columnNameCell == null)
                    return null;

                return new CustomCell(columnNameCell, cellReference.Value, SharedStringTable,
                    GetHyperlinks(sheetName), GetHyperlinkRelationships(sheetName));
            }
            catch (Exception)
            {
                Console.WriteLine("Encountered error while reading cell with reference: " + cellReference);
                throw;
            }
        }

        public Cell GetColumnNameCell(string cellReference, string sheetName, string subHeaderRowNumber)
        {
            var subHeaderCellLocation = GetColumnReference(cellReference) + subHeaderRowNumber;
            return Cell(sheetName, subHeaderCellLocation);
        }

        private ICustomCell CreateCustomDataCell(Cell cell, string sheetName)
        {
            return new CustomCell(cell, SharedStringTable, GetHyperlinks(sheetName),
                GetHyperlinkRelationships(sheetName));
        }
    }
}