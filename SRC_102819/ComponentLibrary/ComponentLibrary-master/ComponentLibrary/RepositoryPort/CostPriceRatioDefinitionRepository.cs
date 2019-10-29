using System.Threading.Tasks;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.ICostPriceRatioDefinitionRepository"/>
	public class CostPriceRatioDefinitionRepository : ICostPriceRatioDefinitionRepository
	{
		private readonly IMongoCollection<CostPriceRatioDefinitionDao> _collection;
		private readonly ISimpleDataTypeFactory _simpleDataTypeFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioDefinitionRepository"/> class.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="simpleDataTypeFactory">The simple data type factory.</param>
		public CostPriceRatioDefinitionRepository(IMongoCollection<CostPriceRatioDefinitionDao> collection,
			ISimpleDataTypeFactory simpleDataTypeFactory)
		{
			_collection = collection;
			_simpleDataTypeFactory = simpleDataTypeFactory;
		}

		/// <summary>
		/// Adds the specified cost price ratio definition.
		/// </summary>
		/// <param name="costPriceRatioDefinition">The cost price ratio definition.</param>
		/// <returns></returns>
		public async Task Add(CostPriceRatioDefinition costPriceRatioDefinition)
		{
			var costPriceRatioDefinitionDao = new CostPriceRatioDefinitionDao(costPriceRatioDefinition);
			await _collection.InsertOneAsync(costPriceRatioDefinitionDao);
		}

		/// <summary>
		/// Finds the by.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public async Task<CostPriceRatioDefinition> FindBy(string name)
		{
			var costPriceRatioDefinitionDao = (await _collection.FindAsync(r => r.Name == name)).FirstOrDefault();
			var costPriceRatioDefinition = await costPriceRatioDefinitionDao.GetDomain(_simpleDataTypeFactory);
			return costPriceRatioDefinition;
		}
	}
}