using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.Helpers;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
	/// <inheritdoc/>
	public class CompositeComponentRepository : ICompositeComponentRepository
	{
		private readonly IMongoCollection<SemiFinishedGoodDao> _semiFinishedGoodCollection;
		private readonly IMongoCollection<PackageDao> _packageCollection;
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _compositeComponentDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeComponentRepository"/> class.
		/// </summary>
		/// <param name="semiFinishedGoodCollection">The semi finished good collection.</param>
		/// <param name="packageCollection">The package collection.</param>
		/// <param name="compositeComponentDefinitionRepository">
		/// The composite component definition repository.
		/// </param>
		public CompositeComponentRepository(IMongoCollection<SemiFinishedGoodDao> semiFinishedGoodCollection,
			IMongoCollection<PackageDao> packageCollection,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> compositeComponentDefinitionRepository)
		{
			_semiFinishedGoodCollection = semiFinishedGoodCollection;
			_compositeComponentDefinitionRepository = compositeComponentDefinitionRepository;
			_packageCollection = packageCollection;
		}

		/// <summary>
		/// Gets the by count the specified filter criteria.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filterCriteria">The filter criteria.</param>
		/// <returns></returns>
		public async Task<long> Count(string type, Dictionary<string, Tuple<string, object>> filterCriteria)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterExpression = MongoCollectionExtensions.GenerateFilterDefinition<SemiFinishedGoodDao>(filterCriteria);
				var count = await _semiFinishedGoodCollection.CountAsync(filterExpression);
				return count;
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterExpression = MongoCollectionExtensions.GenerateFilterDefinition<PackageDao>(filterCriteria);
				var count = await _packageCollection.CountAsync(filterExpression);
				return count;
			}
			else
			{
				throw new NotSupportedException(type + " is not supported.");
			}
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Find(string type, string code, ICompositeComponentDefinition sfgDefintion)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterDefinitionBuilder = new FilterDefinitionBuilder<SemiFinishedGoodDao>();
				var filterDefinition = filterDefinitionBuilder.Eq(SemiFinishedGoodDao.Code, code);
				var dao = (await _semiFinishedGoodCollection.FindAsync(filterDefinition)).SingleOrDefault();
				if (dao == null)
				{
					throw new ResourceNotFoundException(code);
				}
				return dao.ToCompositeComponent(sfgDefintion);
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterDefinitionBuilder = new FilterDefinitionBuilder<PackageDao>();
				var filterDefinition = filterDefinitionBuilder.Eq(PackageDao.Code, code);
				var dao = (await _packageCollection.FindAsync(filterDefinition)).SingleOrDefault();
				if (dao == null)
				{
					throw new ResourceNotFoundException(code);
				}
				return dao.ToCompositeComponent(sfgDefintion);
			}
			else
			{
				throw new NotSupportedException(type + " is not supported.");
			}
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Create(string type, CompositeComponent sfg,
			ICompositeComponentDefinition sfgDefintion)
		{
			var userFullName = ClaimsHelper.GetCurrentUserFullName();
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var semiFinishedGoodDao = new SemiFinishedGoodDao(sfg, BuildSearchKeywords(sfg, sfgDefintion));
				semiFinishedGoodDao.Columns[CompositeComponentDao.DateCreated] = DateTime.UtcNow;
				semiFinishedGoodDao.Columns[CompositeComponentDao.DateLastAmended] = DateTime.UtcNow;
				semiFinishedGoodDao.Columns[CompositeComponentDao.CreatedBy] = userFullName;
				semiFinishedGoodDao.Columns[CompositeComponentDao.LastAmendedBy] = userFullName;
				await _semiFinishedGoodCollection.InsertOneAsync(semiFinishedGoodDao);
				return await Find(type, sfg.Code, sfgDefintion);
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var packageDao = new PackageDao(sfg, BuildSearchKeywords(sfg, sfgDefintion));
				packageDao.Columns[CompositeComponentDao.DateCreated] = DateTime.UtcNow;
				packageDao.Columns[CompositeComponentDao.DateLastAmended] = DateTime.UtcNow;
				packageDao.Columns[CompositeComponentDao.CreatedBy] = userFullName;
				packageDao.Columns[CompositeComponentDao.LastAmendedBy] = userFullName;
				await _packageCollection.InsertOneAsync(packageDao);
				return await Find(type, sfg.Code, sfgDefintion);
			}
			else
			{
				throw new NotSupportedException(type + " is not supported.");
			}
		}

		/// <inheritdoc/>
		public async Task<CompositeComponent> Update(string type, CompositeComponent sfg,
			ICompositeComponentDefinition sfgDefintion)
		{
			var userFullName = ClaimsHelper.GetCurrentUserFullName();
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var semiFinishedGoodDao = new SemiFinishedGoodDao(sfg, BuildSearchKeywords(sfg, sfgDefintion));
				semiFinishedGoodDao.Columns[CompositeComponentDao.DateLastAmended] = DateTime.UtcNow;
				semiFinishedGoodDao.Columns[CompositeComponentDao.LastAmendedBy] = userFullName;
				await _semiFinishedGoodCollection.ReplaceOneAsync(
					Builders<SemiFinishedGoodDao>.Filter.Eq(SemiFinishedGoodDao.Code, sfg.Code), semiFinishedGoodDao);
				return await Find(type, sfg.Code, sfgDefintion);
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var packageDao = new PackageDao(sfg, BuildSearchKeywords(sfg, sfgDefintion));
				packageDao.Columns[CompositeComponentDao.DateLastAmended] = DateTime.UtcNow;
				packageDao.Columns[CompositeComponentDao.LastAmendedBy] = userFullName;
				await _packageCollection.ReplaceOneAsync(
					Builders<PackageDao>.Filter.Eq(PackageDao.Code, sfg.Code), packageDao);
				return await Find(type, sfg.Code, sfgDefintion);
			}
			else
			{
				throw new NotSupportedException(type + " is not supported.");
			}
		}

		/// <summary>
		/// Builds the search keywords based on data provided.
		/// </summary>
		/// <param name="sfg">The SFG.</param>
		/// <param name="sfgDefintion">The SFG defintion.</param>
		/// <returns>List of search keywords.</returns>
	    private List<string> BuildSearchKeywords(CompositeComponent sfg, ICompositeComponentDefinition sfgDefintion)
	    {
	        var values = new List<string>();
	        var searcheableColumns = sfgDefintion.Headers.SelectMany(h => h.Columns).Where(c => c.IsSearchable).ToList();

	        foreach (var searcheableColumn in searcheableColumns)
	        {
	            var searchableColumnData =
	                sfg.Headers.SelectMany(h => h.Columns)
	                    .FirstOrDefault(c => c.Key == searcheableColumn.Key);
	            values.AddRange(GetKeywords(searchableColumnData, searcheableColumn));
	        }
	        return values;
	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnData"></param>
        /// <param name="columnDefinition"></param>
        /// <returns>List of keywords for each columnDefinition</returns>
        private List<string> GetKeywords(IColumnData columnData, IColumnDefinition columnDefinition)
	    {
	        if (columnData?.Value == null)
	            return new List<string>();
	        if (columnDefinition.DataType is ArrayDataType)
	        {
	            return ((object[])columnData.Value).Select(data => data.ToString()).ToList();
	        }
	        var value = columnData.Value?.ToString();
	        var keywords = new List<string>();
	        if (value != HeaderColumnDataBuilder.Na)
	            keywords.Add(value);
	        return keywords;
	    }

        /// <summary>
        /// Finds by the specified filter criteria.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        public async Task<List<CompositeComponent>> Find(string type, Dictionary<string, Tuple<string, object>> filterCriteria,
			string sortColumn, SortOrder sortOrder, int pageNumber, int batchSize)
		{
			if (type.Equals("sfg", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterExpression = MongoCollectionExtensions.GenerateFilterDefinition<SemiFinishedGoodDao>(filterCriteria);
				sortColumn = string.IsNullOrEmpty(sortColumn) ? "sfg_code" : sortColumn;
				var findOptions = MongoCollectionExtensions.GenerateFindOptions<SemiFinishedGoodDao>(sortColumn, sortOrder,
					pageNumber, batchSize);
				findOptions.Projection = Builders<SemiFinishedGoodDao>.Projection.Exclude(CompositeComponentDao.Composition);

				var searchResults = (await _semiFinishedGoodCollection.FindAsync(filterExpression, findOptions)).ToList();

				var compositeComponentDefinition = await _compositeComponentDefinitionRepository.Find("sfg", Keys.Sfg.SfgDefinitionGroup);
				var compositeComponents =
					searchResults.Distinct()
						.Select(searchResult => searchResult.ToCompositeComponent(compositeComponentDefinition))
						.ToList();

				if (!compositeComponents.Any()) throw new ResourceNotFoundException("SFGs");
				return compositeComponents;
			}
			else if (type.Equals("package", StringComparison.InvariantCultureIgnoreCase))
			{
				var filterExpression = MongoCollectionExtensions.GenerateFilterDefinition<PackageDao>(filterCriteria);
				sortColumn = string.IsNullOrEmpty(sortColumn) ? "pkg_code" : sortColumn;
				var findOptions = MongoCollectionExtensions.GenerateFindOptions<PackageDao>(sortColumn, sortOrder,
					pageNumber, batchSize);
				findOptions.Projection = Builders<PackageDao>.Projection.Exclude(CompositeComponentDao.Composition);

				var searchResults = (await _packageCollection.FindAsync(filterExpression, findOptions)).ToList();

				var compositeComponentDefinition =
					await _compositeComponentDefinitionRepository.Find("package", Keys.Package.PackageDefinitionGroup);
				var compositeComponents =
					searchResults.Distinct()
						.Select(searchResult => searchResult.ToCompositeComponent(compositeComponentDefinition))
						.ToList();

				if (!compositeComponents.Any()) throw new ResourceNotFoundException("Packages");
				return compositeComponents;
			}
			else
			{
				throw new NotSupportedException(type + " is not supported.");
			}
		}

		/// <inheritdoc/>
		public async Task<long> GetTotalCountByGroupAndColumnName(string type, string group, string columnName, List<string> keywords)
		{
			if (String.IsNullOrEmpty(type))
			{
				throw new ArgumentException("Invalid Composite Component Type");
			}
			string regex = "";
			long totalCount = 0;
			if (keywords != null)
			{
				regex = GenerateRegex(keywords);
			}
			switch (type.ToLower())
			{
				case "package":
					{
						var builders = Builders<PackageDao>.Filter;
						var filters = new List<FilterDefinition<PackageDao>>
					{
						builders.Regex(PackageDao.PkgLevel1, $"((?i){group}(?-i))"),
						builders.Ne(columnName, BsonNull.Value)
					};
						if (!String.IsNullOrEmpty(regex))
						{
							filters.Add(builders.Or(new List<FilterDefinition<PackageDao>>
						{
							builders.Regex(PackageDao.Code, regex),
							builders.Regex(PackageDao.ShortDescription, regex)
						}));
						}
						var filterDefinition = builders.And(filters);
						totalCount = await _packageCollection.CountAsync(filterDefinition);
						break;
					}
				case "sfg":
					{
						var builders = Builders<SemiFinishedGoodDao>.Filter;
						var filters = new List<FilterDefinition<SemiFinishedGoodDao>>
					{
						builders.Regex(SemiFinishedGoodDao.SfgLevel1, $"((?i){group}(?-i))"),
						builders.Ne(columnName, BsonNull.Value)
					};
						if (!String.IsNullOrEmpty(regex))
						{
							filters.Add(builders.Or(new List<FilterDefinition<SemiFinishedGoodDao>>
						{
							builders.Regex(SemiFinishedGoodDao.Code, regex),
							builders.Regex(SemiFinishedGoodDao.ShortDescription, regex)
						}));
						}
						var filterDefinition = builders.And(filters);
						totalCount = await _semiFinishedGoodCollection.CountAsync(filterDefinition);
						break;
					}
				default:
					break;
			}
			return totalCount;
		}

		/// <inheritdoc/>
		public async Task<List<CompositeComponent>> GetByGroupAndColumnNameAndKeyWords(string type, string group, string columnName, List<string> keywords,
			int pageNumber = -1, int batchSize = -1)
		{
			if (String.IsNullOrEmpty(type))
			{
				throw new ArgumentException("Invalid Composite Component Type");
			}
			string regex = "";
			if (keywords != null)
			{
				regex = GenerateRegex(keywords);
			}
			List<CompositeComponent> result = new List<CompositeComponent>();
			switch (type.ToLower())
			{
				case "sfg":
					{
						var builders = Builders<SemiFinishedGoodDao>.Filter;
						var filters = new List<FilterDefinition<SemiFinishedGoodDao>>
					{
						builders.Regex(SemiFinishedGoodDao.SfgLevel1, $"((?i){group}(?-i))"),
						builders.Ne(columnName, BsonNull.Value)
					};
						if (!String.IsNullOrEmpty(regex))
						{
							filters.Add(builders.Or(new List<FilterDefinition<SemiFinishedGoodDao>>
						{
							builders.Regex(SemiFinishedGoodDao.Code, regex),
							builders.Regex(SemiFinishedGoodDao.ShortDescription, regex)
						}));
						}
						var filterDefinition = builders.And(filters);
						var findOptions = new FindOptions<SemiFinishedGoodDao>();

						findOptions.Projection =
							Builders<SemiFinishedGoodDao>.Projection.Exclude(CompositeComponentDao.Composition);
						if (pageNumber > 0 && batchSize > 0)
						{
							findOptions.Limit = batchSize;
							findOptions.Skip = (pageNumber - 1) * batchSize;
						}
						var compositeComponentDefinition =
							await _compositeComponentDefinitionRepository.Find("sfg", Keys.Sfg.SfgDefinitionGroup);

						var columnDefinition = compositeComponentDefinition.Headers.SelectMany(
								sfgHeaderDefinition => sfgHeaderDefinition.Columns)
							.FirstOrDefault(
								sfgDefinitionHeaderColumn =>
									string.Equals(sfgDefinitionHeaderColumn.Key, columnName,
										StringComparison.InvariantCultureIgnoreCase));

						ValidateColumnAsAttachmentType(columnDefinition, columnName);
						var sfgDaos = (await _semiFinishedGoodCollection.FindAsync(filterDefinition, findOptions)).ToList();

						result = sfgDaos.Select(sfg => sfg.ToCompositeComponent(compositeComponentDefinition)).ToList();
						break;
					}
				case "package":
					{
						var builders = Builders<PackageDao>.Filter;
						var filters = new List<FilterDefinition<PackageDao>>
					{
						builders.Regex(PackageDao.PkgLevel1, $"((?i){group}(?-i))"),
						builders.Ne(columnName, BsonNull.Value)
					};
						if (!String.IsNullOrEmpty(regex))
						{
							filters.Add(builders.Or(new List<FilterDefinition<PackageDao>>
						{
							builders.Regex(PackageDao.Code, regex),
							builders.Regex(PackageDao.ShortDescription, regex)
						}));
						}
						var filterDefinition = builders.And(filters);
						var findOptions = new FindOptions<PackageDao>
						{
							Projection = Builders<PackageDao>.Projection.Exclude(CompositeComponentDao.Composition)
						};

						if (pageNumber > 0 && batchSize > 0)
						{
							findOptions.Limit = batchSize;
							findOptions.Skip = (pageNumber - 1) * batchSize;
						}
						var compositeComponentDefinition =
							await _compositeComponentDefinitionRepository.Find("package", Keys.Package.PackageDefinitionGroup);

						var columnDefinition = compositeComponentDefinition.Headers.SelectMany(
								pkgHeaderDefinition => pkgHeaderDefinition.Columns)
							.FirstOrDefault(
								pkgDefinitionHeaderColumn =>
									string.Equals(pkgDefinitionHeaderColumn.Key, columnName,
										StringComparison.InvariantCultureIgnoreCase));

						ValidateColumnAsAttachmentType(columnDefinition, columnName);
						var pkgDaos = (await _packageCollection.FindAsync(filterDefinition, findOptions)).ToList();

						result = pkgDaos.Select(sfg => sfg.ToCompositeComponent(compositeComponentDefinition)).ToList();
						break;
					}
				default:
					break;
			}

			return result;
		}

		private void ValidateColumnAsAttachmentType(IColumnDefinition columnDefinition, string columnName)
		{
			if (columnDefinition == null)
				throw new ArgumentException($"{columnName} is not valid column in the material definition.");

			if (!columnDefinition.IsAttachmentType())
				throw new ArgumentException($"{columnName} is neither static file data type nor check list data type.");
		}

		private string GenerateRegex(List<string> keywordsList)
		{
			return $"/{GenerateRegexPattern(keywordsList)}/i";
		}

		private string GenerateRegexPattern(List<string> keywordsList)
		{
			return $"({string.Join("|", keywordsList.ToArray())})";
		}
	}
}