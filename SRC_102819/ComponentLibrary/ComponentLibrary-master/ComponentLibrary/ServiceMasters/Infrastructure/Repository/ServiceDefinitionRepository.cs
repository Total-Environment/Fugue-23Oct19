using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository
{
	/// <inheritdoc/>
	public class ServiceDefinitionRepository : IComponentDefinitionRepository<IServiceDefinition>
	{
		private readonly IDependencyDefinitionRepository _dependencyDefinitionRepository;
		private readonly IDataTypeFactory _factory;
		private readonly IMongoCollection<ServiceDefinitionDao> _mongoCollection;
		private readonly MongoCollectionHelper<ServiceDefinitionDao, IServiceDefinition> _mongoCollectionHelper;
		private readonly ICodePrefixTypeMappingRepository _codePrefixTypeMappingRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDefinitionRepository"/> class.
		/// </summary>
		/// <param name="mongoCollection">The mongo collection.</param>
		/// <param name="factory">The factory.</param>
		/// <param name="dependencyDefinitionRepository">The dependency definition repository.</param>
		/// <param name="codePrefixTypeMappingRepository">The code prefix type mapping repository.</param>
		public ServiceDefinitionRepository(IMongoCollection<ServiceDefinitionDao> mongoCollection,
			IDataTypeFactory factory, IDependencyDefinitionRepository dependencyDefinitionRepository,
			ICodePrefixTypeMappingRepository codePrefixTypeMappingRepository)
		{
			_mongoCollection = mongoCollection;
			_dependencyDefinitionRepository = dependencyDefinitionRepository;
			_codePrefixTypeMappingRepository = codePrefixTypeMappingRepository;
			_factory = factory;
			_mongoCollectionHelper = new MongoCollectionHelper<ServiceDefinitionDao, IServiceDefinition>(_mongoCollection);
		}

		/// <inheritdoc/>
		public async Task Add(IServiceDefinition componentDefinition)
		{
			var serviceDefinitionDao = new ServiceDefinitionDao(componentDefinition);
			await _mongoCollectionHelper.Add(serviceDefinitionDao, componentDefinition);
			await _codePrefixTypeMappingRepository.Add(new CodePrefixTypeMapping(componentDefinition.Code, ComponentType.Service));
		}

		/// <inheritdoc/>
		public async Task<IServiceDefinition> Find(string name)
		{
			var dto = await _mongoCollectionHelper.Find(name);
			return await dto.GetDomain(_factory, _dependencyDefinitionRepository);
		}

		/// <inheritdoc/>
		public async Task Update(IServiceDefinition serviceDefinition)
		{
			var dao = new ServiceDefinitionDao(serviceDefinition);
			_mongoCollectionHelper.Update(dao);
		}
	}
}