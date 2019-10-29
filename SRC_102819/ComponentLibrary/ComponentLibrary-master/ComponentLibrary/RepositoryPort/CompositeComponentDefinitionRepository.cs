using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <inheritdoc/>
	public class CompositeComponentDefinitionRepository : ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>
	{
		private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
		private readonly IDataTypeFactory _factory;
		private readonly IMongoCollection<SemiFinishedGoodDefinitionDao> _mongoCollection;
		private readonly IMongoCollection<PackageDefinitionDao> _packageCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentDefinitionRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		/// <param name="packageCollection">The package collection.</param>
		/// <param name="factory">The factory.</param>
		/// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
		public CompositeComponentDefinitionRepository(IMongoCollection<SemiFinishedGoodDefinitionDao> mongoCollection,
			IMongoCollection<PackageDefinitionDao> packageCollection, IDataTypeFactory factory,
			IDependencyDefinitionRepository dependencyDefinitionRepository)
		{
			_mongoCollection = mongoCollection;
			_dependencyDefinitionRepository = dependencyDefinitionRepository;
			_packageCollection = packageCollection;
			_factory = factory;
		}

		/// <inheritdoc/>
		public async Task Add(string type, ICompositeComponentDefinition serviceDefinition)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var serviceDefinitionDao = new SemiFinishedGoodDefinitionDao(serviceDefinition);
				try
				{
					var sfg = await Find(type, Keys.Sfg.SfgDefinitionGroup);
					if (sfg != null)
					{
						throw new DuplicateResourceException("Sfg");
					}
				}
				catch (ResourceNotFoundException e)
				{
				}
				var definition =
				(await
					_mongoCollection.FindAsync(
						r => (r.Name == serviceDefinition.Name) || (r.Code == serviceDefinition.Code))).FirstOrDefault();
				if (definition != null)
				{
					var message = "";
					if (definition.Name == serviceDefinition.Name)
						message = $"Name: {definition.Name}";
					else if (definition.Code == serviceDefinition.Code)
						message = $"Code: {definition.Code}";
					throw new DuplicateResourceException($"{message} already exists.");
				}
				await _mongoCollection.InsertOneAsync(serviceDefinitionDao);
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var serviceDefinitionDao = new PackageDefinitionDao(serviceDefinition);
				try
				{
					var sfg = await Find(type, Keys.Package.PackageDefinitionGroup);
					if (sfg != null)
					{
						throw new DuplicateResourceException("Package");
					}
				}
				catch (ResourceNotFoundException e)
				{
				}
				var definition =
				(await
					_packageCollection.FindAsync(
						r => (r.Name == serviceDefinition.Name) || (r.Code == serviceDefinition.Code))).FirstOrDefault();
				if (definition != null)
				{
					var message = "";
					if (definition.Name == serviceDefinition.Name)
						message = $"Name: {definition.Name}";
					else if (definition.Code == serviceDefinition.Code)
						message = $"Code: {definition.Code}";
					throw new DuplicateResourceException($"{message} already exists.");
				}
				await _packageCollection.InsertOneAsync(serviceDefinitionDao);
			}
			else
				throw new NotSupportedException(type + " is not supported.");
		}

		/// <inheritdoc/>
		public async Task<ICompositeComponentDefinition> Find(string type, string name)
		{
            name = type == "sfg" ? "Generic SFG" : "Generic Package";
            if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var dto = (await _mongoCollection.FindAsync(r => r.Name == name)).FirstOrDefault();
				if (dto == null)
					throw new ResourceNotFoundException(name);
				return await dto.GetDomain(_factory, _dependencyDefinitionRepository);
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var dto = (await _packageCollection.FindAsync(r => r.Name == name)).FirstOrDefault();
				if (dto == null)
					throw new ResourceNotFoundException(name);
				return await dto.GetDomain(_factory, _dependencyDefinitionRepository);
			}
			else
				throw new NotSupportedException(type + " is not supported.");
		}

		/// <inheritdoc/>
		public async Task Update(string type, ICompositeComponentDefinition serviceDefinition)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var dao = new SemiFinishedGoodDefinitionDao(serviceDefinition);
				await _mongoCollection.ReplaceOneAsync(m => m.Code == dao.Code, dao);
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var dao = new PackageDefinitionDao(serviceDefinition);
				await _packageCollection.ReplaceOneAsync(m => m.Code == dao.Code, dao);
			}
			else
				throw new NotSupportedException(type + " is not supported.");
		}
	}
}