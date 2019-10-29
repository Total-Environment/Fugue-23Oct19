using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.Extensions;
using TE.ComponentLibrary.ComponentLibrary.Helpers;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Core;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// The Material Repository
    /// </summary>
    public class MaterialRepository : IMaterialRepository
    {
        private readonly IComponentDefinitionRepository<AssetDefinition> _assetDefinitionRepository;
        private readonly IComponentDefinitionRepository<IMaterialDefinition> _materialDefinitionRepository;
        private readonly IBrandDefinitionRepository _brandDefinitionRepository;
        private readonly IBank _bank;
        private readonly IMongoCollection<MaterialDao> _mongoCollection;
        private readonly IFilterCriteriaBuilder _filterCriteriaBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialRepository"/> class.
        /// </summary>
        /// <param name="mongoCollection">The mongo collection.</param>
        /// <param name="materialDefinitionRepository">The material definition repository.</param>
        /// <param name="assetDefinitionRepository">The asset definition repository.</param>
        /// <param name="brandDefinitionRepository">The brand definition repository.</param>
        /// <param name="bankRepository">The bank repository.</param>
        public MaterialRepository(IMongoCollection<MaterialDao> mongoCollection,
            IComponentDefinitionRepository<IMaterialDefinition> materialDefinitionRepository,
            IComponentDefinitionRepository<AssetDefinition> assetDefinitionRepository,
            IBrandDefinitionRepository brandDefinitionRepository, IBank bank, IFilterCriteriaBuilder filterCriteriaBuilder)
        {
            _mongoCollection = mongoCollection;
            _materialDefinitionRepository = materialDefinitionRepository;
            _assetDefinitionRepository = assetDefinitionRepository;
            _brandDefinitionRepository = brandDefinitionRepository;
            _bank = bank;
            _filterCriteriaBuilder = filterCriteriaBuilder;
        }

        /// <inheritdoc/>
        public async Task Add(IMaterial material)
        {
            if (material.Id == null)
                throw new ArgumentNullException(nameof(material), "Id is required.");
            var materialIdWithoutGroup = material.Id.Remove(0, 3).TrimStart('0');

            material.AmendedAt = material.CreatedAt = DateTime.UtcNow;
            material.AmendedBy = material.CreatedBy = ClaimsHelper.GetCurrentUserFullName();
            await CheckBrandCodeDuplicacy(material);
            var filterExpression = GenerateFilterForMaterialCode(materialIdWithoutGroup);
            var searchResults = (await
                _mongoCollection.FindAsync(filterExpression)).ToList();
            if (!searchResults.Any())
            {
                var materialDao = new MaterialDao(material);
                await _mongoCollection.InsertOneAsync(materialDao);
            }
            else
            {
                throw new DuplicateResourceException($"material code: {materialIdWithoutGroup} is already exists");
            }
        }

        /// <inheritdoc/>
        public async Task<int> Count(List<string> list, string componentLevel2)
        {
            var filterDefinition = GenerateFilterDefinition(list, componentLevel2);
            var result = await _mongoCollection.FindAsync(filterDefinition);

            return result.ToList().Distinct().Count();
        }

        /// <inheritdoc/>
        public async Task<long> Count(Dictionary<string, Tuple<string, object>> filterCriteria)
        {
            var filterExpression = MongoCollectionExtensions.GenerateFilterDefinition<MaterialDao>(filterCriteria);
            var count = await CountWithFilter(filterExpression);
            return count;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MaterialRateSearchResult>> GetAllRates(List<FilterData> filterDatas)
        {
            /*db.getCollection('materials').aggregate([
            { "$unwind": "$rates._v"},
            { "$sort": { "rates._v.AppliedOn": -1} },
            {
                "$group": {
                    _id: { "Material Code": "$material_code", "Location": "$rates._v.Location", "TypeofPurchase": "$rates._v.TypeOfPurchase"},
                    "Result": {"$first": {"MaterialRate": "$$ROOT.rates._v", "material_name": "$$ROOT.material_name"}}
                }
            }
            ])*/
            var filter = MongoCollectionExtensions.GenerateFilterDefinition<BsonDocument>(
                _filterCriteriaBuilder.BuildRateFilters(filterDatas, "material"));
            var list = await _mongoCollection.Aggregate()
                .Unwind(new StringFieldDefinition<MaterialDao>("rates._v"))
                .Match(filter)
                .Sort(new BsonDocument { { "rates._v.AppliedOn", -1 } })
                .Group(new BsonDocument
                {
                    {
                        "_id",
                        new BsonDocument
                        {
                            {"material_code", "$material_code"},
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
                                    {"MaterialRate", "$$ROOT.rates._v"},
                                    {"material_name", "$$ROOT.material_name"},
                                    {"short_description", "$$ROOT.short_description" }
                                }
                            }
                        }
                    }
                }).ToListAsync();
            
            return list.Select(row =>
            {
                var result = row.GetValue("Result").AsBsonDocument;
                var materialRateDao = BsonSerializer.Deserialize<MaterialRateDao>(result.GetValue("MaterialRate").AsBsonDocument);
                var materialName = result.GetValue("material_name").AsString;
                var shortDescription = result.GetValue("short_description").AsString;
                return new MaterialRateSearchResult(materialName, materialRateDao.Domain(_bank),shortDescription);
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<IMaterial> Find(string id)
        {
            var dao = await GetDao(id);
            if (dao == null)
                throw new ResourceNotFoundException(id);
            var material = await dao.GetDomain(_materialDefinitionRepository, _assetDefinitionRepository,
                _brandDefinitionRepository);
            return material;
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> GetByGroupAndColumnName(string group, string columnName, int pageNumber = -1,
            int batchSize = -1)
        {
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>
            {
                builder.Regex(ComponentDao.MaterialLevel2, $"((?i){group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value)
            };
            var filterDefinition = builder.And(filters);

            var findOptions = new FindOptions<MaterialDao>();
            if (pageNumber > 0 && batchSize > 0)
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }

            var materialDaos = (await _mongoCollection.FindAsync(filterDefinition, findOptions)).ToList();
            var materials = new List<IMaterial>();
            foreach (var materialDao in materialDaos)
                materials.Add(await materialDao.GetDomain(_materialDefinitionRepository, _assetDefinitionRepository,
                    _brandDefinitionRepository));
            return materials;
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> GetByGroupAndColumnNameAndKeyWords(string group, string columnName,
            List<string> keywords, int pageNumber = -1, int batchSize = -1)
        {
            var regex = GenerateRegex(keywords);
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>
            {
                builder.Regex(ComponentDao.MaterialLevel2, $"((?i){group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value),
                builder.Or(new List<FilterDefinition<MaterialDao>>
                {
                    builder.Regex(MaterialDao.MaterialCode, regex),
                    builder.Regex(ComponentDao.ShortDescription, regex)
                })
            };
            var filterDefinition = builder.And(filters);

            var findOptions = new FindOptions<MaterialDao>();
            if (pageNumber > 0 && batchSize > 0)
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }

            var materialDaos = (await _mongoCollection.FindAsync(filterDefinition, findOptions)).ToList();
            var materials = new List<IMaterial>();
            foreach (var materialDao in materialDaos)
                materials.Add(await materialDao.GetDomain(_materialDefinitionRepository, _assetDefinitionRepository,
                    _brandDefinitionRepository));
            return materials;
        }

        /// <inheritdoc/>
        public async Task<long> GetTotalCountByGroupAndColumnName(string group, string columnName)
        {
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>
            {
                builder.Regex(ComponentDao.MaterialLevel2, $"((?i){group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value)
            };
            var filterDefinition = builder.And(filters);

            var totalCount = await _mongoCollection.CountAsync(filterDefinition);
            return totalCount;
        }

        /// <inheritdoc/>
        public async Task<long> GetTotalCountByGroupAndColumnNameAndKeyWords(string group, string columnName,
            List<string> keywords)
        {
            var regex = GenerateRegex(keywords);
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>
            {
                builder.Regex(ComponentDao.MaterialLevel2, $"((?i){group}(?-i))"),
                builder.Ne(columnName, BsonNull.Value),
                builder.Or(new List<FilterDefinition<MaterialDao>>
                {
                    builder.Regex(MaterialDao.MaterialCode, regex),
                    builder.Regex(ComponentDao.ShortDescription, regex)
                })
            };
            var filterDefinition = builder.And(filters);

            var totalCount = await _mongoCollection.CountAsync(filterDefinition);
            return totalCount;
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> ListComponents(int pageNumber = 1, int batchSize = -1)
        {
            var filterExpression = GenerateFilterDefinition(new List<string>(), "");
            var sortDefinition = GenerateSortDefinition(ComponentDao.DateCreated, SortOrder.Descending);
            var materials = await
                SearchWithFilterAndSort(filterExpression, sortDefinition, pageNumber,
                    batchSize);
            if (!materials.Any()) throw new ResourceNotFoundException("Materials");
            return
                materials;
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> Search(List<string> searchKeywords, string materialLevel2, int pageNumber = -1,
            int batchSize = -1, string sortCriteria = ComponentDao.MaterialName,
            SortOrder sortOrder = SortOrder.Descending)
        {
            //Have to do this because https://blogs.msdn.microsoft.com/ericlippert/2011/05/12/optional-argument-corner-cases-part-two/
            sortCriteria = string.IsNullOrEmpty(sortCriteria) ? ComponentDao.MaterialName : sortCriteria;
            var filterExpression = GenerateFilterDefinition(searchKeywords, materialLevel2);
            var sortExpression = GenerateSortDefinition(sortCriteria, sortOrder);

            var materials = await
                SearchWithFilterAndSort(filterExpression, sortExpression, pageNumber,
                    batchSize);
            if (!materials.Any()) throw new ResourceNotFoundException("Materials");
            return
                materials;
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> Search(List<string> searchKeywords, int pageNumber = -1, int batchSize = -1)
        {
            var filterExpression = GenerateFilterDefinition(searchKeywords, string.Empty);
            var materials = await SearchWithFilter(filterExpression, pageNumber, batchSize);
            if (!materials.Any()) throw new ResourceNotFoundException("Materials");
            return
                materials;
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> Search(Dictionary<string, Tuple<string, object>> filterCriteria,
            int pageNumber = -1, int batchSize = -1, string sortColumn = ComponentDao.MaterialName,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            var filterExpression = MongoCollectionExtensions.GenerateFilterDefinition<MaterialDao>(filterCriteria);
            sortColumn = string.IsNullOrEmpty(sortColumn) ? ComponentDao.MaterialName : sortColumn;
            var sortExpression = GenerateSortDefinition(sortColumn, sortOrder);
            var materials = await SearchWithFilterAndSort(filterExpression, sortExpression, pageNumber, batchSize);
            if (!materials.Any()) throw new ResourceNotFoundException("Materials");
            return materials;
        }

        /// <inheritdoc/>
        public async Task<List<string>> SearchInGroup(List<string> keywords)
        {
            var regex = GenerateRegex(keywords);
            var aggregate = _mongoCollection.Aggregate()
                .Match(new BsonDocument { { $"{ComponentDao.SearchKeywords}._v", new BsonRegularExpression(regex) } })
                .Group(new BsonDocument { { "_id", $"${ComponentDao.MaterialLevel2}" } });
            var list = await aggregate.ToListAsync();
            if (list.Count < 1) throw new ResourceNotFoundException($"Group with material having {keywords}");
            var list1 = list.Select(e => e[0].ToString()).ToList();
            return list1;
        }

        /// <inheritdoc/>
        public async Task Update(IMaterial material)
        {
            var dao = await GetDao(material.Id);
            if (dao == null)
                throw new ResourceNotFoundException("Material Dao");
            await CheckBrandCodeDuplicacy(material);
            SetCreatedDetailsFromDB(material, dao);
            material.AmendedBy = ClaimsHelper.GetCurrentUserFullName();
            material.AmendedAt = DateTime.UtcNow;
            dao.SetDomain(material);
            await _mongoCollection.ReplaceOneAsync(
                Builders<MaterialDao>.Filter.Eq(MaterialDao.MaterialCode, material.Id), dao);
        }

        //TODO: Move to a utility file as not directly needed in Material Repository
        private async Task CheckBrandCodeDuplicacy(IMaterial material)
        {
            var brandObjects = material["Purchase"]?["Approved Brands"] as object[];
            var brandCodes = new List<string>();

            if (brandObjects != null)
            {
                foreach (var brandObject in brandObjects)
                {
                    var brand = brandObject as Brand;
                    var brandCode = Convert.ToString(brand[MaterialDao.BrandCode]);
                    if (brandCodes.Contains(brandCode))
                    {
                        throw new DuplicateResourceException($"Duplicate Brand Code {brandCode}");
                    }
                    brandCodes.Add(brandCode);
                }

                foreach (var brandObject in brandObjects)
                {
                    var brand = brandObject as Brand;
                    var brandCode = Convert.ToString(brand[MaterialDao.BrandCode]);
                    var filterExpression = GenerateFilterForBrandCode(brandCode, material.Id);
                    var searchResults = (await _mongoCollection.FindAsync(filterExpression)).ToList();
                    if (searchResults.Any())
                    {
                        throw new DuplicateResourceException($"Duplicate Brand Code {brandCode}");
                    }
                }
            }
        }

        private FilterDefinition<MaterialDao> GenerateFilterForBrandCode(string brandCode, string materialCode)
        {
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>
            {
                builder.Ne(MaterialDao.MaterialCode, materialCode),
                builder.Regex($"{MaterialDao.ApprovedBrands}._v._v.{MaterialDao.BrandCode}.Value",
                    new BsonRegularExpression(new Regex(brandCode, RegexOptions.IgnoreCase)))
            };

            var filterDefinition = builder.And(filters);
            return filterDefinition;
        }

        private static FilterDefinition<MaterialDao> GenerateFilterForMaterialCode(string materialId)
        {
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>
            {
                builder.Regex(MaterialDao.MaterialCode, $"([a-zA-Z][0]*)({materialId}$)")
            };

            var filterDefinition = builder.And(filters);
            return filterDefinition;
        }

        private static SortDefinition<MaterialDao> GenerateSortDefinition(string sortColumn,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            sortColumn = sortColumn.ToLower();
            var builder = Builders<MaterialDao>.Sort;
            if (sortOrder == SortOrder.Descending)
                return builder.Descending(new StringFieldDefinition<MaterialDao>(sortColumn));
            return builder.Ascending(new StringFieldDefinition<MaterialDao>(sortColumn));
        }

        private static void SetCreatedDetailsFromDB(IMaterial material, MaterialDao dao)
        {
            material.CreatedAt = (DateTime)dao.Columns[ComponentDao.DateCreated];
            material.CreatedBy = (string)dao.Columns[ComponentDao.CreatedBy];
        }

        private async Task<long> CountWithFilter(FilterDefinition<MaterialDao> expression)
        {
            var count = (await _mongoCollection.CountAsync(expression));
            return count;
        }

        private FilterDefinition<MaterialDao> GenerateFilterDefinition(List<string> keywords,
            string materialLevel2)
        {
            var builder = Builders<MaterialDao>.Filter;
            var filters = new List<FilterDefinition<MaterialDao>>();
            if (!string.IsNullOrEmpty(materialLevel2))
                filters.Add(builder.Regex(ComponentDao.MaterialLevel2, $"((?i){materialLevel2}(?-i))"));

            filters.Add(builder.Regex($"{ComponentDao.SearchKeywords}._v", GenerateRegex(keywords)));

            var filterDefinition = builder.And(filters);
            return filterDefinition;
        }

        private string GenerateRegex(List<string> keywordsList)
        {
            return $"/{GenerateRegexPattern(keywordsList)}/i";
        }

        private string GenerateRegexPattern(List<string> keywordsList)
        {
            return $"({string.Join("|", keywordsList.ToArray())})";
        }

        private async Task<MaterialDao> GetDao(string id)
        {
            return
                (await _mongoCollection.FindAsync(Builders<MaterialDao>.Filter.Eq(MaterialDao.MaterialCode, id)))
                .FirstOrDefault();
        }

        private async Task<List<IMaterial>> SearchWithFilter(FilterDefinition<MaterialDao> expression,
            int pageNumber = -1, int batchSize = -1)
        {
            var foundMaterialList = new List<IMaterial>();
            var searchResultsForAllKeywords = new List<MaterialDao>();
            var findOptions = new FindOptions<MaterialDao>();
            if (pageNumber > 0 && batchSize > 0)
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }

            var searchResults = (await
                IMongoCollectionExtensions.FindAsync(_mongoCollection, expression,
                    findOptions)).ToList();
            searchResultsForAllKeywords.AddRange(searchResults);

            foreach (var searchResult in searchResultsForAllKeywords.Distinct())
                foundMaterialList.Add(await searchResult.GetDomain(_materialDefinitionRepository,
                    _assetDefinitionRepository, _brandDefinitionRepository));

            return foundMaterialList;
        }

        private async Task<List<IMaterial>> SearchWithFilterAndSort(FilterDefinition<MaterialDao> expression,
            SortDefinition<MaterialDao> sortDefintion, int pageNumber = -1, int batchSize = -1)
        {
            var foundMaterialList = new List<IMaterial>();
            var searchResultsForAllKeywords = new List<MaterialDao>();
            var searchResults = await SearchResultsWithoutKeywords(expression, sortDefintion, pageNumber, batchSize);
            searchResultsForAllKeywords.AddRange(searchResults);

            foreach (var searchResult in searchResultsForAllKeywords.Distinct())
                foundMaterialList.Add(await searchResult.GetDomainWithoutAsset(_materialDefinitionRepository,
                    _brandDefinitionRepository));

            return foundMaterialList;
        }

        private async Task<List<MaterialDao>> SearchResultsWithoutKeywords(FilterDefinition<MaterialDao> expression, SortDefinition<MaterialDao> sortDefintion, int pageNumber,
            int batchSize)
        {
            var findOptions = new FindOptions<MaterialDao>();
            if (pageNumber > 0 && batchSize > 0)
            {
                findOptions.Limit = batchSize;
                findOptions.Skip = (pageNumber - 1) * batchSize;
            }
            findOptions.Sort = sortDefintion;

            var searchResults = (await
                _mongoCollection.FindAsync(expression,
                    findOptions)).ToList();
            return searchResults;
        }

        /// <inheritdoc />
        public async Task<long> GetTotalBrandCountByGroupAndColumnNameAndKeywords(string group, string columnName,
            List<string> keywords)
        {
            var regex = keywords != null && keywords.Count > 0 ? GenerateRegexPattern(keywords) : "";

            var matchGroupAndFilterOutNullBrands = new BsonDocument
            {
                {
                    "$match", new BsonDocument
                    {
                        {
                            ComponentDao.MaterialLevel2, group
                        },
                        {
                            "approved_brands", new BsonDocument
                            {
                                { "$ne", BsonNull.Value }
                            }
                        }
                    }
                }
            };

            var projectMaterialCodeAndBrands = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            "brands", "$approved_brands._v._v"
                        },
                        {
                            "material_code", 1
                        }
                    }
                }
            };

            var unwindBrands = new BsonDocument
            {
                {
                    "$unwind", "$brands"
                }
            };

            var matchNotNullColumnValues = new BsonDocument
            {
                {
                    "$match", new BsonDocument {
                        {
                            $"brands.{columnName}.Value",
                            new BsonDocument
                            {
                                {
                                    "$ne", BsonNull.Value
                                }
                            }
                        }
                    }
                }
            };

            var groupTotalCount = new BsonDocument
            {
                {
                    "$group",
                    new BsonDocument
                    {
                        {
                            "_id", BsonString.Create("")
                        },
                        {
                            "total_count", new BsonDocument
                            {
                                {
                                    "$sum", 1
                                }
                            }
                        }
                    }
                }
            };

            var projectTotalCount = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            "total_count", 1
                        },
                        {
                            "_id", 0
                        }
                    }
                }
            };

            var matchKeyWordOrArr = new BsonArray
            {
                new BsonDocument
                {
                    {
                        "material_code", BsonRegularExpression.Create(new Regex(regex, RegexOptions.IgnoreCase))
                    }
                },
                new BsonDocument
                {
                    {
                        "brands.brand_code.Value", BsonRegularExpression.Create(new Regex(regex, RegexOptions.IgnoreCase))
                    }
                },
                new BsonDocument
                {
                    {
                        "brands.manufacturers_name.Value", BsonRegularExpression.Create(new Regex(regex, RegexOptions.IgnoreCase))
                    }
                }
            };

            var matchKeyWords = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "$or",
                            matchKeyWordOrArr
                        }
                    }
                }
            };

            var query = _mongoCollection.Aggregate()
                .AppendStage<BsonDocument>(matchGroupAndFilterOutNullBrands)
                .AppendStage<BsonDocument>(projectMaterialCodeAndBrands)
                .AppendStage<BsonDocument>(unwindBrands)
                .AppendStage<BsonDocument>(matchNotNullColumnValues);

            if (!String.IsNullOrEmpty(regex))
            {
                query = query.AppendStage<BsonDocument>(matchKeyWords);
            }

            var result = await query
                .AppendStage<BsonDocument>(groupTotalCount)
                .AppendStage<BsonDocument>(projectTotalCount)
                .ToListAsync();

            var totalCount = result.Select(c => c.GetValue("total_count")).FirstOrDefault();

            return totalCount != null ? totalCount.ToInt32() : 0;
        }

        public async Task<List<Dictionary<string, object>>> GetBrandAttachmentsByGroupAndColumnNameKeywods(string group, string brandColumnName, ISimpleColumnDefinition columnDataType,
            List<string> keywords, int pageNumber = -1, int batchSize = -1)
        {
            var regex = keywords != null && keywords.Count > 0 ? GenerateRegexPattern(keywords) : "";

            var isFileColumnArray = columnDataType.DataType is ArrayDataType;

            var matchGroupAndFilterOutNullBrands = new BsonDocument
            {
                {
                    "$match", new BsonDocument
                    {
                        {
                            ComponentDao.MaterialLevel2, group
                        },
                        {
                            "approved_brands", new BsonDocument
                            {
                                { "$ne", BsonNull.Value }
                            }
                        }
                    }
                }
            };

            var projectMaterialCodeAndBrands = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            MaterialDao.MaterialCode, 1
                        },
                        {
                            "brands", "$approved_brands._v._v"
                        }
                    }
                }
            };

            var unwindBrands = new BsonDocument
            {
                {
                    "$unwind", "$brands"
                }
            };

            var filesProjection = isFileColumnArray ? "._v" : "";

            var projectMaterialCodeAndBrandFiles = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            MaterialDao.MaterialCode, 1
                        },
                        {
                            "manufacturers_name", "$brands.manufacturer's_name.Value"
                        },
                        {
                            "brand_code", "$brands.brand_code.Value"
                        },
                        {
                            "files", $"$brands.{brandColumnName}.Value{filesProjection}"
                        }
                    }
                }
            };

            var filesMapping = new BsonDocument
            {
                {
                    "$map",
                    new BsonDocument
                    {
                        {
                            "input", "$files"
                        },
                        {
                            "as", "file"
                        },
                        {
                            "in",
                            new BsonDocument
                            {
                                {
                                    "id", "$$file._id"
                                },
                                {
                                    "type", "$$file._t"
                                },
                                {
                                    "name", "$$file.Name"
                                }
                            }
                        }
                    }
                }
            };

            var matchKeyWordOrArr = new BsonArray
            {
                new BsonDocument
                {
                    {
                        "material_code", BsonRegularExpression.Create(new Regex(regex, RegexOptions.IgnoreCase))
                    }
                },
                new BsonDocument
                {
                    {
                        "brands.brand_code.Value", BsonRegularExpression.Create(new Regex(regex, RegexOptions.IgnoreCase))
                    }
                },
                new BsonDocument
                {
                    {
                        "brands.manufacturer's_name.Value", BsonRegularExpression.Create(new Regex(regex, RegexOptions.IgnoreCase))
                    }
                }
            };

            var matchKeyWords = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            "$or",
                            matchKeyWordOrArr
                        }
                    }
                }
            };
            var omitEmptyFiles = !isFileColumnArray
                ? new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {
                                "files",
                                new BsonDocument
                                {
                                    {
                                        "$ne",
                                        BsonNull.Value
                                    }
                                }
                            }
                        }
                    }
                }
                : new BsonDocument
                {
                    {
                        "$match",
                        new BsonDocument
                        {
                            {
                                "files",
                                new BsonDocument
                                {
                                    {
                                        "$exists",
                                        true
                                    }
                                }
                            }
                        }
                    }
                };

            var projectFinalResult = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            MaterialDao.MaterialCode, 1
                        },
                        {
                            "_id", 0
                        },
                        {
                            "manufacturers_name", 1
                        },
                        {
                            "brand_code", 1
                        },
                        {
                            "files", filesMapping
                        }
                    }
                }
            };

            var groupSingleStaticFile = new BsonDocument
            {
                {
                    "$group",
                    new BsonDocument
                    {
                        {
                            "_id",
                            new BsonDocument
                            {
                                {
                                    "_id", "$_id"
                                },
                                {
                                    "material_code", "$material_code"
                                },
                                {
                                    "manufacturers_name", "$manufacturers_name"
                                },
                                {
                                    "brand_code", "$brand_code"
                                }
                            }
                        },
                        {
                            "files",
                            new BsonDocument
                            {
                                {
                                    "$push", "$files"
                                }
                            }
                        }
                    }
                }
            };

            var projectSingleStaticFileToArray = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            "_id", "$_id._id"
                        },
                        {
                            "material_code", "$_id.material_code"
                        },
                        {
                            "manufacturers_name", "$_id.manufacturers_name"
                        },
                        {
                            "brand_code", "$_id.brand_code"
                        },
                        {
                            "files", 1
                        }
                    }
                }
            };

            var query = _mongoCollection.Aggregate()
                .AppendStage<BsonDocument>(matchGroupAndFilterOutNullBrands)
                .AppendStage<BsonDocument>(projectMaterialCodeAndBrands)
                .AppendStage<BsonDocument>(unwindBrands);

            if (!String.IsNullOrEmpty(regex))
            {
                query = query.AppendStage<BsonDocument>(matchKeyWords);
            }

            query = query.AppendStage<BsonDocument>(projectMaterialCodeAndBrandFiles)
                .AppendStage<BsonDocument>(omitEmptyFiles);

            if (!isFileColumnArray)
            {
                query = query.AppendStage<BsonDocument>(groupSingleStaticFile)
                    .AppendStage<BsonDocument>(projectSingleStaticFileToArray);
            }

            query = query.AppendStage<BsonDocument>(projectFinalResult);

            if (pageNumber > 0 && batchSize > 0)
            {
                query = query.AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$skip", (pageNumber - 1) * batchSize
                    }
                }).AppendStage<BsonDocument>(new BsonDocument
                {
                    {
                        "$limit", batchSize
                    }
                });
            }

            var result = await query.ToListAsync();

            return result.Select(c => c.ToDictionary()).ToList();
        }
    }
}