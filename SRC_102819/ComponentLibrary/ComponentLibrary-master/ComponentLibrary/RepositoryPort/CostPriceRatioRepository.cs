using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.RepositoryPort.ICostPriceRatioRepository"/>
	public class CostPriceRatioRepository : ICostPriceRatioRepository
	{
		private readonly IMongoCollection<CostPriceRatioDao> _costPriceRatioCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="CostPriceRatioRepository"/> class.
		/// </summary>
		/// <param name="costPriceRatioCollection">The cost price ratio collection.</param>
		public CostPriceRatioRepository(IMongoCollection<CostPriceRatioDao> costPriceRatioCollection)
		{
			_costPriceRatioCollection = costPriceRatioCollection;
		}

		/// <summary>
		/// Creates the specified cost price ratio.
		/// </summary>
		/// <param name="costPriceRatio">The cost price ratio.</param>
		/// <returns></returns>
		public async Task<CostPriceRatio> Create(CostPriceRatio costPriceRatio)
		{
			var costPriceRatioDao = new CostPriceRatioDao(costPriceRatio);

			var builder = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				builder.Eq(dao => dao.AppliedFrom, costPriceRatioDao.AppliedFrom),
				builder.Eq(dao => dao.ComponentType, costPriceRatioDao.ComponentType),
				costPriceRatioDao.Level1 == null
					? builder.Eq(dao => dao.Level1, null)
					: builder.Regex(dao => dao.Level1, $"((?i){costPriceRatioDao.Level1}(?-i))"),
				costPriceRatioDao.Level2 == null
					? builder.Eq(dao => dao.Level2, null)
					: builder.Regex(dao => dao.Level2, $"((?i){costPriceRatioDao.Level2}(?-i))"),
				costPriceRatioDao.Level3 == null
					? builder.Eq(dao => dao.Level3, null)
					: builder.Regex(dao => dao.Level3, $"((?i){costPriceRatioDao.Level3}(?-i))"),
				costPriceRatioDao.Code == null
					? builder.Eq(dao => dao.Code, null)
					: builder.Regex(dao => dao.Code, $"((?i){costPriceRatioDao.Code}(?-i))"),
				costPriceRatioDao.ProjectCode == null
					? builder.Eq(dao => dao.ProjectCode, null)
					: builder.Regex(dao => dao.ProjectCode, $"((?i){costPriceRatioDao.ProjectCode}(?-i))")
			};
			var filterDefinition = builder.And(filters);
			var existingCount = await _costPriceRatioCollection.CountAsync(filterDefinition);
			if (existingCount > 0)
			{
				var combination = new StringBuilder();
				if (costPriceRatioDao.Level1 != null) combination.Append(costPriceRatioDao.Level1 + ", ");
				if (costPriceRatioDao.Level2 != null) combination.Append(costPriceRatioDao.Level2 + ", ");
				if (costPriceRatioDao.Level3 != null) combination.Append(costPriceRatioDao.Level3 + ", ");
				if (costPriceRatioDao.Code != null) combination.Append(costPriceRatioDao.Code + ", ");
				if (costPriceRatioDao.ProjectCode != null) combination.Append(costPriceRatioDao.ProjectCode + ", ");
				combination.Append(costPriceRatioDao.AppliedFrom.InIst().Date);
				throw new DuplicateResourceException(
					$"CPR for this combination {combination} already exists. Please revisit and submit.");
			}

			await _costPriceRatioCollection.InsertOneAsync(costPriceRatioDao);
			return costPriceRatio;
		}

		/// <summary>
		/// Gets the cost price rato.
		/// </summary>
		/// <param name="appliedOn">The applied on.</param>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="level1">The level1.</param>
		/// <param name="level2">The level2.</param>
		/// <param name="level3">The level3.</param>
		/// <param name="code">The code.</param>
		/// <param name="projectCode">The project code.</param>
		/// <param name="costPriceRatioDefinition">The cost price ratio definition.</param>
		/// <returns></returns>
		public async Task<CostPriceRatio> GetCostPriceRato(DateTime appliedOn, ComponentType componentType, string level1, string level2, string level3,
			string code, string projectCode, CostPriceRatioDefinition costPriceRatioDefinition)
		{
			CostPriceRatioDao costPriceRatioDao = null;
			if (projectCode != null)
			{
				costPriceRatioDao = await GetCostPriceRatioByCodeAndProjectCode(appliedOn, componentType, code, projectCode);
			}

			if (costPriceRatioDao == null)
			{
				costPriceRatioDao = await GetCostPriceRatioByCode(appliedOn, componentType, code);
			}

			if (costPriceRatioDao == null)
			{
				costPriceRatioDao = await GetCostPriceRatioByLevels(appliedOn, componentType, level1, level2, level3);
			}

			if (costPriceRatioDao == null)
			{
				var combination = new StringBuilder();
				combination.Append($"{componentType}");
				combination.Append($", {code}");
				if (projectCode != null) combination.Append($", {projectCode}");
				throw new ResourceNotFoundException($"CostPriceRatio for {combination} on {appliedOn}");
			}
			else
				return costPriceRatioDao.ToCostPriceRatio(costPriceRatioDefinition);
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByLevels(DateTime appliedOn, ComponentType componentType, string level1, string level2, string level3)
		{
			CostPriceRatioDao costPriceRatioDao;
			if (componentType == ComponentType.Material)
				costPriceRatioDao = await GetCostPriceRatioByMaterialLevels(appliedOn, level1, level2, level3);
			else
			{
				costPriceRatioDao = await GetCostPriceRatioByLevel1And2(appliedOn, componentType, level1, level2);
				if (costPriceRatioDao == null)
					costPriceRatioDao = await GetCostPriceRatioByLevel1(appliedOn, componentType, level1);
			}
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByLevel1(DateTime appliedOn, ComponentType componentType, string level1)
		{
			var filter = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				filter.Lte(dao => dao.AppliedFrom, appliedOn),
				filter.Eq(dao => dao.ComponentType, componentType),
				filter.Eq(dao => dao.Level1, level1),
			};
			var filterDefinition = filter.And(filters);
			var findOptions = new FindOptions<CostPriceRatioDao>();
			var sort = Builders<CostPriceRatioDao>.Sort;
			findOptions.Sort = sort.Descending(dao => dao.AppliedFrom);
			findOptions.Limit = 1;
			var costPriceRatioDao = (await _costPriceRatioCollection.FindAsync(filterDefinition, findOptions)).FirstOrDefault();
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByLevel1And2(DateTime appliedOn, ComponentType componentType, string level1, string level2)
		{
			var filter = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				filter.Lte(dao => dao.AppliedFrom, appliedOn),
				filter.Eq(dao => dao.ComponentType, componentType),
				filter.Eq(dao => dao.Level1, level1),
				filter.Eq(dao => dao.Level2, level2),
			};
			var filterDefinition = filter.And(filters);
			var findOptions = new FindOptions<CostPriceRatioDao>();
			var sort = Builders<CostPriceRatioDao>.Sort;
			findOptions.Sort = sort.Descending(dao => dao.AppliedFrom);
			findOptions.Limit = 1;
			var costPriceRatioDao = (await _costPriceRatioCollection.FindAsync(filterDefinition, findOptions)).FirstOrDefault();
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByMaterialLevels(DateTime appliedOn, string level1, string level2, string level3)
		{
			CostPriceRatioDao costPriceRatioDao = await GetCostPriceRatioByMaterialLevel1And2And3(appliedOn, level1, level2, level3);
			if (costPriceRatioDao == null)
				costPriceRatioDao = await GetCostPriceRatioByMaterialLevel1And2(appliedOn, level1, level2);
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByMaterialLevel1And2(DateTime appliedOn, string level1, string level2)
		{
			var filter = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				filter.Lte(dao => dao.AppliedFrom, appliedOn),
				filter.Eq(dao => dao.ComponentType, ComponentType.Material),
				filter.Eq(dao => dao.Level1, level1),
				filter.Eq(dao => dao.Level2, level2),
			};
			var filterDefinition = filter.And(filters);
			var findOptions = new FindOptions<CostPriceRatioDao>();
			var sort = Builders<CostPriceRatioDao>.Sort;
			findOptions.Sort = sort.Descending(dao => dao.AppliedFrom);
			findOptions.Limit = 1;
			var costPriceRatioDao = (await _costPriceRatioCollection.FindAsync(filterDefinition, findOptions)).FirstOrDefault();
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByMaterialLevel1And2And3(DateTime appliedOn, string level1, string level2, string level3)
		{
			var filter = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				filter.Lte(dao => dao.AppliedFrom, appliedOn),
				filter.Eq(dao => dao.ComponentType, ComponentType.Material),
				filter.Eq(dao => dao.Level1, level1),
				filter.Eq(dao => dao.Level2, level2),
				filter.Eq(dao => dao.Level3, level3),
			};
			var filterDefinition = filter.And(filters);
			var findOptions = new FindOptions<CostPriceRatioDao>();
			var sort = Builders<CostPriceRatioDao>.Sort;
			findOptions.Sort = sort.Descending(dao => dao.AppliedFrom);
			findOptions.Limit = 1;
			var costPriceRatioDao = (await _costPriceRatioCollection.FindAsync(filterDefinition, findOptions)).FirstOrDefault();
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByCode(DateTime appliedOn, ComponentType componentType, string code)
		{
			var filter = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				filter.Lte(dao => dao.AppliedFrom, appliedOn),
				filter.Eq(dao => dao.ComponentType, componentType),
				filter.Eq(dao => dao.Code, code),
				filter.Eq(dao => dao.ProjectCode, null)
			};
			var filterDefinition = filter.And(filters);
			var findOptions = new FindOptions<CostPriceRatioDao>();
			var sort = Builders<CostPriceRatioDao>.Sort;
			findOptions.Sort = sort.Descending(dao => dao.AppliedFrom);
			findOptions.Limit = 1;
			var costPriceRatioDao = (await _costPriceRatioCollection.FindAsync(filterDefinition, findOptions)).FirstOrDefault();
			return costPriceRatioDao;
		}

		private async Task<CostPriceRatioDao> GetCostPriceRatioByCodeAndProjectCode(DateTime appliedOn, ComponentType componentType, string code,
			string projectCode)
		{
			var filter = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				filter.Lte(dao => dao.AppliedFrom, appliedOn),
				filter.Eq(dao => dao.ComponentType, componentType),
				filter.Eq(dao => dao.Code, code),
				filter.Eq(dao => dao.ProjectCode, projectCode)
			};
			var filterDefinition = filter.And(filters);
			var findOptions = new FindOptions<CostPriceRatioDao>();
			var sort = Builders<CostPriceRatioDao>.Sort;
			findOptions.Sort = sort.Descending(dao => dao.AppliedFrom);
			findOptions.Limit = 1;
			var costPriceRatioDao = (await _costPriceRatioCollection.FindAsync(filterDefinition, findOptions)).FirstOrDefault();
			return costPriceRatioDao;
		}

		/// <summary>
		/// ///
		/// </summary>
		/// <param name="appliedOn"></param>
		/// <param name="componentType"></param>
		/// <param name="costPriceRatioDefinition"></param>
		/// <returns></returns>
		public async Task<CostPriceRatioList> GetCostPriceRatioList(DateTime appliedOn, ComponentType componentType,
																	CostPriceRatioDefinition costPriceRatioDefinition)
		{
			var builder = Builders<CostPriceRatioDao>.Filter;
			var filters = new List<FilterDefinition<CostPriceRatioDao>>
			{
				builder.Eq(dao => dao.ComponentType, componentType),
				builder.Lte(dao => dao.AppliedFrom, appliedOn),
			};
			var filterDefinition = builder.And(filters);
			var costPriceRatioDaos = (await _costPriceRatioCollection.FindAsync(filterDefinition)).ToList();
			return new CostPriceRatioList(costPriceRatioDaos, costPriceRatioDefinition);
		}
	}
}