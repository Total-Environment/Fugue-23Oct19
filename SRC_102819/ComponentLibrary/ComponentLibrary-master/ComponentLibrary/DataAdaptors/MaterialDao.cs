using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Domain;
using TE.ComponentLibrary.ComponentLibrary.RateMasters.Infrastructure.Repository.Dao;

namespace TE.ComponentLibrary.ComponentLibrary.DataAdaptors
{
    /// <summary>
    /// Represents the DAO for a Material
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository.Dao.ComponentDao" />
    [BsonSerializer(typeof(ComponentDaoSerializer))]
    public class MaterialDao : ComponentDao
    {
        /// <summary>
        /// The material code
        /// </summary>
        public const string MaterialCode = "material_code";

        /// <summary>
        /// The rates
        /// </summary>
        public const string Rates = "rates";

        /// <summary>
        /// The type of purchase
        /// </summary>
        public const string TypeOfPurchase = "TypeOfPurchase";

        /// <summary>
        /// The location
        /// </summary>
        public const string Location = "Location";

        /// <summary>
        /// The applied on
        /// </summary>
        public const string AppliedOn = "AppliedOn";

        /// <summary>
        /// The approved brands
        /// </summary>
        public const string ApprovedBrands = "approved_brands";

        /// <summary>
        /// The brand code
        /// </summary>
        public const string BrandCode = "brand_code";

        /// <summary>
        /// The purchase
        /// </summary>
        public const string Purchase = "purchase";

        /// <summary>
        /// The general
        /// </summary>
        public const string General = "general";

        /// <summary>
        /// The unit of measure
        /// </summary>
        public const string UnitOfMeasure = "unit_of_measure";

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDao" /> class.
        /// </summary>
        public MaterialDao()
        {
            Columns = new Dictionary<string, object>();
            ComponentDaoCode = MaterialCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialDao"/> class.
        /// </summary>
        /// <param name="material">The material data.</param>
        public MaterialDao(IMaterial material) : this()
        {
            SetDomain(material);
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="materialDefinitionRepository">The material definition repository.</param>
        /// <param name="assetDefinitionRepository"></param>
        /// <param name="brandDefinitionRepository"></param>
        /// <returns>Task&lt;Component&gt;.</returns>
        public async Task<Material> GetDomain(
            IComponentDefinitionRepository<IMaterialDefinition> materialDefinitionRepository,
            IComponentDefinitionRepository<AssetDefinition> assetDefinitionRepository,
            IBrandDefinitionRepository brandDefinitionRepository)
        {
            var group = (string)Columns[Group];
            object canBeUsedAsAssetObject;
            var containsCanBeUsedAsAnAsset = Columns.TryGetValue(CanBeUsedAsAnAsset, out canBeUsedAsAssetObject);
            var materialDefinition = await materialDefinitionRepository.Find(group);
            var canBeUsedAsAnAssetValue = false;
            if (canBeUsedAsAssetObject != null)
                canBeUsedAsAnAssetValue = (bool) await new BooleanDataType().Parse(canBeUsedAsAssetObject);
            if (containsCanBeUsedAsAnAsset && canBeUsedAsAnAssetValue)
            {
                try
                {
                    var assetDefinition = await assetDefinitionRepository.Find(group);
                    materialDefinition = assetDefinition.Merge(materialDefinition);
                }
                catch (ResourceNotFoundException ex)
                {
                }
            }
            var brandDefinition = await brandDefinitionRepository.FindBy("Generic Brand");
            return BuildMaterial(materialDefinition, brandDefinition);
        }

        /// <summary>
        /// Gets the domain without asset.
        /// </summary>
        /// <param name="materialDefinitionRepository">The material definition repository.</param>
        /// <param name="brandDefinitionRepository">The brand definition repository.</param>
        /// <returns></returns>
        public async Task<Material> GetDomainWithoutAsset(
            IComponentDefinitionRepository<IMaterialDefinition> materialDefinitionRepository,
            IBrandDefinitionRepository brandDefinitionRepository)
        {
            var group = (string)Columns[Group];
            var materialDefinition = await materialDefinitionRepository.Find(group);
            var brandDefinition = await brandDefinitionRepository.FindBy("Generic Brand");
            return BuildMaterial(materialDefinition, brandDefinition);
        }

        /// <summary>
        /// Sets the domain.
        /// </summary>
        /// <param name="material">The value.</param>
        public void SetDomain(IMaterial material)
        {
            foreach (var headerDataDao in material.Headers)
                foreach (var columnDataDao in headerDataDao.Columns)
                {
                    var columnKey = columnDataDao.Key.ToLower();
                    Columns[columnKey] = columnDataDao.Value;
                }
            SetAutogeneratedValues(material);
            SetSearchKeywords(material);
        }

        private Material BuildMaterial(IMaterialDefinition materialDefinition, IBrandDefinition brandDefinition)
        {
            var headers = new List<IHeaderData>();
            foreach (var materialDefinitionHeader in materialDefinition.Headers)
            {
                var header = new HeaderData(materialDefinitionHeader.Name, materialDefinitionHeader.Key);
                var columns = new List<IColumnData>();
                foreach (var columnDefinition in materialDefinitionHeader.Columns)
                {
                    if (Columns.ContainsKey(columnDefinition.Key))
                    {
                        if ((columnDefinition.DataType as ArrayDataType)?.DataType is BrandDataType)
                        {
                            var brandObjects = Columns[columnDefinition.Key] as object[];
                            if (brandObjects != null)
                            {
                                foreach (var brandObject in brandObjects)
                                {
                                    var brand = (Brand)brandObject;
                                    brand.BrandDefinition = brandDefinition;
                                }
                                columns.Add(new ColumnData(columnDefinition.Name, columnDefinition.Key,
                                    brandObjects));
                            }
                            else
                            {
                                columns.Add(new ColumnData(columnDefinition.Name, columnDefinition.Key,
                                    null));
                            }
                        }
                        else
                        {
                            columns.Add(new ColumnData(columnDefinition.Name, columnDefinition.Key,
                                Columns[columnDefinition.Key]));
                        }
                    }
                    else
                    {
                        columns.Add(new ColumnData(columnDefinition.Name, columnDefinition.Key,
                            null));
                    }
                }
                header.Columns = columns;
                headers.Add(header);
            }

            var material = new Material(headers, materialDefinition)
            {
                Id = (string)Columns[ComponentDaoCode],
                Group = (string)Columns[Group],
                CreatedAt = (DateTime)Columns[DateCreated],
                AmendedAt = (DateTime)Columns[DateLastAmended],
                CreatedBy = (string)Columns[CreatedBy],
                AmendedBy = (string)Columns[LastAmendedBy]
            };
            material.AppendSearchKeywords((List<string>)Columns[SearchKeywords]);
            return material;
        }

        private void SetAutogeneratedValues(IMaterial material)
        {
            Columns[ComponentDaoCode] = material.Id;
            Columns[Group] = material.Group;
            Columns[CreatedBy] = material.CreatedBy;
            Columns[DateCreated] = material.CreatedAt;
            Columns[DateLastAmended] = material.AmendedAt;
            Columns[LastAmendedBy] = material.AmendedBy;
        }

        private void SetSearchKeywords(IMaterial material)
        {
            Columns[SearchKeywords] = material.SearchKeywords?.ToList();
        }

        /// <summary>
        /// Adds the material rate to material DAO.
        /// </summary>
        /// <param name="materialRate">The material rate.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MaterialRateDao AddRate(MaterialRateDao materialRate)
        {
            if (!Columns.ContainsKey(Rates))
                Columns.Add(Rates, new List<MaterialRateDao>());

            var rates = (List<MaterialRateDao>)Columns[Rates];

            if (Enumerable.Contains(rates, materialRate))
            {
                throw new DuplicateResourceException(
                    $"This Material Rate is already defined for materialId: {Columns[MaterialCode]}, location: {materialRate.Location}, appliedOn: {materialRate.AppliedOn}");
            }

            rates.Add(materialRate);
            return materialRate;
        }

        /// <summary>
        /// Gets the rates.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<MaterialRateDao> GetRateDaos()
        {
            if (!Columns.ContainsKey(Rates))
                Columns.Add(Rates, new List<MaterialRateDao>());

            return (List<MaterialRateDao>)Columns[Rates];
        }
    }
}