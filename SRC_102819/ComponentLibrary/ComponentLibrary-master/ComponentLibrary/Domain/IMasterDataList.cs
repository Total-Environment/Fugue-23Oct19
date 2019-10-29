namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents an interface for master data list
    /// </summary>
    public interface IMasterDataList
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        string Id { get; set; }

        /// <summary>
        ///     Determines whether the specified data has value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     <c>true</c> if the specified data has value; otherwise, <c>false</c>.
        /// </returns>
        bool HasValueIgnoreCase(string data);

        /// <summary>
        ///     Parses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        MasterDataValue ParseIgnoreCase(object data);
    }
}