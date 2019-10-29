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
	public class DependencyDefinitionRepository : IDependencyDefinitionRepository
	{
		private readonly IMongoCollection<DependencyDefinitionDao> _mongoCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyDefinitionRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		public DependencyDefinitionRepository(IMongoCollection<DependencyDefinitionDao> mongoCollection)
		{
			_mongoCollection = mongoCollection;
		}

		/// <inheritdoc/>
		public async Task CreateDependencyDefinition(DependencyDefinition dependencyDefinition)
		{
			var previous = (await _mongoCollection.FindAsync(d => d.Name.Equals(dependencyDefinition.Name)))
				.FirstOrDefault();
			if (previous != null)
				throw new DuplicateResourceException($"Dependency definition with name : {dependencyDefinition.Name} already exist.");

			await _mongoCollection.InsertOneAsync(new DependencyDefinitionDao(dependencyDefinition));
		}

		/// <inheritdoc/>
		public async Task<DependencyDefinition> GetDependencyDefinition(string name)
		{
			var dao = (await _mongoCollection.FindAsync(d => d.Name.Equals(name))).FirstOrDefault();
			if (dao == null)
				throw new ResourceNotFoundException($"DependencyDefinition with name : {name}");
			return dao.Domain();
		}

		/// <inheritdoc/>
		public async Task UpdateDependencyDefinition(DependencyDefinition dependencyDefinition)
		{
			await _mongoCollection.ReplaceOneAsync(
				Builders<DependencyDefinitionDao>.Filter.Eq("Name", dependencyDefinition.Name),
				new DependencyDefinitionDao(dependencyDefinition));
		}

        /// <inheritdoc/>
	    public async Task PatchDependencyDefinition(IEnumerable<DependentBlock> domain, List<string> columnsList, string name)
        {
            var result = domain.Select(d => new DependentBlockDao(d)).ToList();
              var update = Builders<DependencyDefinitionDao>.Update.Set("DependentBlocks", result);
             // var update = Builders<DependencyDefinitionDao>.Update.Set("ColumnList", new List<string>{ "Mtr Level 1", "Mtr Level 2", "Mtr Level 3", "Mtr Level 4", "Mtr Level 5", "Mtr Level 6", "Mtr Level 7"});
            if (update != null)
                await _mongoCollection.UpdateOneAsync(Builders<DependencyDefinitionDao>.Filter.Eq("Name", name),update);
            else
            {
                throw new ArgumentException("Could not add new dependency Blocks");
            }
        }
    }
}