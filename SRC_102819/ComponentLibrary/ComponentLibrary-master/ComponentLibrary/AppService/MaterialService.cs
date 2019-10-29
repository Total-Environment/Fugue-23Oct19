using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Domain.DataTypes;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
    /// <summary>
    /// Represents a Material Service
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IMaterialService"/>
    public class MaterialService : IMaterialService
    {
        private const string GenericBrandDefinition = "Generic Brand";
        private readonly IComponentDefinitionRepository<AssetDefinition> _assetDefinitionRepo;
        private readonly IBrandDefinitionRepository _brandDefinitionRepository;
        private readonly IFilterCriteriaBuilder _filterCriteriaBuilder;
        private readonly IMaterialBuilder _materialBuilder;
        private readonly IComponentDefinitionRepository<IMaterialDefinition> _materialDefinitionRepository;
        private readonly IMaterialRepository _materialRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialService"/> class.
        /// </summary>
        /// <param name="definitionRepository">The definition repository.</param>
        /// <param name="filterCriteriaBuilder">The filter criteria builder.</param>
        /// <param name="materialRepository">The material repository.</param>
        /// <param name="materialBuilder">The material builder.</param>
        /// <param name="assetDefinitionRepo">The asset definition repo.</param>
        /// <param name="brandDefinitionRepository">The brand definition repository.</param>
        public MaterialService(IComponentDefinitionRepository<IMaterialDefinition> definitionRepository,
            IFilterCriteriaBuilder filterCriteriaBuilder,
            IMaterialRepository materialRepository, IMaterialBuilder materialBuilder,
            IComponentDefinitionRepository<AssetDefinition> assetDefinitionRepo,
            IBrandDefinitionRepository brandDefinitionRepository)
        {
            _materialDefinitionRepository = definitionRepository;
            _filterCriteriaBuilder = filterCriteriaBuilder;
            _materialRepository = materialRepository;
            _materialBuilder = materialBuilder;
            _assetDefinitionRepo = assetDefinitionRepo;
            _brandDefinitionRepository = brandDefinitionRepository;
        }

        /// <inheritdoc/>
        public async Task<long> CountWithinGroup(List<FilterData> filterData, string group, List<string> searchKeywords)
        {
            var materialDefinition = await _materialDefinitionRepository.Find(group);
            var brandDefinition = await _brandDefinitionRepository.FindBy(GenericBrandDefinition);
            var filterCriteria = _filterCriteriaBuilder.Build(materialDefinition, brandDefinition, filterData, group,
                searchKeywords);
            return await _materialRepository.Count(filterCriteria);
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="searchKeywords"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<long> Count(List<string> searchKeywords, string group)
        {
            return await _materialRepository.Count(searchKeywords, group);
        }

        /// <inheritdoc/>
        public async Task<IMaterial> Create(IMaterial materialData)
        {
            var materialDefinition = await GetMaterialDefinition(materialData);
            var material = await _materialBuilder.BuildAsync(materialData, materialDefinition);
            await _materialRepository.Add(material);

            return await _materialRepository.Find(material.Id);
        }

        /// <inheritdoc/>
        public async Task<IMaterial> Find(string materialId)
        {
            return await _materialRepository.Find(materialId);
        }

        /// <inheritdoc/>
        public async Task<long> GetCountOfMaterialsHavingAttachmentColumnDataInGroup(string materialGroup,
            string columnName, List<string> keywords = null)
        {
            if (keywords == null)
                return await _materialRepository.GetTotalCountByGroupAndColumnName(materialGroup, columnName);
            return await _materialRepository.GetTotalCountByGroupAndColumnNameAndKeyWords(materialGroup, columnName,
                keywords);
        }

        /// <inheritdoc/>
        public async Task<long> GetMaterialCount()
        {
            return await _materialRepository.Count(new List<string>(), "");
        }

        /// <inheritdoc/>
        public async Task<IList<IMaterial>> GetMaterialHavingAttachmentColumnDataInGroup(string materialGroup,
            string columnName, int pageNumber, int batchSize, List<string> keywords = null)
        {
            var materialDefinitionForGroup = await GetMaterialDefinitionForGroup(materialGroup);
            var columnDefinition = GetSpecificHeaderAndColumn(materialDefinitionForGroup, columnName);
            ValidateColumnAsAttachmentType(columnDefinition, columnName);
            return await FetchMaterials(materialGroup, columnDefinition.Key, pageNumber, batchSize, keywords);
        }

        /// <inheritdoc/>
        public Task<List<IMaterial>> GetRecentMaterials(int pageNumber, int batchSize)
        {
            return _materialRepository.ListComponents(pageNumber, batchSize);
        }

        /// <inheritdoc/>
        public async Task<List<string>> SearchForGroups(List<string> keywords)
        {
            return await _materialRepository.SearchInGroup(keywords);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MaterialRateSearchResult>> GetAllRates(List<FilterData> filters)
        {
            return await _materialRepository.GetAllRates(filters);
        }

        /// <inheritdoc/>
        public async Task<List<IMaterial>> SearchWithinGroup(List<FilterData> filterData, string group,
            List<string> searchKeywords, int pageNumber, int batchSize, string sortColumn = "material Name",
            SortOrder sortOrder = SortOrder.Ascending)
        {
            var materialDefinition = await _materialDefinitionRepository.Find(group);
            var brandDefinition = await _brandDefinitionRepository.FindBy(GenericBrandDefinition);
            var filterCriteria = _filterCriteriaBuilder.Build(materialDefinition, brandDefinition, filterData, group,
                searchKeywords);
            return await _materialRepository.Search(filterCriteria, pageNumber, batchSize, sortColumn, sortOrder);
        }

        /// <summary>
        /// Search with keywords and group name
        /// </summary>
        /// <param name="searchKeywords"></param>
        /// <param name="materialLevel2"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public async Task<List<IMaterial>> Search(List<string> searchKeywords, string materialLevel2, int pageNumber,
            int batchSize, string sortColumn, SortOrder sortOrder)
        {
            return await _materialRepository.Search(searchKeywords, materialLevel2, pageNumber, batchSize, sortColumn, sortOrder);
        }

        /// <summary>
        /// Search with keywords without group name
        /// </summary>
        /// <param name="searchKeywords"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task<List<IMaterial>> Search(List<string> searchKeywords, int pageNumber, int batchSize)
        {
            return await _materialRepository.Search(searchKeywords, pageNumber, batchSize);
        }

        /// <inheritdoc/>
        public async Task<IMaterial> Update(IMaterial materialData)
        {
            var materialDefinition = await GetMaterialDefinition(materialData);
            var material = await _materialBuilder.BuildAsync(materialData, materialDefinition);
            await _materialRepository.Update(material);

            return await _materialRepository.Find(material.Id);
        }

        private async Task<List<IMaterial>> FetchMaterials(string materialGroup, string columnKey, int pageNumber,
            int batchSize, List<string> keywords = null)
        {
            if (keywords == null)
                return await _materialRepository.GetByGroupAndColumnName(materialGroup, columnKey, pageNumber, batchSize);
            return await _materialRepository.GetByGroupAndColumnNameAndKeyWords(materialGroup, columnKey, keywords,
                pageNumber, batchSize);
        }

        private async Task<AssetDefinition> GetAssetDefinition(string materialGroup)
        {
            AssetDefinition assetDefinition;
            try
            {
                assetDefinition = await _assetDefinitionRepo.Find(materialGroup);
            }
            catch (ResourceNotFoundException exception)
            {
                throw new ArgumentException("Invalid material group: No Asset definition found.", exception);
            }
            return assetDefinition;
        }

        private async Task<IMaterialDefinition> GetMaterialDefinition(IMaterial materialData)
        {
            IMaterialDefinition materialDefinition;
            var materialGroup = MaterialGroup(materialData);

            try
            {
                materialDefinition = await _materialDefinitionRepository.Find(materialGroup);
            }
            catch (ResourceNotFoundException exception)
            {
                throw new ArgumentException("Invalid material group: No definition found.", exception);
            }
            if (!await IsAsset(materialData)) return materialDefinition;
            var assetDefinition = await GetAssetDefinition(materialGroup);
            materialDefinition = assetDefinition.Merge(materialDefinition);
            return materialDefinition;
        }

        private async Task<IMaterialDefinition> GetMaterialDefinitionForGroup(string materialGroup)
        {
            try
            {
                return await _materialDefinitionRepository.Find(materialGroup);
            }
            catch (ResourceNotFoundException e)
            {
                throw new ArgumentException($"Material definition for group {materialGroup} not found.", e);
            }
        }

        private IColumnDefinition GetSpecificHeaderAndColumn(IMaterialDefinition materialDefinitionForGroup,
            string columnKey)
        {
            return materialDefinitionForGroup.Headers.SelectMany(
                    materialDefinitionHeader => materialDefinitionHeader.Columns)
                .FirstOrDefault(
                    materialDefinitionHeaderColumn =>
                        string.Equals(materialDefinitionHeaderColumn.Key, columnKey,
                            StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<bool> IsAsset(IMaterial materialData)
        {
            var canBeUsedAsAsset = ((dynamic)materialData).general.can_be_used_as_an_asset.Value;
            if (canBeUsedAsAsset != null)
                return (bool)await new BooleanDataType().Parse(canBeUsedAsAsset);
            return false;
        }

        private string MaterialGroup(IMaterial material)
        {
            var materialGroup = ((dynamic)material).classification.material_level_2.Value as string;
            if (materialGroup == null)
                throw new ArgumentException("Material Level 2 not found.");
            return materialGroup;
        }

        private void ValidateColumnAsAttachmentType(IColumnDefinition columnDefinition, string columnName)
        {
            if (columnDefinition == null)
                throw new ArgumentException($"{columnName} is not valid column in the material definition.");

            if (!columnDefinition.IsAttachmentType())
                throw new ArgumentException($"{columnName} is neither static file data type nor check list data type.");
        }

        /// <summary>
        /// Get brand attachment count.
        /// </summary>
        /// <param name="materialGroup"></param>
        /// <param name="columnName"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public async Task<long> GetCountOfBrandsHavingAttachmentColumnDataInGroup(string materialGroup, string columnName, List<string> keywords = null)
        {
            return await _materialRepository.GetTotalBrandCountByGroupAndColumnNameAndKeywords(materialGroup, columnName,
                keywords);
        }

        /// <summary>
        /// Get brand attachments.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="brandColumnKey"></param>
        /// <param name="keywords"></param>
        /// <param name="pageNumber"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public async Task<List<Dictionary<string, object>>> GetBrandAttachmentsByGroupAndColumnNameKeywods(string group, string brandColumnKey, List<string> keywords,
            int pageNumber, int batchSize)
        {
            var columnDataType = (await _brandDefinitionRepository.FindBy(GenericBrandDefinition)).Columns
                .FirstOrDefault(c => c.Key == brandColumnKey);

            return await _materialRepository.GetBrandAttachmentsByGroupAndColumnNameKeywods(group, brandColumnKey, columnDataType,
                keywords, pageNumber, batchSize);
        }
    }
}