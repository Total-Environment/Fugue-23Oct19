using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <inheritdoc/>
    public class MasterDataRepository : IMasterDataRepository
    {
        private readonly IMongoCollection<MasterDataListDao> _mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        public MasterDataRepository(IMongoCollection<MasterDataListDao> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        /// <inheritdoc/>
        public async Task<string> Add(MasterDataList masterList)
        {
            if ((await _mongoCollection.FindAsync(l => l.Name == masterList.Name)).FirstOrDefault() != null)
            {
                throw new DuplicateResourceException($"{masterList.Name} already exists.");
            }
            var masterDataListDao = new MasterDataListDao(masterList);
            await _mongoCollection.InsertOneAsync(masterDataListDao);
            return masterDataListDao.ObjectId.ToString();
        }

        /// <inheritdoc/>
        public IEnumerable<IMasterDataList> Find()
        {
            var listAsync = _mongoCollection.AsQueryable().ToList();

            return listAsync.Select(e => e.MasterDataList);
        }

        /// <inheritdoc/>
        public async Task<MasterDataList> Patch(MasterDataList masterDataList)
        {
            var dao = new MasterDataListDao(masterDataList);
            await _mongoCollection.ReplaceOneAsync(m => m.Name == masterDataList.Name, dao);
            return dao.MasterDataList;
        }

        /// <inheritdoc/>
        public async Task<MasterDataList> Find(string id)
        {
            try
            {
                var objectId = ObjectId.Parse(id);
                var masterDataListDao = (await _mongoCollection.FindAsync(l => l.ObjectId == objectId))
                    .FirstOrDefault();
                if (masterDataListDao == null)
                {
                    throw new ResourceNotFoundException($"MasterDataList {id}");
                }
                return masterDataListDao.MasterDataList;
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Master Data id is not valid for {id}.");
            }
        }

        /// <inheritdoc/>
        public async Task<MasterDataList> FindByName(string name)
        {
            try
            {
                var masterDataListDao = (await _mongoCollection.FindAsync(l => l.Name == name))
                    .FirstOrDefault();
                if (masterDataListDao == null)
                {
                    throw new ResourceNotFoundException($"MasterDataList {name}");
                }
                return masterDataListDao.MasterDataList;
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Master Data id is not valid for {name}.");
            }
        }

        /// <summary>
        /// Existses the specified master data list name.
        /// </summary>
        /// <param name="masterDataListName">Name of the master data list.</param>
        /// <param name="masterDataValue">The master data value.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ResourceNotFoundException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public async Task<bool> Exists(string masterDataListName, string masterDataValue)
        {
            try
            {
                var masterDataListDao =
                    (await _mongoCollection.FindAsync(l => l.Name == masterDataListName)).FirstOrDefault();
                if (masterDataListDao == null)
                {
                    throw new ResourceNotFoundException($"MasterDataList {masterDataListName}");
                }
                var exists =
                    masterDataListDao.Values.Exists(
                        x => string.Equals(x.Value, masterDataValue, StringComparison.InvariantCultureIgnoreCase));
                return exists;
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Master Data id is not valid for {masterDataListName}.");
            }
        }
    }
}