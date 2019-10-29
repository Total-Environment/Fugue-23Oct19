using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.ICompositeComponentMappingRepository"/>
	public class CompositeComponentMappingRepository : ICompositeComponentMappingRepository
	{
		private readonly IMongoCollection<SemiFinishedGoodMappingDao> _semiFinishedGoodMappingCollection;
		private readonly IMongoCollection<PackageMappingDao> _packageMappingCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentMappingRepository"/> class.
		/// </summary>
		/// <param name="semiFinishedGoodMappingCollection">The semi finished good mapping collection.</param>
		/// <param name="packageMappingCollection">The package mapping collection.</param>
		public CompositeComponentMappingRepository(IMongoCollection<SemiFinishedGoodMappingDao> semiFinishedGoodMappingCollection,
			IMongoCollection<PackageMappingDao> packageMappingCollection)
		{
			_semiFinishedGoodMappingCollection = semiFinishedGoodMappingCollection;
			_packageMappingCollection = packageMappingCollection;
		}

		/// <summary>
		/// Gets the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public Task<CompositeComponentMapping> Get(string type)
		{
			var dao = GetDao(type);
			return Task.FromResult(new CompositeComponentMapping(dao.ServiceColumnMapping, dao.GroupCodeMapping));
		}

		/// <summary>
		/// Saves the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="mapping">The mapping.</param>
		/// <returns></returns>
		/// <exception cref="System.NotSupportedException"></exception>
		public async Task<CompositeComponentMapping> Save(string type, CompositeComponentMapping mapping)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
				_semiFinishedGoodMappingCollection.InsertOne(new SemiFinishedGoodMappingDao(mapping));
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
				_packageMappingCollection.InsertOne(new PackageMappingDao(mapping));
			else
				throw new NotSupportedException($"{type} is not supported.");

			return await Get(type);
		}

		private CompositeComponentMappingDao GetDao(string type)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterDefinitionBuilder = new FilterDefinitionBuilder<SemiFinishedGoodMappingDao>();
				var filterDefintion = filterDefinitionBuilder.Empty;
				var dao = (_semiFinishedGoodMappingCollection.FindAsync(filterDefintion)).Result.FirstOrDefault();
				return dao;
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterDefinitionBuilder = new FilterDefinitionBuilder<PackageMappingDao>();
				var filterDefintion = filterDefinitionBuilder.Empty;
				var dao = (_packageMappingCollection.FindAsync(filterDefintion)).Result.FirstOrDefault();
				return dao;
			}
			else
				throw new NotSupportedException($"{type} is not supported.");
		}
	}
}