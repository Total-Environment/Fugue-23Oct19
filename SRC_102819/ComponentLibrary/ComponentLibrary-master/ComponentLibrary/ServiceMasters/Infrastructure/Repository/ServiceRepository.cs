using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.Helpers;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository
{
    /// <summary>
    /// Class ServiceRepository.
    /// </summary>
    public class ServiceRepository : IServiceRepository
    {
        private readonly IMongoCollection<ServiceDao> _mongoCollection;
        private readonly IComponentDefinitionRepository<IServiceDefinition> _serviceDefinitionRepository;
        private readonly IBank _bank;
        private readonly IFilterCriteriaBuilder _filterCriteriaBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        /// <param name="serviceDefinitionRepository">The service definition repository.</param>
        /// <param name="bankRepository">Bank Repository</param>
        /// <param name="filterCriteriaBuilder"></param>
        public ServiceRepository(IMongoCollection<ServiceDao> mongoCollection,
            IComponentDefinitionRepository<IServiceDefinition> serviceDefinitionRepository,
            IBank bank, IFilterCriteriaBuilder filterCriteriaBuilder)
        {
            _mongoCollection = mongoCollection;
            _serviceDefinitionRepository = serviceDefinitionRepository;
            _bank = bank;
            _filterCriteriaBuilder = filterCriteriaBuilder;
        }

        /// <inheritdoc/>
        public async Task Add(Service service)
        {
            if (service.Id == null)
                throw new ArgumentNullException(nameof(service), "Id is required.");
            var serviceIdWithoutGroup = service.Id.Remove(0, 3).TrimStart('0');
            service.AmendedAt = service.CreatedAt = DateTime.UtcNow;
            service.AmendedBy = service.CreatedBy = ClaimsHelper.GetCurrentUserFullName();

            var filterExpression = GenerateFilterForServiceCode(serviceIdWithoutGroup);

            var serviceWithSameId = (await _mongoCollection.FindAsync(filterExpression)).ToList();
            if (!serviceWithSameId.Any())
            {
                var serviceDao = new ServiceDao(service);
                await _mongoCollection.InsertOneAsync(serviceDao);
            }
            else
            {
                throw new InvalidOperationException($"service code: {serviceIdWithoutGroup} is already exists");
            }
        }

        /// <inheritdoc/>
        public async Task<int> Count(List<string> list, string componentLevel2)
        {
            var filterDefinition = GenerateFilterDefinitions(list, componentLevel2);
            var result = await _mongoCollection.FindAsync(filterDefinition);

            return result.ToList().Distinct().Count();
        }

        /// <inheritdoc/>
        public async Task<IService> Find(string id)
        {
            var dao = await GetDao(id);
            if (dao == null)
                throw new ResourceNotFoundException(id);
            return await dao.GetDomain(_serviceDefinitionRepository);
        }

        /// <inheritdoc/>
        public async Task<List<Service>> GetByGroupAndColumnName(string group, string columnName, int pageNumber = -1,
            int batchSize = -1)
        {
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>
            {
                builder.Regex(ComponentDao.ServiceLevel1, $"((?i){@group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value)
            };
            var filterDefinition = builder.And(filters);

            var findOptions = new FindOptions<ServiceDao>();
            if (pageNumber > 0 && batchSize > 0)
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }

            var serviceDaos = (await _mongoCollection.FindAsync(filterDefinition, findOptions)).ToList();
            var services = new List<Service>();
            foreach (var serviceDao in serviceDaos)
            {
                services.Add(await serviceDao.GetDomain(_serviceDefinitionRepository));
            }
            return services;
        }

        /// <inheritdoc/>
        public async Task<List<Service>> GetByGroupAndColumnNameAndKeyWords(string @group, string columnName,
            List<string> keywords, int pageNumber, int batchSize)
        {
            var regex = GenerateRegex(keywords);
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>
            {
                builder.Regex(ComponentDao.ServiceLevel1, $"((?i){group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value),
                builder.Or(new List<FilterDefinition<ServiceDao>>
                {
                    builder.Regex(ServiceDao.ServiceCode, regex),
                    builder.Regex(ComponentDao.ShortDescription, regex)
                })
            };
            var filterDefinition = builder.And(filters);

            var findOptions = new FindOptions<ServiceDao>();
            if (pageNumber > 0 && batchSize > 0)
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }

            var serviceDaos = (await _mongoCollection.FindAsync(filterDefinition, findOptions)).ToList();
            var services = new List<Service>();
            foreach (var serviceDao in serviceDaos)
            {
                services.Add(await serviceDao.GetDomain(_serviceDefinitionRepository));
            }
            return services;
        }

        /// <inheritdoc/>
        public async Task<long> GetTotalCountByGroupAndColumnName(string @group, string columnName)
        {
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>
            {
                builder.Regex(ComponentDao.ServiceLevel1, $"((?i){@group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value)
            };
            var filterDefinition = builder.And(filters);

            var totalCount = await _mongoCollection.CountAsync(filterDefinition);
            return totalCount;
        }

        /// <inheritdoc/>
        public async Task<long> GetTotalCountByGroupAndColumnNameAndKeyWords(string @group, string columnName,
            List<string> keywords)
        {
            var regex = GenerateRegex(keywords);
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>
            {
                builder.Regex(ComponentDao.ServiceLevel1, $"((?i){group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value),
                builder.Or(new List<FilterDefinition<ServiceDao>>
                {
                    builder.Regex(ServiceDao.ServiceCode, regex),
                    builder.Regex(ComponentDao.ShortDescription, regex)
                })
            };
            var filterDefinition = builder.And(filters);

            var totalCount = await _mongoCollection.CountAsync(filterDefinition);
            return totalCount;
        }

        /// <inheritdoc/>
        public async Task<List<Service>> ListComponents(int pageNumber = -1, int batchSize = -1)
        {
            var filterExpression = GenerateFilterDefinitions(new List<string> { }, string.Empty);
            var sortDefinition = GenerateSortDefinition(ComponentDao.DateCreated);
            var services = await
                SearchWithFilterAndSort(filterExpression, sortDefinition, pageNumber,
                    batchSize);
            if (!services.Any()) throw new ResourceNotFoundException("Services");
            return
                services;
        }

        /// <inheritdoc/>
        public async Task<List<Service>> Search(List<string> searchKeywords, string serviceLevel1, int pageNumber = -1,
            int batchSize = -1, string sortCriteria = ComponentDao.ServiceCode,
            SortOrder sortOrder = SortOrder.Descending)
        {
            //Have to do this because https://blogs.msdn.microsoft.com/ericlippert/2011/05/12/optional-argument-corner-cases-part-two/
            sortCriteria = string.IsNullOrEmpty(sortCriteria) ? ComponentDao.ServiceCode : sortCriteria;
            var filterExpression = GenerateFilterDefinition(searchKeywords, serviceLevel1);
            var sortExpression = GenerateSortDefinition(sortCriteria, sortOrder);

            var services = await
                SearchWithFilterAndSort(filterExpression, sortExpression, pageNumber,
                    batchSize);
            if (!services.Any()) throw new ResourceNotFoundException("Services");
            return
                services;
        }

        /// <inheritdoc/>
        public async Task<List<Service>> Search(List<string> searchKeywords, int pageNumber = -1, int batchSize = -1)
        {
            var filterDefinition = GenerateFilterDefinitions(searchKeywords, string.Empty);
            var services = await SearchWithFilter(searchKeywords, filterDefinition, pageNumber, batchSize);
            if (!services.Any()) throw new ResourceNotFoundException("Services");
            return
                services;
        }

        /// <inheritdoc/>
        public async Task<List<string>> SearchInGroup(List<string> keywords)
        {
            keywords = keywords.Select(k => k.ToUpper()).ToList();
            var regex = GenerateRegex(keywords);
            var aggregate = _mongoCollection.Aggregate()
                .Match(new BsonDocument { { $"{ComponentDao.SearchKeywords}._v", new BsonRegularExpression(regex) } })
                .Group(new BsonDocument { { "_id", $"${ComponentDao.ServiceLevel1}" } });
            var list = await aggregate.ToListAsync();
            if (list.Count < 1) throw new ResourceNotFoundException($"Group with service having {keywords}");
            var list1 = list.Select(e => e[0].ToString()).ToList();
            return list1;
        }

        /// <inheritdoc/>
        public async Task Update(IService service)
        {
            var dao = await GetDao(service.Id);
            if (dao == null)
                throw new ResourceNotFoundException("Service Dao");
            SetCreatedDetailsFromDB(service, dao);
            service.AmendedBy = ClaimsHelper.GetCurrentUserFullName();
            service.AmendedAt = DateTime.UtcNow;
            dao.SetDomain(service);
            await _mongoCollection.ReplaceOneAsync(Builders<ServiceDao>.Filter.Eq(ServiceDao.ServiceCode, service.Id),
                dao);
        }

        private static SortDefinition<ServiceDao> GenerateSortDefinition(string sortField,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            sortField = sortField.ToLower();
            var builder = Builders<ServiceDao>.Sort;
            if (sortOrder == SortOrder.Descending)
                return builder.Descending(new StringFieldDefinition<ServiceDao>(sortField));
            return builder.Ascending(new StringFieldDefinition<ServiceDao>(sortField));
        }

        private static SortDefinition<ServiceDao> GenerateSortDefinition(string searchField)
        {
            var builder = Builders<ServiceDao>.Sort;
            return builder.Descending(new StringFieldDefinition<ServiceDao>(searchField));
        }

        private static void SetCreatedDetailsFromDB(IService service, ServiceDao dao)
        {
            service.CreatedAt = (DateTime)dao.Columns[ComponentDao.DateCreated];
            service.CreatedBy = (string)dao.Columns[ComponentDao.CreatedBy];
        }

        private FilterDefinition<ServiceDao> GenerateFilterDefinitions(List<string> keywords,
            string serviceLevel1)
        {
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>();
            if (!string.IsNullOrEmpty(serviceLevel1))
                filters.Add(builder.Regex(ComponentDao.ServiceLevel1, $"((?i){serviceLevel1}(?-i))"));
            filters.Add(builder.Regex($"{ComponentDao.SearchKeywords}._v", GenerateRegex(keywords)));

            var filterDefinition = builder.And(filters);
            return filterDefinition;
        }

        private FilterDefinition<ServiceDao> GenerateFilterForServiceCode(string serviceId)
        {
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>();
            filters.Add(builder.Regex(ServiceDao.ServiceCode, $"([a-zA-Z][0]*)({serviceId})$"));

            var filterDefinition = builder.And(filters);
            return filterDefinition;
        }

        private string GenerateRegex(List<string> keywordsList)
        {
            return $"/({string.Join("|", keywordsList.ToArray())})/i";
        }

        private async Task<ServiceDao> GetDao(string id)
        {
            return
                (await _mongoCollection.FindAsync(Builders<ServiceDao>.Filter.Eq(ServiceDao.ServiceCode, id)))
                .FirstOrDefault();
        }

        private async Task<List<Service>> SearchWithFilter(List<string> searchKeywords,
            FilterDefinition<ServiceDao> expresssion, int pageNumber = -1, int batchSize = -1)
        {
            var foundServiceList = new List<Service>();
            var searchResultsForAllKeywords = new List<ServiceDao>();
            var findOptions = new FindOptions<ServiceDao>();

            if ((pageNumber > 0) && (batchSize > 0))
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }

            var searchResults = (await
                IMongoCollectionExtensions.FindAsync(_mongoCollection, expresssion,
                    findOptions)).ToList();
            searchResultsForAllKeywords.AddRange(searchResults);

            foreach (var searchResult in searchResultsForAllKeywords.Distinct())
                foundServiceList.Add(await searchResult.GetDomain(_serviceDefinitionRepository));

            return foundServiceList;
        }

        private async Task<List<Service>> SearchWithFilterAndSort(FilterDefinition<ServiceDao> expresssion,
            SortDefinition<ServiceDao> sortDefinition, int pageNumber = -1, int batchSize = -1)
        {
            var foundServiceList = new List<Service>();
            var searchResultsForAllKeywords = new List<ServiceDao>();
            var findOptions = new FindOptions<ServiceDao>();

            if ((pageNumber > 0) && (batchSize > 0))
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }
            findOptions.Sort = sortDefinition;

            var searchResults = (await
                IMongoCollectionExtensions.FindAsync(_mongoCollection, expresssion,
                    findOptions)).ToList();
            searchResultsForAllKeywords.AddRange(searchResults);

            foreach (var searchResult in searchResultsForAllKeywords.Distinct())
                foundServiceList.Add(await searchResult.GetDomain(_serviceDefinitionRepository));

            return foundServiceList;
        }

        /// <inheritdoc/>
        public async Task<List<Service>> Search(Dictionary<string, Tuple<string, object>> filterCriteria,
            int pageNumber, int batchSize, string sortColumn, SortOrder sortOrder)
        {
            var filterExpression = GenerateFilterDefinition(filterCriteria);
            sortColumn = string.IsNullOrEmpty(sortColumn) ? ComponentDao.ServiceCode : sortColumn;
            var sortExpression = GenerateSortDefinition(sortColumn, sortOrder);
            var services = await SearchWithFilterAndSort(filterExpression, sortExpression, pageNumber, batchSize);
            if (!services.Any()) throw new ResourceNotFoundException("Services");
            return services;
        }

        /// <inheritdoc/>
        public async Task<long> Count(Dictionary<string, Tuple<string, object>> filterCriteria)
        {
            var filterExpression = GenerateFilterDefinition(filterCriteria);
            var count = await CountWithFilter(filterExpression);
            return count;
        }

        private FilterDefinition<ServiceDao> GenerateFilterDefinition(List<string> keywords,
            string serviceLevel1)
        {
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>();
            if (!string.IsNullOrEmpty(serviceLevel1))
                filters.Add(builder.Regex(ComponentDao.ServiceLevel1, $"((?i){serviceLevel1}(?-i))"));

            filters.Add(builder.Regex($"{ComponentDao.SearchKeywords}._v", GenerateRegex(keywords)));

            var filterDefinition = builder.And(filters);
            return filterDefinition;
        }

        private FilterDefinition<ServiceDao> GenerateFilterDefinition(
            Dictionary<string, Tuple<string, object>> filterCriteria)
        {
            var builder = Builders<ServiceDao>.Filter;
            var filters = new List<FilterDefinition<ServiceDao>>();
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
                        var subFilters = new List<FilterDefinition<ServiceDao>>
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

        private async Task<long> CountWithFilter(FilterDefinition<ServiceDao> expression)
        {
            var count = (await _mongoCollection.CountAsync(expression));
            return count;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ServiceRateSearchResult>> GetAllRates(List<FilterData> filterDatas)
        {
            /*db.getCollection('services').aggregate([
            { "$unwind": "$rates._v"},
            { "$sort": { "rates._v.AppliedOn": -1} },
            {
                "$group": {
                    _id: { "Service Code": "$service_code", "Location": "$rates._v.Location", "TypeofPurchase": "$rates._v.TypeOfPurchase"},
                    "Result": {"$first": {"ServiceRate": "$$ROOT.rates._v", "short_description": "$$ROOT.short_description"}}
                }
            }
            ])*/
            var filter = MongoCollectionExtensions.GenerateFilterDefinition<BsonDocument>(
                _filterCriteriaBuilder.BuildRateFilters(filterDatas, "service"));
            var list = await _mongoCollection.Aggregate()
                .Unwind(new StringFieldDefinition<ServiceDao>("rates._v"))
                .Match(filter)
                .Sort(new BsonDocument { { "rates._v.AppliedOn", -1 } })
                .Group(new BsonDocument
                {
                    {
                        "_id",
                        new BsonDocument
                        {
                            {"service_code", "$service_code"},
                            {"Location", "$rates._v.Location"},
                            {"TypeOfPurchase", "$rates._v.TypeOfPurchase"}
                        }
                    },
                    {
                        "Result",
                        new BsonDocument
                        {
                            {
                                "$first",
                                new BsonDocument
                                {
                                    {"ServiceRate", "$$ROOT.rates._v"},
                                    {"short_description", "$$ROOT.short_description"}
                                }
                            }
                        }
                    }
                }).ToListAsync();
            
            return list.Select(row =>
            {
                var result = row.GetValue("Result").AsBsonDocument;
                var serviceRateDao =
                    BsonSerializer.Deserialize<ServiceRateDao>(result.GetValue("ServiceRate").AsBsonDocument);
                var shortDescription = result.GetValue("short_description").AsString;
                return new ServiceRateSearchResult(shortDescription, serviceRateDao.Domain(_bank));
            }).ToList();
        }
    }
}