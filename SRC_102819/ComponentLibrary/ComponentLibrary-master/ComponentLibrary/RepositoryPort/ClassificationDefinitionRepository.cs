using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// Repository for Service Classification Definition
    /// </summary>
    /// <seealso cref="IClassificationDefinitionRepository" />
    public class ClassificationDefinitionRepository : IClassificationDefinitionRepository
    {
        private readonly IMongoCollection<ClassificationDefinitionDao> _mongoCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationDefinitionRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        public ClassificationDefinitionRepository(
            IMongoCollection<ClassificationDefinitionDao> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        /// <inheritdoc/>
        public async Task CreateClassificationDefinition(ClassificationDefinitionDao classificationDefinitionDao)
        {
            if (classificationDefinitionDao == null)
                throw new ArgumentException(nameof(classificationDefinitionDao));

            classificationDefinitionDao.ValidateTreePath();

            var existing =
                (await
                        _mongoCollection.FindAsync(
                            d => d.Value.ToUpper() == classificationDefinitionDao.Value.ToUpper()
                            && d.ComponentType.ToUpper() == classificationDefinitionDao.ComponentType.ToUpper()
                            ))
                    .FirstOrDefault();
            if (existing == null)
                await _mongoCollection.InsertOneAsync(classificationDefinitionDao);
            else
            {
                if (
                    !string.Equals(existing.Description, classificationDefinitionDao.Description,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException("Definition for " + classificationDefinitionDao.Value +
                                                " at the given hierarchy is already available. Duplication or updating the existing definition is not allowed.");
                existing.MergeNewTreePath(
                    classificationDefinitionDao.ClassificationDefinitionDaos?.FirstOrDefault());
                await
                    _mongoCollection.ReplaceOneAsync(
                        Builders<ClassificationDefinitionDao>.Filter.Eq("_id", existing.ObjectId), existing);
            }
        }

        /// <inheritdoc/>
        public async Task<ClassificationDefinitionDao> Find(string groupLevel, string componentType)
        {
            var existing = (await _mongoCollection.FindAsync(d => d.Value == groupLevel && d.ComponentType == componentType)).FirstOrDefault();
            return existing;
        }
    }
}