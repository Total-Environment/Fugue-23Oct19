using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Class HeaderDataDao.
    /// </summary>
    public class HeaderDataDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderDataDao"/> class.
        /// </summary>
        /// <param name="headerData">The header data DAO.</param>
        public HeaderDataDao(IHeaderData headerData)
        {
            HeaderData = headerData;
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<ColumnDataDao> Columns { get; set; }

        /// <summary>
        /// Gets or sets the header data.
        /// </summary>
        /// <value>The header data.</value>
        [BsonIgnore]
        public IHeaderData HeaderData
        {
            get { return new HeaderData(Name, Key) { Columns = Columns.Select(c => c.ColumnData) }; }
            set
            {
                Name = value.Name;
                Columns = value.Columns.Select(c => new ColumnDataDao(c)).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Object.</returns>
        public object this[string columnName]
        {
            get { return Columns.FirstOrDefault(c => c.Name == columnName)?.Value; }
        }
    }
}