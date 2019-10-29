using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using TE.ComponentLibrary.ComponentLibrary.DataAdaptors;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IMongoCollection<MaterialDao> _materialCollection;
        private readonly IMongoCollection<ServiceDao> _serviceCollection;
        private readonly IMongoCollection<SemiFinishedGoodDao> _sfgCollection;
        private readonly IMongoCollection<PackageDao> _packageCollection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialCollection"></param>
        /// <param name="serviceCollection"></param>
        /// <param name="sfgCollection"></param>
        /// <param name="packageCollection"></param>
        public ValidationService(IMongoCollection<MaterialDao> materialCollection,
            IMongoCollection<ServiceDao> serviceCollection,
            IMongoCollection<SemiFinishedGoodDao> sfgCollection,
            IMongoCollection<PackageDao> packageCollection)
        {
            _materialCollection = materialCollection;
            _serviceCollection = serviceCollection;
            _sfgCollection = sfgCollection;
            _packageCollection = packageCollection;
        }

        /// <inheritdoc />
        public async Task<List<string>> ValidateAssemblyCodes(List<string> assemblyCodes, string type)
        {
            Func<string, string> GetCodeKey = t =>
            {
                var key = "";
                switch (t)
                {
                    case "material":
                        key = MaterialDao.MaterialCode;
                        break;
                    case "service":
                        key = ServiceDao.ServiceCode;
                        break;
                    case "sfg":
                        key = SemiFinishedGoodDao.Code;
                        break;
                    case "packages":
                        key = PackageDao.Code;
                        break;
                }
                return key;
            };

            var codeType = GetCodeKey(type);
            var matchAssemblyCodes = new BsonDocument
            {
                {
                    "$match",
                    new BsonDocument
                    {
                        {
                            codeType,
                            new BsonDocument
                            {
                                {
                                    "$in",
                                    new BsonArray(assemblyCodes)
                                }
                            }
                        }
                    }
                }
            };

            var projection = new BsonDocument
            {
                {
                    "$project",
                    new BsonDocument
                    {
                        {
                            codeType,
                            1
                        }
                    }
                }
            };
            List<BsonDocument> result = new List<BsonDocument>();
            switch (type)
            {
                case "material":
                    result = await _materialCollection.Aggregate()
                        .AppendStage<BsonDocument>(matchAssemblyCodes)
                        .AppendStage<BsonDocument>(projection)
                        .ToListAsync();
                    break;
                case "service":
                    result = await _serviceCollection.Aggregate()
                        .AppendStage<BsonDocument>(matchAssemblyCodes)
                        .AppendStage<BsonDocument>(projection)
                        .ToListAsync();
                    break;
                case "sfg":
                    result = await _sfgCollection.Aggregate()
                        .AppendStage<BsonDocument>(matchAssemblyCodes)
                        .AppendStage<BsonDocument>(projection)
                        .ToListAsync();
                    break;
                case "packages":
                    result = await _packageCollection.Aggregate()
                        .AppendStage<BsonDocument>(matchAssemblyCodes)
                        .AppendStage<BsonDocument>(projection)
                        .ToListAsync();
                    break;
            }

            return result.Select(c => c.GetValue(codeType).AsString).ToList();
        }
    }
}