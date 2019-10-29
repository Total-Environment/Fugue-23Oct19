using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
    /// <summary>
    ///     Represents a repository for static files
    /// </summary>
    public interface IStaticFileRepository
    {
        /// <summary>
        ///     Adds the specified static file.
        /// </summary>
        /// <param name="staticFile">The static file.</param>
        /// <returns></returns>
        Task<StaticFile> Add(StaticFile staticFile);

        /// <summary>
        ///     Gets the by identifier.
        /// </summary>
        /// <param name="staticFileId">The static file identifier.</param>
        /// <returns></returns>
        Task<StaticFile> GetById(string staticFileId);

        /// <summary>
        ///     Finds the name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<StaticFile> FindByName(string name);
    }
}