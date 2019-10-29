using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.RepositoryPort
{
    /// <summary>
    /// Component Repository
    /// </summary>
    public class ComponentRepository : IComponentRepository
    {
        private readonly IComponentDefinitionRepository<AssetDefinition> _assetDefinitionRepository;
        private readonly IComponentDefinitionRepository<IMaterialDefinition> _materialDefinitionRepository;

        private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>
            _compositeComponentDefinitionRepository;

        private readonly IBrandDefinitionRepository _brandDefinitionRepository;
        private readonly IMongoCollection<MaterialDao> _materialCollection;
        private readonly IMongoCollection<PackageDao> _packageCollection;
        private readonly IMongoCollection<SemiFinishedGoodDao> _sfgCollection;

        private const string QueryNin = "$nin";
        private const string QueryOr = "$or";
        private const string UomPrefix = "uom";
        private const string RfaFileLinkKey = "rfa_file_link";
        private const string JsonFileLinkKey = "json_file_link";
        private const string RevitFamilyTypeKey = "revit_family_type";
        private const string EDesignHeaderKey = "edesign_specifications";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialCollection"></param>
        /// <param name="brandDefinitionRepository"></param>
        /// <param name="materialDefinitionRepository"></param>
        /// <param name="assetDefinitionRepository"></param>
        /// <param name="packageCollection"></param>
        /// <param name="compositeComponentDefinitionRepository"></param>
        /// <param name="sfgCollection"></param>
        public ComponentRepository(IMongoCollection<MaterialDao> materialCollection,
            IBrandDefinitionRepository brandDefinitionRepository,
            IComponentDefinitionRepository<IMaterialDefinition> materialDefinitionRepository,
            IComponentDefinitionRepository<AssetDefinition> assetDefinitionRepository,
            IMongoCollection<PackageDao> packageCollection,
            ICompositeComponentDefinitionRepository<ICompositeComponentDefinition>
                compositeComponentDefinitionRepository,
            IMongoCollection<SemiFinishedGoodDao> sfgCollection)
        {
            _materialCollection = materialCollection;
            _brandDefinitionRepository = brandDefinitionRepository;
            _materialDefinitionRepository = materialDefinitionRepository;
            _assetDefinitionRepository = assetDefinitionRepository;
            _packageCollection = packageCollection;
            _compositeComponentDefinitionRepository = compositeComponentDefinitionRepository;
            _sfgCollection = sfgCollection;
        }

        private static List<FilterDefinition<T>> GetReplacementQuery<T>(JArray searchQuery)
        {
            return searchQuery.SelectMany(c => GetReplacementQuery<T>(c.ToObject<JObject>())).ToList();
        }

        private static List<FilterDefinition<T>> GetReplacementQuery<T>(JObject searchQuery)
        {
            var builder = Builders<T>.Filter;
            var keys = searchQuery.Properties();
            return keys.Where(c => c.Value != null).Select(c =>
            {
                if (c.Name.StartsWith($"{UomPrefix}."))
                {
                    var key = c.Name.Split('.').Last();
                    if (String.IsNullOrEmpty(key))
                    {
                        return null;
                    }
                    return builder.Eq($"{key}.Value", c.Value.ToObject<double>());
                }
                if (c.Value is JValue && (c.Value.Type == JTokenType.Integer || c.Value.Type == JTokenType.Float))
                {
                    return builder.Eq(c.Name, c.Value.ToObject<double>());
                }
                if (c.Value is JValue && c.Value.Type == JTokenType.String)
                {
                    return builder.Eq(c.Name, c.Value.ToString());
                }
                if (c.Value is JArray && c.Name == QueryOr)
                {
                    return builder.Or(GetReplacementQuery<T>(c.Value.ToObject<JArray>()));
                }
                if (c.Value is JObject && c.Value[QueryNin] is JArray)
                {
                    return builder.Nin(c.Name, (JArray) c.Value[QueryNin]);
                }
                return null;
            }).Where(c => c != null).ToList();
        }

        /// <summary>
        /// Find component replacement for a given search query
        /// </summary>
        /// <param name="type"></param>
        /// <param name="searchQuery"></param>
        /// <param name="merge"></param>
        public async Task<object> FindReplacements(ComponentType type, JObject searchQuery, bool merge = false)
        {
            if (type == ComponentType.Material)
            {
                var builder = Builders<MaterialDao>.Filter;
                var filters = GetReplacementQuery<MaterialDao>(searchQuery);

                var materialDaos = (await _materialCollection.FindAsync(builder.And(filters))).ToList();

                var materials = new List<IMaterial>();
                materialDaos.ForEach(async dao =>
                {
                    materials.Add(await dao.GetDomain(_materialDefinitionRepository, _assetDefinitionRepository,
                        _brandDefinitionRepository));
                });
                return materials;
            }
            if (type == ComponentType.Package)
            {

                var compositeComponentDefinition =
                    await _compositeComponentDefinitionRepository.Find("package", Keys.Package.PackageDefinitionGroup);
                var builder = Builders<PackageDao>.Filter;
                var filters = GetReplacementQuery<PackageDao>(searchQuery);
                var packageDaos = (await _packageCollection.FindAsync(builder.And(filters))).ToList();
                return packageDaos.Select(c => c.ToCompositeComponent(compositeComponentDefinition)).ToList();
            }
            if (type == ComponentType.SFG)
            {

                var compositeComponentDefinition =
                    await _compositeComponentDefinitionRepository.Find("sfg", Keys.Sfg.SfgDefinitionGroup);
                var builder = Builders<SemiFinishedGoodDao>.Filter;
                var filters = GetReplacementQuery<SemiFinishedGoodDao>(searchQuery);
                var sfgDaos = (await _sfgCollection.FindAsync(builder.And(filters))).ToList();
                return sfgDaos.Select(c => c.ToCompositeComponent(compositeComponentDefinition)).ToList();
            }

            // Return empty list
            return new List<object>();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateRfaDetails(ComponentType type, string code, string rfaLink, string jsonLink,
            string revitFamilyType)
        {
            if (type == ComponentType.Material)
            {
                var filter = Builders<MaterialDao>.Filter.Eq(MaterialDao.MaterialCode, code);
                var component = (await _materialCollection.FindAsync(filter)).FirstOrDefault();
                if (component == null)
                {
                    throw new Exception($"Component with code {code} was not found");
                }
                var updateQuery = Builders<MaterialDao>.Update
                    .Set(RfaFileLinkKey, rfaLink ?? component.Columns[RfaFileLinkKey]?.ToString())
                    .Set(JsonFileLinkKey, jsonLink ?? component.Columns[JsonFileLinkKey]?.ToString())
                    .Set(RevitFamilyTypeKey, revitFamilyType ?? component.Columns[RevitFamilyTypeKey]?.ToString());
                await _materialCollection.UpdateOneAsync(filter, updateQuery);
                return true;
            }

            if (type == ComponentType.Package)
            {
                var filter = Builders<PackageDao>.Filter.Eq(PackageDao.Code, code);
                var component = (await _packageCollection.FindAsync(filter)).FirstOrDefault();
                if (component == null)
                {
                    throw new Exception($"Component with code {code} was not found");
                }
                var updateQuery = Builders<PackageDao>.Update
                    .Set(RfaFileLinkKey, rfaLink ?? component.Columns[RfaFileLinkKey]?.ToString())
                    .Set(JsonFileLinkKey, jsonLink ?? component.Columns[JsonFileLinkKey]?.ToString())
                    .Set(RevitFamilyTypeKey, revitFamilyType ?? component.Columns[RevitFamilyTypeKey]?.ToString());
                await _packageCollection.UpdateOneAsync(filter, updateQuery);
                return true;
            }
            return false;
        }
    }
}