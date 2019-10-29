using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    /// Class ColumnDataDao.
    /// </summary>
    public class ColumnDataDao
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDataDao"/> class.
        /// </summary>
        /// <param name="columnData">The column data.</param>
        public ColumnDataDao(IColumnData columnData)
        {
            ColumnData = columnData;
        }

        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        /// <value>The column data.</value>
        [BsonIgnore]
        public IColumnData ColumnData
        {
            get { return new ColumnData(Name, Key, Value) { Key = Key }; }
            set
            {
                Name = value.Name;
                Value = value.Value;
                Key = value.Key;
            }
        }

        /// <summary>
        /// Gets and Sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
    }
}