using System;
using System.Collections.Generic;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class Header
    {
        private List<Tuple<ICustomCell, string>> _columns;

        public Header(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public List<Tuple<ICustomCell, string>> ColumnCells
        {
            get { return _columns ?? (_columns = new List<Tuple<ICustomCell, string>>()); }
            set { _columns = value; }
        }
    }
}