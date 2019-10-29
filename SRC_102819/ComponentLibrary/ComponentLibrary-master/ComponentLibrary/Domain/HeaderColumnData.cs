using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Datastructure for header column mapping.
    /// </summary>
    public class HeaderColumnData
    {
        private List<IHeaderData> _headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderColumnData"/> class.
        /// </summary>
        public HeaderColumnData()
        {
            _headers = new List<IHeaderData>();
        }

        /// <summary>
        /// Gets the header column data.
        /// </summary>
        /// <returns></returns>
        public List<IHeaderData> GetData()
        {
            return _headers;
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="columnKey">The column key.</param>
        /// <param name="columnValue">The column value.</param>
        public void Insert(string key, string columnKey, object columnValue)
        {
            var headerData = _headers.SingleOrDefault(h => h.Key == key);
            if (headerData == null)
            {
                headerData = new HeaderData("", key);

                _headers.Add(headerData);
            }

            var columnData = headerData.Columns.SingleOrDefault(c => c.Key == columnKey);
            if (columnData == null)
            {
                headerData.AddColumns(new ColumnData("", columnKey, columnValue));
            }
            else
            {
                columnData.Value = columnValue;
            }
        }
    }
}