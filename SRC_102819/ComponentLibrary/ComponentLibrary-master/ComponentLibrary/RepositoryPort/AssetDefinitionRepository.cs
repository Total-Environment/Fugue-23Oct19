using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// Asset Definition repository
    /// </summary>
    public class AssetDefinitionRepository : IComponentDefinitionRepository<AssetDefinition>
    {
        private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
        private readonly IDataTypeFactory _factory;
        private readonly IMongoCollection<AssetDefinitionDao> _mongoCollection;

        /// <summary>
        /// Initializes AssetDefinitionRepository with MaterialDefinitionRepository
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <param name="dependencyDefinitionRepository"></param>
        /// <param name="factory"></param>
        public AssetDefinitionRepository(IMongoCollection<AssetDefinitionDao> mongoCollection,
            IDependencyDefinitionRepository dependencyDefinitionRepository,
            IDataTypeFactory factory)
        {
            _mongoCollection = mongoCollection;
            _dependencyDefinitionRepository = dependencyDefinitionRepository;
            _factory = factory;
        }

        /// <inheritdoc/>
        public async Task Add(AssetDefinition assetDefinition)
        {
            var definition = await FindAssetDefinition(assetDefinition);
            if (definition != null)
            {
                var msg = "";
                if (definition.Name == assetDefinition.Name)
                    msg = $"Asset definition with name : {assetDefinition.Name} already Exist.";
                if (definition.Code == assetDefinition.Code)
                    msg = $"Asset definition witj code : {assetDefinition.Code} already Exist.";
                throw new DuplicateResourceException(msg);
            }
            await _mongoCollection.InsertOneAsync(new AssetDefinitionDao(assetDefinition));
        }

        /// <inheritdoc/>
        public async Task<AssetDefinition> Find(string name)
        {
            Func<Task<AssetDefinition>> getter = async () =>
            {
                var dto = (await _mongoCollection.FindAsync(r => r.Name == name)).FirstOrDefault();
                if (dto == null)
                    throw new ResourceNotFoundException($"Asset definition with name: {name}");
                return await dto.GetDomain(_factory, _dependencyDefinitionRepository);
            };
            return await Cache.GetOrAdd(name, getter);
        }

        /// <inheritdoc/>
        public async Task Update(AssetDefinition assetDefinition)
        {
            var assetDefinitionDao = await FindAssetDefinition(assetDefinition);
            if (assetDefinitionDao == null)
                throw new ResourceNotFoundException($"Asset definition with name: {assetDefinition.Name}");
            await _mongoCollection.ReplaceOneAsync(a => a.Name == assetDefinition.Name,
                new AssetDefinitionDao(assetDefinition));
            Cache.Remove<AssetDefinition>(assetDefinition.Name);
        }

        private async Task<AssetDefinitionDao> FindAssetDefinition(AssetDefinition assetDefinition)
        {
            return
            (await
                _mongoCollection.FindAsync(
                    r => r.Name == assetDefinition.Name || r.Code == assetDefinition.Code)).FirstOrDefault();
        }
    }
}