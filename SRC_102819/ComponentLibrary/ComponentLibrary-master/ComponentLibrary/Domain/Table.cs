using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a table with headers and rows.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Table
    {
        /// <summary>
        /// The data
        /// </summary>
        [JsonProperty("Entries")] public IEnumerable<Entry> _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        /// <param name="headerCells">The header cells.</param>
        /// <param name="entries">The entries.</param>
        /// <exception cref="System.ArgumentException">
        ///     Table cannot be empty.
        ///     or
        ///     Entry size cannot exceed header size.
        /// </exception>
        public Table(IEnumerable<TextCell> headerCells, IEnumerable<Entry> entries)
        {
            if (headerCells == null || entries == null || headerCells.Count() == 0 || entries.Count() == 0)
                throw new ArgumentException("Table cannot be empty.");
            var headerSize = headerCells.Count();
            if (entries.Max(e => e.Count) > headerSize)
                throw new ArgumentException("Entry size cannot exceed header size.");

            _data = new List<Entry> {new Entry(headerCells)};
            _data = _data.Concat(entries);
        }

        /// <summary>
        ///     Contents this instance.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Entry> Content()
        {
            return _data.Skip(1);
        }

        /// <summary>
        ///     Headers this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Table is empty</exception>
        public Entry Header()
        {
            if (!_data.Any())
                throw new InvalidOperationException("Table is empty");
            return _data.First();
        }
    }
}