using System.Collections.Generic;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    public class Column
    {
        public Column(string name, string key, IDataType dataType)
        {
            Name = name;
            Key = key;
            DataType = dataType;
        }

        public IDataType DataType { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public class Header
    {
        public Header(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public List<Column> Columns { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public void AddColumn(Column column)
        {
            Columns.Add(column);
        }
    }
}