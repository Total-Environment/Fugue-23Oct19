using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao
{
    /// <summary>
    ///     Represents the DAO for Dependent Blocks
    /// </summary>
    public class DependentBlockDao
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DependentBlockDao" /> class.
        /// </summary>
        /// <param name="dependentBlock">The dependent block.</param>
        public DependentBlockDao(DependentBlock dependentBlock)
        {
            Data = dependentBlock.Data;
        }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        /// <value>
        ///     The data.
        /// </value>
        public string[] Data { get; set; }

        /// <summary>
        /// Domains this instance.
        /// </summary>
        /// <returns></returns>
        public DependentBlock Domain()
        {
            return new DependentBlock(Data);
        }
    }
}