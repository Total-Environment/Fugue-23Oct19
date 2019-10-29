using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// The Material Definition Repository
    /// </summary>
    /// <seealso cref="IComponentDefinitionRepository{IMaterialDefinition}" />
    public class MaterialDefinitionRepository : IComponentDefinitionRepository<IMaterialDefinition>
    {
        private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
        private readonly IDataTypeFactory _factory;
        private readonly IMongoCollection<MaterialDefinitionDao> _mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDefinitionRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
        public MaterialDefinitionRepository(IMongoCollection<MaterialDefinitionDao> mongoCollection,
            IDataTypeFactory factory, IDependencyDefinitionRepository dependencyDefinitionRepository)
        {
            _mongoCollection = mongoCollection;
            _dependencyDefinitionRepository = dependencyDefinitionRepository;
            _factory = factory;
        }

        /// <inheritdoc/>
        public async Task Add(IMaterialDefinition materialDefinition)
        {
            var definition =
            (await
                _mongoCollection.FindAsync(
                    r => r.Name == materialDefinition.Name || r.Code == materialDefinition.Code)).FirstOrDefault();
            if (definition != null)
            {
                var message = "";
                if (definition.Name == materialDefinition.Name)
                    message = $"Name: {definition.Name}";
                else if (definition.Code == materialDefinition.Code)
                    message = $"Code: {definition.Code}";
                throw new DuplicateResourceException($"{message} already exists.");
            }
            await _mongoCollection.InsertOneAsync(new MaterialDefinitionDao(materialDefinition));
        }

        /// <inheritdoc/>
        public async Task<IMaterialDefinition> Find(string name)
        {
            Func<Task<IMaterialDefinition>> getter =
                async () =>
                {
                    var dto = (await _mongoCollection.FindAsync(r => r.Name == name)).FirstOrDefault();
                    if (dto == null)
                        throw new ResourceNotFoundException(name);

                    return await dto.GetDomain(_factory, _dependencyDefinitionRepository);
                };
            return await Cache.GetOrAdd(name, getter);
        }

        /// <inheritdoc/>
        public async Task Update(IMaterialDefinition materialDefinition)
        {
            var dao = new MaterialDefinitionDao(materialDefinition);
            await _mongoCollection.ReplaceOneAsync(m => m.Code == dao.Code, dao);
            Cache.Remove<IMaterialDefinition>(materialDefinition.Name);
        }
    }
}