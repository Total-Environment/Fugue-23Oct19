using System;
using System.IO;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.Shared.CloudServiceFramework.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <inheritdoc/>
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IStaticFileRepository _staticFileRepository;
        
        /// <summary>
        /// Constructor of DocumentRepository.
        /// </summary>
        /// <param name="blobStorageService"></param>
        /// <param name="staticFileRepository"></param>
        public DocumentRepository(IBlobStorageService blobStorageService, IStaticFileRepository staticFileRepository)
        {
            _blobStorageService = blobStorageService;
            _staticFileRepository = staticFileRepository;
        }

        /// <inheritdoc/>
        public async Task<StaticFile> Upload(string fileName, Stream stream)
        {
            await _blobStorageService.Upload(fileName, stream, "static-files");
            return await _staticFileRepository.Add(new StaticFile(Guid.NewGuid().ToString("N"), fileName));
        }
    }
}