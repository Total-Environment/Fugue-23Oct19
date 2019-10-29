using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a checklist repository.
    /// </summary>
    public interface ICheckListRepository
    {
        /// <summary>
        ///     Adds the specified check list.
        /// </summary>
        /// <param name="checkList">The check list.</param>
        /// <returns></returns>
        Task<CheckList> Add(CheckList checkList);

        /// <summary>
        ///     Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<CheckList> GetById(string id);
    }
}