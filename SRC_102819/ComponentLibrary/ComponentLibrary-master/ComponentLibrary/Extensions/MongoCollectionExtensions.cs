using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.Extensions
{
	/// <summary>
	/// An Extensions class for mongo collection methods
	/// </summary>
	public static class MongoCollectionExtensions
	{
		/// <summary>
		/// Finds the with sort and page asynchronous.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <returns></returns>
		public static async Task<PaginatedAndSortedList<T>> FindWithSortAndPageAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter, int pageNumber = 1, int batchSize = 0, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending)
		{
			if (batchSize < 1)
			{
				batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
			}

			var findOptions = new FindOptions<T>
			{
				Sort = sortOrder == SortOrder.Ascending ? new SortDefinitionBuilder<T>().Ascending(sortColumn) : new SortDefinitionBuilder<T>().Descending(sortColumn),
				Skip = (pageNumber - 1) * batchSize,
				Limit = batchSize
			};

			var records = (await collection.FindAsync(filter, findOptions)).ToList();

			var totalRecords = await collection.CountAsync(filter);
			return new PaginatedAndSortedList<T>(records, pageNumber, totalRecords, batchSize, sortColumn, sortOrder);
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="pageNumber"></param>
		/// <param name="batchSize"></param>
		/// <param name="sortColumn"></param>
		/// <param name="sortOrder"></param>
		/// <returns></returns>
		public static async Task<PaginatedAndSortedList<T>> FindWithSortAndPageAsync<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter, int pageNumber = 1, int batchSize = 0, string sortColumn = "AppliedFrom", SortOrder sortOrder = SortOrder.Descending)
		{
			if (batchSize < 1)
			{
				batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);
			}

			var findOptions = new FindOptions<T>
			{
				Sort = sortOrder == SortOrder.Ascending ? new SortDefinitionBuilder<T>().Ascending(sortColumn) : new SortDefinitionBuilder<T>().Descending(sortColumn),
				Skip = (pageNumber - 1) * batchSize,
				Limit = batchSize
			};

			var records = (await collection.FindAsync(filter, findOptions)).ToList();

			var totalRecords = await collection.CountAsync(filter);
			return new PaginatedAndSortedList<T>(records, pageNumber, totalRecords, batchSize, sortColumn, sortOrder);
		}

		/// <summary>
		/// Generates the filter definition.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filterCriteria">The filter criteria.</param>
		/// <returns></returns>
		/// <exception cref="System.NotSupportedException"></exception>
		public static FilterDefinition<T> GenerateFilterDefinition<T>(Dictionary<string, Tuple<string, object>> filterCriteria)
		{
			var builder = Builders<T>.Filter;
			var filters = new List<FilterDefinition<T>>();
			foreach (var filterCriterion in filterCriteria)
			{
				switch (filterCriterion.Value.Item1)
				{
					case "Regex":
						filters.Add(builder.Regex(filterCriterion.Key, filterCriterion.Value.Item2 as string));
						break;

					case "Eq":
						filters.Add(builder.Eq(filterCriterion.Key, filterCriterion.Value.Item2));
						break;

					case "Lte":
						filters.Add(builder.Lte(filterCriterion.Key, filterCriterion.Value.Item2));
						break;

					case "Gte":
						var subFilters = new List<FilterDefinition<T>>
						{
							builder.Eq(filterCriterion.Key, BsonNull.Value),
							builder.Gte(filterCriterion.Key, filterCriterion.Value.Item2)
						};
						filters.Add(builder.Or(subFilters));
						break;

					default:
						throw new NotSupportedException(filterCriterion.Value.Item1 + " is not supported.");
				}
			}
			var filterDefinition = builder.And(filters);
			return filterDefinition;
		}

		/// <summary>
		/// Generates the sort definition.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <returns></returns>
		public static SortDefinition<T> GenerateSortDefinition<T>(string sortColumn, SortOrder sortOrder = SortOrder.Ascending)
		{
			sortColumn = sortColumn.ToLower();
			var builder = Builders<T>.Sort;
			if (sortOrder == SortOrder.Descending)
				return builder.Descending(new StringFieldDefinition<T>(sortColumn));
			return builder.Ascending(new StringFieldDefinition<T>(sortColumn));
		}

		/// <summary>
		/// Generates the find options.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sortColumn">The sort column.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="pageNumber">The page number.</param>
		/// <param name="batchSize">Size of the batch.</param>
		/// <returns></returns>
		public static FindOptions<T> GenerateFindOptions<T>(string sortColumn, SortOrder sortOrder = SortOrder.Ascending,
			int pageNumber = -1, int batchSize = -1)
		{
			sortColumn = sortColumn.ToLower();
			var builder = Builders<T>.Sort;
			var sortDefintion = sortOrder == SortOrder.Descending
				? builder.Descending(new StringFieldDefinition<T>(sortColumn))
				: builder.Ascending(new StringFieldDefinition<T>(sortColumn));

			var findOptions = new FindOptions<T> { Sort = sortDefintion };
			if (pageNumber > 0 && batchSize > 0)
			{
				findOptions.Limit = batchSize;
				findOptions.Skip = (pageNumber - 1) * batchSize;
			}

			return findOptions;
		}
	}
}