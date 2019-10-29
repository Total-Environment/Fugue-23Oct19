using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <inheritdoc/>
    public class StaticFileRepository : IStaticFileRepository
    {
        private readonly IMongoRepository<StaticFileDao> _mongoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileRepository"/> class.
        /// </summary>
        /// <param name="mongoRepository">The mongo repository.</param>
        public StaticFileRepository(IMongoRepository<StaticFileDao> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        /// <summary>
        /// Adds the specified static file.
        /// </summary>
        /// <param name="staticFile">The static file.</param>
        /// <returns></returns>
        public async Task<StaticFile> Add(StaticFile staticFile)
        {
           var staticFileDao = await _mongoRepository.Add(new StaticFileDao(staticFile));

            return staticFileDao.StaticFile;
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="staticFileId">The static file identifier.</param>
        /// <returns></returns>
        /// <exception cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.ResourceNotFoundException">Static File with " + staticFileId + " is not found</exception>
        public async Task<StaticFile> GetById(string staticFileId)
        {
            var staticFileDao = await _mongoRepository.FindBy("StaticFileId", staticFileId);

            if (staticFileDao == null)
            {
                throw new ResourceNotFoundException("Static File with " + staticFileId + " is not found");
            }

            return staticFileDao.StaticFile;
        }

        /// <summary>
        /// Finds the name of the by.
        /// </summary>
        /// <param name="staticFileName">Name of the static file.</param>
        /// <returns></returns>
        /// <exception cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.ResourceNotFoundException">Static File with " + staticFileName + " is not found</exception>
        public async Task<StaticFile> FindByName(string staticFileName)
        {
            var staticFileDao = await _mongoRepository.FindBy("Name", staticFileName);

            if (staticFileDao == null)
            {
                throw new ResourceNotFoundException("Static File with " + staticFileName + " is not found");
            }

            return staticFileDao.StaticFile;
        }
    }
}