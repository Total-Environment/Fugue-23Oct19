namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Filter criteria for component filter.
    /// </summary>
    public class FilterData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterData"/> class.
        /// </summary>
        public FilterData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterData"/> class.
        /// </summary>
        /// <param name="columnKey">The column key.</param>
        /// <param name="columnValue">The column value.</param>
        public FilterData(string columnKey, string columnValue)
        {
            ColumnKey = columnKey;
            ColumnValue = columnValue;
        }

        /// <summary>
        /// Gets or sets the column key.
        /// </summary>
        /// <value>The column key.</value>
        public string ColumnKey { get; set; }

        /// <summary>
        /// Gets or sets the column value.
        /// </summary>
        /// <value>The column value.</value>
        public string ColumnValue { get; set; }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        protected bool Equals(FilterData other)
        {
            return string.Equals(ColumnKey, other.ColumnKey) && string.Equals(ColumnValue, other.ColumnValue);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FilterData)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ColumnKey.GetHashCode();
                hashCode = (hashCode * 397) ^ ColumnValue.GetHashCode();
                return hashCode;
            }
        }
    }
}