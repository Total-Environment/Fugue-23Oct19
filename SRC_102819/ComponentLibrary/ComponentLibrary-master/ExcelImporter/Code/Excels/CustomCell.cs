using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class CustomCell : ICustomCell
    {
        private readonly string _cellReference;
        private readonly IEnumerable<HyperlinkRelationship> _hyperlinkRelationships;
        private readonly IEnumerable<Hyperlink> _hyperlinks;
        private readonly SharedStringTable _sharedStringTable;

        public CustomCell(Cell cell, SharedStringTable sharedStringTable, IEnumerable<Hyperlink> hyperlinks,
            IEnumerable<HyperlinkRelationship> hyperlinkRelationships) : this(cell, null, sharedStringTable,
                hyperlinks,hyperlinkRelationships)
        {
           
        }

        public CustomCell(Cell cell, string cellReference, SharedStringTable sharedStringTable,
            IEnumerable<Hyperlink> hyperlinks,
            IEnumerable<HyperlinkRelationship> hyperlinkRelationships)
        {
            _sharedStringTable = sharedStringTable;
            _hyperlinks = hyperlinks;
            Value = CellValue(cell);
            TypeOfData = FetchDataType(cell);
            _cellReference = cellReference ?? cell.CellReference.Value;
            _hyperlinkRelationships = hyperlinkRelationships;
        }

        public TypeOfData TypeOfData { get; }
        public string Value { get; }

        public string GetHyperLink()
        {
            var hyperLink = _hyperlinks.FirstOrDefault(hyperlink => hyperlink.Reference.Value.Equals(_cellReference));
            var hyperlinksRelation = _hyperlinkRelationships.SingleOrDefault(i => i.Id == hyperLink?.Id);
            return hyperlinksRelation?.Uri.ToString();
        }

        private string CellValue(CellType cell)
        {
            if (string.IsNullOrEmpty(cell?.InnerText)) return null;
            var value = cell.CellValue.InnerText;
            switch (cell.DataType?.Value)
            {
                case CellValues.SharedString:
                    return _sharedStringTable.ElementAt(int.Parse(value)).InnerText;
                case CellValues.Boolean:
                    return "0".Equals(value) ? "False" : "True";

                case CellValues.Number:
                case CellValues.Error:
                case CellValues.String:
                case CellValues.InlineString:
                case CellValues.Date:
                case null:
                {
                    try
                    {
                        var number = double.Parse(value);
                        number = Math.Truncate(number * 1000) / 1000;
                        return number.ToString();
                    }
                    catch (Exception ex)
                    {
                        return value;
                    }
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static TypeOfData FetchDataType(Cell cell)
        {
            if (cell == null) return TypeOfData.Null;

            switch (cell.DataType?.Value)
            {
                case CellValues.String:
                case CellValues.InlineString:
                case CellValues.SharedString:
                    return TypeOfData.String;
                case CellValues.Boolean:
                    return TypeOfData.Bool;

                case CellValues.Number:
                    return TypeOfData.Integer;

                case CellValues.Error:

                case CellValues.Date:
                    return TypeOfData.Date;
                case null:
                    return TypeOfData.Null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}