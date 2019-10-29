namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    /// Represents null column data
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.Domain.IColumnData" />
    public class NullColumnData : IColumnData
    {
        /// <inheritdoc />
        public object Value
        {
            get { return null; }
            set {  }
        }

        /// <inheritdoc />
        public string Name { get; set; }
        
        /// <inheritdoc />
        public string Key { get; set; }
    }
}