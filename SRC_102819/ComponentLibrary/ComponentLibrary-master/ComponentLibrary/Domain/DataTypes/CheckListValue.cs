namespace TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes
{
    /// <summary>
    ///     Represents the check list value.
    /// </summary>
    public class CheckListValue
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckListValue" /> class.
        /// </summary>
        /// <param name="checkListId">The check list identifier.</param>
        public CheckListValue(string checkListId)
        {
            Id = checkListId;
        }

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents its value and type.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Id;
        }
    }
}