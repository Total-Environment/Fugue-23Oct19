using System.IO;
using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{

    /// <summary>
    /// The repository for documents
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Uploads the specified file by its name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        Task<StaticFile> Upload(string fileName, Stream stream);
    }
}