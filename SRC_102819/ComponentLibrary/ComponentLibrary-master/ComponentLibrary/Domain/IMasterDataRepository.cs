using System.Collections.Generic;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Interface IMasterDataRepository
    /// </summary>
    public interface IMasterDataRepository
    {
        /// <summary>
        ///     Finds the specified list name.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <returns>Task&lt;MasterDataList&gt;.</returns>
        Task<MasterDataList> Find(string listName);

        /// <summary>
        ///     Adds the specified master data list.
        /// </summary>
        /// <param name="masterDataList">The master data list.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> Add(MasterDataList masterDataList);

        /// <summary>
        ///     Finds this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;IMasterDataList&gt;.</returns>
        IEnumerable<IMasterDataList> Find();

        /// <summary>
        ///     Patches the specified master data list.
        /// </summary>
        /// <param name="masterDataList">The master data list.</param>
        /// <returns>Task&lt;MasterDataList&gt;.</returns>
        Task<MasterDataList> Patch(MasterDataList masterDataList);

        /// <summary>
        ///     Existses the specified master data list name.
        /// </summary>
        /// <param name="masterDataListName">Name of the master data list.</param>
        /// <param name="masterDataValue">The master data value.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> Exists(string masterDataListName, string masterDataValue);

        /// <summary>
        ///     Finds master data by Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<MasterDataList> FindByName(string name);
    }
}