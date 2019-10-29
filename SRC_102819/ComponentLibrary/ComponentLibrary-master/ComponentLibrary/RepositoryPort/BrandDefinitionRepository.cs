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
    /// The Repository for Brand Definitions
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.Domain.IBrandDefinitionRepository"/>
    public class BrandDefinitionRepository : IBrandDefinitionRepository
    {
        private readonly ISimpleDataTypeFactory _simpleDataTypeFactory;
        private readonly IMongoCollection<BrandDefinitionDao> _mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrandDefinitionRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        /// <param name="simpleDataTypeFactory">The data type factory.</param>
        public BrandDefinitionRepository(IMongoCollection<BrandDefinitionDao> mongoCollection,
            ISimpleDataTypeFactory simpleDataTypeFactory)
        {
            _mongoCollection = mongoCollection;
            _simpleDataTypeFactory = simpleDataTypeFactory;
        }

        /// <inheritdoc/>
        public async Task Add(BrandDefinition brandDefinition)
        {
            var definition = await FindBrandDefinition(brandDefinition);
            if (definition != null)
            {
                var msg = "";
                if (definition.Name == brandDefinition.Name)
                    msg = $"Brand definition with name : {brandDefinition.Name} already Exist.";

                throw new DuplicateResourceException(msg);
            }
            await _mongoCollection.InsertOneAsync(new BrandDefinitionDao(brandDefinition));
        }

        /// <inheritdoc/>
        public async Task<IBrandDefinition> FindBy(string name)
        {
            Func<Task<BrandDefinition>> getter = async () =>
            {
                var dao = (await _mongoCollection.FindAsync(r => r.Name == name)).FirstOrDefault();
                if (dao == null)
                    throw new ResourceNotFoundException($"Brand definition with name: {name}");
                var brandDefinition = await dao.GetDomain(_simpleDataTypeFactory);
                return brandDefinition;
            };
            return await Cache.GetOrAdd(name, getter);
        }

        private async Task<BrandDefinitionDao> FindBrandDefinition(BrandDefinition brandDefinition)
        {
            return
            (await
                _mongoCollection.FindAsync(
                    r => r.Name == brandDefinition.Name)).FirstOrDefault();
        }
    }
}