using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository
{
    public class MongoCollectionHelper<T1, T2> where T1 : ComponentDefinitionDao where T2 : IComponentDefinition
    {
        private readonly IMongoCollection<T1> _mongoCollection;

        public MongoCollectionHelper(IMongoCollection<T1> collection)
        {
            _mongoCollection = collection;
        }

        public async Task Add(T1 componentDefinitionDao, T2 componentDefinition)
        {
            var definition =
            (await
                _mongoCollection.FindAsync(
                    r => (r.Name == componentDefinition.Name) || (r.Code == componentDefinition.Code))).FirstOrDefault();
            if (definition != null)
            {
                var message = "";
                if (definition.Name == componentDefinition.Name)
                    message = $"Name: {definition.Name}";
                else if (definition.Code == componentDefinition.Code)
                    message = $"Code: {definition.Code}";
                throw new DuplicateResourceException($"{message} already exists.");
            }
            await _mongoCollection.InsertOneAsync(componentDefinitionDao);
        }

        public async Task<T1> Find(string name)
        {
            var dto = (await _mongoCollection.FindAsync(r => r.Name == name)).FirstOrDefault();
            if (dto == null)
                throw new ResourceNotFoundException(name);
            return dto;
        }

        public async Task Update(T1 componentDefinitionDao)
        {
            await _mongoCollection.ReplaceOneAsync(m => m.Code == componentDefinitionDao.Code, componentDefinitionDao);
        }
    }
}