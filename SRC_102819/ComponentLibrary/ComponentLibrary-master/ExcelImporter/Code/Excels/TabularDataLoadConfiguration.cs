using System.Collections.Generic;

namespace TE.ComponentLibrary.ExcelImporter.Code.Excels
{
    public class TabularDataLoadConfiguration
    {
        public virtual IEnumerable<KeyValuePair<string, string>> HeaderConfiguration { get; set; }
        public virtual string NullColumnReference { get; set; }
        public virtual int DataRowIndex { get; set; }
        public virtual string TitleCellReference { get; set; }
    }
}