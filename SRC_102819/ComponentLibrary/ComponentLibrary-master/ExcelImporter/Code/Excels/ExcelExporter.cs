using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using TE.Diagnostics.Logging;
using DomainHeader = TE.ComponentLibrary.ExcelImporter.Code.Excels.Header;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class ExcelExporter
    {
        private readonly IConfiguration _configuration;
        private readonly ExcelReader _excelReader;
        private readonly Logger _logger;

        public ExcelExporter(IConfiguration configuration, string excelPath)
        {
            _configuration = configuration;
            _logger = new Logger(typeof(ExcelExporter));
            _excelReader = new ExcelReader(excelPath);
        }

        public void ExportExcel(string sheetName, IJsonExporter jsonExporter, string endPoint)
        {
            try
            {
                var rows = _excelReader.GetRows(sheetName, _configuration, 5).ToList();
                var startLoggingInfo =
                    $"Started writing {rows.Count} rows for sheet named {sheetName} of excel document {new Configuration().ExcelPath}";
                _logger.Info(startLoggingInfo);
                Console.WriteLine(startLoggingInfo);
                foreach (var row in rows)
                {
                    var startInfo = $"Started exporting row with index {row.RowIndex}";
                    Console.WriteLine(startInfo);
                    try
                    {
                        var columnNameCellValuePairs = _excelReader.ReadRowWithColumnNames(row, sheetName, "3");
                        jsonExporter.ExportJson(columnNameCellValuePairs, endPoint);

                        var endInfo = $"Created : Rox Index {row.RowIndex}";
                        _logger.Info(endInfo);
                        Console.WriteLine(endInfo);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"ERROR : RoxIndex {row.RowIndex} : {ex.StackTrace}. Continuing with next row..");
                    }
                }
                Console.WriteLine("Importing data complete");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
            }
        }

        public List<DomainHeader> GetHeaders(string sheetName, string subHeaderRowNumber)
        {
            var sheet = _excelReader.GetSheet(sheetName);
            var headerMergeCells = _excelReader.GetMergeCells(sheet);
            return headerMergeCells.Select(cell => CreateHeader(cell, sheetName, subHeaderRowNumber)).ToList();
        }

        private DomainHeader CreateHeader(OpenXmlElement headerMergeCell, string sheetName, string subHeaderRowNumber)
        {
            var cellRefs = _excelReader.FetchMergeCellReferences((MergeCell)headerMergeCell);
            var header = new DomainHeader(GetMergeCellName((MergeCell)headerMergeCell, sheetName));

            header.ColumnCells.AddRange(cellRefs.Select(cellRef => CreateColumnWithUnits(sheetName, cellRef, subHeaderRowNumber)).ToList());
            return header;
        }

        private Tuple<ICustomCell, string> CreateColumnWithUnits(string sheetName, string cellRef, string subHeaderRowNumber)
        {
            Tuple<ICustomCell, string> tuple = null;
            tuple = new Tuple<ICustomCell, string>(CreateCustomCellForColumnName(cellRef, sheetName, subHeaderRowNumber),
                GetBelowCellValue(cellRef, sheetName));

            return tuple;
        }

        private ICustomCell CreateCustomCellForColumnName(StringValue cellReference, string sheetName, string subHeaderRowNumber)
        {
            try
            {
                var columnNameCell = _excelReader.GetColumnNameCell(cellReference.Value, sheetName, subHeaderRowNumber);
                if (columnNameCell == null)
                    return null;

                return new CustomCell(columnNameCell, cellReference.Value, _excelReader.SharedStringTable,
                    _excelReader.GetHyperlinks(sheetName), _excelReader.GetHyperlinkRelationships(sheetName));
            }
            catch (Exception)
            {
                Console.WriteLine("Encountered error while reading cell with reference: " + cellReference);
                throw;
            }
        }

        private string GetBelowCellValue(string cellRef, string sheetName)
        {
            try
            {
                var rowNumber = _excelReader.GetRowReference(cellRef) + 1;
                var columnLetter = _excelReader.GetColumnReference(cellRef);

                return _excelReader.ParseCell(columnLetter + rowNumber, sheetName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encountered error while reading cell with reference: " + cellRef);
                _logger.Error("Encountered error while reading cell with reference: " + cellRef + " " + ex.Message);
                throw ex;
            }
        }

        private string GetMergeCellName(MergeCell headerMergeCell, string sheetName)
        {
            var firstCellReference = headerMergeCell.Reference.Value.Split(':').First();
            var firstCell = _excelReader.Cell(sheetName, firstCellReference);
            return
                new CustomCell(firstCell, _excelReader.SharedStringTable, _excelReader.GetHyperlinks(sheetName),
                    _excelReader.GetHyperlinkRelationships(sheetName)).Value;
        }
    }
}